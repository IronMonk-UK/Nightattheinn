using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drink : MonoBehaviour
{
	[SerializeField] Bar bar;

	public Bar Bar { set { bar = value; } }

    void Start()
    {
        
    }
	
    void Update()
    {
		if (!GameManager.instance.WaveBreak) {
			Destroy(gameObject);
		}
    }

	private void OnTriggerEnter(Collider other) {
		Debug.Log("Adventurer entered area: Trigger");
	}
}
