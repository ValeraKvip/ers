using Unity.Netcode;
using UnityEngine;

public class NetworkUI : MonoBehaviour
{
    public bool IsSpawned => NetworkGameController.Singelton?.IsSpawned ?? false;

    public void OnNetworkSpawn()
    {
        if (!NetworkManager.Singleton.IsClient)
        {
            enabled = false;
            Destroy(this);
            return;
        }      
    }

    public virtual void OnGameFinished()
    {

    }
}
