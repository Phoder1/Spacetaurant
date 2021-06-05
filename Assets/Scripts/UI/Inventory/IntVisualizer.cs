using Sirenix.OdinInspector;
using Spacetaurant.Crafting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spacetaurant
{
    public class IntVisualizer : MonoBehaviour
    {
        [SerializeField, SceneObjectsOnly]
        private GameObject[] _gameObjects;
        public void UpdateObjects(int num)
        {

            for (int i = 0; i < _gameObjects.Length; i++)
                _gameObjects[i].SetActive(i < num);
        }
    }
}
