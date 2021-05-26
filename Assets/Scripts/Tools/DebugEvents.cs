using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spacetaurant
{
    public class DebugEvents : MonoBehaviour
    {
        [Button]
        public void DebugLog(string logText) => Debug.Log(logText);
    }
}
