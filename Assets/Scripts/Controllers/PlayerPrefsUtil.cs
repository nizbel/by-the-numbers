using UnityEngine;

public static class PlayerPrefsUtil
{
    public const string PLAY_MUSIC_PREF = "playMusic";
    public const string PLAY_SFX_PREF = "playSFX";
    public const string SHOW_SUBTITLES_PREF = "useSubtitles";

    public static bool GetBoolPref(string pref) {
        return PlayerPrefs.GetInt(pref, 1) == 1;
    }

    public static void SetBoolPref(string pref, bool value) {
        PlayerPrefs.SetInt(pref, BoolToInt(value));
    }

    private static int BoolToInt(bool value) {
        return (value ? 1 : 0);
    }
}
