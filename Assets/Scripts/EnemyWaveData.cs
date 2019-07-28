using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Wave Data", menuName = "Enemy Wave Data")]
public class EnemyWaveData : ScriptableObject{
	[Header("Enemies and Spawn Points")]
	[SerializeField] EnemyData[] enemyData;
	[SerializeField] GameObject[] enemySpawns;
	[Header("Wave Order")]
	[SerializeField] EnemyData[] enemyOrder;
	[SerializeField] GameObject[] spawnOrder;
	
	public EnemyData[] EnemyOrder { get { return enemyOrder; } }
	public GameObject[] SpawnOrder { get { return spawnOrder; } }
}
