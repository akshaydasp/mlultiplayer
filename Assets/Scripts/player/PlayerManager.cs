using Photon.Pun.Demo.PunBasics;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        CamFollow _cameraWork = this.gameObject.GetComponent<CamFollow>();

        if (_cameraWork != null)
        {
            if (photonView.IsMine)
            {
                _cameraWork.OnStartFollowing();
            }
        }
        else
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
        }
    }
}
