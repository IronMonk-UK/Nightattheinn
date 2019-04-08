using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	[Header("Arrays")]
	[SerializeField] Vector3[] zombieSpawns;
	[SerializeField] GameObject[] adventurers;
	[SerializeField] SkillData[] primarySkills;
	//0 - Mage | 1 - Ranger | 2 - Warrior
	[SerializeField] ClassData[] characters;

	[Header("Floats & Integers")]
	[SerializeField] float time;

	[Header("Prefabs")]
	[SerializeField] GameObject zombie;

	[Header("Debug Tools")]
	[SerializeField] bool spawnZombies;

	public static GameManager gm;
	float spawnTime;

	public GameObject[] Adventurers { get { return adventurers; } }
	public float _Time { get { return time; } }
	public SkillData[] PrimarySkills { get { return primarySkills; } }
	public ClassData[] Characters { get { return characters; } }

	void Awake() {
		gm = this;
		DontDestroyOnLoad(gameObject);
		adventurers = GameObject.FindGameObjectsWithTag("Adventurer");
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
		Instantiate(zombie, zombieSpawns[i], Quaternion.identity);
	}
}
