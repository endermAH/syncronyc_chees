using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DevToDev.Analytics;

public class ChessUIManager : MonoBehaviour
{
	[Header("Dependencies")]
	[SerializeField] private NetworkManager networkManager;

	[Header("Buttons")]
	[SerializeField] private Button SwitchGameMode;
	[SerializeField] private Button MultiplayerStartGame;
	[SerializeField] private Button SingleplayerStartGame;
	[SerializeField] private Button StartGame;

	[Header("Texts")]
	[SerializeField] private Text finishText;
	[SerializeField] private Text errorText;
	[SerializeField] private Text waitingText;

	[Header("Screen Gameobjects")]
	[SerializeField] private GameObject GameOverScreen;
	[SerializeField] private GameObject GameModeSelectionScreen;

	[Header("Panel Gameobjects")]
	[SerializeField] private GameObject MultiplayerGameModePanel;
	[SerializeField] private GameObject MultiplayerClassicGameModePanel;
	[SerializeField] private GameObject SingleplayerGameModePanel;
	[SerializeField] private GameObject LevelSelectionPanel;
	[SerializeField] private GameObject WaitingPanel;
	[SerializeField] private GameObject WinPanel;
	[SerializeField] private GameObject LosePanel;
	[SerializeField] private GameObject ErrorPanel;
	[SerializeField] private GameObject StartMultigameBackpanel;

	[Header("Other UI")]
	[SerializeField] private GameObject loader;
	[SerializeField] private Image loader_succes;
	[SerializeField] private GameObject Background;


	private void Awake()
	{
		OnGameLaunched();
	}

	public void OnGameLaunched()
	{
		Background.SetActive(true);

		GameModeSelectionScreen.SetActive(true);
		MultiplayerGameModePanel.SetActive(true);
		SingleplayerGameModePanel.SetActive(true);
		MultiplayerClassicGameModePanel.SetActive(true);

		StartMultigameBackpanel.SetActive(false);
		WaitingPanel.SetActive(false);
		LevelSelectionPanel.SetActive(false);
		ErrorPanel.SetActive(false);
	}

	public void OnSinglePlayerModeSelected()
	{
		GameOverScreen.SetActive(false);
		GameModeSelectionScreen.SetActive(false);
		DTDAnalytics.CustomEvent(eventName: "Select singleplayer mode");
	}

	public void OnMultiPlayerModeSelected()
	{
		GameOverScreen.SetActive(false);
		GameModeSelectionScreen.SetActive(false);
		LevelSelectionPanel.SetActive(true);
		DTDAnalytics.CustomEvent(eventName: "Select multiplayer mode");
	}

	public void OnMultiPlayerClassicModeSelected()
	{
		GameOverScreen.SetActive(false);
		GameModeSelectionScreen.SetActive(false);
		LevelSelectionPanel.SetActive(true);
		DTDAnalytics.CustomEvent(eventName: "Select multiplayer classic mode");
	}

	internal void OnGameFinished(TeamColor winner)
	{
		Background.SetActive(true);
		GameOverScreen.SetActive(true);
		WinPanel.SetActive(false);
		LosePanel.SetActive(false);
		DTDAnalytics.CustomEvent(eventName: "Game finished");

		if (networkManager.IsWinner(winner))
		{
			finishText.text = string.Format("You won!");
			WinPanel.SetActive(true);
			LosePanel.SetActive(false);
        }
        else
        {
			finishText.text = string.Format("Maybe next time");
			WinPanel.SetActive(false);
			LosePanel.SetActive(true);
		}
		
	}

	internal void OnGameFinishedSingle(TeamColor winner)
    {
		OnGameLaunched();
		DTDAnalytics.CustomEvent(eventName: "Game finished");

		WinPanel.SetActive(false);
		LosePanel.SetActive(false);

		GameOverScreen.SetActive(true);
		finishText.text = string.Format("{0} won", winner);
		WinPanel.SetActive(true);
		LosePanel.SetActive(false);
	}

	public void OnConnect(int level)
	{
		networkManager.SetPlayerLevel((ChessLevel)level);
		networkManager.Connect();
	}
	public void ChooseMode(int mode)
	{
		networkManager.SetPlayerMode((Mode)mode);		
	}

	internal GameObject ShowTeamSelectionScreen()
	{
		GameOverScreen.SetActive(false);
		GameModeSelectionScreen.SetActive(false);
		LevelSelectionPanel.SetActive(false);

		WaitingPanel.SetActive(true);
		return WaitingPanel;
	}
    public void OnRoomPrepare()
    {
		loader.SetActive(true);
		waitingText.gameObject.SetActive(true);

		StartMultigameBackpanel.SetActive(false);
		WaitingPanel.transform.GetChild(0).gameObject.SetActive(true);
		StartGame.gameObject.SetActive(false);
		loader_succes.gameObject.SetActive(false);
	}

    public void OnRoomReady()
    {
		StartMultigameBackpanel.SetActive(true);
		StartGame.gameObject.SetActive(true);
		loader_succes.gameObject.SetActive(true);

		WaitingPanel.transform.GetChild(0).gameObject.SetActive(false);
		loader.SetActive(false);
		waitingText.gameObject.SetActive(false);
    }

	// Сейчас выключаем все экраны
    public void OnGameStarted()
	{
		StartMultigameBackpanel.SetActive(false);
		Background.SetActive(false);
		GameOverScreen.SetActive(false);
		WaitingPanel.SetActive(false);
		GameModeSelectionScreen.SetActive(false);
	}

	// Выбор команды по кнопкам "черные"/"белые"
	public void SelectTeam(int team)
	{
		networkManager.SetPlayerTeam(team);		
	}
	// Используется
	public void SelectTeam()
	{
		int team = UnityEngine.Random.Range(0, 1);
		networkManager.SetPlayerTeam(team);
	}

	public void SetConnectionStatusText(string status) { }
}
