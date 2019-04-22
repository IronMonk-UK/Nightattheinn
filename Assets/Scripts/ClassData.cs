using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character Data", menuName = "Character Data")]
public class ClassData : ScriptableObject
{
	[Header("Class Name")]
	[SerializeField] string className;
	[Header("Floats & Integers")]
	[SerializeField] float movementSpeed;
	[SerializeField] float attackSpeed;
	[SerializeField] int health;
	[SerializeField] int mana;
	[Header("Skills")]
	[SerializeField] SkillData primarySkill;
	[SerializeField] SkillData secondarySkill;
	[SerializeField] Adventurer.AdventurerClass adventurerClass;
	[SerializeField] GameObject weapon;

	public string ClassName { get { return className; } }
	public float MovementSpeed { get { return movementSpeed; } }
	public float AttackSpeed { get { return attackSpeed; } }
	public int Health { get { return health; } }
	public int Mana { get { return mana; } }
	public SkillData PrimarySkill { get { return primarySkill; } }
	public SkillData SecondarySkill { get { return secondarySkill; } }
	public Adventurer.AdventurerClass _AdventurerClass { get { return adventurerClass; } }
	public GameObject Weapon { get { return weapon; } }


}
