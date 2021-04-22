using Menutee;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour {
    public string Key;
    public UIElementManager DefaultInput;
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
