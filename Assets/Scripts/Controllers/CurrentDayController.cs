using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentDayController : MonoBehaviour
{
    private const int AMOUNT_POSSIBLE_DAYS = 2;
    //private List<int> daysAvailable = new List<int> {
    //    1, 2, 5, 6, 7, 8, 9, 13, 14, 15, 16, 17, 18, 21, 22, 23, 24, 25, 26, 27, 
    //    29, 30, 31, 32, 35, 37, 38, 39, 41, 44, 46, 47, 48, 49, 50, 52, 53, 54, 
    //    55, 56, 58, 59, 60, 62, 63, 64, 67, 68, 69, 70, 72, 74, 75, 78, 79, 81, 
    //    82, 83, 84, 85, 87, 88, 89, 90 };

    private static List<int> daysAvailable = new List<int> {
        1, 2, 5, 6, 72};


    // Start is called before the first frame update
    void Start()
    {
        // Define next day    
        DefineNextDay();
    }

    void DefineNextDay() {
        int currentDay = GameController.controller.GetCurrentDay();

        if (currentDay != GetLastDay()) {
            int nextDay = 0;
            // TODO Check if current day has restrictions
            if (2 != 2) { }

            else {
                List<int> possibleDays = GetPossibleNextDays(currentDay);
                nextDay = possibleDays[Random.Range(0, possibleDays.Count)];
            }
            GameController.controller.SetCurrentDay(nextDay);
            GameController.controller.ChangeState(GameController.GAMEPLAY_STORY);
        }
        else {
            GameController.controller.ChangeState(GameController.GAME_OVER_STORY);
        }

        Destroy(this);
    }

    private List<int> GetPossibleNextDays(int currentDay) {
        List<int> possibleDays = new List<int>();

        if (currentDay == 2) {
            possibleDays.Add(5);
        }
        else {
            int startingIndex = daysAvailable.IndexOf(currentDay) + 1;

            // Define amount of days in the resulting list
            int amount = AMOUNT_POSSIBLE_DAYS + startingIndex <= daysAvailable.Count ? AMOUNT_POSSIBLE_DAYS : (daysAvailable.Count - startingIndex);
            possibleDays.AddRange(daysAvailable.GetRange(startingIndex, amount));
        }

        return possibleDays;
    }

    public int GetLastDay() {
        return daysAvailable[daysAvailable.Count - 1];
    }

    public static List<int> GetDaysAvailable() {
        return daysAvailable;
    }
}
