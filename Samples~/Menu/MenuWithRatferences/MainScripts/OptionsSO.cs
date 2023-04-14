using System.Collections;
using System.Collections.Generic;
using Ratferences;
using UnityEngine;

[CreateAssetMenu(menuName = "KH/Example/Options")]
public class OptionsSO : ScriptableObject {
	public FloatReference Option1;
	public BoolReference Option2;
	public IntReference Option3;

	public FloatReference SFXVolume;
	public FloatReference MusicVolume;
}
