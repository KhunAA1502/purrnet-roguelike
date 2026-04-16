using System.Collections.Generic;
using PurrNet.StateMachine;
using UnityEngine;

public class PlayerState_Down : StateNode
{
    [SerializeField] private StateNode aliveState;
    [SerializeField] private float reviveDistance = 2f;
    [SerializeField] private float reviveTime = 3f;
    [SerializeField] private GameObject graphics;
    [SerializeField] private List<MonoBehaviour> components;
    private float reviveProgress;

    override public void Enter(bool asServer)
    {
        base.Enter(asServer);
        graphics.SetActive(true);
        ToggleComponents(false);
    }

    override public void Exit(bool asServer)
    {
        base.Exit(asServer);
        graphics.SetActive(false);
        ToggleComponents(true);
    }
    private void ToggleComponents(bool enabled)
    {
        if (!isOwner) { return; }

        foreach (var component in components)
        {
            component.enabled = enabled;
        }
    }
    public override void StateUpdate(bool asServer)
    {
        base.StateUpdate(asServer);

        if (!isOwner || asServer) { return; }

        bool beingRevived = false;
        foreach (var player in PlayerHealth.AllPlayers.Values)
        {
            if (player.isOwner) { continue; }
            if (Vector3.Distance(player.transform.position, transform.position) > reviveDistance)
            {
                continue;
            }
            beingRevived = true;
            reviveProgress += Time.deltaTime;
        }

        if (!beingRevived)
        {
            reviveProgress = 0f;
        }

        if (reviveProgress >= reviveTime)
        {
            machine.SetState(aliveState);
        }
    }
}
