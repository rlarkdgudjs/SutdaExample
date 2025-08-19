using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


// 클라이언트와 호스트간의 rpc 함수 및 
// 클라이언트측의 연출을 담당하는 매니저 입니다.
public class NetGameManager : NetworkBehaviour
{
    public static NetGameManager Instance { get; private set; }
    private List<CardStruct> myhand = new List<CardStruct>();
    private Dictionary<int, List<CardStruct>> otherhand = new Dictionary<int, List<CardStruct>>();

    public Button callbutton;
    public Button diebutton;
    public Button halfbutton;
    public Button allinbutton;
    private Image callbtnim;
    private Image diebtnim;
    private Image halfbtnim;
    private Image allinbtnim;

    private GameObject myobj;
    private PlayerInfo myinfo;

    public TextMeshProUGUI bank_text;
    private int bankmoney;
    public TextMeshProUGUI callmoney_text;
    private int callmoney;
    private int payedmoney;

    public GameObject[] cardpool;
    private CCard[] cardclass = new CCard[6];

    public GameObject[] playerseat;
    private TextMeshProUGUI[] playernameUI = new TextMeshProUGUI[3];
    private TextMeshProUGUI[] playermoneyUI = new TextMeshProUGUI[3];

    private Vector3[] cardpos =
    {
        new Vector3(-1.2f, -4f),
        new Vector3(-8f, -0.1f),
        new Vector3(6.7f, -0.1f),
        new Vector3(0.1f, -4f),
        new Vector3(-6.7f, -0.1f),
        new Vector3(8f, -0.1f)
    };
    private Vector3[] curpos =
    {
        new Vector3(2f, -3f, 0),
        new Vector3(-8f,2.2f,0),
        new Vector3(7f,2.2f,0)
    };

    public int myseat = 3;
    private int order = 3;

    public GameObject coinprefab;
    public GameObject coinboard;
    public GameObject cursor;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        callbtnim = callbutton.transform.GetComponent<Image>();
        diebtnim = diebutton.transform.GetComponent<Image>();
        halfbtnim = halfbutton.transform.GetComponent<Image>();
        allinbtnim = allinbutton.transform.GetComponent<Image>();

        int i = 0;

