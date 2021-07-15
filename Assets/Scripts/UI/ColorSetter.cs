using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using TMPro;

namespace Spacetaurant
{
    [HideMonoScript]
    public class ColorSetter : MonoBehaviour
    {
        [SerializeField]
        private Color _color;
        [SerializeField]
        private Graphic _graphic;

        public void SetColor(Color color) => _graphic.color = color;
        public void SetColor() => SetColor(_color);
    }
}
