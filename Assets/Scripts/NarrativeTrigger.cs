using UnityEngine;

public class NarrativeTrigger : MonoBehaviour
{
    public int narrativeInd;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            NarrativeManager.Instance.ShowNarrative(narrativeInd);
            Debug.Log("on trigger");
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            NarrativeManager.Instance.HideNarrative();
    }
}