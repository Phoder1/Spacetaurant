using DataSaving;
using Spacetaurant.Crafting;
using System;
using UnityEngine;

namespace Spacetaurant.Restaurant
{
    public class RestaurantManager : MonoSingleton<RestaurantManager>
    {
        #region Serielized
        [SerializeField]
        private Randomizer<ResourceSO> randomFloat;
        #endregion
        #region State
        private DateTime _logoutTime;
        #endregion
        #region Unity callbacks

        #endregion
        #region Public actions
        #endregion
        #region Private actions
        private void Start()
        {
            StartCoroutine(RealTimeHandler.UpdateStartTime());
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var test = DataHandler.GetData<LogOutTime>();
                test.Date = CurrentTime;
                test.Save();
            }
        }
        public void OnLogin()
        {
            TimeSpan _timePassed = CurrentTime - _logoutTime;
        }
        public void OnLogOut()
        {

        }
        #endregion
        #region Private Utillities
        private DateTime CurrentTime => RealTimeHandler.GetTime();
        #endregion

    }
    public static class RestaurantUtillities
    {
        public static DateTime ToDate(this string date) => DateTime.Parse(date);
    }

    [Serializable]
    public class LogOutTime : DirtyData, ISaveable
    {
        [SerializeField]
        private string date = default;

        public string DateString { get => date; set => Setter(ref date, value); }
        public DateTime Date { get => DateString.ToDate(); set => DateString = value.ToString(); }
    }
}
