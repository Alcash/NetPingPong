using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    private NetworkClient _networkClient;
    public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();
    [SerializeField] private float speed = 10f;
    [SerializeField] private GameObject _view;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        SetViewEnable(false);
    }
    private void Update()
    {
        transform.Translate(Position.Value - transform.position);
        if (IsOwner && _view.activeSelf)
        {        
            var xaxis = Input.GetAxis("Horizontal");
            PlayerInputMoveServerRpc(xaxis);
         }
    }

    public void SetViewEnable(bool enable)
    {
        SetViewClientRpc(enable);
    }
    [ClientRpc]
    private void SetViewClientRpc(bool enable)
    {
        _view.SetActive(enable);
    }

    [ServerRpc]
    private void PlayerInputMoveServerRpc(float xaxis)
    {        
        Position.Value += new Vector3(xaxis, 0, 0) * speed * Time.deltaTime;
    }
}
