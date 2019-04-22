using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill Data", menuName = "Skill Data")]
public class SkillData : ScriptableObject{

	[SerializeField] string skillName;
	[SerializeField] int damage;
	[SerializeField] GameObject bulletPrefab;
	[SerializeField] SkillType skillType;
	[SerializeField] Adventurer.AdventurerClass adventurerClass;

	public string SkillName { get { return skillName; } }
	public int Damage { get { return damage; } }
	public GameObject BulletPrefab { get { return bulletPrefab; } }
	public SkillType _SkillType { get { return skillType; } }
	public Adventurer.AdventurerClass _AdventurerClass { get { return adventurerClass; } }

	public enum SkillType { Primary, Secondary, CC }

	public void RangedAttack(Vector3 position, Quaternion rotation, float eulerY, GameObject adventurer) {
		Vector3 spawnPoint = position + (rotation * new Vector3(0.75f, 0, -0.75f));
		Bullet bullet = Instantiate(bulletPrefab, spawnPoint, Quaternion.Euler(90, eulerY - 45, 0)).GetComponent<Bullet>();
		bullet.Adventurer = adventurer;
		bullet.Damage = damage;
	}

	public void MeleeAttack(Animator anim) {
		Debug.Log("Warrior should attack");
		anim.SetTrigger("Attack");
	}

}
