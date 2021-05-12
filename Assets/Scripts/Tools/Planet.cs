using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spacetaurant
{
    public class Planet : MonoBehaviour
    {
        private void OnEnable()
        {
            BlackBoard.planet = this;
        }
        private void OnDisable()
        {
            if (BlackBoard.planet == this)
                BlackBoard.planet = null;
        }
        [Button, ShowIf("@UnityEngine.Application.isPlaying")]
        private void AssignPlanet()
        {
            BlackBoard.planet = this;
            Debug.Log(BlackBoard.planet);
        }
        [Button, ShowIf("@UnityEngine.Application.isPlaying")]
        private void UnassignPlanet()
        {
            if (BlackBoard.planet == this)
                BlackBoard.planet = null;
        }
    }
}
