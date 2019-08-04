using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPanel : MonoBehaviour
{
	[SerializeField] int playerID;
	[SerializeField] Text startText;
	[SerializeField] bool activePlayer;
	[SerializeField] bool keyboard;
	[SerializeField] bool controller;
	[SerializeField] int controllerID;

	[SerializeField] GameObject classSelectPanel;
	[SerializeField] GameObject[] adventurerModels;
	[SerializeField] string[] adventurerNames;
	[SerializeField] GameObject currentClass;
	[SerializeField] string currentName;
	[SerializeField] Text nameText;
	[SerializeField] int classIndex;
	[SerializeField] Button nextClass;
	[SerializeField] Button prevClass;
	[SerializeField] Button selectClass;
    // Start is called before the first frame update
    void Start() {
		classSelectPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
		if (startText.enabled) {
			if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown("joystick button 0")) && !activePlayer) {
				activePlayer = true;
				GameManager.instance.PlayerCount++;
				playerID = GameManager.instance.PlayerCount;
				startText.enabled = false;
				classSelectPanel.SetActive(true);

				if (Input.GetKeyDown(KeyCode.Space)) {
					keyboard = true;
				}
				if (Input.GetKeyDown("joystick button 0")) {
					controller = true;
					for(int i = 0; i < 1; i++) {
						if(Input.GetButtonDown("Joy" + i + "_A")) {
							Debug.Log("Joystick " + i + " detected");
							controllerID = i;
						}
					}
				}
			}
		}
		if (classSelectPanel.activeInHierarchy == true) {
			if (!currentClass) {
				classIndex = 0;
				ChangeClass();
				nextClass.onClick = new Button.ButtonClickedEvent();
				nextClass.onClick.AddListener(NextClass);
				prevClass.onClick = new Button.ButtonClickedEvent();
				prevClass.onClick.AddListener(PreviousClass);
				selectClass.onClick = new Button.ButtonClickedEvent();
				selectClass.onClick.AddListener(delegate { MenuOptions.menuOptions.ChooseClass(classIndex); });
				selectClass.onClick.AddListener(MenuOptions.menuOptions.LoadGame);
			}
		}
    }

	private void ChangeClass() {
		currentClass = adventurerModels[classIndex];
		currentName = adventurerNames[classIndex];
		adventurerModels[classIndex].SetActive(true);
		nameText.text = currentName;
		foreach(GameObject model in adventurerModels) {
			if(model != currentClass && model.activeInHierarchy) {
				model.SetActive(false);
			}
		}
	}

	private void NextClass() {
		if(classIndex < adventurerModels.Length - 1) {
			classIndex++;
			ChangeClass();
		} else {
			classIndex = 0;
			ChangeClass();
		}
	}

	private void PreviousClass() {
		if(classIndex == 0) {
			classIndex = adventurerModels.Length - 1;
			ChangeClass();
		} else {
			classIndex--;
			ChangeClass();
		}
	}
}
