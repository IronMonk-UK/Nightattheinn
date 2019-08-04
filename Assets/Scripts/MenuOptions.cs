using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuOptions : MonoBehaviour
{
	[SerializeField] GameObject classPanel;

	public GameObject ClassPanel { get { return classPanel; } set { classPanel = value; } }

	public static MenuOptions menuOptions;

	private void Awake() {
		if(menuOptions == null) { menuOptions = this; } else { Destroy(this); }
	}

	public void SetClassPaneActive() {
		if (!classPanel.activeInHierarchy) {
			classPanel.SetActive(true);
		} else {
			classPanel.SetActive(false);
		}
	}

	public void ChooseClass(int index) {
		GameManager.instance.PlayerClasses.Add(index);
	}

	public void LoadGame() {
		classPanel = null;
		SceneManager.LoadScene(1);
	}

	public void QuitGame() { Application.Quit(); }

	public void Menu() {
		StartCoroutine(MenuCoroutine());
		GameManager.instance.PlayerClasses.Clear();
	}

	private IEnumerator MenuCoroutine() {
		AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
		while (!asyncLoadLevel.isDone) {
			yield return null;
		}
	}
}
