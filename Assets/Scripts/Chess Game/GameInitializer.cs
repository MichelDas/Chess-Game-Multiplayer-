using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [Header("Game mode dependent objects")]

    [SerializeField] private SinglePlayerGameController singlePlayerGameControllerPrefab;
    [SerializeField] private MultiplayerGameController multiplayerGameControllerPrefab;
    [SerializeField] private SinglePlayerBoard singlePlayerBoard;
    [SerializeField] private MultiplayerBoard multiplayerBoard;

    [Header("Scene references")]

    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private UIManager uIManager;
    [SerializeField] private Transform boardAnchor;
    [SerializeField] private CameraSetup cameraSetup;
    [SerializeField] private Transform chessBoardParent;
    [SerializeField] private GameObject go;
    // this is called from NetworkManager.OnJoinedRoom
    public void CreateMultiplayerBoard()
    {
        Debug.Log("checking if the room is full");
        if (!networkManager.IsRoomFull())
        {
            go = PhotonNetwork.Instantiate(multiplayerBoard.name, boardAnchor.position, boardAnchor.rotation);
            go.SetActive(chessBoardParent);
        }
    }

    public void CreateSingleplayerBoard()
    {
        Instantiate(singlePlayerBoard, boardAnchor.position, boardAnchor.rotation);
    }

    public void InitializeMultiplayerController()
    {
        MultiplayerBoard multiplayerBoard = FindObjectOfType<MultiplayerBoard>();
        if (multiplayerBoard)
        {
            MultiplayerGameController controller = Instantiate(multiplayerGameControllerPrefab);
            controller.SetDependencies(uIManager, multiplayerBoard, cameraSetup);
            controller.CreatePlayers();
            controller.SetMultiplayerDependencies(networkManager);
            networkManager.SetDependencies(controller);
            multiplayerBoard.SetDependencies(controller);
        }
    }

    public void InitializeSingleplayerController()
    {
        SinglePlayerBoard singlePlayerBoard = FindObjectOfType<SinglePlayerBoard>();
        if (singlePlayerBoard)
        {
            SinglePlayerGameController controller = Instantiate(singlePlayerGameControllerPrefab);
            controller.SetDependencies(uIManager, singlePlayerBoard,cameraSetup);
            controller.CreatePlayers();
            singlePlayerBoard.SetDependencies(controller);
            controller.StartNewGame();
        }
    }



}
