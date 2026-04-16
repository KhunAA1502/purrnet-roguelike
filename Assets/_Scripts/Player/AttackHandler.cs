using System.Collections.Generic;
using PurrNet;
using UnityEngine;

public class AttackHandler : NetworkBehaviour, ITick
{
    private Dictionary<string, Attack> activeAttacks = new();
    [SerializeField] private List<AttackData> initialAttacks = new();

    protected override void OnSpawned()
    {
        base.OnSpawned();

        if (!isOwner) { return; }

        InstanceHandler.RegisterInstance(this);
        foreach (var attack in initialAttacks)
        {
            AddAttack(attack);
        }
    }
    protected override void OnDespawned()
    {
        base.OnDespawned();
        if (isOwner)
        {
            InstanceHandler.UnregisterInstance<AttackHandler>();
        }
    }

    public void AddAttack(AttackData data)
    {
        if (activeAttacks.TryGetValue(data.attackId, out Attack atk))
        {
            int newLevel = atk.level + 1;
            atk.Initialize(data, newLevel);
            return;
        }

        Attack attack = Instantiate(data.prefab, transform);
        attack.Initialize(data, 0);
        activeAttacks[data.attackId] = attack;
    }

    public int GetLevel(string id)
    {
        return activeAttacks.TryGetValue(id, out Attack atk) ? atk.level : -1;
    }

    public void OnTick(float delta)
    {
        if (!isOwner) { return; }

        if (!enabled) { return; }

        foreach (var atk in activeAttacks)
        {
            atk.Value.Tick();
        }
    }
}
