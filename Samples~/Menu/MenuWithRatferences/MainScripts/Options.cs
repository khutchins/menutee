using Ratferences;
//using Sirenix.Serialization;
using System.IO;
using UnityEngine;

public class Options : ISavable {
	public float Option1 = 3;
	public bool Option2 = false;
	public int Option3 = 2;
	public float MusicVolume = 0.9f;
	public float SFXVolume = 1f;

	public void Save() {
		// Replace this with serialization code. Odin works well (and is what is used below).
		byte[] bytes = new byte[0];
		//byte[] bytes = SerializationUtility.SerializeValue(this, DataFormat.JSON);
		File.WriteAllBytes(SavePath(), bytes);
	}

	public static Options Load() {
		string path = SavePath();
		if (!File.Exists(path)) return new Options();
		byte[] bytes = File.ReadAllBytes(path);
		// Replace this with deserialization code. Odin works well (and is what is used below).
		return new Options();
		//return SerializationUtility.DeserializeValue<Options>(bytes, DataFormat.JSON);
	}

	private static string SavePath() {
		return Path.Combine(Application.persistentDataPath, "settings.sav");
	}
}
