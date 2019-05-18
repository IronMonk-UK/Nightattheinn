using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	[Header("Arrays")]
	[SerializeField] List<GameObject> adventurers;
	[SerializeField] Vector3[] enemySpawns;
	[SerializeField] SkillData[] primarySkills;
	//0 - Mage | 1 - Ranger | 2 - Warrior
	[SerializeField] ClassData[] characters;
	//0 - Zombie | 1 - Skeleton
	[SerializeField] EnemyData[] enemies;

	[Header("Floats & Integers")]
	[SerializeField] float time;

	[Header("Prefabs")]
	[SerializeField] GameObject enemy;

	[Header("Debug Tools")]
	[SerializeField] bool spawnEnemies;
	[SerializeField] bool freezeZombies;

	public static GameManager instance;
	float spawnTime;

	public List<GameObject> Adventurers { get { return adventurers; } set { adventurers = value; } }
	public float _Time { get { return time; } }
	public SkillData[] PrimarySkills { get { return primarySkills; } }
	public ClassData[] Characters { get { return characters; } }

	void Awake() {
		DontDestroyOnLoad(gameObject);
		if(instance == null) { instance = this; }
		spawnTime = Random.Range(1, 10);
	}

	void Start() {
		
	}
	
	void Update() {
		time += Time.deltaTime;
		if((time >= spawnTime) && spawnEnemies) { SpawnZombie(); }
		if (Input.GetKeyDown(KeyCode.Z)) {
			SpawnZombie();
		}
		if (Input.GetKeyDown(KeyCode.X)) {
			SpawnSkeleton();
		}
	}

	void SpawnZombie() {
		spawnTime = time + Random.Range(1, 10);
		int i = Random.Range(0, enemySpawns.Length);
		Enemy enemyClass = Instantiate(enemy, enemySpawns[i], Quaternion.identity).GetComponent<Enemy>();
		enemyClass._EnemyData = enemies[0];
		if(freezeZombies) {
			enemyClass.FollowAdventurers = false;
		}
	}

	void SpawnSkeleton() {
		spawnTime = time + Random.Range(1, 10);
		int i = Random.Range(0, enemySpawns.Length);
		Enemy enemyClass = Instantiate(enemy, enemySpawns[i], Quaternion.identity).GetComponent<Enemy>();
		enemyClass._EnemyData = enemies[1];
		if(freezeZombies) {
			enemyClass.FollowAdventurers = false;
		}
	}
}