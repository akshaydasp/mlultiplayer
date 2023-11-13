using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
   
    public GameObject winningCanvas; // Reference to the winning Canvas

    private bool isGameOver = false;

    // ... Other variables ...

    void Start()
    {

        winningCanvas.SetActive(false); // Disable the winning Canvas by default
        // ... Initialize other variables ...

        // Check if this is the master client (the first player in the room)
        if (PhotonNetwork.IsMasterClient)
        {
            // Set a timer to check for the winning condition
            StartCoroutine(CheckWinningCondition());
        }
    }

    // ... Other methods ...

    IEnumerator CheckWinningCondition()
    {
        yield return new WaitForSeconds(5f); // Adjust the delay as needed.

        while (!isGameOver)
        {
            // Get the number of players in the room
            int playerCount = PhotonNetwork.PlayerList.Length;

            if (playerCount <= 1)
            {
                // Only one player left in the room, declare the winner
                isGameOver = true;

                // Show the winning Canvas
                winningCanvas.SetActive(true);
            }

            yield return new WaitForSeconds(5f); // Adjust the check interval as needed.
        }
    }


    public void OnApplicationQuit()
    {
        Application.Quit();
    }
}
