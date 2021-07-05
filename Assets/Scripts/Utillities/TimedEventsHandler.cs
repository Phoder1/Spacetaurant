using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spacetaurant
{
    public static class TimedEventsHandler
    {
        public static LinkedList<TimeEvent> timeEvents = new LinkedList<TimeEvent>();
        public static TimeEvent Add(this TimeEvent timeEvent)
        {
            if (timeEvents.Count == 0)
            {
                timeEvents.AddFirst(timeEvent);
                return timeEvent;
            }

            var currentNode = timeEvents.First;
            for (int i = 0; i < timeEvents.Count; i++)
            {
                if (currentNode.Value.time > timeEvent.time)
                {
                    timeEvents.AddBefore(currentNode, timeEvent);
                    return timeEvent;
                }
            }
            timeEvents.AddLast(timeEvent);

            return timeEvent;
        }
        public static void CheckEvents() => CheckEvents(RealTimeHandler.GetTime());
        public static void CheckEvents(DateTime time)
        {
            if (timeEvents.Count == 0)
                return;

            var firstValue = timeEvents.First.Value;
            if (time > firstValue.time)
            {
                firstValue.timeEvent?.Invoke();
                timeEvents.RemoveFirst();
                CheckEvents(time);
                return;
            }
                
        }
    }
    public struct TimeEvent
    {
        public DateTime time;
        public Action timeEvent;

        public TimeEvent(DateTime time, Action timeEvent)
        {
            this.time = time;
            this.timeEvent = timeEvent;
        }
    }
}
