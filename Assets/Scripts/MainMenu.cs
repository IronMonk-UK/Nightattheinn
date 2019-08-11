using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

	[SerializeField] Button startButton;
	[SerializeField] List<PlayerPanel> players;
	[SerializeField] GameObject innSign;
	[SerializeField] GameObject classPanel;
	[SerializeField] GameObject[] playerPanels;
	[SerializeField] int openPanels;
	[SerializeField] InputData keyboard;
	[SerializeField] InputData[] joysticks;
	[SerializeField] bool[] joysticksTaken;

	public List<PlayerPanel> Players { get { return players; } set { players = value; } }
	public InputData Keyboard { get { return keyboard; } }
	public InputData[] Joysticks { get { return joysticks; } }
	public bool[] JoysticksTaken { get { return joysticksTaken; } set { JoysticksTaken = value; } }

	private bool allPlayersReady() {
		if(GameManager.instance.PlayerCount == 0) { return false; }
		foreach(PlayerPanel player in players) {
			if (!player.Ready) { return false; }
		}
		return true;
	}

	private bool keyboardTaken() {
		if(GameManager.instance.PlayerCount == 0) { return false; }
		foreach(PlayerPanel player in players) {
			if (!player.Keyboard) { return false; }
		}
		return true;
	}

	public bool KeyboardTaken { get { return keyboardTaken(); } }

	private void Start() {
		classPanel.SetActive(false);
		foreach(GameObject panel in playerPanels) { panel.SetActive(false); }
		openPanels = 0;
		innSign.SetActive(true);
		SetStartButton();
	}

	public void Update() {		
		if (classPanel.activeInHierarchy) {
			if(openPanels <= GameManager.instance.PlayerCount && openPanels < 2) {
				OpenPlayerPanel(openPanels);
			}
		}

		if (allPlayersReady()) {
			foreach(PlayerPanel player in players) {
				ChooseClass(player.ClassIndex, player._Input);
			}
			MenuOptions.menuOptions.LoadGame();
		}
	}
	private void SetStartButton() {
		startButton.onClick = new Button.ButtonClickedEvent();
		startButton.onClick.AddListener(SetClassPanelActive);
	}

	private void SetClassPanelActive() {
		if (!classPanel.activeInHierarchy) {
			classPanel.SetActive(true);
			OpenPlayerPanel(0);
			EventSystem.current.SetSelectedGameObject(null);
		} else {
			classPanel.SetActive(false);
			innSign.SetActive(true);
			openPanels = 0;
		}
	}

	private void OpenPlayerPanel(int index) {
		innSign.SetActive(false);
		playerPanels[index].SetActive(true);
		openPanels++;
	}

	public void ChooseClass(int index, InputData input) {
		GameManager.instance.PlayerClasses.Add(index);
		GameManager.instance.PlayerInputs.Add(input);
	}
}
