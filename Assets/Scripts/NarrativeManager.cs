using UnityEngine;
using TMPro;
using System.Collections.Generic;

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

        // check if narrative has objective
        foreach (var obj in narrativeObjectives)
        {
            if (obj.narrativeIndex == index)
            {
                if (!objectiveText.text.Contains(obj.objectiveText))
                {
                    // add to objectives on a new line or set
                    if (string.IsNullOrEmpty(objectiveText.text))
                        objectiveText.text = obj.objectiveText;
                    else
                        objectiveText.text += "\n" + obj.objectiveText;
                    break;
                }
            }
        }
    }

    public void HideNarrative()
    {
        textBackground.SetActive(false);
    }

    public void RemoveObjective(int narrativeIndex)
    {
        foreach (var obj in narrativeObjectives)
        {
            if (obj.narrativeIndex == narrativeIndex)
            {
                // Split lines, remove the matching one, rejoin
                var lines = new List<string>(objectiveText.text.Split('\n'));
                lines.Remove(obj.objectiveText);
                objectiveText.text = string.Join("\n", lines);
                break;
            }
        }
    }
}

[System.Serializable]
public class NarrativeObjective
{
    public int narrativeIndex;
    public string objectiveText;
}