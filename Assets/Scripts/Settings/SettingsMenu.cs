using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    // Audio settings
    [SerializeField] private AudioMixer myMixer; // Audio mixer to control volume
    [SerializeField] private Slider masterSlider; // Slider to adjust master volume
    [SerializeField] private Slider musicSlider; // Slider to adjust music volume
    [SerializeField] private Slider sfxSlider;   // Slider to adjust SFX volume

    void Start()
    {
        // Initialize maters volume slider
        if (masterSlider != null && myMixer != null)
        {
            float masterVolume = PlayerPrefs.GetFloat("Master Volume", 1.0f);
            masterSlider.value = masterVolume;
            myMixer.SetFloat("master", Mathf.Log10(masterVolume) * 20);
            masterSlider.onValueChanged.AddListener(delegate { SetMasterVolume(); });
        }
        
        // Initialize music volume slider
        if (musicSlider != null && myMixer != null)
        {
            float musicVolume = PlayerPrefs.GetFloat("Music Volume", 1.0f);
            musicSlider.value = musicVolume;
            myMixer.SetFloat("music", Mathf.Log10(musicVolume) * 20);
            musicSlider.onValueChanged.AddListener(delegate { SetMusicVolume(); });
        }

        // Initialize SFX volume slider
        if (sfxSlider != null && myMixer != null)
        {
            float sfxVolume = PlayerPrefs.GetFloat("SFX Volume", 1.0f);
            sfxSlider.value = sfxVolume;
            myMixer.SetFloat("sfx", Mathf.Log10(sfxVolume) * 20);
            sfxSlider.onValueChanged.AddListener(delegate { SetSFXVolume(); });
            Debug.Log($"SFX volume initialized to: {sfxVolume}");
        }
    }


    // Sets the master volume based on the slider value.
    public void SetMasterVolume()
    {
        float volume = masterSlider.value; // Get slider value
        myMixer.SetFloat("master", Mathf.Log10(volume) * 20); // Convert to decibels and set master volume
        PlayerPrefs.SetFloat("Master Volume", volume); // Save the volume to PlayerPrefs
        PlayerPrefs.Save();
    }
    // Sets the music volume based on the slider value.
    public void SetMusicVolume()
    {
        float volume = musicSlider.value; 
        myMixer.SetFloat("music", Mathf.Log10(volume) * 20); 
        PlayerPrefs.SetFloat("Music Volume", volume); 
    }


    // Sets the SFX volume based on the slider value.
    public void SetSFXVolume()
    {
        float volume = sfxSlider.value; 
        myMixer.SetFloat("sfx", Mathf.Log10(volume) * 20); 
        PlayerPrefs.SetFloat("SFX Volume", volume); 
        PlayerPrefs.Save();
    }
}
