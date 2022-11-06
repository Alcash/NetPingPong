using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace ProjectCore.Effects
{
    public class LastPlayerBigView : IBonusEffect
    {    
        private PlayerController _playerEffectController;
        private Vector3 _startScale;

        private float _multipleScale = 2;
        private float _effectTime;
        public float EffectTime => _effectTime;
        public LastPlayerBigView(IEnumerable<PlayerController> players, PlayerController lastPlayer, float time)
        {
            _effectTime = time;
            _playerEffectController = lastPlayer;

            _startScale = _playerEffectController.transform.localScale;
        }        

        public void Enable(bool enable)
        {
            Debug.Log($"LastPlayerBigView Enable {enable}");
            EffectClientRpc(enable ? Vector3.one * _multipleScale : _startScale);            
        }

        [ClientRpc]
        private void EffectClientRpc(Vector3 scale)
        {
            Debug.Log($"new scale Client {scale}");
            var localPlayer = NetworkManager.Singleton.ConnectedClients[_playerEffectController.NetworkObject.OwnerClientId].PlayerObject.transform;
            Debug.Log($"new scale Client {0}");
            Debug.Log($"Client {localPlayer.transform.name} {1}");
            localPlayer.transform.localScale = scale;
            Debug.Log($"Client {localPlayer.transform.name} {2}");
            if (scale.x <= 1.1)
                localPlayer.AddComponent<Rigidbody>();
        }
    }
}

