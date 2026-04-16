using PurrNet.StateMachine;
using UnityEngine;

public class GameState_Lost : StateNode
{
    public override void Enter(bool asServer)
    {
        base.Enter(asServer);
        if (asServer) { return; }
        Debug.Log($"You lost!");
    }
}
