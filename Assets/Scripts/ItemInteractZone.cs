using UnityEngine;
using TMPro;

public class ItemInteractZone : MonoBehaviour
{
    public TextMeshPro worldText;
    public string promptMessage = "Press E to pick up";
    public Vector3 textOffset = new Vector3(0, 1.5f, 0);

    private Outline outline;

    void Start()
    {
        outline = GetComponent<Outline>(); 
        worldText.gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            worldText.transform.position = transform.position + textOffset;
            worldText.gameObject.SetActive(true);
            if (outline != null) outline.enabled = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            worldText.gameObject.SetActive(false);
            if (outline != null) outline.enabled = false;
        }
    }
}