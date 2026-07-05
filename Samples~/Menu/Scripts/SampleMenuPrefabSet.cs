using UnityEngine;

/// <summary>
/// A shareable set of element prefabs used to build menus.
///
/// To add prefabs for element types beyond the ones below, just add them
/// here. You may also want to change the meta file so reimporting the sample
/// doesn't overwrite the reference to this.
/// </summary>
[CreateAssetMenu(menuName = "Menutee/Sample Menu Prefab Set")]
public class SampleMenuPrefabSet : ScriptableObject {
	public GameObject ButtonPrefab;
	public GameObject SliderPrefab;
	public GameObject TogglePrefab;
	public GameObject DropdownPrefab;
	public GameObject TextPrefab;
	public GameObject OptionSelectPrefab;
}
