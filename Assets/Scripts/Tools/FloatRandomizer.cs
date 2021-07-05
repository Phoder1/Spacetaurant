using System;
using UnityEngine;

namespace Spacetaurant
{
    public class FloatRandomizer : Randomizer<float>
    {
        public void FillWithRange(float minimum, float maximum, int resolution = 20) => FillWithRange(minimum, maximum, AnimationCurve.Constant(0, 1, 1), resolution);
        public void FillWithRange(float minimum, float maximum, AnimationCurve chanceCurve, int resolution = 20)
        {
            if (minimum >= maximum || maximum - minimum == Mathf.Infinity)
                throw new InvalidOperationException();

            float _timeDelta = (maximum - minimum) / resolution;

            bool chanceCurveIsConstant = chanceCurve == null || chanceCurve.keys.Length == 0 || Array.TrueForAll(chanceCurve.keys, (x) => x.value == chanceCurve.keys[0].value);
            for (int i = 0; i <= resolution; i++)
            {
                float chance;
                if (chanceCurveIsConstant)
                    chance = 1;
                else
                    chance = chanceCurve.Evaluate((float)i / (float)resolution);

                if (chance <= 0.0001f)
                    continue;

                float value = minimum + _timeDelta * i;

                Add(new Option<float>(value, chance));
            }
        }
        public float GetAverageResult()
        {
            if (options.Count == 0)
                return 0;

            float totalWeight = GetTotalWeight();
            float totalValue = 0;

            foreach (var option in options)
                totalValue += option.Content * (option.Weight / totalWeight);

            return totalValue;
        }
    }
}
