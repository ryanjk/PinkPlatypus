using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using ScheduleEntry = ScheduleData.ScheduleEntry;

/**
* ScheduleScript
* @author Ryan Kitner

* Class for reading and accessing a schedule database
*/

public class ScheduleScript : MonoBehaviour {

    /**
    * Load a schedule from a file
    * @param filename - filename of schedule data to load
    */
    public void loadSchedule(string filename) {

        _scheduleData = SaveDataScript.save_data.schedule;

        /*IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(".\\Assets\\Resources\\" + filename, FileMode.Open);
        _scheduleData = (ScheduleData)formatter.Deserialize(stream);
        stream.Close(); */
    }

    /**
    * Get the entry that is closest from the bottom
    * e.g. if schedule = { 0:00, 0:30, 1:00, 1:30 }, getLowerEntry(1, 45) = 1:30 
    * @param hour - the hour of the entry
    * @param minute - the minute of the entry
    * @return ScheduleEntry - a copy of the entry
    */
    public ScheduleEntry getLowerEntry(int hour, int minute) {
        if (_scheduleData == null) {
            return null;
        }
        return _scheduleData.getLowerEntry(hour, minute).clone();
    }

    /**
    * Get the entry that is closest from the top
    * e.g. if schedule = { 0:00, 0:30, 1:00, 1:30 }, getUpperEntry(1, 45) = 0:00 
    * @param hour - the hour of the entry
    * @param minute - the minute of the entry
    * @return ScheduleEntry - a copy of the entry
    */
    public ScheduleEntry getUpperEntry(int hour, int minute) {
        if (_scheduleData == null) {
            return null;
        }
        return _scheduleData.getUpperEntry(hour, minute).clone();
    }

    /**
    Get a random entry that's in the given world
    */
    public ScheduleEntry getRandomEntry(string world) {
        return _scheduleData.getRandomEntry(world);
    }

    /**
    Get an entry that is closest to the given point in the world
    */
    public ScheduleEntry getClosestEntry(string in_world, int[] point) {
        return _scheduleData.getClosestEntry(in_world, point);
    }

    private ScheduleData _scheduleData;

}
