using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : PoolableObject
{
    [SerializeField]
    private float speed = 4.0f;

    private Character owner;
    private Rigidbody rb;
    private Transform oldParent;
    private Vector3 direction;
    private float speedMultiplier;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Execute();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Character>(out Character character))
        {
            if (character != owner)
            {
                SoundManager.Instance.PlayHitSound(transform.position);

                character.Hit(owner);

                Despawn();

                if (!owner.IsDead())
                {
                    owner.LevelUp(character);
                }
            }
        }

        if (other.TryGetComponent<Obstacle>(out Obstacle obstacle))
        {
            Despawn();
        }
    }

    public override void ReturnToPool()
    {
        base.ReturnToPool();

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public Character GetOwner()
    {
        return owner;
    }

    public void SetDirection(Vector3 direction)
    {
        this.direction = direction;
    }

    public void SetSpeedMultiplier(float speedMultiplier)
    {
        this.speedMultiplier = speedMultiplier;
    }

    public void Fly(Character owner)
    {
        this.owner = owner;
        transform.SetParent(null);
        SetDirection(transform.forward);
        SetSpeedMultiplier(1.0f);
    }

    protected virtual void Execute()
    {
        transform.position += direction * speed * speedMultiplier * owner.GetMultiplier() * Time.deltaTime;
    }

    protected virtual void Despawn()
    {
        if (owner.IsDead())
        {
            Destroy(gameObject);
        }
        else
        {
            ReturnToPool();
        }
    }
}
