using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	[Header("Arrays")]
	[SerializeField] List<GameObject> adventurers;
	[SerializeField] List<int> playerClasses;
	[SerializeField] Vector3[] enemySpawns;
	[SerializeField] Vector3[] adventurerSpawns;
	[SerializeField] SkillData[] primarySkills;
	//0 - Fighter | 1 - Mage | 2 - Ranger
	[SerializeField] ClassData[] characters;
	//0 - Zombie | 1 - Skeleton
	[SerializeField] EnemyData[] enemies;

	[Header("Floats & Integers")]
	[SerializeField] float time;
	[SerializeField] int playerCount;

	[Header("Prefabs")]
	[SerializeField] GameObject adventurer;
	[SerializeField] GameObject enemy;

	[Header("UI")]
	[SerializeField] GameObject playerUI;
	[SerializeField] Canvas canvas;

	[Header("Debug Tools")]
	[SerializeField] bool spawnEnemies;
	[SerializeField] bool freezeZombies;

	public static GameManager instance;
	float spawnTime;

	public List<GameObject> Adventurers { get { return adventurers; } set { adventurers = value; } }
	public List<int> PlayerClasses { get { return playerClasses; } set { playerClasses = value; } }
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
		if (SceneManager.GetActiveScene().buildIndex == 1) {
			if (!canvas) { canvas = Canvas.FindObjectOfType<Canvas>(); }
			if(adventurers.Count < playerCount) {
				for(int i = 0; i < playerCount; i++) {
					Adventurer newAd = Instantiate(adventurer, adventurerSpawns[i], Quaternion.identity).GetComponent<Adventurer>();
					PlayerUI newUI = Instantiate(playerUI, canvas.transform, false).GetComponent<PlayerUI>();
					newAd.CharacterClassData = characters[playerClasses[i]];
					newAd.PlayerNumber = i + 1;
					newUI.Player = newAd;
					newUI.SetToPlayer();
				}
			}
			time += Time.deltaTime;
			if ((time >= spawnTime) && spawnEnemies) { SpawnEnemy(); }
			if (Input.GetKeyDown(KeyCode.Z)) {
				SpawnZombie();
			}
			if (Input.GetKeyDown(KeyCode.X)) {
				SpawnSkeleton();
			}
		}
	}

	void SpawnEnemy() {
		float enemySwitch = Random.Range(0, 2);
		Debug.Log("Enemy Switch: " + enemySwitch);
		switch (enemySwitch) {
			case 0: SpawnZombie(); break;
			case 1: SpawnSkeleton(); break;
			default: Debug.Log("Spawning Error!"); break;

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