using System.Collections;
using System.Collections.Generic;
using Ratferences;
using UnityEngine;

[CreateAssetMenu(menuName = "Menutee/Sample/Options")]
public class SampleOptionsSO : ScriptableObject {
	public FloatReference Option1;
	public BoolReference Option2;
	public IntReference Option3;

	public FloatReference SFXVolume;
	public FloatReference MusicVolume;
}
