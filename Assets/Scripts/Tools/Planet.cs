using Sirenix.OdinInspector;
using UnityEngine;

namespace Spacetaurant
{
    public class Planet : MonoBehaviour
    {
        [SerializeField]
        private bool _isMainPlanet;
        private void OnEnable()
        {
            if (_isMainPlanet)
                AssignPlanet();
        }
        private void OnDisable()
        {
            if (BlackBoard.planet == this)
                UnassignPlanet();
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
