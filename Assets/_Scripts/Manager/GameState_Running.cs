using PurrNet;
using PurrNet.StateMachine;
using UnityEngine;

public class GameState_Running : StateNode
{
    [SerializeField] private StateNode lostState;
    [SerializeField] private StateNode levelState;

    public override void Enter(bool asServer)
    {
        base.Enter(asServer);

        if (!asServer) { return; }

        PlayerHealth.OnPlayerDied += OnPlayerDied;
        LevelManager.OnLevelChanged += OnLevelChange;
    }
    public override void Exit(bool asServer)
    {
        base.Exit(asServer);
        PlayerHealth.OnPlayerDied -= OnPlayerDied;
        LevelManager.OnLevelChanged -= OnLevelChange;
    }
    private void OnLevelChange(int newLevel)
    {
        machine.SetState(levelState);
    }
    private void OnPlayerDied(PlayerID playerId)
    {
        foreach (var player in PlayerHealth.AllPlayers.Values)
        {
            if (!player.IsDowned)
            {
                return;
            }
        }
        machine.SetState(lostState);
    }
}
