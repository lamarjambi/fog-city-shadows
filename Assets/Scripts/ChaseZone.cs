using UnityEngine;

public class ChaseZone : MonoBehaviour
{
    public MonsterAI monster; 

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            monster.PlayerEntered();
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            monster.PlayerExited();
    }
}