using PurrNet;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyMovement : NetworkBehaviour, ITick
{
    [SerializeField] private float speed = 2f;
    private Rigidbody enemyRigidbody;
    private PlayerHealth targetPlayer;
    private float lastPlayerCheck;

    private void Awake()
    {
        TryGetComponent(out enemyRigidbody);
    }

    protected override void OnSpawned()
    {
        base.OnSpawned();

        enabled = isServer;
    }
    private void FixedUpdate()
    {
        if (!isServer)
        {
            return;
        }
        if (!targetPlayer)
        {
            return;
        }

        Vector3 targetPos = targetPlayer.transform.position;
        Vector3 direction = (targetPos - transform.position).normalized;
        direction.y = 0;
        enemyRigidbody.linearVelocity = direction * speed;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        enemyRigidbody.MoveRotation(Quaternion.Slerp(transform.rotation, lookRotation, Time.fixedDeltaTime * 10f));
    }

    public void OnTick(float delta)
    {
        if (!isServer)
        {
            return;
        }
        if (lastPlayerCheck + 1 > Time.time)
        {
            return;
        }
        lastPlayerCheck = Time.time;

        var allPlayers = PlayerHealth.AllPlayers;
        if (allPlayers.Count <= 0) { return; }

        PlayerID closestPlayer = default;
        float closestDistance = float.MaxValue;
        foreach (var player in allPlayers)
        {
            Vector3 playerPos = player.Value.transform.position;
            float distance = Vector3.Distance(playerPos, transform.position);
            if (distance < closestDistance && !player.Value.IsDowned)
            {
                closestPlayer = player.Key;
                closestDistance = distance;
            }
        }

        if (closestPlayer == default) { return; }

        PlayerHealth.AllPlayers.TryGetValue(closestPlayer, out targetPlayer);
    }
}
