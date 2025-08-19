using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

// ȣ��Ʈ�� ������ �����ϴ� �Ŵ��� �Դϴ�.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int callmoney;
    public int ante;

    private List<CardStruct> card_stack = new List<CardStruct>();
    private Dictionary<NetworkClient, List<CardStruct>> playerCardDict = new Dictionary<NetworkClient, List<CardStruct>>(); // ���ӿ� �����ϴ� �÷��̾� �ڵ� ��
    private List<NetworkClient> playerList; // ���� ��ȿ� �÷��̾� ���
    private Queue<(NetworkClient,int)> playerQueue = new Queue<(NetworkClient, int)>();

    public TextMeshProUGUI Banktext;
    public int Bankint = 0;

    private int order = 0;

    public List<int> TestCardorder = new List<int>();
    #region ���� paircardstruct ver
    //Dictionary<PairCardStruct, int> score_hard = new Dictionary<PairCardStruct, int>
    //{
    //    {new PairCardStruct((3,Cardtype.Gwang),(8,Cardtype.Gwang)),29},
    //    {new PairCardStruct((1,Cardtype.Gwang),(8,Cardtype.Gwang)),28},
    //    {new PairCardStruct((1,Cardtype.Gwang),(3,Cardtype.Gwang)),27},
    //    {new PairCardStruct((10,Cardtype.Normal),(10,Cardtype.Normal)),26},
    //    {new PairCardStruct((9,Cardtype.Special),(9,Cardtype.Normal)),25},
    //    {new PairCardStruct((8,Cardtype.Gwang),(8,Cardtype.Normal)),24},
    //    {new PairCardStruct((7,Cardtype.Special),(7,Cardtype.Normal)),23},
    //    {new PairCardStruct((6,Cardtype.Normal),(6,Cardtype.Normal)),22},
    //    {new PairCardStruct((5,Cardtype.Normal),(5,Cardtype.Normal)),21},
    //    {new PairCardStruct((4,Cardtype.Special),(4,Cardtype.Normal)),20},
    //    {new PairCardStruct((3,Cardtype.Gwang),(3,Cardtype.Normal)),19},
    //    {new PairCardStruct((2,Cardtype.Normal),(2,Cardtype.Normal)),18},
    //    {new PairCardStruct((1,Cardtype.Gwang),(1,Cardtype.Normal)),17},
    //    {new PairCardStruct((1,Cardtype.Gwang),(2,Cardtype.Normal)),16},
    //    {new PairCardStruct((1,Cardtype.Normal),(2,Cardtype.Normal)),16},
    //    {new PairCardStruct((1,Cardtype.Gwang),(4,Cardtype.Normal)),15},
    //    {new PairCardStruct((1,Cardtype.Gwang),(4,Cardtype.Special)),15},
    //    {new PairCardStruct((1,Cardtype.Normal),(4,Cardtype.Normal)),15},
    //    {new PairCardStruct((1,Cardtype.Normal),(4,Cardtype.Special)),15},
    //    {new PairCardStruct((1,Cardtype.Gwang),(9,Cardtype.Normal)),14},
    //    {new PairCardStruct((1,Cardtype.Gwang),(9,Cardtype.Special)),14},
    //    {new PairCardStruct((1,Cardtype.Normal),(9,Cardtype.Normal)),14},
    //    {new PairCardStruct((1,Cardtype.Normal),(9,Cardtype.Special)),14},
    //    {new PairCardStruct((1,Cardtype.Gwang),(10,Cardtype.Normal)),13},
    //    {new PairCardStruct((1,Cardtype.Normal),(10,Cardtype.Normal)),13},
    //    {new PairCardStruct((4,Cardtype.Normal),(10,Cardtype.Normal)),12},
    //    {new PairCardStruct((4,Cardtype.Special),(10,Cardtype.Normal)),12},
    //    {new PairCardStruct((4,Cardtype.Normal),(6,Cardtype.Normal)),11},
    //    {new PairCardStruct((4,Cardtype.Special),(6,Cardtype.Normal)),11}
    //};
    #endregion

    #region ����
    Dictionary<(int, int), int> scorelist_Gwang = new Dictionary<(int, int), int>
    {
        { (3,8),29 },
        { (1,8),28 },
        { (1,3),27 },
        { (8,3),29 },
        { (8,1),28 },
        { (3,1),27 }
    };
    Dictionary<(int, int), int> scorelist_Combi = new Dictionary<(int, int), int>
    {
        {(1,2),16 },
        {(1,4),15 },
        {(1,9),14 },
        {(1,10),13 },
        {(4,10),12 },
        {(4,6),11 },
        {(2,1),16 },
        {(4,1),15 },
        {(9,1),14 },
        {(10,1),13 },
        {(10,4),12 },
        {(6,4),11 }
    };
    Dictionary<(int, int), int> scorelist_Catch = new Dictionary<(int, int), int>
    {
        {(4,9),-3 },
        {(3,7),-1 },
        {(4,7),-2 },
        {(9,4),-3 },
        {(7,3),-1 },
        {(7,4),-2 }
    };

    
    #endregion
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is create

    #region ��Ʈ��ũ
    async void OnClientConnected(ulong clientid)
    {
        await UniTask.Delay(100); 
        Debug.Log($"Client {clientid} connected.");
        NetGameManager.Instance.TextUpdateClientRpc();
    }

    void OnClientDisconnected(ulong clientid)
    {
        Debug.Log($"Client {clientid} disconnected.");
        NetGameManager.Instance.TextUpdateClientRpc();
    }

    public void GoMenu()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
            Destroy(NetworkManager.Singleton.gameObject); // NetworkManager ������Ʈ ��������� ����
        }

        SceneManager.LoadScene("Main");
    }
    #endregion
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)& NetworkManager.Singleton.IsHost)
        {
            {
                GameStart();
            }
        }
        if(Input.GetKeyDown(KeyCode.T) & NetworkManager.Singleton.IsHost)
        {
            TestStart();
        }
    }

    async void TestStart()
    {
        playerList = NetworkManager.Singleton.ConnectedClientsList.ToList();
        NetGameManager.Instance.ReCallCardClientRpc();
        await UniTask.Delay(700); // 1�� ���
        MakePlayerList();
        TestCard();
        await WaitForBetting();
        DecisionWin();
        
        NetGameManager.Instance.TextUpdateClientRpc();
    }
    void TestCard()
    {
        if (card_stack != null)
        {
            InitCardStack();
        }
        MakeCardStack();
        int player_num = playerCardDict.Count;

        int j = 0;
        for (int i = 0; i < 2; i++)
        {

            foreach (var player in playerCardDict)
            {
                playerCardDict[player.Key].Add(card_stack[TestCardorder[j]]);

                j++;
                string cardinfo = string.Join(", ",
                playerCardDict[player.Key].Select(card => $"({card.month}, {card.cardtype})"));
                Debug.Log($"Player {player.Key.ClientId}: {cardinfo}");
            }
        }
        foreach (var value in playerCardDict)
        {
            var i = playerList.IndexOf(value.Key);
            var card = value.Value;

            NetGameManager.Instance.GiveCardClientRpc(i, card[0], card[1], new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new List<ulong> { value.Key.ClientId }
                }
            });

        }

    }

    #region ��������
    async void GameStart()
    {
        playerList = NetworkManager.Singleton.ConnectedClientsList.ToList();
        NetGameManager.Instance.ReCallCardClientRpc();
        await UniTask.Delay(800); 
        GetAnte(); //�ǵ��ޱ�
        //���ǽ��� + �ǵ��ޱ�
        MakePlayerList();
        CardSpread();
        await WaitForBetting();
        DecisionWin();
        
        await UniTask.Delay(100); 
        NetGameManager.Instance.TextUpdateClientRpc();
        NetGameManager.Instance.BankUpdateClientRpc(Bankint, callmoney);
        
    }

    async void GameRestart(List<NetworkClient> playerlist)
    {
        await UniTask.Delay(3000);
        playerList = NetworkManager.Singleton.ConnectedClientsList.ToList();
        NetGameManager.Instance.ReCallCardClientRpc();
        await UniTask.Delay(800);
        //���� �˶� + �ǵ���ġ�� + �����ڹޱ�
        MakeRePlayerList(playerlist);
        CardSpread();
        await WaitForBetting();
        DecisionWin();
        
        await UniTask.Delay(100);
        NetGameManager.Instance.TextUpdateClientRpc();
        NetGameManager.Instance.BankUpdateClientRpc(Bankint, callmoney);
        
    }

    void GameEnd()
    {
        InitCardStack();
        InitPlayerHand();
    }


    #endregion

    #region ���÷���
    private int betinfo = 3;
    private bool isNext = false;

    // ����ť�� �̿��� �����ο��� ���������� �����Ѵ�
    async UniTask WaitForBetting()
    {
        Queue<(NetworkClient, int)> waitPlayerQueue = new Queue<(NetworkClient, int)>();
        int foldCount = 0;
        foreach (var player in playerCardDict)
        {
            playerQueue.Enqueue((player.Key, 0));
        }
        int initPlayerCount = playerQueue.Count;

        for (int i = 0; i < 3; i++)
        {
            if (playerQueue.Count == 0)
                return;

            var (player, playerPayedMoney) = playerQueue.Dequeue();
            var playerInfo = player.PlayerObject.GetComponent<PlayerInfo>();
            if (playerInfo == null)
            {
                Debug.LogWarning($"PlayerInfo�� ã�� �� �����ϴ�. ClientId: {player.ClientId}");
                continue;
            }
            if (foldCount == initPlayerCount - 1)
                return;

            int playerPos = playerList.IndexOf(player);
            NetGameManager.Instance.CallBettingClientRpc(new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new List<ulong> { player.ClientId }
                }
            });
            NetGameManager.Instance.CursorMoveClientRpc(playerPos);
            Debug.Log($"�÷��̾� {player.ClientId}�� ������ ��ٸ��ϴ�.");

            await UniTask.WaitUntil(() => isNext);

            int payMoney = 0;
            switch (betinfo)
            {
                case 0: // Call
                    payMoney = callmoney - playerPayedMoney;
                    if (playerInfo.Money.Value < payMoney)
                    {
                        Debug.Log($"�÷��̾� {player.ClientId}�� �ݱݾ׺��� ���� �ݾ��� �����ϰ� �ֽ��ϴ� ��:{playerInfo.Money.Value}.");
                        payMoney = playerInfo.Money.Value;
                    }
                    PlayCall(player, payMoney);
                    waitPlayerQueue.Enqueue((player, playerPayedMoney + payMoney));
                    NetGameManager.Instance.CoinSpawnClientRpc(playerPos);
                    NetGameManager.Instance.PayedMoneyClientRpc(payMoney, new ClientRpcParams
                    {
                        Send = new ClientRpcSendParams
                        {
                            TargetClientIds = new List<ulong> { player.ClientId }
                        }
                    });
                    Debug.Log($"�÷��̾� {player.ClientId}�� {callmoney}��ŭ ���߽��ϴ�.");
                    break;

                case 1: // Half
                    int tempCall = callmoney;
                    callmoney += (Bankint / 2);
                    payMoney = callmoney - playerPayedMoney;
                    if (playerInfo.Money.Value < payMoney)
                    {
                        Debug.Log($"�÷��̾� {player.ClientId}�� half��ݾ׺��� ���� �ݾ��� �����ϰ� �ֽ��ϴ� ��:{playerInfo.Money.Value}.");
                        payMoney = playerInfo.Money.Value;
                        callmoney = tempCall + payMoney;
                    }
                    PlayCall(player, payMoney);
                    NetGameManager.Instance.CoinSpawnClientRpc(playerPos);
                    NetGameManager.Instance.PayedMoneyClientRpc(payMoney, new ClientRpcParams
                    {
                        Send = new ClientRpcSendParams
                        {
                            TargetClientIds = new List<ulong> { player.ClientId }
                        }
                    });
                    foreach (var value in waitPlayerQueue)
                        playerQueue.Enqueue(value);
                    waitPlayerQueue.Clear();
                    waitPlayerQueue.Enqueue((player, playerPayedMoney + payMoney));
                    i = 0;
                    initPlayerCount = playerQueue.Count + waitPlayerQueue.Count;
                    Debug.Log($"�÷��̾� {player.ClientId}�� �ݱݾ� : {tempCall} + ��1/2 : {Bankint / 2}��ŭ Half���߽��ϴ�.");
                    break;

                case 2: // All-in
                    if (playerInfo.Money.Value > callmoney)
                        callmoney = playerInfo.Money.Value + playerPayedMoney;
                    payMoney = playerInfo.Money.Value;
                    PlayCall(player, payMoney);
                    NetGameManager.Instance.CoinSpawnClientRpc(playerPos);
                    NetGameManager.Instance.PayedMoneyClientRpc(payMoney, new ClientRpcParams
                    {
                        Send = new ClientRpcSendParams
                        {
                            TargetClientIds = new List<ulong> { player.ClientId }
                        }
                    });
                    foreach (var value in waitPlayerQueue)
                        playerQueue.Enqueue(value);
                    waitPlayerQueue.Clear();
                    waitPlayerQueue.Enqueue((player, playerPayedMoney + payMoney));
                    i = 0;
                    initPlayerCount = playerQueue.Count + waitPlayerQueue.Count;
                    Debug.Log($"�÷��̾� {player.ClientId}�� {payMoney} ��ŭ ALLin�߽��ϴ�.");
                    break;

                case 3: // Fold
                    PlayDie(player);
                    foldCount++;
                    Debug.Log($"�÷��̾� {player.ClientId}�� �����߽��ϴ�.");
                    break;
            }

            isNext = false;
            betinfo = 3;

            Banktext.text = $"Total : {Bankint}";
            await UniTask.Delay(100);
            NetGameManager.Instance.BankUpdateClientRpc(Bankint, callmoney);
            NetGameManager.Instance.MoneyUpdateClientRpc();
        }

        NetGameManager.Instance.PayedMoneyClientRpc(0, new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = NetworkManager.Singleton.ConnectedClientsIds
            }
        });

        Debug.Log("��� �÷��̾��� ������ �Ϸ�Ǿ����ϴ�.");
    }
    void GetAnte()
    {
        int i = 0;
        foreach (var player in playerList)
        {
            var playerInfo = player.PlayerObject.GetComponent<PlayerInfo>();
            playerInfo.ChangeMoney(-ante);
            Bankint += ante;
            NetGameManager.Instance.CoinSpawnClientRpc(i);
            i++;
        }
        callmoney = ante;
        NetGameManager.Instance.MoneyUpdateClientRpc();
        Banktext.text = $"Total : {Bankint}";
        NetGameManager.Instance.BankUpdateClientRpc(Bankint,callmoney);
    }
    void PlayCall(NetworkClient player,int money)
    {
        player.PlayerObject.GetComponent<PlayerInfo>().ChangeMoney(-money);
        Bankint += money;
    }
    void PlayDie(NetworkClient player)
    {
        playerCardDict.Remove(player);
    }
    void EarnMoney(NetworkClient player, int money)
    {
        player.PlayerObject.GetComponent<PlayerInfo>().ChangeMoney(money);
        Bankint = 0;
    }

    public void GetBetInfo(int num)
    {
        betinfo = num;
        isNext = true;
    }
    #endregion

    #region �÷��̾� �� ����

    
    void MakePlayerList()
    {
        var list = LeftRotate<NetworkClient>(playerList, order);
        
        if (playerCardDict != null)
        {
            playerCardDict.Clear();
        }
        foreach (var client in list)
        {
            var playerID = client.ClientId;
            if (!playerCardDict.ContainsKey(client))
            {
                playerCardDict.Add(client, new List<CardStruct>());
            }
            else
            {
                playerCardDict[client].Clear();
            }
        }
        NetGameManager.Instance.SpreadCard1ClientRpc(playerCardDict.Count, order);
        order++;
        if (order > list.Count)
        {
            order = 0;
        }

    }

    void MakeRePlayerList(List<NetworkClient> list)
    {
        int num1, num2;
        num1 = playerList.IndexOf(list[0]);
        var temp = playerList.Except(list).ToList();
        

        if (playerCardDict != null)
        {
            playerCardDict.Clear();
        }

        foreach (var client in list)
        {
            var playerID = client;
            if (!playerCardDict.ContainsKey(client))
            {
                playerCardDict.Add(client, new List<CardStruct>());
            }
            else
            {
                playerCardDict[client].Clear();
            }
        }
        if (temp.Count == 0)
        {
            Debug.Log("MakeReCount0");
            NetGameManager.Instance.SpreadCard1ClientRpc(playerCardDict.Count, num1);
            return;
        }
        num2 = playerList.IndexOf(temp[0]);
        Debug.Log("MakeReCount1");
        NetGameManager.Instance.SpreadCard2ClientRpc(num1,num2);


    }

    void InitPlayerHand()
    {
        playerCardDict.Clear();
    }
    #endregion

    #region �е�����
    void CardSpread()
    {
        if (card_stack != null)
        {
            InitCardStack();
        }
        MakeCardStack();
        ShuffleCardStack();
        DecisionCard();
        SendCard();
        
    }
    void MakeCardStack()
    {
        var cardTypes = new (Cardtype, Cardtype)[]
        {
                (Cardtype.Gwang, Cardtype.Normal),
                (Cardtype.Normal, Cardtype.Normal),
                (Cardtype.Gwang, Cardtype.Normal),
                (Cardtype.Special, Cardtype.Normal),
                (Cardtype.Normal, Cardtype.Normal),
                (Cardtype.Normal, Cardtype.Normal),
                (Cardtype.Special, Cardtype.Normal),
                (Cardtype.Gwang, Cardtype.Normal),
                (Cardtype.Special, Cardtype.Normal),
                (Cardtype.Normal, Cardtype.Normal),
        };

        for (int i = 0; i < cardTypes.Length; i++)
        {
            card_stack.Add(new CardStruct(i + 1, cardTypes[i].Item1, i + i));
            card_stack.Add(new CardStruct(i + 1, cardTypes[i].Item2, i + i + 1));
        }

    }
    void ShuffleCardStack()
    {
        for (int i = card_stack.Count - 1; i > 0; i--)
        {
            int k = UnityEngine.Random.Range(0, i + 1);
            CardStruct temp = card_stack[i];
            card_stack[i] = card_stack[k];
            card_stack[k] = temp;
        }
    }
    // ��ü �ڵ� ������ ȣ��Ʈ�� �˰�����
    void DecisionCard()
    {
        int player_num = playerCardDict.Count;
        int j = 0;

        for (int i = 0; i < 2; i++)
        {

            foreach (var player in playerCardDict)
            {
                playerCardDict[player.Key].Add(card_stack[j]);
                j++;

                string cardinfo = string.Join(", ",
                playerCardDict[player.Key].Select(card => $"({card.month}, {card.cardtype})"));
                Debug.Log($"Player {player.Key.ClientId}: {cardinfo}");
            }
        }

    }
    // �� Ŭ���̾�Ʈ���� ����ī�� ���� ����
    void SendCard()
    {
        
        foreach (var value in playerCardDict)
        {
            var i = playerList.IndexOf(value.Key);
            var card = value.Value;
            
            NetGameManager.Instance.GiveCardClientRpc(i,card[0], card[1], new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new List<ulong> { value.Key.ClientId }
                }
            });
            
        }
    }
    void InitCardStack()
    {
        card_stack.Clear();
    }

    
    #endregion

    #region ���а���
    // �÷��̾� ī���� ������ ����� Ư�������� ������� ���� �� ����,���� ����
    void DecisionWin()
    {
        bool isRestart = false;
        var playerScoreDict = new Dictionary<NetworkClient, int>();
        if(playerCardDict.Count == 0)
        {
            Debug.Log("�÷��̾ �����ϴ�.");
            return;
        }
        if (playerCardDict.Count == 1)
        {
            var player = playerCardDict.FirstOrDefault();
            EarnMoney(player.Key, Bankint);
            int num = playerList.IndexOf(player.Key);
            NetGameManager.Instance.CoinDestroyClientRpc(num);
            Debug.Log($"��� �����Ͽ� Player {player.Key.ClientId} wins!");
            return;
        }
        foreach (var player in playerCardDict)
        {
            playerScoreDict.Add(player.Key, ScoreCalcul(player.Value));
            Debug.Log(ScoreCalcul(player.Value));
        }
        int maxValue = playerScoreDict.Values.Max();
        List<NetworkClient> resultClientList = new List<NetworkClient>();
        //-4�۱��� -3���� -2������ -1������
        // ������ �Ǵ�
        if (TryGetKeysByValue<NetworkClient, int>(playerScoreDict, -1, out resultClientList))
        {
            if (16 < maxValue && maxValue < 26)
            {
                playerScoreDict[resultClientList[0]] = 26;
            }
            else playerScoreDict[resultClientList[0]] = 0;

        }
        // ������ �Ǵ�
        else if (TryGetKeysByValue<NetworkClient, int>(playerScoreDict, -2, out resultClientList))
        {
            if (26 < maxValue && maxValue < 29)
            {
                playerScoreDict[resultClientList[0]] = 29;
            }
            else playerScoreDict[resultClientList[0]] = 1;
        }
        // �۱��� ����
        else if (TryGetKeysByValue<NetworkClient, int>(playerScoreDict, -4, out resultClientList))
        {
            if (maxValue < 26)
            {
                isRestart = true;
                var teli = playerScoreDict.Keys.ToList();
                var rotated = RotateToAnchor(teli, resultClientList[0]);
                foreach (var player in playerScoreDict)
                {
                    var i = playerList.IndexOf(player.Key);
                    var card = playerCardDict[player.Key];
                    NetGameManager.Instance.GetOtherHandClientRpc(i, card[0], card[1]);
                }
                GameRestart(rotated);
                return;

            }
            else playerScoreDict[resultClientList[0]] = 3;
        }
        // ���� ����
        else if (TryGetKeysByValue<NetworkClient, int>(playerScoreDict, -3, out resultClientList))
        {
            if (maxValue < 17)
            {
                isRestart = true;
                var teli = playerScoreDict.Keys.ToList();
                var rotated = RotateToAnchor(teli, resultClientList[0]);
                foreach (var player in playerScoreDict)
                {
                    var i = playerList.IndexOf(player.Key);
                    var card = playerCardDict[player.Key];
                    NetGameManager.Instance.GetOtherHandClientRpc(i, card[0], card[1]);
                }
                GameRestart(rotated);
                
                return;

            }
            else playerScoreDict[resultClientList[0]] = 3;
        }
        // ������ ����
        else if (TryGetTieKeysByValue<NetworkClient, int>(playerScoreDict, maxValue, out resultClientList))
        {
            isRestart = true;
            foreach (var player in playerScoreDict)
            {
                var i = playerList.IndexOf(player.Key);
                var card = playerCardDict[player.Key];
                NetGameManager.Instance.GetOtherHandClientRpc(i, card[0], card[1]);
            }
            GameRestart(resultClientList);
            return;
        }

        // ���� ����
        maxValue = playerScoreDict.Values.Max();

        // palyerCardDict������� �а����ϱ�
        foreach (var player in playerScoreDict)
        {
            var i = playerList.IndexOf(player.Key);
            var card = playerCardDict[player.Key];
            NetGameManager.Instance.GetOtherHandClientRpc(i, card[0], card[1]);
        }
       

        var winner = playerScoreDict.First(kv => kv.Value == maxValue).Key;
        EarnMoney(winner, Bankint);

        Debug.Log($"Player(s) win: {string.Join(", ", winner.ClientId)}");

        int winnerPos = playerList.IndexOf(winner);
        NetGameManager.Instance.CoinDestroyClientRpc(winnerPos);

    }

    int ScoreCalcul(List<CardStruct> hand)
    {
        int score = (hand[0].month + hand[1].month) % 10;
        int temp;
        // ���� ����� �ϳ��� üũ
        if (scorelist_Combi.TryGetValue((hand[0].month, hand[1].month), out temp))
        {
            score = temp;
            return score;
        }
        else if (hand[0].month == hand[1].month)
        {
            score = 16 + hand[0].month;
            return score;
        }
        else if (scorelist_Gwang.TryGetValue((hand[0].month, hand[1].month), out temp))
        {
            if (hand[0].cardtype == hand[1].cardtype && hand[0].cardtype == Cardtype.Gwang)
            {
                score = temp;
                return score;
            }
        }
        else if (scorelist_Catch.TryGetValue((hand[0].month, hand[1].month), out temp))
        {
            if (temp == -2)
            {
                if (hand[0].cardtype == hand[1].cardtype && hand[0].cardtype == Cardtype.Special)
                {
                    score = temp;
                    return score;
                }
                else return 1;
            }
            else if (temp == -1)
            {
                if (hand[0].cardtype == Cardtype.Gwang && hand[1].cardtype == Cardtype.Special)
                {
                    score = temp;
                    return score;
                }
                else if (hand[1].cardtype == Cardtype.Gwang && hand[0].cardtype == Cardtype.Special)
                {
                    score = temp;
                    return score;
                }
                 
                else return 0;
            }
            else if (temp == -3)
            {
                if (hand[0].cardtype == Cardtype.Special && hand[1].cardtype == Cardtype.Special)
                {
                    return -4;
                }
                else
                {
                    score = temp;
                    return score;
                }

            }
        }

        return score;
    }
    #endregion



    #region ��ƿ��Ƽ
    // ����Ʈ�� �������� ȸ��
    List<T> LeftRotate<T>(IReadOnlyList<T> list, int count)
    {
        int n = list.Count;
        count %= n; 

        var rotated = new List<T>(n);

        for (int i = 0; i < n; i++)
        {
            rotated.Add(list[(i + count) % n]);
        }

        return rotated;
    }
    // ������ Ű���� �����ϴ��� �Ǵ��ϴ� �޼���
    bool TryGetKeyByValue<TKey, TValue>(
    Dictionary<TKey, TValue> dict,
    TValue value,
    out TKey key)
    {
        
        foreach (var kv in dict)
        {
            if (EqualityComparer<TValue>.Default.Equals(kv.Value, value))
            {
                key = kv.Key;
                return true;
            }
        }

        key = default!;
        return false;
    }

    bool TryGetKeysByValue<TKey, TValue>(
    Dictionary<TKey, TValue> dict,
    TValue value,
    out List<TKey> keys)
    {
        keys = dict.Where(pair => EqualityComparer<TValue>.Default.Equals(pair.Value, value))
                   .Select(pair => pair.Key)
                   .ToList();

        return keys.Count > 0;
    }
    bool TryGetTieKeysByValue<TKey, TValue>(
    Dictionary<TKey, TValue> dict,
    TValue value,
    out List<TKey> keys)
    {
        keys = dict.Where(pair => EqualityComparer<TValue>.Default.Equals(pair.Value, value))
                   .Select(pair => pair.Key)
                   .ToList();

        return keys.Count > 1;
    }
    // Ư�� anchor�� �������� ����Ʈ ȸ��
    List<T> RotateToAnchor<T>(List<T> list, T anchor)
    {
        int index = list.IndexOf(anchor);
        if (index == -1)
            throw new ArgumentException("anchor not found in list");

        var result = new List<T>(list.Count);
        int n = list.Count;

        for (int i = 0; i < n; i++)
        {
            result.Add(list[(index + i) % n]);
        }

        return result;
    }
    //List<T> ShiftListByOrder<T>(List<T> originalList, int order)
    //{
    //    int count = originalList.Count;
    //    if (count == 0) return new List<T>();

    //    order %= count; // Ensure order is within bounds  
    //    return originalList.Skip(order).Concat(originalList.Take(order)).ToList();
    //}

    //List<NetworkClient> MakePlayerIDList(Dictionary<NetworkClient, int>.KeyCollection keys)
    //{
    //    var temp = new List<NetworkClient>();
    //    foreach (var key in keys)
    //    {
    //        temp.Add(key);
    //    }

    //    return temp;
    //}

    //List<NetworkClient> MakePlayerIDList(List<NetworkClient> keys)
    //{
    //    var temp = new List<NetworkClient>();
    //    foreach (var key in keys)
    //    {
    //        temp.Add(key);
    //    }

    //    return temp;
    //}

    //List<NetworkClient> GetShiftedOrderList(List<NetworkClient> originalList, int order)
    //{
    //    int count = originalList.Count;
    //    if (count == 0) return new List<NetworkClient>();

    //    order %= count;
    //    return originalList.Skip(order).Concat(originalList.Take(order)).ToList();
    //}
    #endregion



}

