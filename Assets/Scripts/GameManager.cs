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
    [SerializeField] private Vector3[] m_StartPositions;

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
            foreach (var item in NetworkManager.Singleton.ConnectedClients)
            {
                Debug.Log($"ConnectedClients {item.Key}");
            }
            var player = NetworkManager.Singleton.ConnectedClients[cliendId]
                .OwnedObjects.Find(x => x.GetComponent<PlayerController>() != null)
                .GetComponent<PlayerController>();
            playerControllers.Add(player);
        }  
    }

    private void OnDisconnectedClient(ulong obj)
    {
        Debug.Log("OnClientConnectedCallback");
    }
}
