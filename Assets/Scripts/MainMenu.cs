using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	[SerializeField] GameObject classPanel;
	[SerializeField] Button startButton;
	[SerializeField] Button fighterButton;
	[SerializeField] Button mageButton;
	[SerializeField] Button rangerButton;

	private void Start() {
		MenuOptions.menuOptions.ClassPanel = classPanel;
		Debug.Log(MenuOptions.menuOptions.ClassPanel);
		classPanel.SetActive(false);
		SetStartButton();
		SetFighterButton();
		SetMageButton();
		SetRangerButton();
	}

	private void SetStartButton() {
		startButton.onClick = new Button.ButtonClickedEvent();
		startButton.onClick.AddListener(MenuOptions.menuOptions.SetClassPaneActive);
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
}