        foreach (var obj in cardpool)
        {
            var card = obj.GetComponent<CCard>();
            cardclass[i] = card;
            card.targetpos = cardpos[i];
            i++;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var temp = NetworkManager.Singleton.ConnectedClientsList;
        int i = 0;
        foreach (var obj in playerseat)
        {
            var temptrans = obj.transform.Find("Canvas");
            var nametext = temptrans.transform.Find("name").GetComponent<TextMeshProUGUI>();
            var moneytext = temptrans.transform.Find("money").GetComponent<TextMeshProUGUI>();
            playernameUI[i] = nametext;
            playermoneyUI[i] = moneytext;
            i++;
        }
        TextUpdate1();
        DisableBtn();

        ulong myId = NetworkManager.Singleton.LocalClientId;

        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(myId, out var client))
        {
            myobj = client.PlayerObject.gameObject;
            myinfo = myobj.GetComponent<PlayerInfo>();

            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    #region 버튼조절
    void EnableBtn()
    {
        callbutton.interactable = true;
        diebutton.interactable = true;
        callbtnim.color = new Color(255, 255, 255);
        diebtnim.color = new Color(255, 255, 255);
        if (myinfo.Money.Value < callmoney) return; 
        allinbutton.interactable = true;
        if (myinfo.Money.Value < callmoney + bankmoney / 2) return;
        halfbutton.interactable = true;
        halfbtnim.color = new Color(255, 255, 255);
        allinbtnim.color = new Color(255, 255, 255);
    }
    void DisableBtn()
    {
        callbutton.interactable = false;
        diebutton.interactable = false;
        halfbutton.interactable = false;
        allinbutton.interactable = false;
        callbtnim.color = new Color(200, 200, 200);
        diebtnim.color = new Color(200, 200, 200);
        halfbtnim.color = new Color(200, 200, 200);
        allinbtnim.color = new Color(200, 200, 200);

    }

    #endregion

    #region 카드무빙
    

    [ClientRpc]
    public void SpreadCard1ClientRpc(int playernum,int ordernum)
    {
        order = ordernum;
        SendSpreadCard1(playernum);
        
    }
    [ClientRpc]
    public void SpreadCard2ClientRpc(int ordernum, int remove)
    {
        order = ordernum;
        SendSpreadCard2(remove);

    }

    async void SendSpreadCard1(int playernum)
    {
        int[] readOrder = new int[6];

        // 앞 3개 왼쪽 회전
        for (int i = 0; i < 3; i++)
        {
            readOrder[i] = (i + order) % 3;
        }

        // 뒤 3개 왼쪽 회전
        for (int i = 0; i < 3; i++)
        {
            readOrder[i + 3] = ((i + order) % 3) + 3;
        }

        if (playernum == 2) {
            // order =0 => 0134
            // order =1 => 1043
            foreach (var num in readOrder)
            {
                if (num % 3 == 2)
                {
                    continue;
                }
                cardclass[num].GotoHand();
                await UniTask.Delay(300);
            }

        }
        else if (playernum == 3) {
            foreach (var num in readOrder)
            {
                cardclass[num].GotoHand();
                await UniTask.Delay(300);
            }
        }
    }
    async void SendSpreadCard2(int remove)
    {
        List<int> readOrder = new List<int>() { 0,0,0,0,0,0};

        // 앞 3개 왼쪽 회전
        for (int i = 0; i < 3; i++)
        {
            readOrder[i] = (i + order) % 3;
        }

        // 뒤 3개 왼쪽 회전
        for (int i = 0; i < 3; i++)
        {
            readOrder[i + 3] = ((i + order) % 3) + 3;
        }
        for (int i = 0; i < 6; i++)
        {
            if(i%3 == remove)
            {
                readOrder.Remove(i);
            }
        }
        foreach (var num in readOrder)
        {
            Debug.Log("Spread2");
            cardclass[num].GotoHand();
            await UniTask.Delay(300);
        }

    }

    [ClientRpc]

    public void FlipCardClientRpc(int num)
    {
        SendFlipCard(num);
    }

    void SendFlipCard(int num)
    {

    }

    async void FlipMyCard()
    {
        //myseat = 0 > 03
        //myseat = 1 > 14
        //myseat = 2 > 25
        await UniTask.Delay(1000);
        int j = 0;
        for (int i = 0; i < 6; i++)
        {
            if (i % 3 == myseat)
            {
                
                int imnum = myhand[j].img_num;
                cardclass[i].FlipCard(imnum);
                j++;
                await UniTask.Delay(1000);

            }
        }
    }
    #endregion

    #region UI관련
    [ClientRpc]
    public void TextUpdateClientRpc()
    {
        TextUpdate1();
    }

    void TextUpdate1()
    {
        var clientlists = NetworkManager.Singleton.ConnectedClientsList;

        for (int i = 0; i < 3; i++)
        {
            
            if (i >= clientlists.Count)
            {
                playernameUI[i].text = "";
                playermoneyUI[i].text = "";
                return;
            }
            if (clientlists[i].PlayerObject == null)
            {
                playernameUI[i].text = "";
                playermoneyUI[i].text = "";
                return;
            }
            var playerInfo = clientlists[i].PlayerObject.GetComponent<PlayerInfo>();

            playernameUI[i].text = $"name: {playerInfo.name.Value}";
            playermoneyUI[i].text = $"money: {playerInfo.Money.Value}";
        }
    }

    [ClientRpc]
    public void MoneyUpdateClientRpc()
    {
        MoneyUpdae();
    }
    public void MoneyUpdae()
    {
        var clientlists = NetworkManager.Singleton.ConnectedClientsList;
        for (int i = 0; i  < 3; i ++)
        {
            if (i >= clientlists.Count)
            {
                playermoneyUI[i].text = "";
            }
            var playerInfo = clientlists[i].PlayerObject.GetComponent<PlayerInfo>();
            playermoneyUI[i].text = $"money: {playerInfo.Money.Value}";
        }
        
    }
    [ClientRpc]
    public void BankUpdateClientRpc(int num,int callnum)
    {
        bankmoney = num;
        bank_text.text = $"POT : {bankmoney}";
        callmoney = callnum-payedmoney;
        if(callmoney > myinfo.Money.Value)
        {
            callmoney = myinfo.Money.Value;
        }
        callmoney_text.text = $"CALL : {callmoney}";
    }

    [ClientRpc]
    public void PayedMoneyClientRpc(int num,ClientRpcParams clientRpcParams)
    {
        payedmoney = num;
    }

    #endregion

    #region 효과
    [ClientRpc]
    public void CoinSpawnClientRpc(int num)
    {
        CoinEffect(num);
    }
    void CoinEffect(int num)
    {
        var coin = Instantiate(coinprefab, playerseat[num].transform.position, Quaternion.identity);
        coin.transform.SetParent(coinboard.transform);
    }

    [ClientRpc]
    public void CoinDestroyClientRpc(int num)
    {
        CoinDestroy(num);
    }
    void CoinDestroy(int num)
    {
        foreach(Transform child in coinboard.transform)
        {
            cursor.transform.position = curpos[num];
            child.DOMove(playerseat[num].transform.position, 0.2f).OnComplete(() =>
            {
                Destroy(child.gameObject);
            });
        }
    }
    [ClientRpc]
    public void CursorMoveClientRpc(int num)
    {
        CursorMove(num);
    }
    void CursorMove(int num)
    {
        if(!cursor.activeInHierarchy)
        {
            cursor.SetActive(true);
        }
        cursor.transform.position = curpos[num];
    }
    #endregion

   

    [ClientRpc]
    public void ReCallCardClientRpc()
    {
        ReCallCard();
    }
    async void ReCallCard()
    {
        foreach (var card in cardclass)
        {
            card.GotoDeck();
            await UniTask.Delay(100);
        }
    }
    [ClientRpc]
    void DevClientRpc()
    {
        Debug.Log("DevClientRpc");
    }

    [ClientRpc]
    public void GetOtherHandClientRpc(int num, CardStruct card1, CardStruct card2)
    {
        GetOtherHand(num,card1,card2);
    }
    void GetOtherHand(int num, CardStruct card1, CardStruct card2)
    {
        FlipOther(num, card1, card2);
    }
    void FlipOther(int num,CardStruct card1,CardStruct card2)
    {
        if(num == myseat)
        {
            return;
        }
        int imnum1 = card1.img_num;
        int imnum2 = card2.img_num;
        
        if (num == 0)
        {
            cardclass[0].FlipCard(imnum1);
            cardclass[3].FlipCard(imnum2);
        }
        else if (num == 1)
        {
            cardclass[1].FlipCard(imnum1);
            cardclass[4].FlipCard(imnum2);
        }
        else if (num == 2)
        {
            cardclass[2].FlipCard(imnum1);
            cardclass[5].FlipCard(imnum2);
        }
     


    }

    [ClientRpc]
    public void GiveCardClientRpc(int num,CardStruct card1, CardStruct card2, ClientRpcParams clientRpcParams)
    {
        
        myseat = num;
        myhand.Clear();
        myhand.Add(card1);
        myhand.Add(card2);
        Debug.Log($"{card1.month}:{card1.cardtype} , {card2.month}:{card2.cardtype}");
        FlipMyCard();

    }
    //public void SendBettingServer()
    //{
    //    if (IsOwner)
    //    {
    //        SendBettingServerRpc();
    //    }
    //}
    [ServerRpc(RequireOwnership = false)]
    void SendBettingServerRpc(int num)
    {
        GameManager.Instance.GetBetInfo(num);
    }

    async void CallBettingClient()
    {
     
        EnableBtn();
        var callTask = callbutton.OnClickAsync();
        var dieTask = diebutton.OnClickAsync();
        var halfTask = halfbutton.OnClickAsync();
        var allinTask = allinbutton.OnClickAsync();
        var timeoutTask = UniTask.Delay(12000);
        var completedTask = await UniTask.WhenAny(callTask,halfTask,allinTask ,dieTask, timeoutTask);
        if (completedTask == 0)
        {
            Debug.Log("Call button clicked");
            SendBettingServerRpc(0);

            // CallBettingServerRpc();
        }
        else if (completedTask == 1)
        {
            Debug.Log("half button clicked");
            SendBettingServerRpc(1);

            // CallBettingServerRpc();
        }
        else if (completedTask == 2)
        {
            Debug.Log("Allin button clicked");
            SendBettingServerRpc(2);
        }
        else if (completedTask == 3)
        {
            Debug.Log("DIe button clicked");
            SendBettingServerRpc(3);
            // CallBettingServerRpc();
        }
        else if (completedTask == 4)
        {
            Debug.Log("Timeout");
            SendBettingServerRpc(3);
            // CallBettingServerRpc();
        }
        DisableBtn();
        

    }
    [ClientRpc]
    public void CallBettingClientRpc(ClientRpcParams clientRpcParams)
    {
        
        CallBettingClient();
    }
}
