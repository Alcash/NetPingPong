using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    private NetworkClient _networkClient;
    public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();
    private float speed = 0.1f;
    public override void OnNetworkSpawn()
    {
        //OwnerClientId
        Debug.Log($"OnNetworkSpawn {OwnerClientId}");
        if (IsServer)
            _networkClient = NetworkManager.Singleton.ConnectedClients[OwnerClientId];
        //enabled = IsOwner;
        if (IsOwner)
        {
           
            Move();
        }
    }

    

    public void Move()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            var randomPosition = GetRandomPositionOnPlane();
            transform.position = randomPosition;
            Position.Value = randomPosition;
        }
        else
        {
            SubmitPositionRequestServerRpc();
        }
    }

    [ServerRpc]
    void SubmitPositionRequestServerRpc(ServerRpcParams rpcParams = default)
    {
        Position.Value = GetRandomPositionOnPlane();
    }

    static Vector3 GetRandomPositionOnPlane()
    {
        return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
    }

    void Update()
    {


        transform.position = Position.Value;

        if (IsOwner)
        {
            var xaxis = Input.GetAxis("Horizontal");
            var yaxis = Input.GetAxis("Vertical");
            PlayerInputMoveServerRpc(xaxis, yaxis);
        }

    }

    [ServerRpc]
    private void PlayerInputMoveServerRpc(float xaxis, float yaxis)
    {
        Position.Value += new Vector3(xaxis, 0, yaxis) * speed;
    }
}
