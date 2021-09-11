using UnityEngine;

namespace Menutee {
    public class PanelManager : MonoBehaviour {
        [Tooltip("Parent object that panel objects will be placed in. If null, will use this game object.")]
        public GameObject Parent;

        [HideInInspector]
        public string Key;
        [HideInInspector]
        public UIElementManager DefaultInput;
        [HideInInspector]
        public GameObject[] OtherObjects;

        public void SetPanelActive(bool active) {
            gameObject.SetActive(active);
            if (OtherObjects != null) {
                foreach (GameObject obj in OtherObjects) {
                    obj.SetActive(active);
                }
            }
        }
    }
}