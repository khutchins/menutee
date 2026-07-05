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
        public Selectable[] Selectables;
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
        /// Opts a selectable that wasn't created through a PanelObjectConfig into 
        /// the menu's focus system, so it participates in the focus callbacks.
        /// Only needed for external selectables, typically from the panel's
        /// SetCreationCallback where the instance is available.
        /// 
        /// Safe to call on an already-registered selectable (updates the key).
        /// </summary>
        public void RegisterFocusSource(Selectable selectable, string key = null) {
            if (selectable == null) {
                return;
            }
            GameObject go = selectable.gameObject;
            FocusRelay relay = go.GetComponent<FocusRelay>();
            if (relay == null) {
                relay = go.AddComponent<FocusRelay>();
            }
            // Preserve any element already associated while applying the supplied key.
            relay.Configure(this, relay.Element, key);
        }

        /// <summary>
        /// Registers several focus sources at once. See RegisterFocusSource.
        /// </summary>
        public void RegisterFocusSources(params Selectable[] selectables) {
            if (selectables == null) {
                return;
            }
            foreach (Selectable selectable in selectables) {
                RegisterFocusSource(selectable);
            }
        }

        /// <summary>
        /// Removes a selectable previously added with RegisterFocusSource from the
        /// focus system. The relay clears its focus state as it is destroyed.
        /// </summary>
        public void UnregisterFocusSource(Selectable selectable) {
            if (selectable == null) {
                return;
            }
            if (selectable.TryGetComponent<FocusRelay>(out var relay)) {
                Destroy(relay);
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