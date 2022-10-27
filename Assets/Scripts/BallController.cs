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
    void Update()
    {
        if (IsClient)
        {
            transform.Translate(_position.Value - transform.position);
        }
        if (IsServer)
        {
            _position.Value += _moveDirection * m_SpeedMove  * Time.deltaTime;
            if(Mathf.Abs(_position.Value.x ) > m_xRange)
            {
                _moveDirection.x = -_moveDirection.x;
            }
            if (Mathf.Abs(_position.Value.z) > m_yRange)
            {
                _moveDirection.z = -_moveDirection.z;
            }

            m_SpeedMove += m_AccelMove * Time.deltaTime;
        }

       
    }
}
