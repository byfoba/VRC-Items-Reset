using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Components;

/// <summary>
/// Resetea un item a su posición inicial si no hubo interacción durante X segundos.
/// Pensado para objetos pickeables con sincronización de red.
/// </summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class NetworkedItemAutoReset : UdonSharpBehaviour
{
    [Header("Auto Reset")]
    [Tooltip("Segundos de inactividad antes de volver al punto inicial (600 = 10 min).")]
    [SerializeField] private float inactivitySeconds = 600f;

    [Tooltip("Si está activado, se reinicia la velocidad al volver al origen.")]
    [SerializeField] private bool zeroVelocityOnReset = true;

    private VRC_Pickup pickup;
    private VRCObjectSync objectSync;
    private Rigidbody rb;

    private Vector3 startPosition;
    private Quaternion startRotation;

    private int lastInteractionServerMs;
    private bool initialized;

    private void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;

        pickup = (VRC_Pickup)GetComponent(typeof(VRC_Pickup));
        objectSync = (VRCObjectSync)GetComponent(typeof(VRCObjectSync));
        rb = GetComponent<Rigidbody>();

        lastInteractionServerMs = Networking.GetServerTimeInMilliseconds();
        initialized = true;
    }

    private void Update()
    {
        if (!initialized)
        {
            return;
        }

        if (!Networking.IsOwner(gameObject))
        {
            return;
        }

        if (pickup != null && pickup.IsHeld)
        {
            return;
        }

        int nowMs = Networking.GetServerTimeInMilliseconds();
        int elapsedMs = nowMs - lastInteractionServerMs;
        float timeoutMs = inactivitySeconds * 1000f;

        if (elapsedMs >= timeoutMs)
        {
            DoResetNow();
            lastInteractionServerMs = nowMs;
        }
    }

    public override void Interact()
    {
        RegisterTouchFromAnyPlayer();
    }

    public override void OnPickup()
    {
        RegisterTouchFromAnyPlayer();
    }

    public override void OnDrop()
    {
        RegisterTouchFromAnyPlayer();
    }

    public override void OnOwnershipTransferred(VRCPlayerApi player)
    {
        if (Networking.IsOwner(gameObject))
        {
            OwnerRegisterTouch();
        }
    }

    private void RegisterTouchFromAnyPlayer()
    {
        if (Networking.IsOwner(gameObject))
        {
            OwnerRegisterTouch();
            return;
        }

        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(OwnerRegisterTouch));
    }

    public void OwnerRegisterTouch()
    {
        if (!Networking.IsOwner(gameObject))
        {
            return;
        }

        lastInteractionServerMs = Networking.GetServerTimeInMilliseconds();
    }

    public void DoResetNow()
    {
        if (!Networking.IsOwner(gameObject))
        {
            return;
        }

        if (objectSync != null)
        {
            objectSync.FlagDiscontinuity();
        }

        transform.SetPositionAndRotation(startPosition, startRotation);

        if (rb != null && zeroVelocityOnReset)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
