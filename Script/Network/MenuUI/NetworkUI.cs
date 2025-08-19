using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.Collections;

public class NetworkUI : MonoBehaviour
{
    [SerializeField] private Button serverBtn;
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;
    [SerializeField] private TMP_InputField inputName;
    [SerializeField] private Button startBtn;
    [SerializeField] private Button stopBtn;
    [SerializeField] private TMP_InputField inputIp;


    private void Awake()
    {
        serverBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
        });
        hostBtn.onClick.AddListener(StartHostWithname);
        
        clientBtn.onClick.AddListener(StartClientWithname);
        startBtn.onClick.AddListener(StartGame);
        stopBtn.onClick.AddListener(ClickDisconnect);
    }

    void StartHostWithname()
    {
        string playername = inputName.text;

        if (string.IsNullOrEmpty(playername))
        {
            Debug.LogWarning("�̸��� �Է��ϼ���");
            return;
        }
        
        NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.UTF8.GetBytes(playername);
        var ipaddress = inputIp.text;

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipaddress,7777);
        NetworkManager.Singleton.StartHost();
    }
    void StartClientWithname()
    {
        string playername = inputName.text;

        if (string.IsNullOrEmpty(playername))
        {
            Debug.LogWarning("�̸��� �Է��ϼ���");
            return;
        }

        NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.UTF8.GetBytes(playername);
        var ipaddress = inputIp.text;
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipaddress, 7777);
        NetworkManager.Singleton.StartClient();
    }

    void StartGame()
    {
        if (!NetworkManager.Singleton.IsConnectedClient)
        {
            Debug.Log("A");
            return;
        }
        NetworkManager.Singleton.SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
    }
    public void ClickDisconnect()
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

        
    }

  

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
