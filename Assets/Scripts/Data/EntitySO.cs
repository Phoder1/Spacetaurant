using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spacetaurant
{
    public abstract class EntitySO : ScriptableObject
    {
        [HorizontalGroup("Title")]
        [BoxGroup("Title/Info", showLabel: false)]
        [LabelText("Name"), LabelWidth(60)]
        [SerializeField]
        private string _name;
        public string Name => _name;


        [BoxGroup("Title/Info", showLabel: false)]
        [TextArea]
        [SerializeField]
        private string _description;
        public string Description => _description;
    }
}
