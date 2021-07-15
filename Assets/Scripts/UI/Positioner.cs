using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Spacetaurant
{
    [HideMonoScript]
    public class Positioner : MonoBehaviour
    {
        [SerializeField]
        private Vector3 _offset;
        public void Position(Vector3 position) => transform.position = position;
        public void Position(Component refrence) => Position(refrence.transform.position);
        public void Position(Component refrence, Vector3 offset) => Position(refrence.transform.position + offset);
        public void PositionWithOffset(Vector3 position) => Position(position + _offset);
        public void PositionWithOffset(Component refrence) => Position(refrence.transform.position + _offset);

    }
}
