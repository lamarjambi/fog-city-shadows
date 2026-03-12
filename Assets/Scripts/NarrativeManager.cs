using UnityEngine;
using TMPro;

public class NarrativeManager : MonoBehaviour
{
    public static NarrativeManager Instance;

    [TextArea] public string[] narrativeLines;
    public GameObject[] triggers;
    public GameObject textBackground;
    public TextMeshProUGUI narrativeText;

    [Header("Objectives")]
    public TextMeshProUGUI objectiveText;  // drag your objectives UI text here
    public NarrativeObjective[] narrativeObjectives;

    void Awake()
    {
        Instance = this;
        textBackground.SetActive(false);

        for (int i = 0; i < triggers.Length; i++)
            triggers[i].GetComponent<NarrativeTrigger>().narrativeInd = i;
    }

    public void ShowNarrative(int index)
    {
        if (index < 0 || index >= narrativeLines.Length) return;
        narrativeText.text = narrativeLines[index];
        textBackground.SetActive(true);

        // check if this narrative index has an objective
        foreach (var obj in narrativeObjectives)
        {
            if (obj.narrativeIndex == index)
            {
                objectiveText.text = obj.objectiveText;
                break;
            }
        }
    }

    public void HideNarrative()
    {
        textBackground.SetActive(false);
    }
}

[System.Serializable]
public class NarrativeObjective
{
    public int narrativeIndex;
    public string objectiveText;
}