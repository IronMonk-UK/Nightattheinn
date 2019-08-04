using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
	[SerializeField] Text playerText;
	[SerializeField] Text timeText;
	[SerializeField] Text killText;

	public Text PlayerText { get { return playerText; } set { playerText = value; } }
	public Text TimeText { get { return timeText; } set { timeText = value; } }
	public Text KillText { get { return killText; } set { killText = value; } }
}
