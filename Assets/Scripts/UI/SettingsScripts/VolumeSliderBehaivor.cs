using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VolumeSliderBehaivor : MonoBehaviour
{
    [SerializeField] private string TargetSettingsSlider = "Master-VOL";
    private VolumeSettingsManager target;
    Slider _slider;
    void Start()
    {
        target = VolumeSettingsManager.instance;
        _slider = GetComponent<Slider>(); // not using tryget bc it is required component to add the script.
        _slider.onValueChanged.AddListener((_) =>
        {
            target.SetVolume(TargetSettingsSlider, _slider.value);
        });
    }   
}
