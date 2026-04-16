using PurrNet;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    private static readonly Dictionary<PlayerID, PlayerHealth> allPlayers = new();
    public static Dictionary<PlayerID, PlayerHealth> AllPlayers => allPlayers;

    [SerializeField] private SyncVar<int> maxHealth = new SyncVar<int>(100, ownerAuth: true);
    [SerializeField] private SyncVar<int> currentHealth = new SyncVar<int>(100, ownerAuth: true);

    public int MaxHealth => maxHealth.value;
    public int CurrentHealth => currentHealth.value;
    public bool IsDowned => currentHealth.value <= 0;
    public Action<int> onHealthChange;
    public Action onPlayerDied_local;

    public static Action<PlayerID> OnPlayerDied;
    private void Awake()
    {
        currentHealth.onChanged += OnHealthChanged;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        currentHealth.onChanged -= OnHealthChanged;
    }

    [RuntimeInitializeOnLoadMethod]
    private static void Clear()
    {
        allPlayers.Clear();
    }

    override protected void OnSpawned()
    {
        base.OnSpawned();
        if (owner.HasValue)
        {
            allPlayers[owner.Value] = this;
        }
    }

    protected override void OnDespawned()
    {
        base.OnDespawned();
        if (owner.HasValue)
        {
            allPlayers.Remove(owner.Value);
        }
    }
    private void OnHealthChanged(int newHealth)
    {
        onHealthChange?.Invoke(newHealth);
        if (newHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (owner.HasValue)
        {
            OnPlayerDied?.Invoke(owner.Value);
        }
        if (!isOwner) { return; }

        onPlayerDied_local?.Invoke();
        Debug.Log("You are dead!");
    }

    public void ChangeHealth(int change)
    {
        if (!isOwner) { return; }
        currentHealth.value = Mathf.Clamp(currentHealth.value + change, 0, maxHealth.value);
    }

    public void SetHealth(int health)
    {
        if (!isOwner) { return; }
        currentHealth.value = health;
    }
}
