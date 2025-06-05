using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class VolumeSettingsManager : MonoBehaviour
{
    internal static VolumeSettingsManager instance;
    [SerializeField] private List<string> volumeNames;
    [SerializeField] private AudioMixer audioMixer;
    private Dictionary<string, float> volumes;
    internal void SetVolume(string name, float volume)
    {
        if (Application.isEditor)
        {
            Debug.Log(name + " " + volume.ToString());
        }
        if (volumes.ContainsKey(name))
        {
            volumes[name] = volume;
        }
    }
    private void Start()
    {
        if (instance != null) Destroy(gameObject);
        instance = this;
        volumes = new();
        foreach (string name in volumeNames)
        {
            volumes[name] = 0f;
        }
        DontDestroyOnLoad(gameObject);
    }
    private void UpdateVolumes()
    {
        foreach (KeyValuePair<string, float> values in volumes)
        {
            if (audioMixer.GetFloat(values.Key, out _) == true)
            {
                audioMixer.SetFloat(values.Key, values.Value);
            }
        }
    }
    private void LateUpdate()
    {
        UpdateVolumes();
    }
}
