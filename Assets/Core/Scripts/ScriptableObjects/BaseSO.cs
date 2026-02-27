
using UnityEngine;

public class BaseSO : ScriptableObject
{
	// description field that appears as a multi-line text area in the Inspector
	[SerializeField, TextArea] string description;
}