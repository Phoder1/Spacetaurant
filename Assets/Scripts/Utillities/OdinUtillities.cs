using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;

namespace Spacetaurant
{
    public static class OdinUtillities
    {
        public const int EventsPropertyOrder = 999;
        public const int SingletonPropertyOrder = 9998;
        public const int DebugPropertyOrder = 9999;
    }
    #region Ordering
    [IncludeMyAttributes]
    [PropertyOrder(OdinUtillities.DebugPropertyOrder)]
    public class DebugPropertyOrder : Attribute { }
    #endregion
    #region Custom groups
    /// <summary>
    /// Debug group requires a bool property named "_debug" which controls the group visibility.
    /// </summary>
    [IncludeMyAttributes]
    [ColoredFoldoutGroup("Debug",1,1,0,Expanded = false, VisibleIf = "_debug", Order = OdinUtillities.DebugPropertyOrder+1)]
    public class DebugGroup : Attribute { }

    [IncludeMyAttributes]
    [GUIColor(1f, 1f, 1f, 0.7f)]
    [ColoredFoldoutGroup("Events",1,0.2f, 0.2f, Expanded = false, AnimateVisibility = false, Order = OdinUtillities.EventsPropertyOrder + 1)]
    public class EventsGroup : Attribute { }
    public class ColoredFoldoutGroupAttribute : PropertyGroupAttribute
    {
        public float R, G, B, A;
        public bool Expanded = true;

        public ColoredFoldoutGroupAttribute(string path)
            : base(path)
        {
        }

        public ColoredFoldoutGroupAttribute(string path, float r, float g, float b, float a = 1f)
            : base(path)
        {
            this.R = r;
            this.G = g;
            this.B = b;
            this.A = a;
        }

        protected override void CombineValuesWith(PropertyGroupAttribute other)
        {
            var otherAttr = (ColoredFoldoutGroupAttribute)other;

            this.R = Math.Max(otherAttr.R, this.R);
            this.G = Math.Max(otherAttr.G, this.G);
            this.B = Math.Max(otherAttr.B, this.B);
            this.A = Math.Max(otherAttr.A, this.A);
        }
    }
    #endregion
}
