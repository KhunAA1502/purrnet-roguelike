using PurrNet;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SimpleProjectile : NetworkBehaviour
{
    private int damage;
    private float speed;
    private float duration;
    private Rigidbody projectileRigidbody;

    private float lifeTime;

    private void Awake()
    {
        TryGetComponent(out projectileRigidbody);
    }
    protected override void OnSpawned()
    {
        base.OnSpawned();
        enabled = isOwner;
    }
    public void Initialize(int damage, float speed, float duration = 4f)
    {
        this.damage = damage;
        this.speed = speed;
        this.duration = duration;
        projectileRigidbody.linearVelocity = transform.forward * speed;
    }

    private void Update()
    {
        if (!isOwner) { return; }

        lifeTime += Time.deltaTime;
        if (lifeTime >= duration)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isOwner) { return; }

        if (!other.TryGetComponent(out EnemyHealth enemyHealth))
        {
            return;
        }
        if (!enemyHealth.isSpawned) { return; }
        enemyHealth.DealDamage(damage);
        Destroy(gameObject);
    }
}



