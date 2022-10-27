using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking.Types;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;     
    public static GameManager Instance => _instance;

    [SerializeField] private BallController m_BallPrefab;
    [SerializeField] private NetworkObject m_MovePlatform;
    [SerializeField] private Transform[] m_StartPositions;

    private List<PlayerController> playerControllers = new List<PlayerController>();

    private void Awake()
    {
        if(NetworkManager.Singleton.IsServer)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
       
        NetworkManager.Singleton.OnClientConnectedCallback += OnConnectedClient;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnDisconnectedClient;
        NetworkManager.Singleton.ConnectionApprovalCallback += ApproveConnection;
    }

    private void ApproveConnection(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        Debug.Log("ApproveConnection");
        var connectionData = request.Payload;
        var clientId = request.ClientNetworkId;
        if (connectionData.Length > m_StartPositions.Length)
        {
            response.Approved = false;
            return;
        }


        // connection approval will create a player object for you
        response.Approved = true;
        response.CreatePlayerObject = true;
        response.Position = Vector3.zero;
        response.Rotation = Quaternion.identity;   
    }

    private void OnConnectedClient(ulong cliendId)
    {
        if(NetworkManager.Singleton.IsServer)
        {            
            var player = NetworkManager.Singleton.ConnectedClients[cliendId]
                .OwnedObjects.Find(x => x.GetComponent<PlayerController>() != null)
                .GetComponent<PlayerController>();
            playerControllers.Add(player);
            player.Position.Value = m_StartPositions[cliendId].position;
            if (playerControllers.Count == m_StartPositions.Length)
            {
                var inst = Instantiate(m_BallPrefab);
                inst.GetComponent<NetworkObject>().Spawn();
                playerControllers.ForEach(x => x.SetViewEnable(true));
            }
        }  
    }



    private void OnDisconnectedClient(ulong cliendId)
    {
        var player = NetworkManager.Singleton.ConnectedClients[cliendId]
                 .OwnedObjects.Find(x => x.GetComponent<PlayerController>() != null)
                 .GetComponent<PlayerController>();
        playerControllers.Remove(player);
        playerControllers.ForEach(x => x.SetViewEnable(false));
    }
}
