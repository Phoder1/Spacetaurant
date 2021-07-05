using DataSaving;
using System;
using UnityEngine;

namespace Spacetaurant
{
    [Serializable]
    public class ParkingLot : DirtyData, ISaveable
    {
        static ParkingLot() => LoadingManager.CacheAllData += Cache;
        private static void Cache() => DataHandler.Load<ParkingLot>();
        [SerializeField]
        private int _size = 10;
        public int Size { get => _size; set => Setter(ref _size, value); }
        [SerializeField]
        private DirtyDataList<CustomerSlot> _customers = new DirtyDataList<CustomerSlot>(false);
        public DirtyDataList<CustomerSlot> Customers { get => _customers; set => Setter(ref _customers, value); }
        public bool Full => Customers.Count >= Size;
        public override bool IsDirty 
        { 
            get => base.IsDirty || Customers.IsDirty; 
            protected set => base.IsDirty = value; 
        }

        protected override void OnClean()
        {
            base.OnClean();
            Customers.Clean();
        }
        public void FastForwardCustomers(TimeSpan timeSpan)
        {
            DateTime currentTime = RealTimeHandler.GetTime();
            for (int i = 0; i < Customers.Count; i++)
            {
                if (currentTime - Customers[i].ArrivalTime.Date <= timeSpan)
                {
                    Customers.RemoveAt(i);
                    Debug.Log("Removed customer " + i);
                }
            }
        }
    }
    [Serializable]
    public class CustomerSlot : DirtyData
    {
        [SerializeField]
        private DirtyTime _arrivalTime = new DirtyTime(false);
        public DirtyTime ArrivalTime => _arrivalTime;
        [SerializeField]
        private CustomerSO _customer;
        public CustomerSO Customer => _customer;

        public CustomerSlot(CustomerSO customer, DateTime date)
        {
            _customer = customer ?? throw new ArgumentNullException(nameof(customer));
            _arrivalTime.Date = date;
            ValueChanged();
        }
        public TimeEvent AddArrivalTimeEvent(Action callback = null)
            => new TimeEvent(_arrivalTime.Date, () => AddToParkingLot(callback)).Add();
        public void AddToParkingLot( Action callback = null)
        {
            var parkingLot = DataHandler.Load<ParkingLot>();
            parkingLot.Customers.Add(this);
            Debug.Log("Customer arrived!");
            callback?.Invoke();
        }
        public void Left()
        {

        }
    }
}
