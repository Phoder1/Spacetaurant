using DataSaving;
using Sirenix.OdinInspector;
using Spacetaurant.Planets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Spacetaurant.Restaurant
{
    [RequireComponent(typeof(LoadingHandler))]
    public class RestaurantManager : MonoSingleton<RestaurantManager>, IReadyable
    {
        private const int IntervalRandomResolution = 20;

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
#if UNITY_EDITOR
        [SerializeField, DebugPropertyOrder]
        private bool _debug = false;
        [SerializeField, DebugGroup, Tooltip("Offline time offset in minutes"), SuffixLabel("Minutes")]
        private float _offlineTimeOffset = 0;
#endif
        #endregion
        #region Events
        [SerializeField]
        private UnityEvent<CustomerSlot> OnCustomerArrival;
        [SerializeField]
        private UnityEvent<CustomerSlot> OnCustomerDepartue;

        #endregion
        #region State
        private LogOutTime _logoutTime;
        private ParkingLot _parkingLot;
        private readonly FloatRandomizer _intervalRandomizer = new FloatRandomizer();
        private Randomizer<CustomerSO> _customerRandomizer = new Randomizer<CustomerSO>();
        private EventsTimeline _customersEventsTimeline;
        #endregion
        #region Unity callbacks
        private void Start()
        {
            StartCoroutine(RealTimeHandler.UpdateStartTime((x) => UpdatedTime()));
            _logoutTime = DataHandler.Load<LogOutTime>();
            _parkingLot = DataHandler.Load<ParkingLot>();

            void UpdatedTime()
            {
                var startTime = Time.realtimeSinceStartup;

                InitiateRandomizers();

                CalculateOffline();

                GenerateFutureCustomers();

                _parkingLot.OnSizeChanged += (x) => GenerateFutureCustomers();

                _ready = true;
            }
        }
        private void Update()
        {
            if (Ready)
                _customersEventsTimeline.CheckEvents();
        }
        private void InitiateRandomizers()
        {
            _planet.FillRandomizerWithCustomers(ref _customerRandomizer);
            _intervalRandomizer.FillWithRange(_minimumArrivalInterval, _maximumArrivalInterval, _intervalChanceWeight, IntervalRandomResolution);
        }

        private void GenerateFutureCustomers()
        {
            _customersEventsTimeline = new EventsTimeline();

            foreach (var customer in _parkingLot.Customers)
                customer.AddDeparuteTimeEvent(_customersEventsTimeline, CustomerLeft);

            int freeParkingSpots = _parkingLot.Size - _parkingLot.Customers.Count;
            DateTime lastCustomerPlannedArrival = _parkingLot.LastCustomerArrivalTime.Date.Value;

            for (int i = 0; i < freeParkingSpots; i++)
                lastCustomerPlannedArrival = GenerateNextCustomer(lastCustomerPlannedArrival).AddArrivalTimeEvent(_customersEventsTimeline, CustomerArrived).time;
        }
        #endregion
        #region Internal
        private void CalculateOffline()
        {
            TimeSpan CustomerStayTimeSpan = TimeSpan.FromMinutes(ParkingLot.CustomerStayTime);

            //Get the last customers arrival time
            DateTime lastCustomerArrivalTime;

            if (_parkingLot.LastCustomerArrivalTime == null || !_parkingLot.LastCustomerArrivalTime.Date.HasValue)
            {
                _parkingLot.LastCustomerArrivalTime.Date = LogOutTime ?? CurrentTime;

                if (!LogOutTime.HasValue)
                    return;
            }

            lastCustomerArrivalTime = _parkingLot.LastCustomerArrivalTime.Date.Value;
#if UNITY_EDITOR
            if (_debug)
            {
                if (_offlineTimeOffset != 0)
                {
                    TimeSpan offset = TimeSpan.FromMinutes(_offlineTimeOffset);
                    //firstCustomerArrivalTime -= offset;
                    lastCustomerArrivalTime -= offset;
                }
            }
#endif
            DateTime simulationTime = lastCustomerArrivalTime;
            Debug.Log("Time since last customer: " + (CurrentTime - simulationTime));
            //The average time between customers arrival
            TimeSpan averageInterval = TimeSpan.FromMinutes(_intervalRandomizer.GetAverageResult());

            List<CustomerSlot> customersThatLeft = new List<CustomerSlot>();
            EventsTimeline _timeline = new EventsTimeline();

            int numOfCustomersLeft = 0;
            int customersArrived = 0;
            //Generate timevents
            foreach (var customer in _parkingLot.Customers)
                _timeline.Add(customer.GetDepartureTimeEvent(CustomerLeft));

            FillRestaurant();

            TimeEvent timeEvent;

            while (_timeline.timeEvents.Count > 0 && CurrentTime > (timeEvent = _timeline.timeEvents.First.Value).time)
            {
                _timeline.timeEvents.RemoveFirst();
                timeEvent.eventAction.Invoke();
            }

            void CustomerLeft(CustomerSlot customer)
            {
                simulationTime = customer.ArrivalTime + CustomerStayTimeSpan;

                customer.Left();
                numOfCustomersLeft++;

                FillRestaurant();
            }

            void FillRestaurant()
            {
                if (_parkingLot.Customers.Count < _parkingLot.Size && CurrentTime - simulationTime > averageInterval)
                {
                    simulationTime += averageInterval;

                    var customer = new CustomerSlot(_customerRandomizer.GetRandomOption(), simulationTime);
                    customer.Arrived();
                    _timeline.Add(customer.GetDepartureTimeEvent(CustomerLeft));

                    customersArrived++;

                    FillRestaurant();
                }
            }
            Debug.Log("Customers that left: " + numOfCustomersLeft
                + ", Customers that arrived: " + customersArrived
                + ", Number of current customers: " + _parkingLot.Customers.Count);
        }

        private void CustomerLeft(CustomerSlot customer)
        {
            customer.Left();
            Debug.Log("Customer left! Total number of customers: " + _parkingLot.Customers.Count);
            GenerateNextCustomer().AddArrivalTimeEvent(_customersEventsTimeline, CustomerArrived);
        }
        private void CustomerArrived(CustomerSlot customer)
        {
            customer.Arrived();
            Debug.Log("Customer arrived! Total number of customers: " + _parkingLot.Customers.Count);
            customer.AddDeparuteTimeEvent(_customersEventsTimeline, CustomerLeft);
        }
        private CustomerSlot GenerateNextCustomer() => GenerateNextCustomer(CurrentTime);
        private CustomerSlot GenerateNextCustomer(DateTime fromTime)
        {
            var nextCustomer = _customerRandomizer.GetRandomOption();

            var arrivalInterval = TimeSpan.FromMinutes(_intervalRandomizer.GetRandomOption());

            var nextCustomerArrivalTime = fromTime + arrivalInterval;

            return new CustomerSlot(nextCustomer, nextCustomerArrivalTime);
        }
        #endregion
        #region Private Utillities
        private DateTime CurrentTime => RealTimeHandler.GetTime();
        private DateTime? LogOutTime => _logoutTime.Date;
        private float CustomerStayTime => ParkingLot.CustomerStayTime;

        #endregion
        #region IReadyable
        private bool _ready = false;
        public bool Ready
        {
            get => _ready;
            set => Misc.Setter(ref _ready, value, () => OnReady?.Invoke());
        }

        public event Action OnReady;
        #endregion
    }

    [Serializable]
    public class LogOutTime : DirtyTime, ISaveable
    {
        static LogOutTime() => LoadingHandler.CacheAllData += Cache;
        private static void Cache() => DataHandler.Load<LogOutTime>();

        public void BeforeSave()
        {
           Date = RealTimeHandler.GetTime();
        }
    }
}
