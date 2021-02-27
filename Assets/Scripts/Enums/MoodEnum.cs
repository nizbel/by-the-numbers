using UnityEngine;

/*
 *  Moods that control events to show up depending on Olivia 
 */
[SerializeField]
public enum MoodEnum {
    // Positive
    Excited = 1,
    Curious,

    // Neutral
    Worried = 11,
    Contemplative,


    // Negative
    Scared = 21,
    Angry,
    Sad,
    Tense
}
