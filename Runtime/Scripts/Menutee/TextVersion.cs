using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Menutee {
	public class TextVersion : MonoBehaviour {

		public TextMeshProUGUI Text;
		[Tooltip("Format for version string. e.g. 'v{0}' for 'v1.0.0'. If null or empty, uses the format 'v. {0}'.")]
		public string Format;

		void Start() {
			if (Text != null) {
				string format = Format;
				if (format == null || format.Length == 0) {
					format = "v. {0}";
				}
				Text.SetText(string.Format(format, Application.version));
			} else {
				Debug.LogWarning("No text attached to TextVersion!");
			}
		}
	}
}