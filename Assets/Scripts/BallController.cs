using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BallController : NetworkBehaviour
{

    private NetworkVariable<Vector3> _position = new NetworkVariable<Vector3>();
    [SerializeField] private float m_SpeedMove = 10;
    [SerializeField] private float m_AccelMove = 0.1f;
    [SerializeField] private float m_xRange;
    [SerializeField] private float m_yRange;
    private Vector3 _moveDirection = Vector3.right;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        var initMove = UnityEngine.Random.insideUnitCircle;
        _moveDirection.x = initMove.x;
        _moveDirection.z = initMove.y;


    }

    // Update is called once per frame
    private void Update()
    {
        transform.Translate(_position.Value - transform.position);
        if (IsClient)
        {
            
        }
        if (IsServer)
        {
            IncreaseSpeed();
            BorderLimit();
            PlayerPlatformCollision();
        }       
    }

    private void IncreaseSpeed()
    {
        _position.Value += _moveDirection * m_SpeedMove * Time.deltaTime;
    }

    private void BorderLimit()
    {
        if (Mathf.Abs(_position.Value.x) > m_xRange)
        {
            _moveDirection.x = InvertDirection(_moveDirection.x, _position.Value.x);
            
        }
        if (Mathf.Abs(_position.Value.z) > m_yRange)
        {
            _moveDirection.z = InvertDirection(_moveDirection.z, _position.Value.z);
            GameManager.Instance.CollideBorder(_position.Value.z > 0 ? 1 : 0);
        }

        m_SpeedMove += m_AccelMove * Time.deltaTime;
    }

    private void PlayerPlatformCollision()
    {
        foreach (var player in GameManager.Instance.PlayerControllers)
        {
           if(Mathf.Abs(player.transform.position.z) < Mathf.Abs(_position.Value.z))
           {
               if(NearPlayerOnX(_position.Value.x, player.transform))
                {
                    _moveDirection.z = InvertDirection(_moveDirection.z, _position.Value.z);
                    GameManager.Instance.CollidePlayer(player.NetworkObject.OwnerClientId);
                    Debug.Log($"PosZ = {_position.Value.z}");

                }
           }
        } 
    }

    private bool NearPlayerOnX(float x, Transform transformPlayer)
    {
        return transformPlayer.position.x - transformPlayer.localScale.x / 2 < x 
            && transformPlayer.position.x + transformPlayer.localScale.x / 2 > x;
    }

    private float InvertDirection(float vel, float pos)
    {
        return MathF.Abs(vel) * Mathf.Sign(-pos);
    }

}
