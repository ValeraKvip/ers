const express = require('express');
const { Telegraf, Markup } = require('telegraf');
const { message } = require("telegraf/filters");
const path = require('path');
const process = require('process');
const ngrok = require("@ngrok/ngrok");
const bodyParser = require('body-parser');
require('dotenv').config()

const HOOK_PATH = process.env.HOOK_PATH || "hook";


const rules = `
Each player is dealt 26 cards.

Players alternately discard one card from their hand.

When some of the following combinations occur players race to slap the discard pile:
 * Double (e.g. 2-2)
 * Marriage (e.g. K-Q)
 * Sandwich (e.g. 2-5-2)
 * Divorce (e.g. Q-10-K)
 * Three in a Row (e.g. K-1-2, 3-4-5)

The player who slaps first takes all the cards from the pile.

If someone slaps when there is no valid combination, the other player takes all the cards.

The goal of the game is to have all 52 cards in your hand.
`


const app = express();
app.use(bodyParser.urlencoded({ extended: false }))

app.use(bodyParser.json())

// Use the whole root as static files to be able to serve the html file and
// the build folder
app.use(express.static(path.join(__dirname, '/'), {
    setHeaders: function (res, path) {
        if (path.match('.br')) {
            res.set('Content-Encoding', 'br');
            res.set('Content-Type', 'application/wasm');
        }
    }
}));


app.use((req, res, next) => {
    const secret = req.get('X-Telegram-Bot-Api-Secret-Token');

    if (process.env.SECRET_TOKEN !== secret) {
        return res.sendStatus(301);
    }

    next();
})


const bot = new Telegraf(process.env.BOT_TOKEN, {
    telegram: { webhookReply: true },
});


process.once('SIGINT', () => bot.stop('SIGINT'));
process.once('SIGTERM', () => bot.stop('SIGTERM'));

if (process.env.NODE_ENV === 'development') {

    const setupNgrok = async () => {
        await ngrok.authtoken(process.env.NGROK_AUTHTOKEN);
        const url = await ngrok.connect({ addr: process.env.PORT });
        console.log('url', url)
        bot.telegram.setWebhook(`${url}/${HOOK_PATH}`, {
            secret_token: process.env.SECRET_TOKEN,
            allowed_updates: ['message']
        })

        app.post(`/${HOOK_PATH}`, async (req, res) => {
            bot.handleUpdate(req.body, res);
        })

        bot.on(message('text'), async (ctx) => {
            console.log('#msg')
            await ctx.reply(rules, Markup.inlineKeyboard([{
                text: "🤟Let's play🤟!!!",
                web_app: {
                    url:url
                }
            }]));
        });
    }
    setupNgrok();
} else {
    bot.telegram.setWebhook(`${process.env.APP_ENDPOINT}/${HOOK_PATH}`, {
        secret_token: process.env.SECRET_TOKEN,
        allowed_updates: ['message']
    })

    app.post(`/${HOOK_PATH}`, async (req, res) => {
        bot.handleUpdate(req.body, res);
    })

    bot.on(message('text'), async (ctx) => {
        console.log('#msg')
        await ctx.reply(rules, Markup.inlineKeyboard([{
            text: "🤟Let's play🤟!!!",
            web_app: {
                url: process.env.APP_ENDPOINT
            }
        }]));
    });
}





app.listen(process.env.PORT, () => {
    console.log(`Server running at http://localhost:${process.env.PORT}/`);
});