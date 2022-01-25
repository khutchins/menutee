using UnityEngine;
using UnityEngine.UI;

namespace Menutee {
	/// <summary>
	/// Updates the given selectable's palette to match the config.
	/// Useful for non-dynamic elements that are still supposed to
	/// be themed the same.
	/// </summary>
	public class UpdateFromPaletteConfig : MonoBehaviour {
		public PaletteConfig Palette;

		private void Awake() {
			Palette?.ApplyToSelectable(GetComponent<Selectable>());
		}
	}
}