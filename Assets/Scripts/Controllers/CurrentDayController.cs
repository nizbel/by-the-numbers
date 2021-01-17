using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentDayController : MonoBehaviour
{
    private const int FULL_GAME_RUN = 3;

    private const int AMOUNT_POSSIBLE_DAYS = 4;
    //private List<int> daysAvailable = new List<int> {
    //    1, 2, 5, 6, 7, 8, 9, 13, 14, 15, 16, 17, 18, 21, 22, 23, 24, 25, 26, 27, 
    //    29, 30, 31, 32, 35, 37, 38, 39, 41, 44, 46, 47, 48, 49, 50, 52, 53, 54, 
    //    55, 56, 58, 59, 60, 62, 63, 64, 67, 68, 69, 70, 72, 74, 75, 78, 79, 81, 
    //    82, 83, 84, 85, 87, 88, 89, 90 };

    private static List<int> daysAvailable = new List<int> {
        1, 2, 5, 6, 15, 21, 72, 81};

    [SerializeField]
    private GameObject daysDataPrefab;

    void Awake() {
        // TODO Remove once going for production
        DaysData daysData = GameObject.Instantiate(daysDataPrefab).GetComponent<DaysData>();
        foreach (DayData dayData in daysData.GetData()) {
            dayData.elementsInDay = dayData.GetElementsInDay();
        }
    }

    // Start is called before the first frame update
    void Start() {
        // Increment days played counter
        GameController.controller.SetDaysPlayed(GameController.controller.GetDaysPlayed()+1);

        // Define next day    
        DefineNextDay();
    }

    void DefineNextDay() {
        int currentDay = GameController.controller.GetCurrentDay();

        if (currentDay != GetLastDay() && GameController.controller.GetDaysPlayed() < FULL_GAME_RUN) {
            // Check possibilities
            int nextDay = 0;
            // TODO Check if current day has restrictions
            if (2 != 2) { }

            else {
                bool validDay = false;
                do { 
                    List<int> possibleDays = GetPossibleNextDays(currentDay);
                    nextDay = possibleDays[Random.Range(0, possibleDays.Count)];

                    // Check if day is valid
                    DayData dayData = GetDayData(nextDay);

                    // Check if a new element is seen on this day
                    List<ElementsEnum> newElements = CheckForNewElements(dayData.elementsInDay, GameController.GetGameInfo().elementsSeen);
                    validDay = newElements.Count <= 1;
                } while (!validDay);
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

        int startingIndex = daysAvailable.IndexOf(currentDay) + 1;

        // Define amount of days in the resulting list
        int amount = AMOUNT_POSSIBLE_DAYS + startingIndex <= daysAvailable.Count ? AMOUNT_POSSIBLE_DAYS : (daysAvailable.Count - startingIndex);
        possibleDays.AddRange(daysAvailable.GetRange(startingIndex, amount));

        return possibleDays;
    }

    List<ElementsEnum> CheckForNewElements(List<ElementsEnum> currentElements, bool[] elementsSeen) {
        List<ElementsEnum> newElements = new List<ElementsEnum>();
        foreach (ElementsEnum element in currentElements) {
            if (!elementsSeen[(int)element - 1]) {
                newElements.Add(element);
            }
        }
        return newElements;
    }

    public DayData GetDayData(int day) {
        DaysData daysData = GameObject.Instantiate(daysDataPrefab).GetComponent<DaysData>();
        DayData dayData = daysData.GetDayData(day);
        Destroy(daysData);
        return dayData;
    }

    public static int GetInitialDay() {
        StageInfo stage1Data = GameController.GetGameInfo().GetStageInfoByDay(1);
        if (stage1Data != null) {
            if (stage1Data.IsDone()) {
                return daysAvailable[Random.Range(0, 3)];
            }
        }

        // If day one is not done, always start by it
        return 1;
    }

    public int GetLastDay() {
        return daysAvailable[daysAvailable.Count - 1];
    }

    public static List<int> GetDaysAvailable() {
        return daysAvailable;
    }
}
