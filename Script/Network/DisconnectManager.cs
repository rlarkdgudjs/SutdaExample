using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DisconnectManager : MonoBehaviour
{
    public void OnClickDisconnect()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            Debug.Log("ȣ��Ʈ ����");
            NetworkManager.Singleton.Shutdown();
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            Debug.Log("Ŭ���̾�Ʈ ���� ����");
            NetworkManager.Singleton.Shutdown();
        }

        SceneManager.LoadScene("MainMenu");
    }
}
