using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
	[SerializeField] GameObject classPanel;
	[SerializeField] Button startButton;
	[SerializeField] Button fighterButton;
	[SerializeField] Button mageButton;
	[SerializeField] Button rangerButton;

	[SerializeField] GameObject innSign;
	[SerializeField] GameObject classPanel2;
	[SerializeField] GameObject[] playerPanels;
	[SerializeField] int openPanels;

	private void Start() {
		MenuOptions.menuOptions.ClassPanel = classPanel;
		classPanel.SetActive(false);
		classPanel2.SetActive(false);
		foreach(GameObject panel in playerPanels) { panel.SetActive(false); }
		openPanels = 0;
		innSign.SetActive(true);
		SetStartButton();
		SetFighterButton();
		SetMageButton();
		SetRangerButton();
	}

	public void Update() {
		/*
		if (Input.GetButtonDown("Horizontal")) { Debug.Log("Horizontal"); }
		if (Input.GetButtonDown("Vertical")) { Debug.Log("Vertical"); }
		if (Input.GetButtonDown("Fire1")) { Debug.Log("Fire1"); }
		if (Input.GetButtonDown("Fire2")) { Debug.Log("Fire2"); }
		if (Input.GetButtonDown("Fire3")) { Debug.Log("Fire3"); }
		if (Input.GetAxis("Joy1_L_Horizontal") != 0) { Debug.Log("Joy1_L_Horizontal"); }
		if (Input.GetAxis("Joy1_L_Vertical") != 0) { Debug.Log("Joy1_L_Vertical"); }
		if (Input.GetAxis("Joy1_R_Horizontal") != 0) { Debug.Log("Joy1_R_Horizontal"); }
		if (Input.GetAxis("Joy1_R_Vertical") != 0) { Debug.Log("Joy1_R_Vertical"); }
		if (Input.GetAxis("Joy1_Fire1") != 0) { Debug.Log("Joy1_Fire1"); }
		if (Input.GetAxis("Joy1_Fire2") != 0) { Debug.Log("Joy1_Fire2"); }
		if (Input.GetButtonDown("Joy1_Fire3")) { Debug.Log("Joy1_Fire3"); }
		*/
		if (classPanel2.activeInHierarchy) {
			if(openPanels <= GameManager.instance.PlayerCount && openPanels < 1) {
				OpenPlayerPanel(openPanels);
			}
		}
	}

	private void SetStartButton() {
		startButton.onClick = new Button.ButtonClickedEvent();
		//startButton.onClick.AddListener(MenuOptions.menuOptions.SetClassPaneActive);
		startButton.onClick.AddListener(SetClassPanel2Active);
	}

	private void SetFighterButton() {
		fighterButton.onClick = new Button.ButtonClickedEvent();
		fighterButton.onClick.AddListener(delegate { MenuOptions.menuOptions.ChooseClass(0); });
		fighterButton.onClick.AddListener(MenuOptions.menuOptions.LoadGame);
	}

	private void SetMageButton() {
		mageButton.onClick = new Button.ButtonClickedEvent();
		mageButton.onClick.AddListener(delegate { MenuOptions.menuOptions.ChooseClass(1); });
		mageButton.onClick.AddListener(MenuOptions.menuOptions.LoadGame);
	}

	private void SetRangerButton() { 
		rangerButton.onClick = new Button.ButtonClickedEvent();
		rangerButton.onClick.AddListener(delegate { MenuOptions.menuOptions.ChooseClass(2); });
		rangerButton.onClick.AddListener(MenuOptions.menuOptions.LoadGame);
	}

	private void SetClassPanel2Active() {
		if (!classPanel2.activeInHierarchy) {
			classPanel2.SetActive(true);
			OpenPlayerPanel(0);
			EventSystem.current.SetSelectedGameObject(null);
		} else {
			classPanel2.SetActive(false);
			innSign.SetActive(true);
			openPanels = 0;
		}
	}

	private void OpenPlayerPanel(int index) {
		innSign.SetActive(false);
		playerPanels[index].SetActive(true);
		openPanels++;
	}
}
