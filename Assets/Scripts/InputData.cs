using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Input Data", menuName = "Input Data")]
public class InputData : ScriptableObject
{
	[SerializeField] string leftHorizontal;
	[SerializeField] string leftVertical;
	[SerializeField] string rightHorizontal;
	[SerializeField] string rightVertical;
	[SerializeField] string fire1;
	[SerializeField] string fire2;
	[SerializeField] string fire3;
	[SerializeField] string aButton;
	[SerializeField] string bButton;
}
