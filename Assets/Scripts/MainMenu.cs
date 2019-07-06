using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	[SerializeField] GameObject classPanel;

	public static MainMenu mainMenu;

	private void Awake() {
		if(mainMenu == null) { mainMenu = this; } else { Destroy(gameObject); }
		if (classPanel) {
			classPanel.SetActive(false);
		}
	}

	public void OpenClassPanel() { classPanel.SetActive(true); }

	public void ChooseClass(int index) {
		GameManager.instance.PlayerClasses.Add(index);
	}
	public void LoadGame() {
		SceneManager.LoadScene(1);
		GameManager.instance._Time = 0;
		classPanel = null;
	}
	public void ExitGame() { Application.Quit(); }
}
