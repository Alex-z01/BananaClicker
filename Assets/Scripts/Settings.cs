using UnityEngine;

public class Settings : MonoBehaviour
{
    private const string _resolutionPrefKey = "resolution";
    private const string _screenModePrefKey = "screenMode";

    private void Awake()
    {
        // Load the resolution and screen mode settings from PlayerPrefs
        int width = PlayerPrefs.GetInt(_resolutionPrefKey, Screen.currentResolution.width);
        int height = PlayerPrefs.GetInt(_resolutionPrefKey, Screen.currentResolution.height);
        bool fullscreen = PlayerPrefs.GetInt(_screenModePrefKey, 1) == 1;

        // Apply the loaded resolution and screen mode settings
        Screen.SetResolution(width, height, fullscreen);
    }

    private void OnApplicationQuit()
    {
        SetResolutionAndScreenMode(Screen.currentResolution.width, Screen.currentResolution.height, Screen.fullScreen);
    }

    private void SetResolutionAndScreenMode(int width, int height, bool fullscreen)
    {
        // Save the resolution and screen mode settings to PlayerPrefs
        PlayerPrefs.SetInt(_resolutionPrefKey, width);
        PlayerPrefs.SetInt(_screenModePrefKey, fullscreen ? 1 : 0);

        // Apply the new resolution and screen mode settings
        Screen.SetResolution(width, height, fullscreen);
    }

    public void SetScreenMode(int mode)
    {
        Debug.Log(mode);
        switch (mode)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow; 
                break;

            case 1:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;

            case 2:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
        }

        Debug.Log(Screen.fullScreenMode.ToString());
    }

    public void SetResolution(int option)
    {
        Debug.Log(option);
        switch (option)
        {
            case 0:
                Screen.SetResolution(1280, 720, Screen.fullScreen);
                break;

            case 1:
                Screen.SetResolution(1920, 1080, Screen.fullScreen);
                break;

            case 2:
                Screen.SetResolution(2560, 1440, Screen.fullScreen);
                break;

            case 3:
                Screen.SetResolution(3840, 2160, Screen.fullScreen);
                break;
        }

        Debug.Log(Screen.currentResolution.ToString());
    }
}
