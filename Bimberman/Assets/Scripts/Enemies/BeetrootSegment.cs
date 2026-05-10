using UnityEngine;

public class BeetrootSegment : MonoBehaviour
{
    private BeetrootEnemy owner;

    public void Initialize(BeetrootEnemy enemy) => owner = enemy;

        private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc)
            {
                pc.TakeDamage(15);
                if (owner != null)
                    owner.OnSegmentHitPlayer();
            }
        }
    }
}

