using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentDayController : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        // Define next day    
        DefineNextDay();
    }

    void DefineNextDay() {
        int currentDay = GameController.controller.GetCurrentDay();

        // TODO Define a routine to choose the next day
        int nextDay = currentDay + 1;
        GameController.controller.SetCurrentDay(nextDay);

        Destroy(this.gameObject);
    }
}
