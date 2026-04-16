using System;
using PurrNet;
using UnityEngine;

public class LevelManager : NetworkIdentity
{
    [SerializeField] private int expToLevel = 5;
    private SyncVar<int> exp = new();
    private SyncVar<int> level = new();

    public static Action<int> OnExpChanged;
    public static Action<int> OnLevelChanged;

    private int expToNextLevel => expToLevel * (level + 1);

    private void Awake()
    {
        InstanceHandler.RegisterInstance(this);
        exp.onChanged += HandleExpChanged;
        level.onChanged += HandleLevelChanged;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        InstanceHandler.UnregisterInstance<LevelManager>();
        exp.onChanged -= HandleExpChanged;
        level.onChanged -= HandleLevelChanged;
    }
    private void HandleExpChanged(int newExp)
    {
        Debug.Log($"Exp changed to {newExp}");
        OnExpChanged?.Invoke(newExp);
    }

    private void HandleLevelChanged(int newLevel)
    {
        Debug.Log($"Level changed to {newLevel}");
        OnLevelChanged?.Invoke(newLevel);
    }

    public void AddExp(int amount)
    {
        if (!isServer) { return; }
        exp.value += amount;
        CheckForLevel();
    }

    private void CheckForLevel()
    {
        if (exp.value < expToNextLevel)
        {
            return;
        }
        exp.value -= expToNextLevel;
        level.value++;
    }
}
