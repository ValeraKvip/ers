using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class TelegramLobbyController : MonoBehaviour
{
    public Image[] backgrounds;
    public Button[] buttons;
    public TMP_InputField[] inputs;
    public TMP_Text[] infos;
    // Start is called before the first frame update
    public void SetThemeParams(string data)
    {       
        var themeParams = JsonUtility.FromJson<ThemeParams>(data);

        if (themeParams != null)
        {            
            if (ColorUtility.TryParseHtmlString(themeParams.bg_color, out Color bgColor))
            {              
                foreach (var background in backgrounds)
                {
                    background.color = bgColor;
                }
            }           

            if (ColorUtility.TryParseHtmlString(themeParams.button_color, out Color btnColor)
            && ColorUtility.TryParseHtmlString(themeParams.button_text_color, out Color btnTextColor))
            {
                foreach (var btn in buttons)
                {
                    btn.image.color = btnColor;
                    btn.GetComponentInChildren<TMP_Text>().color = btnTextColor;
                }
            }

            if (ColorUtility.TryParseHtmlString(themeParams.secondary_bg_color, out Color secondaryBgColor)
            && ColorUtility.TryParseHtmlString(themeParams.text_color, out Color textColor))
            {
                foreach (var input in inputs)
                {
                    input.image.color = secondaryBgColor;
                    input.GetComponentInChildren<TMP_Text>().color = textColor;
                }

                foreach (var info in infos)
                {
                    info.color = textColor;
                }
            }

        }
    }

    public void SetWebAppUser(string data)
    {

    }

    public virtual void OnQrDetected(string qrString)
    {       
        LobbyController.Singelton.JoinLobby(qrString);
    }

    // Update is called once per frame
    private void Start()
    {       
        TelegramConnect.Ready();
        TelegramConnect.Expand();
        TelegramConnect.RequestThemeParams();
    }

    public void ShowScanQr()
    {
        TelegramConnect.ShowScanQrPopup("Scan Qr to join the game");
    }
}
