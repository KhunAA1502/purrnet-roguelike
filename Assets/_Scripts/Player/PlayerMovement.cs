using PurrNet;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : NetworkIdentity
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private CinemachineCamera playerCamera;
    [SerializeField] private InputReader inputReader;
    private Rigidbody playerRigidbody;

    private void Awake()
    {
        TryGetComponent(out playerRigidbody);
    }

    protected override void OnSpawned()
    {
        base.OnSpawned();

        enabled = isOwner;
        playerCamera.Priority.Value = isOwner ? 10 : 0;

        if (isOwner)
        {
            playerCamera.transform.SetParent(null);
        }
    }

    private void FixedUpdate()
    {
        Vector3 input = new Vector3(inputReader.MovementInput.x, 0, inputReader.MovementInput.y);
        if (input.magnitude > 1)
        {
            input.Normalize();
        }

        Vector3 movement = input * speed;
        playerRigidbody.linearVelocity = movement;


        if (playerRigidbody.linearVelocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(playerRigidbody.linearVelocity, Vector3.up);
        }
    }
}
