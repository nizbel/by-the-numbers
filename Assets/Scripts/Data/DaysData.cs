using UnityEngine;

public class DaysData : MonoBehaviour
{
    [SerializeField]
    DayData[] data;

    public DayData GetDayData(int day) {
        foreach (DayData dayData in data) {
            if (dayData.day == day) {
                return Instantiate(dayData);
            }
        }
        return null;
    }
}
