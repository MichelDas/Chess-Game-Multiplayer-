using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{

    [SerializeField] private UIManager uIManager;

    private void Update()
    {
        uIManager.SetConnectionStatus(PhotonNetwork.NetworkClientState.ToString());
    }

    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    #region Photon Callbacks

    // this callback is called when we have connected to the server
    public override void OnConnectedToMaster()
    {
        Debug.LogError("Connected to server. Looking for random room");
        // after connecting to the server,
        // we will try connecting to a random room
        // the JoinRandomRoom() will called either of two callbacks
        // OnJoinRandomFailed()    or   OnJoinedRoom()
        PhotonNetwork.JoinRandomRoom();
        
    }

    // this is called if JoinRandomRoom() failed
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogError($"Joining random room failed because of {message}. Creating a new room"); 
        PhotonNetwork.CreateRoom(null);
    }

    // This is called when JoinRandomRoom is succeeded
    public override void OnJoinedRoom()
    {
        Debug.LogError($"Player {PhotonNetwork.LocalPlayer.ActorNumber} joined the room");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Player {newPlayer.ActorNumber} entered the room");
    }


    #endregion
}
