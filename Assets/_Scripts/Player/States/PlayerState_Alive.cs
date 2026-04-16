using PurrNet.StateMachine;
using UnityEngine;

public class PlayerState_Alive : StateNode
{
    [SerializeField] private GameObject graphics;
    [SerializeField] private PlayerHealth playerHealth;

    public override void Enter(bool asServer)
    {
        base.Enter();

        if (asServer)
        {
            return;
        }

        graphics.SetActive(true);

        if (isOwner)
        {
            playerHealth.onPlayerDied_local += OnPlayerDead;
            playerHealth.SetHealth(playerHealth.MaxHealth);
        }
    }

    override public void Exit(bool asServer)
    {
        base.Exit(asServer);

        graphics.SetActive(false);
        playerHealth.onPlayerDied_local -= OnPlayerDead;
    }

    private void OnPlayerDead()
    {
        machine.Next();
    }
}
