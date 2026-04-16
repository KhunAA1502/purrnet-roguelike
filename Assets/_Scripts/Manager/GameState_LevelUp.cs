using PurrNet;
using PurrNet.StateMachine;
using System.Collections.Generic;
using UnityEngine;

public class GameState_LevelUp : StateNode
{
    [SerializeField] private StateNode runningState;
    [SerializeField] private List<AttackData> allAttacks = new();

    [Header("UI")]
    [SerializeField] private GameObject levelScreen;
    [SerializeField] private GameObject waitingScreen;
    [SerializeField] private Transform upgradeHolder;
    [SerializeField] private LevelEntry entryPrefab;

    private List<PlayerID> readyPlayers = new();

    private void Awake()
    {
        levelScreen.SetActive(false);
        waitingScreen.SetActive(false);
    }
    public override void Enter(bool asServer)
    {
        base.Enter(asServer);
        if (asServer)
        {
            return;
        }
        readyPlayers.Clear();
        SetupLevelOptions();
        levelScreen.SetActive(true);
        waitingScreen.SetActive(false);
        Time.timeScale = 0f;
    }
    public override void Exit(bool asServer)
    {
        base.Exit(asServer);
        if (asServer)
        {
            return;
        }
        levelScreen.SetActive(false);
        waitingScreen.SetActive(false);
        Time.timeScale = 1f;
    }
    private void SetupLevelOptions()
    {
        foreach (Transform child in upgradeHolder)
        {
            Destroy(child.gameObject);
        }
        List<AttackData> availableAttacks = GetAvailableAttacks();

        if (availableAttacks == null || availableAttacks.Count <= 0)
        {
            Debug.LogWarning("No available attacks found for level up! Skipping...");
            SetReady();
            return;
        }

        var randomAttacks = new List<AttackData>();
        while (randomAttacks.Count < 3 && availableAttacks.Count > 0)
        {
            int randomIndex = Random.Range(0, availableAttacks.Count);
            randomAttacks.Add(availableAttacks[randomIndex]);
            availableAttacks.RemoveAt(randomIndex);
        }

        foreach (var attack in randomAttacks)
        {
            var entry = Instantiate(entryPrefab, upgradeHolder);
            entry.Init(attack, this);
        }
    }

    private List<AttackData> GetAvailableAttacks()
    {
        if (!InstanceHandler.TryGetInstance(out AttackHandler attackHandler))
        {
            return null;
        }

        List<AttackData> availableAttacks = new List<AttackData>();
        foreach (var attack in allAttacks)
        {
            if (attackHandler.GetLevel(attack.attackId) < attack.maxLevel)
            {
                availableAttacks.Add(attack);
            }
        }

        return availableAttacks;
    }

    public void SetReady()
    {
        waitingScreen.SetActive(true);
        SetReadyRpc();
    }

    [ServerRpc(requireOwnership: false)]
    private void SetReadyRpc(RPCInfo info = default)
    {
        if (readyPlayers.Contains(info.sender))
        {
            return;
        }

        readyPlayers.Add(info.sender);

        if (readyPlayers.Count < PlayerHealth.AllPlayers.Count)
        {
            return;
        }

        machine.SetState(runningState);
    }
}
