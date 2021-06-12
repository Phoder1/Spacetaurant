using Sirenix.OdinInspector;
using UnityEngine;

namespace Spacetaurant
{
    public class PlanetAssigner : MonoBehaviour
    {
        private void OnEnable() => AssignPlanet();
        private void OnDisable() => UnassignPlanet();
        [Button, ShowIf("@UnityEngine.Application.isPlaying")]
        private void AssignPlanet() => BlackBoard.planet = this;
        [Button, ShowIf("@UnityEngine.Application.isPlaying")]
        private void UnassignPlanet()
        {
            if (BlackBoard.planet == this)
                BlackBoard.planet = null;
        }
    }
}
