using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SeedItem : MonoBehaviour
{
    public Toggle toggle;
    public TMP_Text label;

    public string seed;

    public void Setup(string seedName)
    {
        seed = seedName;
        label.text = seedName;
        toggle.isOn = false; 
    }

    public bool isToggOn() { 
        return toggle.isOn;
    }

    public void ChangeTogg(bool value) { 
        toggle.isOn = value;
    }

    
}