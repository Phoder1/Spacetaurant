using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spacetaurant
{
    public class EventsTimeline
    {
        public LinkedList<TimeEvent> timeEvents = new LinkedList<TimeEvent>();
        public TimeEvent Add(TimeEvent timeEvent)
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
                currentNode = currentNode.Next;
            }
            timeEvents.AddLast(timeEvent);

            return timeEvent;
        }
        public void CheckEvents() => CheckEvents(RealTimeHandler.GetTime());
        public void CheckEvents(DateTime time)
        {
            if (timeEvents.Count == 0)
                return;

            var firstValue = timeEvents.First.Value;
            if (time > firstValue.time)
            {
                firstValue.eventAction?.Invoke();
                timeEvents.RemoveFirst();
                CheckEvents(time);
                return;
            }
                
        }
    }
    public struct TimeEvent
    {
        public DateTime time;
        public Action eventAction;

        public TimeEvent(DateTime time, Action timeEvent)
        {
            this.time = time;
            this.eventAction = timeEvent;
        }
    }
}
