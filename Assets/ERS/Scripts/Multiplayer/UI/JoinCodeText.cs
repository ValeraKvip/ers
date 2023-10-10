public class JoinCodeText : NetworkUI
{
    void Update()
    {
        if (!IsSpawned)
        {
            return;
        }

        if(NetworkGameController.Singelton.gameData.player1.isInit &&
            NetworkGameController.Singelton.gameData.player2.isInit)
        {
            gameObject.SetActive(false);
        }
    }
}
