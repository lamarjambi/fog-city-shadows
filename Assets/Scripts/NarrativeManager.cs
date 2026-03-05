using UnityEngine;
using TMPro;

public class NarrativeManager : MonoBehaviour
{
    public static NarrativeManager Instance;

    [TextArea] public string[] narrativeLines; 
    public GameObject textBackground;         
    public TextMeshProUGUI narrativeText;      

    void Awake()
    {
        Instance = this;
        textBackground.SetActive(false); 
    }

    public void ShowNarrative(int index)
    {
        if (index < 0 || index >= narrativeLines.Length) return;
        narrativeText.text = narrativeLines[index];
        textBackground.SetActive(true);
    }

    public void HideNarrative()
    {
        textBackground.SetActive(false);
    }
}