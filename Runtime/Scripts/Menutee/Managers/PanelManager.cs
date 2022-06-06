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
        public UIElementManager[] ElementManagers;
        [HideInInspector]
        public MenuManager Manager;
        [HideInInspector]
        public PanelConfig Config;

        public void SetPanelActive(bool active) {
            gameObject.SetActive(active);
            if (active) {
                if (Config != null && Config.OnDisplayCallback != null) {
                    Config.OnDisplayCallback(this.gameObject, this);
                }
                foreach (var manager in ElementManagers) {
                    if (manager.PanelObjectConfig != null && manager.PanelObjectConfig.OnDisplayCallback != null) {
                        manager.PanelObjectConfig.OnDisplayCallback(manager.gameObject, manager);
                    }
                }
            }
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