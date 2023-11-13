using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Manager : MonoBehaviour
{
    public GameObject playerprefab;



    void Start()
    {
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        PhotonNetwork.Instantiate(playerprefab.name, playerprefab.transform.position, playerprefab.transform.rotation);
    }
}
