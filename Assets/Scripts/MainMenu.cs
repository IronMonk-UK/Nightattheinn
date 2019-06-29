using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	[SerializeField] GameObject classPanel;

	private void Awake() {
		classPanel.SetActive(false);
	}

	public void OpenClassPanel() { classPanel.SetActive(true); }

	public void ChooseClass(int index) {
		GameManager.instance.PlayerClasses.Add(index);
	}
	public void LoadGame() { SceneManager.LoadScene(1); }
	public void ExitGame() { Application.Quit(); }
}
