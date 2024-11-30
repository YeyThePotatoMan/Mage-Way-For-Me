using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{

    // Handles saving and loading settings using PlayerPrefs and updating UI sliders in the scene.
    public static SettingsManager Instance { get; private set; }

    [Header("UI Elements")]
    public Slider[] sliders; // Sliders for controlling settings like Master Volume, Music Volume, and SFX Volume.

    [Header("Events")]
    public UnityEvent<string, float> FloatPropertyChangeEvent; // Event triggered when a float property (e.g., volume) changes.

    // Dictionary to store float settings such as volume levels.
    public readonly Dictionary<string, float> FloatSettings = new()
    {
        { "Master Volume", 1.0f },
        { "Music Volume", 1.0f }, 
        { "SFX Volume", 1.0f },   
    };

    private void Awake()
    {
        // Ensure only one instance of SettingsManager exists.
        if (Instance != null && Instance != this)
        {
            Destroy(Instance.gameObject);
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialize events if not already set.
        FloatPropertyChangeEvent ??= new UnityEvent<string, float>();

        // Subscribe to property change events.
        FloatPropertyChangeEvent.AddListener(OnFloatPropertyChanged);

        // Load saved settings or initialize default values.
        foreach (var setting in FloatSettings.ToList())
        {
            if (PlayerPrefs.HasKey(setting.Key))
            {
                FloatSettings[setting.Key] = PlayerPrefs.GetFloat(setting.Key); // Load saved value.
            }
            else
            {
                PlayerPrefs.SetFloat(setting.Key, setting.Value); // Save default value if not found.
            }
        }
    }

    /// Initializes UI sliders with current settings values and subscribes their value change events.
    private void Start()
    {
        foreach (var slider in sliders)
        {
            if (slider != null && FloatSettings.ContainsKey(slider.name))
            {
                slider.value = FloatSettings[slider.name]; // Set slider to match saved setting.
                slider.onValueChanged.AddListener((value) => FloatPropertyChangeEvent.Invoke(slider.name, value)); // Link slider change to event.
            }
        }
    }


    // Handles changes to float settings when updated via UI sliders and saves the new value to PlayerPrefs and updates the internal dictionary.
    private void OnFloatPropertyChanged(string key, float value)
    {
        FloatSettings[key] = value; 
        PlayerPrefs.SetFloat(key, value);
        PlayerPrefs.Save(); 
    }
}
