using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bar : MonoBehaviour
{
	[SerializeField] GameObject[] drinkPositions;
	[SerializeField] bool drinksReady;
	[SerializeField] GameObject drink;

    void Start()
    {
        
    }
	
    void Update()
    {
		if (GameManager.instance.WaveBreak && !drinksReady) {
			PrepareDrinks();
		}
    }

	void PrepareDrinks() {
		foreach(GameObject drinkPosition in drinkPositions) {
			GameObject localDrink = Instantiate(drink, drinkPosition.transform.position, Quaternion.identity);
			localDrink.GetComponent<Drink>().Bar = this;
			drinksReady = true;
		}
	}
}
