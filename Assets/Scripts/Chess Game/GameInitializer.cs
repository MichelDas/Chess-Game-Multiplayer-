using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// The functions in this class are called from other classes.
public class GameInitializer : MonoBehaviour
{
    [Header("Game mode dependent objects")]

    [SerializeField] private SinglePlayerGameController singlePlayerGameControllerPrefab;
    [SerializeField] private MultiplayerGameController multiplayerGameControllerPrefab;
    [SerializeField] private SinglePlayerBoard singlePlayerBoardPrefab;
    [SerializeField] private MultiplayerBoard multiplayerBoardPrefab;

    [Header("Scene references")]

    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private UIManager uIManager;
    [SerializeField] private Transform boardAnchor;
    [SerializeField] private CameraSetup cameraSetup;
    [SerializeField] private Transform chessBoardParent;
     private GameObject go;


    // this is called from NetworkManager.OnJoinedRoom
    public void CreateMultiplayerBoard()
    {
        Debug.Log("checking if the room is full");
        if (!networkManager.IsRoomFull())
        {
            go = PhotonNetwork.Instantiate(multiplayerBoardPrefab.name, boardAnchor.position, boardAnchor.rotation);
            go.SetActive(chessBoardParent);
        }
    }

    // this will be called from network manager
    // when a player is selected   i.e. SelectTeam()
    public void InitializeMultiplayerController()
    {
        MultiplayerBoard multiplayerBoard = FindObjectOfType<MultiplayerBoard>();
        if (multiplayerBoard)
        {
            MultiplayerGameController controller = Instantiate(multiplayerGameControllerPrefab);

            controller.InitializeGame(uIManager, multiplayerBoard, cameraSetup);
            controller.SetNetworkManager(networkManager);

            networkManager.SetDependencies(controller);
            multiplayerBoard.SetDependencies(controller);
        }
    }

    // This is called from SinglePlayerMode Button's OnClick()
    public void InitializeSingleplayerController()
    {
        SinglePlayerBoard singlePlayerBoard = Instantiate(singlePlayerBoardPrefab, boardAnchor.position, boardAnchor.rotation);
        singlePlayerBoard.transform.SetParent(boardAnchor);
        if (singlePlayerBoard)
        {
            SinglePlayerGameController controller = Instantiate(singlePlayerGameControllerPrefab);
            controller.InitializeGame(uIManager, singlePlayerBoard, cameraSetup);
            singlePlayerBoard.SetDependencies(controller); // set the controller in the board
            controller.StartNewGame();
        }
    }



}
