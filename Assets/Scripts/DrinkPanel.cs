/* 
 * Copyright (c) Iron Monk Studios 2022
 * www.ironmonkstudios.co.uk 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrinkPanel : MonoBehaviour
{
	[SerializeField] Drink drink;
	[SerializeField] Text drinkName;
	[SerializeField] Text drinkPrice;
	[SerializeField] bool setText;

	public Drink Drink { set { drink = value; } }
	
    void Update()
    {
		if (!setText) {
			drinkName.text = drink.Skill.SkillName;
			drinkPrice.text = drink.Skill.SkillCost.ToString();
			setText = true;
		}
    }
}
