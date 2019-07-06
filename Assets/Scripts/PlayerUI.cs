using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

	[Header("Health")]
	[SerializeField] Text healthText;
	[SerializeField] Slider healthSlider;
	[SerializeField] Image healthFill;
	[Header("Mana")]
	[SerializeField] Text manaText;
	[SerializeField] Slider manaSlider;
	[SerializeField] Image manaFill;
	[SerializeField] Text manaCostText;
	[Header("Primary Skill")]
	[SerializeField] Text skill01Text;
	[SerializeField] Slider skill01Slider;
	[SerializeField] Image skill01Fill;
	[Header("Secondary Skill")]
	[SerializeField] Text skill02Text;
	[SerializeField] Slider skill02Slider;
	[SerializeField] Image skill02Fill;
	[Header("Misc")]
	[SerializeField] Adventurer player;
	[SerializeField] Text killCount;

	

	public Text HealthText { get { return healthText; } }
	public Slider HealthSlider { get { return healthSlider; } }
	public Image HealthFill { get { return healthFill; } }
	public Text ManaText { get { return manaText; } }
	public Slider ManaSlider { get { return manaSlider; } }
	public Image ManaFill { get { return manaFill; } }
	public Text ManaCostText { get { return manaCostText; } }
	public Text Skill01Text { get { return skill01Text; } }
	public Slider Skill01Slider { get { return skill01Slider; } }
	public Image Skill01Fill { get { return skill01Fill; } }
	public Text Skill02Text { get { return skill02Text; } }
	public Slider Skill02Slider { get { return skill02Slider; } }
	public Image Skill02Fill { get { return skill02Fill; } }
	public Adventurer Player { get { return player; } set { player = value; } }
	public Text KillCount { get { return killCount; } }


	public void SetToPlayer() { player.UI = this; }
}
