using System;
using System.Collections.Generic;

namespace faultnet_demo_api {
    public interface IGlobalTimer {
        long ReadAndRecordTime(int triggerCount);
        long CurrentTime();
        long PollLastRecordedTime();
        int PollTriggerCounter();
    }

    public class UtcTimer : IGlobalTimer {
        private const int MIN_TRIGGER_TIME_MILLIS = 300;
        private const int MAX_TRIGGER_TIME_MILLIS = 1500;
        private static readonly Random RAND = new Random();
        private static object pollLock = new object();          // Multiple request threads may be trying to poll the trigger count, so we hackily lock. This is truly awful design, but meh

        private long theLastRecordedTime = long.MaxValue;
        private long theNextTriggerTime = long.MaxValue;
        private int theTriggerCount = 0;

        public long CurrentTime() => (long) (DateTime.UtcNow - DateTime.MinValue).TotalMilliseconds;
        public long PollLastRecordedTime() => theLastRecordedTime;

        public long ReadAndRecordTime(int triggerCount) {
            lock (pollLock) {
                theLastRecordedTime = theNextTriggerTime = CurrentTime();
                theNextTriggerTime += Interval();
                theTriggerCount = triggerCount;
                return theLastRecordedTime;
            }
        }
        
        public int PollTriggerCounter() {
            lock (pollLock) {
                long curr = CurrentTime();
                // Escape after long periods of time without doing more than 500 RAND calls
                if (curr > theNextTriggerTime + 500 * MIN_TRIGGER_TIME_MILLIS) {
                    theTriggerCount += 500;
                    theNextTriggerTime = curr + Interval();
                }
                while (theNextTriggerTime < curr) {
                    theTriggerCount++;
                    theNextTriggerTime += Interval();
                }
                return theTriggerCount;
            }
        }

        private long Interval() {
            return RAND.Next(MIN_TRIGGER_TIME_MILLIS, MAX_TRIGGER_TIME_MILLIS);
        }
    }
}