using DataSaving;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spacetaurant
{
    [Serializable]
    public class ParkingLot : DirtyData, ISaveable
    {
        public const float CustomerStayTime = 60f;
        static ParkingLot() => LoadingHandler.CacheAllData += Cache;

        public ParkingLot(bool isDirty = true) : base(isDirty) => Init();
        public ParkingLot() => Init();
        private void Init()
        {
            Customers.OnValueChange += ValueChanged;
            LastCustomerArrivalTime.OnValueChange += ValueChanged;
        }
        private static void Cache() => DataHandler.Load<ParkingLot>();
        [SerializeField]
        private int _size = 2;
        public int Size { get => _size; set => Setter(ref _size, value, (x) => OnSizeChanged?.Invoke(x)); }
        public event Action<int> OnSizeChanged;
        [SerializeField]
        private DirtyDataList<CustomerSlot> _customers = new DirtyDataList<CustomerSlot>(false);
        public DirtyDataList<CustomerSlot> Customers { get => _customers; set => Setter(ref _customers, value); }

        [SerializeField]
        private DirtyTime _lastCustomerArrivalTime = new DirtyTime(false);
        public DirtyTime LastCustomerArrivalTime { get => _lastCustomerArrivalTime; set => Setter(ref _lastCustomerArrivalTime, value); }
        public bool Full => Customers.Count >= Size;
        public void CheckCustomersLeft()
        {
            if (Customers.Count == 0)
                return;

            DateTime currentTime = RealTimeHandler.GetTime();
            var stayTimeSpan = TimeSpan.FromMinutes(CustomerStayTime);

            for (int i = 0; i < Customers.Count; i++)
            {
                if (currentTime - Customers[i].ArrivalTime.Date > stayTimeSpan)
                {
                    Customers[i].Left();
                    Customers.RemoveAt(i);
                    Debug.Log("Removed customer " + i);
                }
            }
        }
        public void AddCustomer(CustomerSlot customer)
        {
            Customers.Add(customer);
            LastCustomerArrivalTime.Date = customer.ArrivalTime;
        }
        public void RemoveCustomer(CustomerSlot customer) => Customers.Remove(customer);
        #region IDirty
        protected override void OnClean()
        {
            base.OnClean();
            Customers.Clean();
            LastCustomerArrivalTime.Clean();
        }

        public override bool IsDirty
        {
            get => base.IsDirty || Customers.IsDirty || LastCustomerArrivalTime.IsDirty;
            protected set => base.IsDirty = value;
        }
        #endregion
        public void BeforeSave()
        {
        }
    }
    [Serializable]
    public class CustomerSlot : DirtyData, IEquatable<CustomerSlot>
    {
        [SerializeField]
        private DirtyTime _arrivalTime = new DirtyTime(false);
        public DateTime ArrivalTime => _arrivalTime.Date.Value;
        [SerializeField]
        private CustomerSO _customer;
        public CustomerSO Customer => _customer;
        public CustomerSlot(CustomerSO customer, DateTime date)
        {
            _customer = customer;
            _arrivalTime.Date = date;

            _arrivalTime.OnValueChange += ValueChanged;

            ValueChanged();
        }
        public void Left()
        {
            DataHandler.Load<ParkingLot>().RemoveCustomer(this);
            Customer.Left();
        }
        public void Arrived()
        {
            AddToParkingLot();
        }
        public void AddToParkingLot() => DataHandler.Load<ParkingLot>().AddCustomer(this);
        public void RemoveFromParkingLot() => DataHandler.Load<ParkingLot>().RemoveCustomer(this);
        #region Time events
        //Get arrival
        public TimeEvent GetArrivalTimeEvent(Action callback = null)
            => new TimeEvent(_arrivalTime.Date.Value, callback);
        public TimeEvent GetArrivalTimeEvent(Action<CustomerSlot> callback = null)
            => GetArrivalTimeEvent(() => callback(this));

        //Add arival
        public TimeEvent AddArrivalTimeEvent(EventsTimeline timeline, Action callback)
            => timeline.Add(GetArrivalTimeEvent(callback));
        public TimeEvent AddArrivalTimeEvent(EventsTimeline timeline, Action<CustomerSlot> callback)
            => AddArrivalTimeEvent(timeline, () => callback(this));

        //Get departure
        public TimeEvent GetDepartureTimeEvent(Action callback = null)
            => new TimeEvent(_arrivalTime.Date.Value + TimeSpan.FromMinutes(ParkingLot.CustomerStayTime), callback);
        public TimeEvent GetDepartureTimeEvent(Action<CustomerSlot> callback = null)
            => GetDepartureTimeEvent(() => callback(this));

        //Add departue
        public TimeEvent AddDeparuteTimeEvent(EventsTimeline timeline, Action callback)
            => timeline.Add(GetDepartureTimeEvent(callback));
        public TimeEvent AddDeparuteTimeEvent(EventsTimeline timeline, Action<CustomerSlot> callback)
            => AddDeparuteTimeEvent(timeline, () => callback(this));
        #endregion
        #region IDirty
        protected override void OnClean()
        {
            base.OnClean();
            _arrivalTime.Clean();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CustomerSlot);
        }


        public override bool IsDirty
        {
            get => base.IsDirty || _arrivalTime.IsDirty;
            protected set => base.IsDirty = value;
        }
        #endregion
        #region Operators overrides
        public bool Equals(CustomerSlot other)
        {
            return other != null &&
                   ArrivalTime == other.ArrivalTime &&
                   EqualityComparer<CustomerSO>.Default.Equals(Customer, other.Customer);
        }
        public override int GetHashCode()
        {
            int hashCode = -558905500;
            hashCode = hashCode * -1521134295 + ArrivalTime.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<CustomerSO>.Default.GetHashCode(Customer);
            return hashCode;
        }
        public static bool operator ==(CustomerSlot left, CustomerSlot right)
        {
            return EqualityComparer<CustomerSlot>.Default.Equals(left, right);
        }
        public static bool operator !=(CustomerSlot left, CustomerSlot right)
        {
            return !(left == right);
        }
        #endregion
    }
}
