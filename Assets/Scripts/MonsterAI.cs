using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class MonsterAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform player;
    private bool playerInZone = false;
    private bool jumpscareTriggered = false;

    [SerializeField] private float catchDistance = 5f;
    [SerializeField] private GameObject jumpscareImage;
    [SerializeField] private AudioSource jumpscareSound;
    [SerializeField] private AudioSource monsterSound;
    [SerializeField] private float jumpscareDuration = 3f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        if (jumpscareTriggered) return;

        if (playerInZone)
        {
            agent.SetDestination(player.position);

            float distance = Vector3.Distance(transform.position, player.position);
            Debug.Log("Distance to player: " + distance);
            if (distance <= catchDistance)
            {
                jumpscareTriggered = true;
                Debug.Log("Jumpscare triggered!");
                agent.ResetPath();
                StartCoroutine(JumpscareSequence());
            }
        }
        else
        {
            agent.ResetPath();
        }
    }

    private IEnumerator JumpscareSequence()
    {
        jumpscareImage.SetActive(true);
        jumpscareSound.Play();
        yield return new WaitForSeconds(jumpscareDuration);
        SceneManager.LoadScene("EndScene");
    }

    public void PlayerEntered()
    {
        playerInZone = true;
        monsterSound.Play();
    }

    public void PlayerExited()
    {
        playerInZone = false;
        monsterSound.Stop(); 
    }
}