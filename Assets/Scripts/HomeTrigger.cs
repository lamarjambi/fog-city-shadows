using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("player on home trigger");
            PickUpScript pickUpScript = other.GetComponentInChildren<PickUpScript>();

            if (pickUpScript != null)
{
                bool hasFood = pickUpScript.inventory.Contains("Food");
                bool hasLaundry = pickUpScript.inventory.Contains("Laundry");

                if (hasFood && hasLaundry)
                {
                    Time.timeScale = 1f;
                    SceneManager.LoadScene("EndScene");
                }
                else
                {
                    Debug.Log($"Missing items — Food: {hasFood}, Laundry: {hasLaundry}");
                }
            }
        }
    }
}