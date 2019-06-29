using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

	[SerializeField] Adventurer player;
	[SerializeField] Text healthText;
	[SerializeField] Slider healthSlider;
	[SerializeField] Image healthFill;
	[SerializeField] Text manaText;
	[SerializeField] Slider manaSlider;
	[SerializeField] Image manaFill;
	
	public Adventurer Player { get { return player; } set { player = value; } }

	public Text HealthText { get { return healthText; } }
	public Slider HealthSlider { get { return healthSlider; } }
	public Image HealthFill { get { return healthFill; } }
	public Text ManaText { get { return manaText; } }
	public Slider ManaSlider { get { return manaSlider; } }
	public Image ManaFill { get { return manaFill; } }

	public void SetToPlayer() {
		player.UI = this;
	}
}
