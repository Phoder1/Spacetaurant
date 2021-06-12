using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Misc
{
    public static void Setter<T>(ref T data, T value, Action onValueChanged, Func<bool> changeCondition = null)
    {
        if ((data == null && value == null) || (data != null && data.Equals(value)))
            return;
        data = value;

        if (changeCondition == null || changeCondition())
            onValueChanged?.Invoke();
    }

    [System.Serializable]
    [InlineProperty]
    public class TagFilter
    {
        [SerializeField, ValueDropdown("@UnityEditorInternal.InternalEditorUtility.tags", IsUniqueList = true, FlattenTreeView = true, HideChildProperties = true, DropdownHeight = 180)]
        string[] _tags = default;
        public string[] Tags { get => _tags; }

        /// <summary>
        /// Checks whether the tag exists in the tag filter.
        /// </summary>
        /// <param name="tag">
        /// The tag to check</param>
        /// <param name="onEmptyDefault">
        /// The default value if the tag filter is empty.</param>
        /// <returns></returns>
        public bool Contains(string tag, bool onEmptyDefault = false)
               => Array.Exists(Tags, (x) => tag == x)
             || (onEmptyDefault && Tags.Length == 0);
        public static implicit operator string[](TagFilter tf) => tf.Tags;
    }
    public static class Filters
    {
        /// <summary>
        /// Check if a layermask contains a specific layer 
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="layerWhitelist"></param>
        /// <param name="defaultValue">The default layer if the layermask has no values.</param>
        /// <returns></returns>
        public static bool CheckLayer(int layer, LayerMask layerWhitelist, bool defaultValue = true)
        {
            return
                (layerWhitelist == ~0 && defaultValue)
                || (layerWhitelist & 1 << layer) != 0;
        }
    }
}
