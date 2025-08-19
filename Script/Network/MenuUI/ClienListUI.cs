using Cysharp.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ClienListUI : MonoBehaviour
{
    public TextMeshProUGUI[] playerNameText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    async void OnClientConnected(ulong id)
    {
        await UniTask.Delay(100);
        UpdateName();
    }
    void OnClientDisconnected(ulong id)
    {
        UpdateName();
    }
    void UpdateName()
    {
        int i = 0;
        foreach(var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            var txt = client.PlayerObject.GetComponent<PlayerInfo>().name;
            playerNameText[i].text = txt.Value.ToString();
            i++;
        }
    }
}
