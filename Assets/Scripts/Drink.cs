using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drink : MonoBehaviour
{
	[SerializeField] Bar bar;
	[SerializeField] SkillData skill;
	[SerializeField] Adventurer.AdventurerClass adventurerClass;
	[SerializeField] Canvas canvas;
	[SerializeField] GameObject panel;
	[SerializeField] List<Adventurer> adventurers;

	public Bar Bar { set { bar = value; } }
	public SkillData Skill { get { return skill; } set { skill = value; } }

    void Start()
    {
		canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
    }
	
    void Update()
    {
		if (!GameManager.instance.WaveBreak) {
			Destroy(gameObject);
		}
		if(adventurers.Count > 0) {
			Debug.Log("Adventurer count > 0");
			foreach(Adventurer adventurer in adventurers) {
				if(skill._AdventurerClass == adventurer._AdventurerClass) {
					Debug.Log("Adventurer class matches");
					if(skill._SkillType == SkillData.SkillType.Primary && skill != adventurer.PrimaryAttackData) {
						if ((adventurer.Joystick && Input.GetAxis(adventurer.InputData.Fire1) > 0) || (!adventurer.Joystick && Input.GetButtonDown(adventurer.InputData.Fire1))) {
							
						}
					} else if(skill._SkillType == SkillData.SkillType.Secondary && (skill != adventurer.SecondaryAttack01Data || skill != adventurer.SecondaryAttack02Data)) {
						if ((adventurer.Joystick && Input.GetAxis(adventurer.InputData.Fire2) > 0) || (!adventurer.Joystick && Input.GetButtonDown(adventurer.InputData.Fire2))) {
							
						}
					}
					/*
					else if ((adventurer.Joystick && Input.GetAxis(adventurer.InputData.Fire1) > 0) || (!adventurer.Joystick && Input.GetButtonDown(adventurer.InputData.Fire1))) {
						Debug.Log("Primary Drink == adventurers primary");	
					}
					*/
				}
			}
		}
    }

	private void OnTriggerEnter(Collider other) {
		if(other.gameObject.tag == "Adventurer") {
			adventurers.Add(other.GetComponent<Adventurer>());
		}
		if (!panel) {
			panel = Instantiate(GameManager.instance.DrinkPanel, GameManager.instance.Canvas.transform, false);
			panel.GetComponent<DrinkPanel>().Drink = this;
			float offsetPosY = transform.position.y + 2;
			RectTransform canvasRect = canvas.GetComponent<RectTransform>();
			Vector3 offsetPos = new Vector3(transform.position.x, offsetPosY, transform.position.z);
			Vector2 canvasPos;
			Vector2 screenPoint = Camera.main.WorldToScreenPoint(offsetPos);
			RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, null, out canvasPos);
			panel.transform.localPosition = canvasPos;
		} else if (!panel.activeInHierarchy) {
			panel.SetActive(true);
		}
	}

	private void OnTriggerExit(Collider other) {
		if (adventurers.Contains(other.GetComponent<Adventurer>())) {
			adventurers.Remove(other.GetComponent<Adventurer>());
		}
		if (panel.activeInHierarchy) {
			panel.SetActive(false);
		}
	}
}
