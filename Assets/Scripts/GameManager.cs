using ProjectCore.Effects;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking.Types;

public class GameManager : NetworkBehaviour
{
    private static GameManager _instance;     
    public static GameManager Instance => _instance;

    [SerializeField] private BallController m_BallPrefab;
    [SerializeField] private NetworkObject m_MovePlatform;
    [SerializeField] private Transform[] m_StartPositions;

    private List<PlayerController> _playerControllers = new List<PlayerController>();

    public List<PlayerController> PlayerControllers => _playerControllers;

    private void Awake()
    {
        if (_instance == null)
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
        effectManager = FindObjectOfType<EffectManager>();
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
            _playerControllers.Add(player);
            player.Position.Value = m_StartPositions[cliendId].position;
            if (_playerControllers.Count == m_StartPositions.Length)
            {
                var inst = Instantiate(m_BallPrefab);
                inst.GetComponent<NetworkObject>().Spawn();
                _playerControllers.ForEach(x => x.SetViewEnable(true));
            }
        }  
    }
    private void OnDisconnectedClient(ulong cliendId)
    {
        var player = NetworkManager.Singleton.ConnectedClients[cliendId]
                 .OwnedObjects.Find(x => x.GetComponent<PlayerController>() != null)
                 .GetComponent<PlayerController>();
        _playerControllers.Remove(player);
        _playerControllers.ForEach(x => x.SetViewEnable(false));
    }

    private EffectManager effectManager;

    private void Update()
    {
        if (IsServer)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Debug.Log(_playerControllers[0].name);
                effectManager.TakeBonus(new LastPlayerBigView(_playerControllers, _playerControllers[0], 5));
            }
            if (Input.GetKey(KeyCode.E))
            {
                effectManager.TakeBonus(new LastPlayerBigView(_playerControllers, _playerControllers[1], 5));
            }
        }
    }

    public void CollideBorder(int side)
    {

    }

    public void CollidePlayer(ulong playerId)
    {

    }

    public void CollideEffectBonus(IBonusEffect bonusEffect)
    {

    }
}
