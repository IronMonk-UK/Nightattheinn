/* 
 * Copyright (c) Iron Monk Studios 2022
 * www.ironmonkstudios.co.uk 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bar : MonoBehaviour
{
	[SerializeField] GameObject[] drinkPositions;
	[SerializeField] bool drinksReady;
	[SerializeField] GameObject drink;
	[SerializeField] bool classesSet;
	[SerializeField] List<Adventurer.AdventurerClass> potentialClasses;
	[SerializeField] int classesCount;
	[SerializeField] SkillSetData[] skillSets;

    void Start()
    {       
    }
	
    void Update()
    {
		if (!classesSet && GameManager.instance.Adventurers.Count > 0) { GetClasses(); }
		if (GameManager.instance.WaveBreak && !drinksReady) {
			PrepareDrinks();
		}
		if (!GameManager.instance.WaveBreak && drinksReady) { drinksReady = false; }
    }

	private void GetClasses() {
		Debug.Log("Getting classes");
        foreach (GameObject adventurer in GameManager.instance.Adventurers) {
			if (!potentialClasses.Contains(adventurer.GetComponent<Adventurer>()._AdventurerClass)) {
				potentialClasses.Add(adventurer.GetComponent<Adventurer>()._AdventurerClass);
				classesCount++;
			}
		}
		skillSets = new SkillSetData[classesCount];
		int index = 0;
		foreach(Adventurer.AdventurerClass potentialClass in potentialClasses) {
			foreach(SkillSetData skillSet in GameManager.instance.ClassSkills) {
				if(skillSet.ClassSet == potentialClass) {
					skillSets[index] = skillSet;
					index++;
				}
			}
		}
		classesSet = true;
	}

	void PrepareDrinks() {
		drinksReady = true;
		foreach(GameObject drinkPosition in drinkPositions) {
			Drink localDrink = Instantiate(drink, drinkPosition.transform.position, Quaternion.identity).GetComponent<Drink>();
			localDrink.Bar = this;
			SkillSetData skillSet;
			skillSet = skillSets[UnityEngine.Random.Range(0, skillSets.Length)];
			Debug.Log("Skill Set: " + skillSet + " Skills Count: " + skillSet.Skills.Length);
			localDrink.Skill = skillSet.Skills[UnityEngine.Random.Range(0, skillSet.Skills.Length)];
		}
	}
}
