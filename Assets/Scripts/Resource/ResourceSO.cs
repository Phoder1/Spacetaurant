using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Spacetaurant.Resources
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Resource")]
    public class ResourceSO : ScriptableObject
    {
        public string resourceName;
        [TextArea]
        public string description;
    }
}
