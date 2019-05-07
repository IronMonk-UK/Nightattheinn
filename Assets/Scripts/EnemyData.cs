using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Data", menuName = "Enemy Data")]
public class EnemyData : ScriptableObject {
	[Header("-Generic Variables-")]
	[SerializeField] string enemyName;
	[SerializeField] float speed;
	[SerializeField] int health;
	[SerializeField] int damage;
	[SerializeField] float cooldown;

	[SerializeField] float attackDegrees;
	[SerializeField] float attackRadius;
	
	public string EnemyName { get { return enemyName; } }
	public int Damage { get { return damage; } }
	public float Speed { get { return speed; } }
	public int Health { get { return health; } }
	public float Cooldown { get { return cooldown; } }
}

