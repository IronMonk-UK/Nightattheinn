using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New UI Data", menuName = "UI Data")]
public class UIData : ScriptableObject {
	[SerializeField] GameObject uiPanel;
	[SerializeField] Text healthText;
	[SerializeField] Slider healthSlider;
	[SerializeField] Image healthFill;
	[SerializeField] Text manaText;
	[SerializeField] Slider manaSlider;
	[SerializeField] Image manaFill;
}
