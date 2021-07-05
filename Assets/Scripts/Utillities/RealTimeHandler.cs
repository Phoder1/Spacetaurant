using DataSaving;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spacetaurant
{
    public static class RealTimeHandler
    {
        const string TimeURI = "http://worldclockapi.com/api/json/est/now";
        const int MaxTryTime = 60;
        private static float _lastUpdateTime;
        private static DateTime _realStartTime;
        public static bool _initiated;

        public static IEnumerator UpdateStartTime(Action<DateTime> callback = null)
        {
            var _startTryTime = Time.time;
            var _tryCount = 0;
            yield return TryGetTime();

            callback?.Invoke(GetTime());

            IEnumerator TryGetTime()
            {
                yield return WebFetch.HttpGet<TimeResultWorldClock>(TimeURI, OnSuccess, OnFailure);

                void OnSuccess(HttpResponse<TimeResultWorldClock> response)
                {
                    _lastUpdateTime = Time.time;
                    _realStartTime = DateTime.Parse(response.body.currentDateTime);
                    _initiated = true;
                    OnUpdate();
                }
                void OnFailure(HttpResponse<TimeResultWorldClock> response)
                {
                    _tryCount++;
                    Debug.LogError("Failed to fetch time! retrying! try count: " + _tryCount);
                }
            }

        }
        private static void OnUpdate()
        {
            Debug.Log(GetTime());
        }
        public static DateTime ToDate(this string date) => DateTime.Parse(date);
        public static DateTime GetTime() => _realStartTime + TimeSpan.FromSeconds(Time.time - _lastUpdateTime);
        public class TimeResultWorldTime
        {
            public string abbreviation;
            public string client_ip;
            public string datetime;
            public int day_of_week;
            public int day_of_year;
            public bool dst;
            public DateTime dst_from;
            public int dst_offset;
            public DateTime dst_until;
            public int raw_offset;
            public string timezone;
            public int unixtime;
            public DateTime utc_datetime;
            public string utc_offset;
            public int week_number;
        }
        public class TimeResultWorldClock
        {
            public string Id;
            public string currentDateTime;
            public string utcOffset;
            public bool isDayLightSavingsTime;
            public string dayOfTheWeek;
            public string timeZoneName;
            public long currentFileTime;
            public string ordinalDate;
            public object serviceResponse;
        }
    }
    public class DirtyTime : DirtyData
    {
        [SerializeField]
        private string date = default;

        public DirtyTime(bool isDirty = true) : base(isDirty) { }

        public string DateString { get => date; set => Setter(ref date, value); }
        public DateTime Date { get => DateString.ToDate(); set => DateString = value.ToString(); }

        public static implicit operator DateTime(DirtyTime dirtyTime) => dirtyTime.Date;
        public static implicit operator string(DirtyTime dirtyTime) => dirtyTime.DateString;

    }
}
