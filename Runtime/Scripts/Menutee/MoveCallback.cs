using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Menutee {
    public delegate void InputMove(MoveCallback selected, MoveDirection moveDir);
    public class MoveCallback : MonoBehaviour, IMoveHandler {

        public event InputMove Move;

        public void OnMove(AxisEventData eventData) {
            Move?.Invoke(this, eventData.moveDir);
        }
    }
}