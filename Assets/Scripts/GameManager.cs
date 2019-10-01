using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	[Header("Arrays")]
	[SerializeField] List<GameObject> adventurers;
	[SerializeField] List<int> playerClasses;
	[SerializeField] List<InputData> playerInputs;
	[SerializeField] List<int> playerKills;
	[SerializeField] List<GameObject> activeEnemies;
	[SerializeField] Vector3[] adventurerSpawns;
	[SerializeField] SkillData[] primarySkills;
	//0 - Fighter | 1 - Mage | 2 - Ranger
	[SerializeField] ClassData[] characters;
	[SerializeField] EnemyWaveData[] waves;

	[Header("Floats & Integers")]
	[SerializeField] float time;
	[SerializeField] float minutes;
	[SerializeField] float seconds;
	[SerializeField] float breakEndTime;
	[SerializeField] float waveBreakLength;
	[SerializeField] int playerCount;
	[SerializeField] int minSpawnTime;
	[SerializeField] int maxSpawnTime;
	[SerializeField] int currentWave;
	[SerializeField] int waveProgress;

	[Header("Bools")]
	[SerializeField] bool waveBreak;

	[Header("Camera")]
	[SerializeField] bool cameraTransSet;
	[SerializeField] Vector3 cameraTransform;

	[Header("Strings")]
	[SerializeField] string timeString;

	[Header("Prefabs")]
	[SerializeField] GameObject adventurer;
	[SerializeField] GameObject enemy;

	[Header("UI")]
	[SerializeField] GameObject[] playerUis;
	[SerializeField] GameObject[] gameOverUis;
	[SerializeField] GameObject clockUI;
	[SerializeField] Text clock = null;
	[SerializeField] Canvas canvas;
	[SerializeField] Button restartBtn, mainMenuBtn, quitBtn;

	[Header("Debug Tools")]
	[SerializeField] bool spawnEnemies;
	[SerializeField] bool freezeEnemies;

	private bool allAdventurersDown() {
		if (adventurers.Count == 0) { return false; }
		foreach(GameObject adventurer in adventurers) {
			Debug.Log("Checking " + adventurer.GetComponent<Adventurer>().PlayerNumber);
			if (!adventurer.GetComponent<Adventurer>().Downed) {
				Debug.Log("Returning false");
				return false;
			}
		}
		Debug.Log("Returning true");
		return true;
	}

	public static GameManager instance;
	float spawnTime;

	public List<GameObject> Adventurers { get { return adventurers; } set { adventurers = value; } }
	public List<int> PlayerClasses { get { return playerClasses; } set { playerClasses = value; } }
	public List<InputData> PlayerInputs { get { return playerInputs; } set { playerInputs = value; } }
	public List<GameObject> ActiveEnemies { get { return activeEnemies; } set { activeEnemies = value; } }
	public float _Time { get { return time; } set { time = value; } }
	public SkillData[] PrimarySkills { get { return primarySkills; } }
	public ClassData[] Characters { get { return characters; } }
	public int PlayerCount { get { return playerCount; } set { playerCount = value; } }
	public bool WaveBreak { get { return waveBreak; } }

	void Awake() {
		DontDestroyOnLoad(gameObject);
		if (instance == null) {
			instance = this;
		} else {
			Destroy(gameObject);
		}
		playerCount = 0;
		ResetVariables();
	}

	void Start() {
		
	}

	void Update() {
		if (playerClasses.Capacity < playerCount) {
			playerClasses.Capacity = playerCount;
		}
		if (SceneManager.GetActiveScene().buildIndex == 1) {
			if (!cameraTransSet) {
				cameraTransform = Camera.main.transform.position;
				cameraTransSet = true;
			}
			if (!canvas) { canvas = Canvas.FindObjectOfType<Canvas>(); }
			if (adventurers.Count < playerCount) {
				for(int i = 0; i < playerCount; i++) {
					Debug.Log("Player Classes Count" + playerClasses.Count);
					Adventurer newAd = Instantiate(adventurer, adventurerSpawns[i], Quaternion.identity).GetComponent<Adventurer>();
					PlayerUI newUI = Instantiate(playerUis[i], canvas.transform, false).GetComponent<PlayerUI>();
					newAd.CharacterClassData = characters[playerClasses[i]];
					newAd.InputData = playerInputs[i];
					if (playerInputs[i].AButton != "Submit") {
						newAd.Joystick = true;
					}
					newAd.PlayerNumber = i + 1;
					newUI.Player = newAd;
					newUI.SetToPlayer();
				}
			}
			SetCameraPos();

			time += Time.deltaTime;
			minutes = Mathf.FloorToInt(time / 60);
			seconds = Mathf.FloorToInt(time - minutes * 60);
			timeString = string.Format("{0:00}:{1:00}", minutes, seconds);
			if (!clock) {
				clock = Instantiate(clockUI, canvas.transform, false).GetComponent<Text>();
			}
			clock.text = timeString;
			if (!waveBreak) {
				if ((time >= spawnTime || activeEnemies.Count == 0) && currentWave <= waves.Length) {
					SpawnEnemy();
				} else if (currentWave > waves.Length) {
					GameOver();
				}
				if (allAdventurersDown()) {
					GameOver();
				}
			} else if(time > breakEndTime) {
				Debug.Log("Ending Wave Break");
				waveBreak = false;
			}
		}
		if (SceneManager.GetActiveScene().buildIndex == 2) {
			if (!canvas) {
				canvas = Canvas.FindObjectOfType<Canvas>();
			}
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
			for(int i = 0; i < adventurers.Count; i++) {
				GameOver newUI = Instantiate(gameOverUis[i], canvas.transform, false).GetComponent<GameOver>();
				newUI.PlayerText.text = "Player " + (i + 1);
				newUI.TimeText.text = "Time: " + timeString;
				newUI.KillText.text = "Kills: " + playerKills[i];
			}
			ResetVariables();
		}
	}

	private void SetCameraPos() {
		Vector3 cameraPos = Vector3.zero;
		foreach(GameObject adventurer in adventurers) {
			Vector3 adPos = new Vector3(cameraTransform.x + adventurer.transform.position.x, cameraTransform.y, cameraTransform.z + adventurer.transform.position.z);
			cameraPos += adPos;
		}
		cameraPos /= adventurers.Count;
		if (cameraPos != Vector3.zero && cameraPos.y > 1) {
			Camera.main.transform.position = cameraPos;
		}
	}

	public void GameOver() {
		foreach (GameObject adventurer in adventurers) {
			playerKills.Add(adventurer.GetComponent<Adventurer>().KillCount);
		}
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
		Debug.Log("Current Wave: " + currentWave + " Wave total: " + waves.Length);
		if (currentWave - 1 <= waves.Length && spawnEnemies) {
			Debug.Log("Wave Progress: " + waveProgress + " Wave Length: " + waves[currentWave - 1].EnemyOrder.Length);
			if (waveProgress < waves[currentWave - 1].EnemyOrder.Length) {
				EnemyData currentEnemy = waves[currentWave - 1].EnemyOrder[waveProgress];
				GameObject spawnLocation = waves[currentWave - 1].SpawnOrder[waveProgress];
				Enemy enemyClass = Instantiate(enemy, spawnLocation.transform.position, Quaternion.identity).GetComponent<Enemy>();
				enemyClass._EnemyData = currentEnemy;
				activeEnemies.Add(enemyClass.gameObject);
				if (freezeEnemies) { enemyClass.FollowAdventurers = false; }
				spawnTime = time + Random.Range(minSpawnTime, maxSpawnTime);
				waveProgress++;
			} else {
				Debug.Log("Wave Finished Spawning");
			}
			if (waveProgress >= waves[currentWave - 1].EnemyOrder.Length && activeEnemies.Count == 0) {
				StartWaveBreak();
			}
		}
	}

	private void StartWaveBreak() {
		Debug.Log("Starting Wave Break");
		currentWave++;
		if (currentWave > waves.Length) {
			GameOver();
		}
		breakEndTime = time + waveBreakLength;
		waveBreak = true;
		waveProgress = 0;
	}
}