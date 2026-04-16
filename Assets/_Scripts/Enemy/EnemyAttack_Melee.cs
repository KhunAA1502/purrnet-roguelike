using UnityEngine;

public class EnemyAttack_Melee : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    private float lastAttackTime;

    private void OnCollisionStay(Collision other)
    {
        if (lastAttackTime + 1f > Time.time)
        {
            return;
        }

        if (!other.transform.TryGetComponent(out PlayerHealth playerHealth) || !playerHealth.isOwner)
        {
            return;
        }

        lastAttackTime = Time.time;
        playerHealth.ChangeHealth(-damage);
    }
}
