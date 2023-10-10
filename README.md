# Setup Unity Gaming Services(UGS)
1. Go to [UGS](https://dashboard.unity3d.com/gaming)
2. Create Account/Sign In.
3. Create a new project/Select an existing one.
4. Select **Multiplayer** in left menu.
5. Make sure the payment information is added.
6. Click **Lobby** and follow the instructions.
7. Click **Relay** -> **Open Relay** -> **Get Started**  and follow the instructions.
8. You have connected your unit project to the **UGS**

# Build
1. Launch Unity Editor.
2. `Ctrl+Shift+B`
3. Choose a **WebGL** platform. Install if not installed.
4. Click **build**.
5. Select the folder for the build: [server](/server)
6. Done!

![Build Settings](https://i.imgur.com/9l8o0y0.png)

# "Quick" build
1. `Ctrl+Shift+B`
2. Select **Code Optimization**: "Shorter Build Time"
3. Click **Player Settings...**
4. Other Settings -> **IL2CPP Code Generation** : "Faster runtime"
5. Publish Settings -> **Compression Format** -> "Disabled"
6. This should speed up the assembly a bit. But don't forget to put everything back when you do the production build.

# Create Telegram bot
1. Go to [BotFather](https://t.me/BotFather)
2. Type `/newbot`
3. Give a name
4. Give a username
5. Copy the token. Looks like this: `8162125942:DLFcAWwYQXICFIM2Vus3sGy_OLR3LtFARlE` This is a secret key, do not give it to anyone and do not publish it anywhere.
6. Go to the project root folder -> [server](/server).
7. Create&Open the **".env"**. Paste bot tokens like this:

```js
   BOT_TOKEN="YOUR_BOT_TOKEN"
   SECRET_TOKEN="bfaf9e93-1a0c-4627-9df6-f7c8c0673026" //any random string [a-z , A-Z , _ ,  - , 0-9]
```
![Create telegram bot](https://i.imgur.com/kzqaW71.png)

# Ngrok
Telegram bot requires an SSL connection to the server. Therefore, to connect your development environment, you need ngrok.
1. Go to [ngrok](https://ngrok.com/) and create account.
2. Go to [this](https://dashboard.ngrok.com/get-started/your-authtoken) page to get your auth token.
3. Go to the project root folder -> [server](/server).
4. Create or Open the **".env"**. Paste your auth tokens like this:
```js
  NGROK_AUTHTOKEN="YOUR_AUTH_TOKEN"
  NODE_ENV="development" 
```

# Run dev
1. Run [Build](https://github.com/ValeraKvip/ers/wiki/Guide#build) each time changes were made.
1. Go to the project root folder -> [server](/server) in terminal.
2. At the first launch:  `npm install`.
3. Run `node server.js`

## Test with Telegram
1. Check the Template:
   - `Ctrl+Shift+B`
   - Click `Player Settings...`
   - Expand `Resolution and Presentation` -> Under **WebGL Template** Select `prod`
2. Open your bot on any two Telegram clients.
3. Send anything to get an answer with a button -> click on the button -> the game will be launched (it may take time).
4. Click "Online"
5. Click "Play" - to automatically/randomly find an opponent (since there is only you in the test environment, your opponent will be your second open client). Important! First, click "Play" on one client, wait a few seconds, and then click "Play" on the other.
6. Click "Play With Friends" - To create rooms and a join code. 
   - If the second device does not have a camera, enter the code in the field and press `>`
   - If the second device has a camera, click "Scan QR" and scan the QR.
7. You can simulate a win by clicking the "Win" button in the game. 

## Test without Telegram
If you want to test without Telegram, you can open the game as a regular web page. But then you will need a **Telegram Web App** plug to enable it:
1. `Ctrl+Shift+B`
2. Click `Player Settings...`
3. Expand `Resolution and Presentation` -> Under **WebGL Template** Select `dev`
4. [Re-Build](https://github.com/ValeraKvip/ers/wiki/Guide#build)
5. [Start](https://github.com/ValeraKvip/ers/wiki/Guide#run-dev) the server -> links will be printed to the terminal -> click on them. Open in different browsers and test.

![WebGL Template selection](https://i.imgur.com/t7FzYcN.png)

# Deploy
1. Run [Build](https://github.com/ValeraKvip/ers/wiki/Guide#build)
2. The root directory of the project contains the [server](/server) folder - this is your node js server, which you can deploy to any hosting you like.

## Example Deploy to Railway 
1. Create an account on [Railway](https://railway.app?referralCode=EBtZTQ)
2. Create new github repo.
3. Go to [/server](/server) - Copy&Paste the folder somewhere outside the project (outside your .git) <- Place future builds in this folder.
4. Open the terminal in the copied folder.
5.  `git init`
6.  `git add .`
7.  `git commit -m "first"`
8.  `git branch -M main`
9.  `git remote add origin https://github.com/...your_created_git_repo`
10.  `git push -u origin main`
11.  Go to [Railway](https://railway.app/new?referralCode=EBtZTQ)
12.  Select **Deploy from Git Repo**
13.  Select **Configure GitHub App** and link the created project.
14.  The application will be deployed automatically with each git push.
15.  Select the app in the Railway dashboard. The menu opens. Select the **Variables** tab.
16.  Setup env:
   - BOT_TOKEN - your production telegram bot token
   - HOOK_PATH - any [random](https://guidgenerator.com/) string `a-z` `A-Z` `_`  `-` `0-9`
   - NODE_ENV - You can leave it blank or enter **production**
   - PORT - 433
   - SECRET_TOKEN - any [random](https://guidgenerator.com/) string `a-z` `A-Z` `_`  `-` `0-9`
   - APP_ENDPOINT - Url of your application. Click the **Settings** tab.<br>
     "Networking" -> "Public Networking" -> Copy/Paste. `https://nodejs-production-****.up.railway.app`

# Unity&Telegram Communication
Explore: [Assets/Plugins/plugin.jslib](Assets/Plugins/plugin.jslib)

This script is executed in the js environment in your browser, so it has access to **window** and telegram **WebApp** objects. The Unity script can call all the declared functions and pass primitive data (or complex data in the form of a json string). 

Explore: [Assets/ERS/Scripts/Telergam/TelegramConnect.cs](/Assets/ERS/Scripts/Telergam/TelegramConnect.cs)
This script declares the same functions as in [plugin.jslib](/Assets/Plugins/plugin.jslib)

```js
//Call in Unity c#
TelegramConnect.Hello();
//Will trigger functions in the plugin.jslib
Hello: function () {
  window.alert("Hello, world!");
},
//So the alert will be displayed in the browser
```
Explore: [Assets/WebGLTemplates/prod/index.html](/Assets/WebGLTemplates/prod/index.html)

`unityInstance.SendMessage("Object name", "Receiver function", "string or json string")`
From javascript, sends a string to the GameObject on the Unity scene that contains the script with the specified function.

```js
//Call in javascript
window.unityInstance.SendMessage("TelegramController", "SetWebAppUser", JSON.stringify(window.Telegram.WebApp.initDataUnsafe.user));
//Will trigger GameObject on the scene named "TelegramController" with the attached script "TelegramController.cs" that contains the function "SetWebAppUser"
public void SetWebAppUser(string data)
{
    var webAppUser = JsonUtility.FromJson<WebAppUser>(data);      
}
//So the user will be passed as a json string to the Unity script
```
Explore: [Assets/Scripts/Telegram](/Assets/ERS/Scripts/Telergam) directory - to better understand how it works.

# Troubleshooting
> Install failed: Validation Failed<br>
> Either Can't load WebGL or open the project or Unity at all.

It seems that every year, instead of improving the quality, Unity only increases the number of bugs.<br>
This solution bypasses the ~~Bug Hub~~ **Unity Hub**:<br>
1. If you haven't already, try to install an editor in the Hub (this will fail)
2. In **Unity Hub**, go to preferences -> installs. Find the **Downloads location**.
3. Open a File Explorer, and navigate to the specified **Downloads location**
4. In your File Explorer, there should be an installer(UnitySetup...) in the **Downloads location**.<br>
![UnitySetup...](https://i.imgur.com/BmVTwCZ.png)
5. Click to install.
6. Run Unity through the installed editor.


> Unable to parse Build/Hot xxx.framework.js.br!
> If using custom web server, verify that web server is sending .br files with HTTP Response 
> Header "Content-Encoding: br". 
> Brotli compression may not be supported over HTTP connections. Migrate your server to use HTTPS.

The server should set a header when it returns a compressed file:
```js
'Content-Encoding':'br'; // if Brotli compression used
'Content-Encoding': 'gzip'; // if Gzip compression used
'Content-Type': 'application/wasm';
```
[Check out the documentation](https://docs.unity3d.com/Manual/webgl-deploying.html)


> Invoking error handler due to
> RuntimeError: null function or function signature mismatch

Do not try to hang several scripts on one GameObject and call them - collect all the necessary functions in one script and hang only one script on the receiver(GameObject).

> ERR_NGROK_3200, ERR_NGROK_xxx

The server is not running, or the server has stopped due to an error. Restart the server.

> I launch the game on two devices, but the game does not start, I see the message "Waiting for players"

This is not a problem in the code, but in Unity services. Don't launch the game at the same time, wait until it starts on one device and then launch it on the other. In a real environment where you have thousands of players per minute looking for an opponent, this will not be a problem. 

> The size of the build is too large for web platforms. 

Oh, yes! Based on the build log, the Unity logo takes up the most space🤣🤣🤣, which can be removed by purchasing a Unity license.
