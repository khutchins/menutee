using Ratferences;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsManager : ReferenceBindingSave<Options> {

	public static Options GetOptions() {
		if (SavedOptions == null) {
			SavedOptions = Options.Load();
		}

		return SavedOptions;
	}

	private static Options SavedOptions;

	public OptionsSO Opts;

	private void Awake() {
		// These bindings have to be set up explicity, but they will automatically load at start
		// (if the object is on the scene) and will save if a reference changes (unless it's set up
		// not to). You will need to implement serialization yourself in Options.
		AddFloatBinding(Opts.Option1, (newVal, opts) => opts.Option1 = newVal, (floatRef, opts) => floatRef.Value = opts.Option1);
		AddBoolBinding(Opts.Option2, (newVal, opts) => opts.Option2 = newVal, (boolRef, opts) => boolRef.Value = opts.Option2);
		AddIntBinding(Opts.Option3, (newVal, opts) => opts.Option3 = newVal, (intRef, opts) => intRef.Value = opts.Option3);
		AddFloatBinding(Opts.MusicVolume, (newVal, opts) => opts.MusicVolume = newVal, (floatRef, opts) => floatRef.Value = opts.MusicVolume);
		AddFloatBinding(Opts.SFXVolume, (newVal, opts) => opts.SFXVolume = newVal, (floatRef, opts) => floatRef.Value = opts.SFXVolume);

		SetInitialSOValues();
	}

	protected override Options GetSave() {
		return GetOptions();
	}
}