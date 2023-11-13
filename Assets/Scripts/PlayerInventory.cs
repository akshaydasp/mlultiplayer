using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviourPun
{
    public GameObject hat; // Reference to the hat object.
    private Transform playerHead;

    public void PickUpHat()
    {
        if (!photonView.IsMine) return;

        // Set the hat to be active in the player's inventory.
        hat.SetActive(true);

        if (playerHead == null)
        {
            playerHead = transform.Find("Head");
            // You may need to find the player's head by tag, name, or other means.
            // Example: playerHead = transform.Find("Head");
        }

        if (playerHead != null)
        {
            // Position the hat on the player's head.
            hat.transform.position = playerHead.position;
            hat.transform.rotation = playerHead.rotation;

            // Set the player's head as the parent of the hat.
            hat.transform.parent = playerHead;
        }

        // Inform other players that this player has the hat.
        photonView.RPC("SetHatActive", RpcTarget.All, true);
    }


    [PunRPC]
    private void SetHatActive(bool isActive)
    {
        hat.SetActive(isActive);
    }
}
