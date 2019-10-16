using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Data", menuName = "Enemy Data")]
public class EnemyData : ScriptableObject {
	[Header("-Generic Variables-")]
	[SerializeField] Enemy.EnemyClass enemyClass;
	[SerializeField] Material material;
	[SerializeField] GameObject[] models;
	[SerializeField] string enemyName;
	[SerializeField] float speed;
	[SerializeField] int health;
	[SerializeField] int damage;
	[SerializeField] float cooldown;
	[SerializeField] bool skirmisher;
	[SerializeField] int skirmishDistance;
	[SerializeField] int goldValue;

	[SerializeField] bool ranged; //True - Ranged unit | False - Melee unit
	[SerializeField] float thrust;
	[SerializeField] GameObject bulletPrefab;

	[SerializeField] float attackRadius;
	[SerializeField] float attackRange;

	[SerializeField] AudioClip[] audioClips;

	[SerializeField] SkillData[] bossAttacks;
	
	public Enemy.EnemyClass _EnemyClass { get { return enemyClass; } }
	public string EnemyName { get { return enemyName; } }
	public int Damage { get { return damage; } }
	public float Speed { get { return speed; } }
	public int Health { get { return health; } }
	public float Cooldown { get { return cooldown; } }
	public bool Ranged { get { return ranged; } }
	public bool Skirmisher { get { return skirmisher; } }
	public int SkirmishDistance { get { return skirmishDistance; } }
	public float Thrust { get { return thrust; } }
	public GameObject BulletPrefab { get { return bulletPrefab; } }
	public Material _Material { get { return material; } }
	public GameObject[] Models { get { return models; } }
	public float AttackRadius { get { return attackRadius; } }
	public float AttackRange { get { return attackRange; } }
	public AudioClip[] _AudioClips { get { return audioClips; } }
	public int GoldValue { get { return goldValue; } }
	public SkillData[] BossAttacks { get { return bossAttacks; } }
}

