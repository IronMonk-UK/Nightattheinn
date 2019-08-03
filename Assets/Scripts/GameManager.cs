using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	[Header("Arrays")]
	[SerializeField] List<GameObject> adventurers;
	[SerializeField] List<int> playerClasses;
	[SerializeField] List<int> playerKills;
	//[SerializeField] Vector3[] enemySpawns;
	[SerializeField] Vector3[] adventurerSpawns;
	[SerializeField] SkillData[] primarySkills;
	//0 - Fighter | 1 - Mage | 2 - Ranger
	[SerializeField] ClassData[] characters;
	//0 - Zombie | 1 - Skeleton
	//[SerializeField] EnemyData[] enemies;
	[SerializeField] EnemyWaveData[] waves;

	[Header("Floats & Integers")]
	[SerializeField] float time;
	[SerializeField] float minutes;
	[SerializeField] float seconds;
	[SerializeField] int playerCount;
	[SerializeField] int minSpawnTime;
	[SerializeField] int maxSpawnTime;
	[SerializeField] int currentWave;
	[SerializeField] int waveProgress;

	[Header("Strings")]
	[SerializeField] string timeString;

	[Header("Prefabs")]
	[SerializeField] GameObject adventurer;
	[SerializeField] GameObject enemy;

	[Header("UI")]
	[SerializeField] GameObject playerUI;
	[SerializeField] GameObject gameOverUI;
	[SerializeField] GameObject clockUI;
	[SerializeField] Text clock = null;
	[SerializeField] Canvas canvas;
	[SerializeField] Button restartBtn, mainMenuBtn, quitBtn;

	[Header("Debug Tools")]
	[SerializeField] bool spawnEnemies;
	[SerializeField] bool freezeEnemies;

	public static GameManager instance;
	float spawnTime;

	public List<GameObject> Adventurers { get { return adventurers; } set { adventurers = value; } }
	public List<int> PlayerClasses { get { return playerClasses; } set { playerClasses = value; } }
	public float _Time { get { return time; } set { time = value; } }
	public SkillData[] PrimarySkills { get { return primarySkills; } }
	public ClassData[] Characters { get { return characters; } }

	void Awake() {
		DontDestroyOnLoad(gameObject);
		if(instance == null) { instance = this; } else { Destroy(gameObject); }
		ResetVariables();
	}

	void Start() {
		
	}

	void Update() {
		Debug.Log("RH: " + Input.GetAxis("RStick Horizontal"));
		Debug.Log("RV: " + Input.GetAxis("RStick Vertical"));
		if (SceneManager.GetActiveScene().buildIndex == 1) {
			if (!canvas) { canvas = Canvas.FindObjectOfType<Canvas>(); }
			if(adventurers.Count < playerCount) {
				for(int i = 0; i < playerCount; i++) {
					Debug.Log("Player Classes Count" + playerClasses.Count);
					Adventurer newAd = Instantiate(adventurer, adventurerSpawns[i], Quaternion.identity).GetComponent<Adventurer>();
					PlayerUI newUI = Instantiate(playerUI, canvas.transform, false).GetComponent<PlayerUI>();
					newAd.CharacterClassData = characters[playerClasses[i]];
					newAd.PlayerNumber = i + 1;
					newUI.Player = newAd;
					newUI.SetToPlayer();
				}
			}
			time += Time.deltaTime;
			minutes = Mathf.FloorToInt(time / 60);
			seconds = Mathf.FloorToInt(time - minutes * 60);
			timeString = string.Format("{0:00}:{1:00}", minutes, seconds);
			if (!clock) {
				clock = Instantiate(clockUI, canvas.transform, false).GetComponent<Text>();
			}
			clock.text = timeString;
			//if ((time >= spawnTime) && spawnEnemies) { SpawnEnemy(); }
			if((time >= spawnTime) && (currentWave >= waves.Length)) {
				Debug.Log("Spawning Enemy");
				SpawnEnemy();
			}
			else if(currentWave < waves.Length) { Debug.Log("You are winrar!"); }
			/*
			if (Input.GetKeyDown(KeyCode.Z)) {
				SpawnZombie();
			}
			if (Input.GetKeyDown(KeyCode.X)) {
				SpawnSkeleton();
			}
			*/
		}
		if (SceneManager.GetActiveScene().buildIndex == 2) {
			if (!canvas) { canvas = Canvas.FindObjectOfType<Canvas>(); }
			if (!restartBtn) {
				restartBtn = GameObject.FindGameObjectWithTag("RestartBtn").GetComponent<Button>();
				restartBtn.onClick.AddListener(MenuOptions.menuOptions.LoadGame);
			}
			if (!mainMenuBtn) {
				mainMenuBtn = GameObject.FindGameObjectWithTag("MainMenuBtn").GetComponent<Button>();
				mainMenuBtn.onClick.AddListener(MenuOptions.menuOptions.Menu);
			}
			if (!quitBtn) {
				quitBtn = GameObject.FindGameObjectWithTag("QuitBtn").GetComponent<Button>();
				quitBtn.onClick.AddListener(MenuOptions.menuOptions.QuitGame);
			}
			foreach (GameObject adventurer in adventurers) {
				GameOverUI newUI = Instantiate(gameOverUI, canvas.transform, false).GetComponent<GameOverUI>();
				newUI.PlayerText.text = "Player " + (adventurers.IndexOf(adventurer) + 1);
				newUI.TimeText.text = "Time: " + timeString;
				newUI.KillText.text = "Kills: " + playerKills[adventurers.IndexOf(adventurer)];
			}
			ResetVariables();
		}
	}

	public void GameOver() {
		foreach (GameObject adventurer in adventurers) { playerKills.Add(adventurer.GetComponent<Adventurer>().KillCount); }
		SceneManager.LoadScene(2);
		canvas = null;
	}

	private void ResetVariables() {
		playerKills.Clear();
		adventurers.Clear();
		currentWave = 1;
		waveProgress = 0;
		time = 0;
		spawnTime = Random.Range(minSpawnTime, maxSpawnTime);
	}

	private void SpawnEnemy() {
		if(currentWave - 1 < waves.Length && spawnEnemies) {
			if(waveProgress < waves[currentWave - 1].EnemyOrder.Length) {
				EnemyData currentEnemy = waves[currentWave - 1].EnemyOrder[waveProgress];
				GameObject spawnLocation = waves[currentWave - 1].SpawnOrder[waveProgress];
				Enemy enemyClass = Instantiate(enemy, spawnLocation.transform.position, Quaternion.identity).GetComponent<Enemy>();
				enemyClass._EnemyData = currentEnemy;
				if(freezeEnemies) {
					enemyClass.FollowAdventurers = false;
				}
				spawnTime = time + Random.Range(minSpawnTime, maxSpawnTime);
				waveProgress++;
				Debug.Log("Wave Progress: " + waveProgress + " Wave Length: " + waves[currentWave - 1].EnemyOrder.Length);
				if(waveProgress >= waves[currentWave - 1].EnemyOrder.Length) {
					currentWave++;
					Debug.Log("Current Wave: " + currentWave);
				}
			} else {
				Debug.Log("Wave Finished Spawning");
			}
		}
	}

	/*
	private void SpawnEnemy() {
		int enemyIndex = Random.Range(0, 2);
		spawnTime = time + Random.Range(minSpawnTime, maxSpawnTime);
		int i = Random.Range(0, enemySpawns.Length);
		Enemy enemyClass = Instantiate(enemy, enemySpawns[i], Quaternion.identity).GetComponent<Enemy>();
		enemyClass._EnemyData = enemies[enemyIndex];
		if(freezeEnemies) {
			enemyClass.FollowAdventurers = false;
		}

	}
	*/
	/*
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
	*/
}