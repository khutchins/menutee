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
            if (OtherObjects != null) {
                foreach (GameObject obj in OtherObjects) {
                    obj.SetActive(active);
                }
            }
        }

        /// <summary>
        /// Fires this panel's and its elements' OnDisplayCallbacks. The MenuManager
        /// calls this when the panel becomes the active (navigated-to) panel, so it
        /// fires exactly once per navigation — before any entrance animation, and
        /// not on menu cover/uncover (which is a menu-level visibility change, not a
        /// panel display change).
        /// </summary>
        public void InvokeDisplayCallbacks() {
            if (Config != null && Config.OnDisplayCallback != null) {
                Config.OnDisplayCallback(gameObject, this);
            }
            foreach (var manager in ElementManagers) {
                if (manager != null && manager.PanelObjectConfig != null && manager.PanelObjectConfig.OnDisplayCallback != null) {
                    manager.PanelObjectConfig.OnDisplayCallback(manager.gameObject, manager);
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