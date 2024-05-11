using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InfoButton : MonoBehaviour
{
    private bool displayingInfo = false;
    private string originalText;
    [SerializeField] private TextMeshProUGUI menuText;
    [SerializeField] [TextArea] private string infoText;
    [SerializeField] private AudioClip selectSound;

    void Start()
    {
        originalText = menuText.text;
    }

    public void DisplayInfo()
    {
        SoundFXManager.Instance.PlaySound(selectSound);
        
        if (displayingInfo)
        {
            displayingInfo = false;
            menuText.text = originalText;
        }
        else if (!displayingInfo)
        {
            displayingInfo = true;
            menuText.text = infoText;
        }
    }
}
