using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	[Header("Arrays")]
	[SerializeField] List<GameObject> adventurers;
	[SerializeField] Vector3[] zombieSpawns;
	[SerializeField] SkillData[] primarySkills;
	//0 - Mage | 1 - Ranger | 2 - Warrior
	[SerializeField] ClassData[] characters;

	[Header("Floats & Integers")]
	[SerializeField] float time;

	[Header("Prefabs")]
	[SerializeField] GameObject zombie;

	[Header("Debug Tools")]
	[SerializeField] bool spawnZombies;
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

	void Start () {
		
	}
	
	void Update () {
		time += Time.deltaTime;
		if((time >= spawnTime) && spawnZombies) { SpawnZombie(); }
	}

	void SpawnZombie() {
		spawnTime = time + Random.Range(1, 10);
		int i = Random.Range(0, zombieSpawns.Length);
		Enemy zombieClass = Instantiate(zombie, zombieSpawns[i], Quaternion.identity).GetComponent<Enemy>();
		if (freezeZombies) {
			zombieClass.followAdventurers = false;
		}
	}
}