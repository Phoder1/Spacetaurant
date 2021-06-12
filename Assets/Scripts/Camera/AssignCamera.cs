using UnityEngine;

namespace Spacetaurant
{
    [RequireComponent(typeof(Camera))]
    public class AssignCamera : MonoBehaviour
    {
        private void OnEnable()
        {
            BlackBoard.ingameCamera = GetComponent<Camera>();
        }
        private void OnDisable()
        {
            if (BlackBoard.ingameCamera == this)
                BlackBoard.ingameCamera = null;
            ;
        }
    }
}
