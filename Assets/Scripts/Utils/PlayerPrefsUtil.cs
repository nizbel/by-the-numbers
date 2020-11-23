using UnityEngine;

public static class PlayerPrefsUtil
{
    public const string PLAY_MUSIC_PREF = "playMusic";
    public const string PLAY_SFX_PREF = "playSFX";
    public const string SHOW_SUBTITLES_PREF = "useSubtitles";
    public const string FULL_SCREEN_PREF = "fullScreen";
    public const string RESOLUTION_PREF = "resolution";

    public static bool GetBoolPref(string pref) {
        return PlayerPrefs.GetInt(pref, 1) == 1;
    }

    public static void SetBoolPref(string pref, bool value) {
        PlayerPrefs.SetInt(pref, BoolToInt(value));
    }

    private static int BoolToInt(bool value) {
        return (value ? 1 : 0);
    }

    public static (int,int) GetResolutionPref() {
        switch (PlayerPrefs.GetInt(RESOLUTION_PREF, GameController.WINDOWS_FHD_RES)) {
            case GameController.WINDOWS_FHD_RES:
                return (GameController.FHD_WINDOWS_RES_X, GameController.FHD_WINDOWS_RES_Y);
            case GameController.WINDOWS_HD_RES:
                return (GameController.HD_WINDOWS_RES_X, GameController.HD_WINDOWS_RES_Y);
        }
        return (GameController.FHD_WINDOWS_RES_X, GameController.FHD_WINDOWS_RES_Y);
    }
}
