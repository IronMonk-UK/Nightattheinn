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

	public string LeftHorizontal { get { return leftHorizontal; } }
	public string LeftVertical { get { return leftVertical; } }
	public string RightHorizontal { get { return rightHorizontal; } }
	public string RightVertical { get { return rightVertical; } }
	public string Fire1 { get { return fire1; } }
	public string Fire2 { get { return fire2; } }
	public string Fire3 { get { return fire3; } }
	public string AButton { get { return aButton; } }
	public string BButton { get { return bButton; } }
}
