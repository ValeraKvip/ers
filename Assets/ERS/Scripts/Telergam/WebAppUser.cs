
using Unity.Netcode;

/**
* This object contains the data of the Mini App user.
* @see https://core.telegram.org/bots/webapps#webappuser
**/
[System.Serializable]
public class WebAppUser : INetworkSerializable
{
   public int id;
   public bool is_bot;
   public string first_name;
   public string last_name;
   public string username;
   public string language_code;
   public bool is_premium;
   public bool added_to_attachment_menu;
   public bool allows_write_to_pm;
   public string photo_url;


   public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
   {
      serializer.SerializeValue(ref id);
      serializer.SerializeValue(ref is_bot);
      serializer.SerializeValue(ref first_name);
      serializer.SerializeValue(ref last_name);
      serializer.SerializeValue(ref username);
      serializer.SerializeValue(ref language_code);
      serializer.SerializeValue(ref is_premium);
      serializer.SerializeValue(ref added_to_attachment_menu);
      serializer.SerializeValue(ref allows_write_to_pm);
      serializer.SerializeValue(ref photo_url);

   }
}