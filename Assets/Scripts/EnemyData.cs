using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Data", menuName = "Enemy Data")]
public class EnemyData : ScriptableObject {
	[Header("-Generic Variables-")]
	[SerializeField] Material colour;
	[SerializeField] GameObject model;
	[SerializeField] string enemyName;
	[SerializeField] float speed;
	[SerializeField] int health;
	[SerializeField] int damage;
	[SerializeField] float cooldown;
	[SerializeField] bool skirmisher;

	[SerializeField] bool ranged; //True - Ranged unit | False - Melee unit
	[SerializeField] float thrust;
	[SerializeField] GameObject bulletPrefab;

	[SerializeField] float attackDegrees;
	[SerializeField] float attackRadius;
	
	public string EnemyName { get { return enemyName; } }
	public int Damage { get { return damage; } }
	public float Speed { get { return speed; } }
	public int Health { get { return health; } }
	public float Cooldown { get { return cooldown; } }
	public bool Ranged { get { return ranged; } }
	public bool Skirmisher { get { return skirmisher; } }
	public float Thrust { get { return thrust; } }
	public GameObject BulletPrefab { get { return bulletPrefab; } }
	public Material Colour { get { return colour; } }
	public GameObject Model { get { return model; } }
	public float AttackDegrees { get { return attackDegrees; } }
	public float AttackRadius { get { return attackRadius; } }
}

