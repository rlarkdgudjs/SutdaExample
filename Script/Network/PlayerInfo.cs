using System.Globalization;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode.Transports.UTP;
using UnityEngine.XR;
using System.Collections.Generic;

public class PlayerInfo : NetworkBehaviour
{
    public NetworkVariable<int> Money = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,    // 모두 읽을 수 있음
        NetworkVariableWritePermission.Server      // 서버만 값 수정 가능
    );
    public NetworkVariable<FixedString32Bytes> name = new NetworkVariable<FixedString32Bytes>("",NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
    
    public override void OnNetworkSpawn()
    {
        if(IsServer)
        {
            Money.Value = 50000;
        }
        name.OnValueChanged += OnPlayerNameChanged;

        if (IsOwner)
        {
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(OwnerClientId, out var client))
            {
                string playerName = System.Text.Encoding.UTF8.GetString(NetworkManager.Singleton.NetworkConfig.ConnectionData);

                if (!string.IsNullOrEmpty(playerName))
                {
                    name.Value = new FixedString32Bytes(playerName);
                    Debug.Log($"서버에서 이름 설정: {playerName}");
                }
            }
           
        }
        

    }
    // 서버에서만 작동 돈 수정
    public void ChangeMoney(int value)
    {
        if (IsServer)
        {
            Money.Value += value;
            Debug.Log($"[{OwnerClientId}] Money Change to {Money.Value}");
        }
    }
    [ClientRpc]
    public void CallDebugClientRpc()
    {
        CallDebug();
    }
    public void CallDebug()
    {
        if (IsOwner)
        {
            Debug.Log(OwnerClientId);
            Debug.Log(NetworkManager.Singleton.LocalClientId);
        }
        Debug.Log("BB");
    }
    
    
  
    private void OnPlayerNameChanged(FixedString32Bytes oldName, FixedString32Bytes newName)
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H)&&IsOwner)
        {
            Debug.Log(IsServer);
            Debug.Log(IsHost);
            Debug.Log(IsClient);
            Debug.Log(OwnerClientId);
            Debug.Log(name.Value);
        }

        // 테스트용
        if (IsOwner && Input.GetKeyDown(KeyCode.M))
        {
            Money.Value += 100;
            Debug.Log($"[{OwnerClientId}] Money increased to {Money.Value}");
        }
        
    }

  

}
