using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Menutee {
	public class TextVersion : MonoBehaviour {

		public TextMeshProUGUI Text;

		// Start is called before the first frame update
		void Start() {
			if (Text != null) {
				Text.SetText("v. " + Application.version);
			} else {
				Debug.LogWarning("No text attached to TextVersion!");
			}
		}

		// Update is called once per frame
		void Update() {

		}
	}
}