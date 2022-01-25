using UnityEngine;
using UnityEngine.UI;

namespace Menutee {
    public class PanelManager : MonoBehaviour {
        [Tooltip("Parent object that panel objects will be placed in. If null, will use this game object.")]
        public GameObject Parent;

        [HideInInspector]
        public string Key;
        [HideInInspector]
        public GameObject DefaultInput;
        [HideInInspector]
        public GameObject[] OtherObjects;
        [HideInInspector]
        public MenuManager Manager;

        public void SetPanelActive(bool active) {
            gameObject.SetActive(active);
            if (OtherObjects != null) {
                foreach (GameObject obj in OtherObjects) {
                    obj.SetActive(active);
                }
            }
        }

        /// <summary>
        /// Pops the currently panel from the menu.
        /// </summary>
        public void Pop() {
            Manager.PopPanel();
		}

        /// <summary>
        /// Pushes the panel matching that key to the menu.
        /// </summary>
        /// <param name="keyToBePushed"></param>
        public void Push(string keyToBePushed) {
            Manager.PushPanel(keyToBePushed);
		}
    }
}