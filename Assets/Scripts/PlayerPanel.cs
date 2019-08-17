using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
	[SerializeField] MainMenu menuScript;
	[SerializeField] bool ready;
	[SerializeField] int startRotation;
	[SerializeField] int rotateSpeed;
	[SerializeField] InputData input;

	public bool Ready { get { return ready; } set { ready = value; } }
	public bool Keyboard { get { return keyboard; } set { keyboard = value; } }
	public int ClassIndex { get { return classIndex; } }
	public InputData _Input { get { return input; } }
    // Start is called before the first frame update
    void Start() {
		classSelectPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
		if (startText.enabled) {
			SetInput();
		}
		if (classSelectPanel.activeInHierarchy == true) {
			ClassSelectPanel();
		}
	}

	private void ClassSelectPanel() {
		if (!currentClass) {
			classIndex = 0;
			EventSystem.current.SetSelectedGameObject(selectClass.gameObject);
			ChangeClass();
			nextClass.onClick = new Button.ButtonClickedEvent();
			nextClass.onClick.AddListener(NextClass);
			prevClass.onClick = new Button.ButtonClickedEvent();
			prevClass.onClick.AddListener(PreviousClass);
			selectClass.onClick = new Button.ButtonClickedEvent();
			selectClass.onClick.AddListener(SetReady);
		} else if (currentClass) {
			currentClass.transform.Rotate(new Vector3(0, rotateSpeed * Time.deltaTime, 0));
		}
	}

	private void SetInput() {		
		if (((Input.GetKeyDown(KeyCode.Space) && !menuScript.KeyboardTaken) || Input.GetKeyDown("joystick button 0")) && !activePlayer) {
			bool controllerTaken = false;			
			if (Input.GetKeyDown("joystick button 0")) {
				for (int i = 0; i < 3; i++) {
					if (menuScript.JoysticksTaken[i]) {
						Debug.Log("Controller taken");
						controllerTaken = true;
					}
					if (Input.GetButtonDown("Joy" + i + "_Submit") && !menuScript.JoysticksTaken[i]) {
						Debug.Log("Joystick " + i + " detected");
						controller = true;
						controllerID = i;
						input = menuScript.Joysticks[i];
						menuScript.JoysticksTaken[i] = true;
						Debug.Log("Joystick Taken: " + menuScript.JoysticksTaken[i]);
					}
				}
			}
			if (Input.GetKeyDown(KeyCode.Space)) {
				keyboard = true;
				input = menuScript.Keyboard;
			}
			if (!controllerTaken) {
				activePlayer = true;
				GameManager.instance.PlayerCount++;
				playerID = GameManager.instance.PlayerCount;
				startText.enabled = false;
				classSelectPanel.SetActive(true);
				menuScript.Players.Add(this);
			}
		}
	}

	private void ChangeClass() {
		currentClass = adventurerModels[classIndex];
		currentName = adventurerNames[classIndex];
		adventurerModels[classIndex].SetActive(true);
		nameText.text = currentName;
		currentClass.transform.rotation = Quaternion.Euler(0, startRotation, 0);
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

	private void SetReady() {
		if (!ready) {
			ready = true;
			selectClass.GetComponentInChildren<Text>().text = "Ready!";
		} else {
			ready = false;
			selectClass.GetComponentInChildren<Text>().text = "Select";
		}
	}
}
