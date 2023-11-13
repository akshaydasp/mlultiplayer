using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class Launcher : MonoBehaviourPunCallbacks
{
    public GameObject Connected;
    public GameObject DisConnected;

    public GameObject playernamecanvas;

    public InputField playername;

    public void OnClick_ConnectBtn()
    {
        playernamecanvas.SetActive(true);
       
    }

    public void ConnectButton()
    {       
        PhotonNetwork.NickName = playername.text;
        PhotonNetwork.ConnectUsingSettings();
        playernamecanvas.SetActive(false);
    }
    public void OnClick_RetryBtn()
    {
        
        DisConnected.SetActive(false);

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        DisConnected.SetActive(true);
    }
    public override void OnJoinedLobby()
    {
        if(DisConnected.activeSelf)
                 DisConnected.SetActive(false);

            Connected.SetActive(true);
             
    }

}


