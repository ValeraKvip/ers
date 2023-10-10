using UnityEngine;
using System.Runtime.InteropServices;

public class TelegramConnect : MonoBehaviour
{

    [DllImport("__Internal")]
    public static extern void Hello();


    [DllImport("__Internal")]
    public static extern void RequestThemeParams();

    [DllImport("__Internal")]
    public static extern void RequestUserData();

    [DllImport("__Internal")]
    public static extern void ShowMainButton(string text);

    [DllImport("__Internal")]
    public static extern void HideMainButton();

    [DllImport("__Internal")]
    public static extern void MainButtonShowProgress();

    [DllImport("__Internal")]
    public static extern void MainButtonHideProgress();

    [DllImport("__Internal")]
    public static extern void ShowBackButton();

    [DllImport("__Internal")]
    public static extern void HideBackButton();

    [DllImport("__Internal")]
    public static extern void ShowAlert(string text);

    [DllImport("__Internal")]
    public static extern void ShowShareJoinCode(string code);

    [DllImport("__Internal")]
    public static extern void Ready();

    [DllImport("__Internal")]
    public static extern void Close();

    [DllImport("__Internal")]
    public static extern void Expand();

    [DllImport("__Internal")]
    public static extern void HapticFeedback(string level);

    [DllImport("__Internal")]
    public static extern void ShowScanQrPopup(string text);

    [DllImport("__Internal")]
    public static extern void CloseScanQrPopup();
}