using PurrNet;
using PurrNet.Utils;
using System;
using UnityEngine;

public class EnemyHealth : NetworkIdentity
{
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private int expForKill = 1;

    [SerializeField, PurrReadOnly] private int currentHealth;

    public static Action<EnemyHealth> OnEnemyKilled;
    private void Awake()
    {
        currentHealth = maxHealth;
    }

    [ServerRpc(requireOwnership: false)]
    public void DealDamage(int damage)
    {
        if (!isSpawned) { return; }

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        OnEnemyKilled?.Invoke(this);
        if (InstanceHandler.TryGetInstance<LevelManager>(out LevelManager levelManager))
        {
            levelManager.AddExp(expForKill);
        }
        Destroy(gameObject);
    }
}
