using DataSaving;
using Sirenix.OdinInspector;
using Spacetaurant.Planets;
using System;
using UnityEngine;

namespace Spacetaurant.Restaurant
{
    public class RestaurantManager : MonoSingleton<RestaurantManager>
    {
        private const int IntervalRandomResolution = 20;
        private const int CustomerStayTime = 60;
        #region Serielized
        [FoldoutGroup("Customer arrival intrerval")]
        [InfoBox("In minutes", InfoMessageType.None)]
        [SerializeField]
        private float _minimumArrivalInterval = 3;
        [FoldoutGroup("Customer arrival intrerval")]
        [SerializeField]
        private float _maximumArrivalInterval = 6;
        [FoldoutGroup("Customer arrival intrerval")]
        [SerializeField]
        private AnimationCurve _intervalChanceWeight;
        [SerializeField]
        private PlanetSO _planet;
        #endregion
        #region State
        private LogOutTime _logoutTime;
        private ParkingLot _parkingLot;
        private readonly FloatRandomizer _intervalRandomizer = new FloatRandomizer();
        private Randomizer<CustomerSO> _customerRandomizer = new Randomizer<CustomerSO>();
        #endregion
        #region Unity callbacks

        #endregion
        #region Public actions
        #endregion
        #region Private actions
        private void Start()
        {
            _logoutTime = DataHandler.Load<LogOutTime>();
            _parkingLot = DataHandler.Load<ParkingLot>();
            _planet.FillRandomizerWithCustomers(ref _customerRandomizer);
            _intervalRandomizer.FillWithRange(_minimumArrivalInterval, _maximumArrivalInterval, _intervalChanceWeight, IntervalRandomResolution);
            GenerateOfflineCustomers();
            GenerateCustomer(CurrentTime)?.AddArrivalTimeEvent();
            //Debug.Log("Next customer time: " + nextCustomerArrivalTime + ", MinutesFromNow: " + arrivalInterval);
        }

        private void GenerateOfflineCustomers()
        {
            float averageInterval = _intervalRandomizer.GetAverageResult();
            if (!_parkingLot.Full)
            {
                DateTime firstCustomerArrivalTime = _parkingLot.Customers[0].ArrivalTime;
                DateTime lastCustomerArrivalTime = _parkingLot.Customers[_parkingLot.Customers.Count - 1].ArrivalTime;
                double timeSinceFirstCustomer = (CurrentTime - firstCustomerArrivalTime).TotalMinutes;
                double timeSinceLastCustomer = (CurrentTime - lastCustomerArrivalTime).TotalMinutes;

                int customersLeft = Mathf.FloorToInt((1f / (float)CustomerStayTime) * (float)timeSinceFirstCustomer);
                int customersArrived = Mathf.FloorToInt((1f / averageInterval) * (float)timeSinceLastCustomer);
                var numOfCustomers = customersArrived - customersLeft;
                var customer = _customerRandomizer.GetRandomOptions(numOfCustomers);
            }
        }

        private void Update()
        {
            TimedEventsHandler.CheckEvents();
        }

        private void OnDestroy()
        {
            _logoutTime.Date = CurrentTime;
            _logoutTime.Save();
        }
        private CustomerSlot GenerateCustomer(DateTime fromTime)
        {
            var nextCustomer = _customerRandomizer.GetRandomOption();

            var arrivalInterval = TimeSpan.FromMinutes(_intervalRandomizer.GetRandomOption());

            var nextCustomerArrivalTime = fromTime + arrivalInterval;

            return new CustomerSlot(nextCustomer, nextCustomerArrivalTime);
        }
        public void OnLogin()
        {
        }
        public void OnLogOut()
        {

        }
        #endregion
        #region Private Utillities
        private DateTime CurrentTime => RealTimeHandler.GetTime();
        private DateTime LogOutTime => _logoutTime.Date;
        #endregion

    }

    [Serializable]
    public class LogOutTime : DirtyTime, ISaveable
    {
        static LogOutTime() => LoadingManager.CacheAllData += Cache;
        private static void Cache() => DataHandler.Load<LogOutTime>();

    }
}
