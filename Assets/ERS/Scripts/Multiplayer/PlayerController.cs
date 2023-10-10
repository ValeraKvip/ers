using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    private const string PLAYER_ID_KEY = "pik";
    public override void OnNetworkSpawn()
    {       
        base.OnNetworkSpawn();
      
        if (IsClient)
        {
            if (NetworkGameController.Singelton.IsSpawned)
            {
                Connect();
            }
            else
            {
                NetworkGameController.Singelton.OnNetworkSpawned.AddListener(Connect);
            }                                  
        }
    }

    private void Connect()
    {
        if (IsClient)
        {
          
            var guidStr = PlayerPrefs.GetString(PLAYER_ID_KEY);           
            if (string.IsNullOrEmpty(guidStr))
            {
                guidStr = Guid.NewGuid().ToString();            
                PlayerPrefs.SetString(PLAYER_ID_KEY, guidStr);
            }
          
            NetworkGameController.Singelton?.ConnectClientServerRpc(guidStr);
              TelegramConnect.RequestUserData();
        }
    }
}
