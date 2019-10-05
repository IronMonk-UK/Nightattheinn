using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Drink : MonoBehaviour
{
	[SerializeField] Bar bar;
	[SerializeField] SkillData skill;
	[SerializeField] Adventurer.AdventurerClass adventurerClass;
	[SerializeField] Canvas canvas;
	[SerializeField] GameObject panel;

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
    }

	private void OnTriggerEnter(Collider other) {
		if (!panel) {
			panel = Instantiate(GameManager.instance.DrinkPanel, GameManager.instance.Canvas.transform, false);
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
		if (panel.activeInHierarchy) {
			panel.SetActive(false);
		}
	}
}
