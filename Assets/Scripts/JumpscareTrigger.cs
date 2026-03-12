using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JumpscareTrigger : MonoBehaviour
{
    [SerializeField] private GameObject jumpscareImage;
    [SerializeField] private AudioSource jumpscareSound;
    [SerializeField] private float jumpscareDuration = 3f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(JumpscareSequence());
        }
    }

    private IEnumerator JumpscareSequence()
    {
        jumpscareImage.SetActive(true);
        jumpscareSound.Play();
        yield return new WaitForSeconds(jumpscareDuration);
        SceneManager.LoadScene("EndScene");
    }
}