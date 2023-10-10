using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
public class QrDisplay : MonoBehaviour
{
    public Image qrContainer;
    private string prevQr;
  
    public void ShowQr()
    {
        if (prevQr == LobbyController.Singelton.JoinCode)
        {
            qrContainer.gameObject.SetActive(true);
            return;
        }       
        Texture2D qr = generateQR(LobbyController.Singelton.JoinCode);

        qrContainer.sprite = Sprite.Create(qr, new Rect(0.0f, 0.0f, qr.width, qr.height), new Vector2(0.5f, 0.5f));
    }

    public void HideQr()
    {
        qrContainer.gameObject.SetActive(false);
    }


    public Texture2D generateQR(string text)
    {
        BarcodeWriter barcodeWriter = new BarcodeWriter();
        barcodeWriter.Format = BarcodeFormat.QR_CODE;
        EncodingOptions encodingOptions = new EncodingOptions()
        {
            Width = 256,
            Height = 256
        };
        barcodeWriter.Options = encodingOptions;

        Color32[] color32 = barcodeWriter.Write(text);
        Texture2D texture = new Texture2D(256, 256);
        texture.SetPixels32(color32);
        texture.Apply();

        return texture;
    }
}
