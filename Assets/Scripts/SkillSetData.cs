using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill Set Data", menuName = "Skill Set Data")]
public class SkillSetData : ScriptableObject{
	[SerializeField] Adventurer.AdventurerClass classSet;
	[SerializeField] SkillData[] skills;

	public Adventurer.AdventurerClass ClassSet { get { return classSet; } }
	public SkillData[] Skills { get { return skills; } }
}

