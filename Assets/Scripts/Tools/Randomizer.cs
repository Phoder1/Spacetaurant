using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spacetaurant
{
    [Serializable, InlineProperty, HideLabel]
    public class Randomizer<T>
    {
        [SerializeField, ListDrawerSettings(Expanded = true, AlwaysAddDefaultValue = true)]
        private List<Option<T>> options = new List<Option<T>>();
        public Option<T> this[T content] => options.Find(OptionMatch(content));
#if UNITY_EDITOR
        [Button]
        private void DebugRandomOption() => Debug.Log(GetRandomOption()?.ToString() ?? "Null");
        [Button]
        private void DebugTotalWeight() => Debug.Log(GetTotalWeight());
#endif
        public float GetTotalWeight()
        {
            var weight = 0f;
            foreach (var option in options)
                weight += option.Weight;

            return weight;
        }
        public T GetRandomOption()
        {
            if (options.Count == 0)
                throw new ArgumentNullException();

            var value = UnityEngine.Random.Range(0, GetTotalWeight());

            for (int i = 0; i < options.Count; i++)
            {
                value -= options[i].Weight;
                if (value <= 0)
                    return options[i].Content;
            }
            return options[0].Content;
        }
        /// <summary>
        /// Adding an option to the randomizer.
        /// </summary>
        /// <param name="content">The content to add to the randomizer</param>
        /// <param name="weight">The chance weight the option will be rolled, relative to other options. 1 is the default.</param>
        /// <param name="additive">If true, in case of duplication, where the option already exists in the options pool, it will combine the options weights.</param>
        public void Add(Option<T> newOption, bool additive = true)
        {
            var option = this[newOption.Content];
            if (option == null)
                options.Add(newOption);
            else if (additive)
                option.Weight += newOption.Weight;
        }
        /// <summary>
        /// Removing an option from the randomizer.
        /// </summary>
        /// <param name="content">The content to remove from the randomizer.</param>
        /// <returns></returns>
        public bool Remove(T content)
        {
            int index = options.FindIndex(OptionMatch(content));
            if (index == -1)
                return false;

            options.RemoveAt(index);
            return true;

        }
        /// <summary>
        /// Increase (or decrease) the chance of one of the options.
        /// </summary>
        /// <param name="content">The content of the option to affect.</param>
        /// <param name="weightAmount">The amount to increase (or decrease).</param>
        public void AddWeight(T content, float weightAmount)
        {
            if (content == null || weightAmount == 0)
                return;
            var option = this[content];

            if (option == null)
                return;

            option.Weight += weightAmount;
        }
        private Predicate<IOption<T>> OptionMatch(T option)
            => (x) => x.Content.Equals(option);


    }
    /// <summary>
    /// The interface for any kind of option for the randomizer.
    /// </summary>
    /// <typeparam name="T">The type of content the option holds</typeparam>
    public interface IOption<T>
    {
        /// <summary>
        /// The content the option holds.
        /// </summary>
        T Content { get; set; }
        /// <summary>
        /// The chance weight the option will be rolled, relative to other options. 1 is the default.
        /// </summary>
        float Weight { get; set; }
    }
    /// <summary>
    /// An option the randomizer holds.
    /// </summary>
    [Serializable]
    public class Option<T> : IOption<T>
    {
        [HorizontalGroup(LabelWidth = 70)]
        public T content = default;
        [HorizontalGroup(LabelWidth = 70)]
        public float weight = 1;

        public Option(T content, float weight = 1)
        {
            this.content = content;
            this.weight = weight;
        }

        public virtual T Content { get => content; set => content = value; }
        public virtual float Weight { get => weight; set => weight = value; }
    }
}
