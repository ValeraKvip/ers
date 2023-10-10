using Unity.Netcode;
using UnityEngine;


public class TelegramController : ThemeController
{
    private void Awake()
    {
        TelegramConnect.CloseScanQrPopup();
    }
    private void Start() {
         TelegramConnect.RequestThemeParams();
    }

    public void SetWebAppUser(string data)
    {
        var webAppUser = JsonUtility.FromJson<WebAppUser>(data);
        NetworkGameController.Singelton.UpdatePlayerInfoServerRpc(webAppUser);
    }


    public void OnCombinationDetected(NetworkString combination)
    {
        TelegramConnect.HapticFeedback("success");

        var LocalClientId = NetworkManager.Singleton.LocalClientId;

        var data = NetworkGameController.Singelton.gameData;
        TelegramConnect.HapticFeedback("success");

    }
}
