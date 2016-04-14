using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

/**
* ScheduleData class used to represent the schedule data and handle certain queries
* @author Ryan

* Time is assumed to be in 24h format. (e.g. 0:34, 3:30, 13:50)
* The data is sorted by time internally which simplifies certain access methods
*/
[Serializable]
public class ScheduleData {
    
    /**
    * ScheduleData constructor
    */
    public ScheduleData() {
        _scheduleData = new List<ScheduleEntry>();
    }

    /**
    * Insert an entry into the database
    * @param entry - the entry to insert
    */
    public void insertEntry(ScheduleEntry entry) {
        _scheduleData.Add(entry);
        _scheduleData.Sort();
    }

    /**
    * Get the entry that is closest from the bottom
    * e.g. if schedule = { 0:00, 0:30, 1:00, 1:30 }, getLowerEntry(1, 45) = 1:30 
    * @param hour - the hour of the entry
    * @param minute - the minute of the entry
    * @return ScheduleEntry - the entry
    */
    public ScheduleEntry getLowerEntry(int hour, int minute) {
        var i = 0;
        var current_min = _scheduleData[_scheduleData.Count - 1];
        while (i < _scheduleData.Count) {
            var entry = _scheduleData[i];
            if (hour > entry.hour || (hour == entry.hour && minute >= entry.minute)) {
                current_min = entry;
                ++i;
            }
            else {
                return current_min;
            }
        }
        return current_min;
    }

    /**
    * Get the entry that is closest from the top
    * e.g. if schedule = { 0:00, 0:30, 1:00, 1:30 }, getUpperEntry(1, 45) = 0:00 
    * @param hour - the hour of the entry
    * @param minute - the minute of the entry
    * @return ScheduleEntry - the entry
    */
    public ScheduleEntry getUpperEntry(int hour, int minute) {
        var current_max = _scheduleData[0];
        var i = _scheduleData.Count - 1;
        while (i >= 0) {
            var entry = _scheduleData[i];
            if (hour < entry.hour || (hour == entry.hour && minute <= entry.minute)) {
                current_max = entry;
                --i;
            }
            else {
                return current_max;
            }
        }
        return current_max;
    }

    public ScheduleEntry getRandomEntry(string from_world) {
        var entries_from_world = new List<ScheduleEntry>();
        foreach (var entry in _scheduleData) {
            if (entry.world_id == from_world) {
                entries_from_world.Add(entry);
            }
        }

        return entries_from_world[UnityEngine.Random.Range(0, entries_from_world.Count)];
    }

    public ScheduleEntry getClosestEntry(string in_world, int[] point) {
        var x = point[0];
        var y = point[1];

        var min_dist = int.MaxValue;
        var min_entry = new ScheduleEntry();
        for (int i = 1; i < _scheduleData.Count; i += 2) {

            var entry = _scheduleData[i];
            if (in_world != entry.world_id) {
                continue;
            }

            var x_diff = x - entry.x_pos;
            var y_diff = y - entry.y_pos;

            var dist = x_diff * x_diff + y_diff * y_diff;
            if (dist < min_dist) {
                min_dist = dist;
                min_entry = entry;
            }
        }

        return min_entry;
    }

    public void addPadding(int minutes) {
        for (int i = 1; i < _scheduleData.Count; ++i) {
            var cur_entry = _scheduleData[i];
            var prev_entry = _scheduleData[i - 1];
            if (cur_entry.PositionEquals(prev_entry) || cur_entry.world_id != prev_entry.world_id) {
                push_entries_back(minutes, i);
            }
        }
    }

    private void push_entries_back(int num_minutes, int starting_entry_index) {
        for (int i = starting_entry_index; i < _scheduleData.Count; ++i) {
            _scheduleData[i].add_minutes(num_minutes); 
        }
    }

    /**
    * Save the database to disk
    * @param filename - name of file on disk
    */
    public void saveToDisk(string filename) {

        SaveDataScript.save_data.schedule = this;
        SaveDataScript.save();
    }

    public override string ToString() {
        var s = "";
        foreach (var entry in _scheduleData) {
            s += string.Format("{0,2}:{1,2}: World {2,-2} at {3,-3},{4,-3}{5}", entry.hour, entry.minute, entry.world_id, entry.x_pos, entry.y_pos, Environment.NewLine);
        }
        return s;
    }

    private List<ScheduleEntry> _scheduleData;

    /**
    * ScheduleEntry used to represent an entry in the database
    */
    [Serializable]
    public class ScheduleEntry : IComparable<ScheduleEntry> {
        public int hour;
        public int minute;
        public string world_id;
        public int x_pos;
        public int y_pos;

        public ScheduleEntry clone() {
            var c = new ScheduleEntry();
            c.hour = hour;
            c.minute = minute;
            c.world_id = world_id;
            c.x_pos = x_pos;
            c.y_pos = y_pos;
            return c;
        }

        public bool PositionEquals(object entry) {
            if (entry == null || !(entry is ScheduleEntry)) {
                return false;
            }

            var other = entry as ScheduleEntry;
            if ((world_id == other.world_id) && (x_pos == other.x_pos) && (y_pos == other.y_pos)) {
                return true;
            }

            return false;
        }

        public void add_minutes(int minutes) {
            var sum = minute + minutes;
            if ( sum >= 60 ) {
                minute += minutes;
                minute = minute % 60;                
                hour = (hour + 1);
            }
            else {
                minute = sum;
            }
        }

        public int CompareTo(ScheduleEntry other) {
            return new ScheduleEntryComparer().Compare(this, other);
        }

        private class ScheduleEntryComparer : IComparer<ScheduleEntry> {
            public int Compare(ScheduleEntry x, ScheduleEntry y) {
                if (x == null) {
                    if (y == null) {
                        return 0;
                    }
                    else {
                        return -1;
                    }
                }
                else if (y == null) {
                    return 1;
                }
                else if (x.hour < y.hour) {
                    return -1;
                }
                else if (x.hour > y.hour) {
                    return 1;
                }
                else {
                    if (x.minute < y.minute) {
                        return -1;
                    }
                    else if (x.minute > y.minute) {
                        return 1;
                    }
                    else {
                        return 0;
                    }
                }
            }
        }
    }


}
