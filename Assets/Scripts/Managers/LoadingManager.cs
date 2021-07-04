using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataSaving;
using Spacetaurant.Containers;
using System;
using UnityEngine.SceneManagement;

namespace Spacetaurant
{
    public class LoadingManager : MonoBehaviour
    {
        public static event Action CacheAllData;
        private void Awake()
        {
            CacheAllData?.Invoke();
            StartCoroutine(RealTimeHandler.UpdateStartTime((x) => TimeUpdated()));
        }
        private void TimeUpdated()
        {
            Debug.Log("Loading next scene");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
