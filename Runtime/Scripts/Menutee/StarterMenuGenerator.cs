using UnityEngine;

namespace Menutee {
	/// <summary>
	/// Transition aid for the old lMenuGenerator approach.
	/// </summary>
	public class StarterMenuGenerator : MenuGenerator {
		[Header("Prefabs")]
		public GameObject ButtonPrefab;
		public GameObject SliderPrefab;
		public GameObject TogglePrefab;
		public GameObject DropdownPrefab;

		[Header("Appearance")]
		[Tooltip("Palette to be used for overriding selected, highlighted, etc. element states.")]
		public PaletteConfig PaletteConfig;
	}
}
