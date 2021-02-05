using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/Speech")]
public class Speech : ScriptableObject {
    public AudioClip audio;

    public List<SpeechPart> speechParts;
}