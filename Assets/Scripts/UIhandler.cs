using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class UIhandler : MonoBehaviourPunCallbacks
{
    public InputField CreateRoomIF;
    public InputField JoinRoomIF;

   

    public void OnClick_JoinRoom()
    {
        PhotonNetwork.JoinRoom(JoinRoomIF.text, null);
    }
    public void OnClick_CreateRoom()
    {
        PhotonNetwork.CreateRoom(CreateRoomIF.text,new RoomOptions { MaxPlayers = 5 },null);
    }

    public override void OnJoinedRoom()
    {
        print("join succeed");

        PhotonNetwork.LoadLevel(1);
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        print("Join room failed" + returnCode + "Message" + message);
    }

    
}
