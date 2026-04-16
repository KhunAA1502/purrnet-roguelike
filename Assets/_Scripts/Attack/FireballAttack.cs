using UnityEngine;

public class FireballAttack : Attack
{
    [SerializeField] private SimpleProjectile projectile;
    private FireballData.LevelData levelData;
    private float lastCastTime;
    protected override void OnInitialize()
    {
        FireballData fireballData = (FireballData)data;
        levelData = fireballData.GetLevelData(level);
    }
    public override void Tick()
    {
        if (lastCastTime + levelData.cooldown > Time.time)
        {
            return;
        }
        lastCastTime = Time.time;

        for (int i = 0; i < levelData.projectileCount; i++)
        {
            var direction = Random.insideUnitSphere;
            direction.y = 0;
            direction.Normalize();
            SimpleProjectile projectile = Instantiate(this.projectile, transform.position + Vector3.up * 0.5f, Quaternion.LookRotation(direction));
            projectile.Initialize(levelData.damage, levelData.speed);
        }
    }
}
