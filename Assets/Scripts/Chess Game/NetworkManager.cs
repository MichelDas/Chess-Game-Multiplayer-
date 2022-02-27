using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class NetworkManager : MonoBehaviourPunCallbacks
{

    private const string LEVEL = "level";
    private const string TEAM = "team";
    private const byte MAX_PLAYERS = 2;

    [SerializeField] private UIManager uIManager;
    [SerializeField] private GameInitializer gameInitializer;
    private MultiplayerGameController gameController;

    private ChessLevel playerLevel;
    private bool isWaitingforSecondPlayer;

    private void Awake()
    {
        // this will sync the scene in all the clients
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // this is called from the gameInitializer 
    public void SetDependencies(MultiplayerGameController gameController)
    {
        this.gameController = gameController;
    }

    private void Update()
    {
        if(!isWaitingforSecondPlayer)
            uIManager.SetConnectionStatus(PhotonNetwork.NetworkClientState.ToString());
    }

    public void Connect(ChessLevel level)
    {
        isWaitingforSecondPlayer = false;
        playerLevel = level;
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { LEVEL, level } });

        if (PhotonNetwork.IsConnected)
        {
            Debug.Log($"Connected to Server. Looking for random room with level {playerLevel}");
            PhotonNetwork.JoinRandomRoom( new ExitGames.Client.Photon.Hashtable() { { LEVEL, playerLevel } }, MAX_PLAYERS);
        }
        else
        {  
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    internal bool IsRoomFull()
    {
        return PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers;
    }

    #region Photon Callbacks

    // this callback is called when we have connected to the server
    public override void OnConnectedToMaster()
    {
        Debug.Log($"Connected to Server. Looking for random room with level {playerLevel} ");
        // after connecting to the server,
        // we will try connecting to a random room
        // the JoinRandomRoom() will called either of two callbacks
        // OnJoinRandomFailed()    or   OnJoinedRoom()
        PhotonNetwork.JoinRandomRoom(new ExitGames.Client.Photon.Hashtable() { { LEVEL, playerLevel } }, MAX_PLAYERS);
    }

    // this is called if JoinRandomRoom() failed
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"Joining random room failed because of {message}. Creating a new room with player level {playerLevel}"); 
        PhotonNetwork.CreateRoom(null, new RoomOptions
        {
            CustomRoomPropertiesForLobby = new string[] { LEVEL },
            MaxPlayers = MAX_PLAYERS,
            CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { LEVEL, playerLevel } }
        });
    }

    public void SelectTeam(int team)
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { TEAM, team } });
        gameInitializer.InitializeMultiplayerController();
        gameController.SetLocalPlayer((TeamColor)team);
        gameController.StartNewGame();
        gameController.SetupCAmera((TeamColor)team);
        isWaitingforSecondPlayer = true;
        uIManager.SetConnectionStatus("Waiting for player to join");
    }

    // This is called when JoinRandomRoom is succeeded
    public override void OnJoinedRoom()
    {
        Debug.Log($"Player {PhotonNetwork.LocalPlayer.ActorNumber} joined the room with level {(ChessLevel)PhotonNetwork.CurrentRoom.CustomProperties[LEVEL]}");
        gameInitializer.CreateMultiplayerBoard(); // this is better done with events
        PrepareTeamSelectionOptions();
        uIManager.ShowTeamSelectionScreen();
    }

    private void PrepareTeamSelectionOptions()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            var player = PhotonNetwork.CurrentRoom.GetPlayer(1);
            if (player.CustomProperties.ContainsKey(TEAM))
            {
                var occupiedTeam = player.CustomProperties[TEAM];
                uIManager.RestrictTeamChoice((TeamColor)occupiedTeam);
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Player {newPlayer.ActorNumber} entered the room");
    }

    #endregion
}
