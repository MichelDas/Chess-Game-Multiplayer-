using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Scene Dependencies")]
    [SerializeField] private NetworkManager networkManager;

    [Header("Buttons")]
    [SerializeField] private Button blackTeamButton;

    

    [SerializeField] private Button whiteTeamButton;

    [Header("Texts")]
    [SerializeField] private Text resultText;
    [SerializeField] private Text connectionStatusText;

    [Header("Screen Gameobjects")]
    [SerializeField] private GameObject gameoverScreen;
    [SerializeField] private GameObject connectScreen;
    [SerializeField] private GameObject teamSelectionScreen;
    [SerializeField] private GameObject gameModeSelectionScreen;

    [Header("Other UI")]
    [SerializeField] private Dropdown gameLevelSelection;

    private void Awake()
    {
        gameLevelSelection.AddOptions(Enum.GetNames(typeof(ChessLevel)).ToList());
        OnGameLaunched();

    }

    private void OnGameLaunched()
    {
        DisableAllScreen();
        gameModeSelectionScreen.SetActive(true);
    }

    // This is called when SingleplayerMode button is clicked
    public void OnSinglePlayerModeSelected()
    {
        DisableAllScreen();
    }

    // This is called when Multiplayer Mode button is clicked
    public void OnMultiplayerModeSelected()
    {
        connectionStatusText.gameObject.SetActive(true);
        DisableAllScreen();
        connectScreen.SetActive(true);
    }

    // this is called when connect button is clicked
    public void OnConnect()
    {
        networkManager.SetPlayerLevel((ChessLevel)gameLevelSelection.value);
        networkManager.Connect();
    }

    private void DisableAllScreen()
    {
        gameoverScreen.SetActive(false);
        connectScreen.SetActive(false);
        teamSelectionScreen.SetActive(false);
        gameModeSelectionScreen.SetActive(false);
    }

    public void SetConnectionStatus(string v)
    {
        connectionStatusText.text = v;
    }

    internal void ShowTeamSelectionScreen()
    {
        DisableAllScreen();
        teamSelectionScreen.SetActive(true);
    }

    public void SelectTeam(int team)
    {
        networkManager.SelectTeam(team);
    }

    public void RestrictTeamChoice(TeamColor occpiedTeam)
    {
        Button buttonToDeactivate = occpiedTeam == TeamColor.White ? whiteTeamButton : blackTeamButton;
        buttonToDeactivate.interactable = false;
    }
}
