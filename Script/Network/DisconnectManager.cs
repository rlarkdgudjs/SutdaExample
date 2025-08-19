using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DisconnectManager : MonoBehaviour
{
    public void OnClickDisconnect()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            Debug.Log("호스트 종료");
            NetworkManager.Singleton.Shutdown();
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            Debug.Log("클라이언트 연결 종료");
            NetworkManager.Singleton.Shutdown();
        }

        SceneManager.LoadScene("MainMenu");
    }
}
