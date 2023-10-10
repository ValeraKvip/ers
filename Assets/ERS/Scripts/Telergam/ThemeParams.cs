
using Unity.Netcode;

/**
* Mini Apps can adjust the appearance of the interface to match the Telegram user's app in real time.
 This object contains the user's current theme settings.
* @see https://core.telegram.org/bots/webapps#themeparams
**/
[System.Serializable]
public class ThemeParams : INetworkSerializable
{


    public string bg_color;
    public string text_color;
    public string hint_color;
    public string link_color;
    public string button_color;
    public string button_text_color;
    public string secondary_bg_color;



    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref bg_color);
        serializer.SerializeValue(ref text_color);
        serializer.SerializeValue(ref hint_color);
        serializer.SerializeValue(ref link_color);
        serializer.SerializeValue(ref button_color);
        serializer.SerializeValue(ref button_text_color);
        serializer.SerializeValue(ref secondary_bg_color);
    }
}