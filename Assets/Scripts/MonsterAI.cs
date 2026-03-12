using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform player;
    private bool playerInZone = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        if (playerInZone)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            agent.ResetPath(); // monster stops when player leaves
        }
    }

    public void PlayerEntered() { playerInZone = true; }
    public void PlayerExited()  { playerInZone = false; }
}