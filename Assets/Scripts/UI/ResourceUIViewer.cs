using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceUIViewer : MonoBehaviour
{
    [Header("Set-up")]
    [SerializeField] private Image resourceImageUI;
    [SerializeField] private TMP_Text resourceNameUI;
    [SerializeField] private TMP_Text resourceGainUI;
    [Header("Other Settings")]
    [SerializeField] private Sprite blankSprite;
    [SerializeField] private string blankName;
    internal Resource referencedres;
    internal Sprite _resSprite;
    internal string _resName;
    internal string _resOtherText;
    private void LateUpdate()
    {
        resourceImageUI.sprite = _resSprite ?? blankSprite;
        resourceNameUI.text = _resName ?? blankName;
        resourceGainUI.text = _resOtherText ?? "N/A\nN/A";
    }
}
