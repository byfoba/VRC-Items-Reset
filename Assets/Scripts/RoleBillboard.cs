using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

/// <summary>
/// Mantiene un badge sobre la cabeza de un jugador y orientado hacia la vista local.
/// </summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class RoleBillboard : UdonSharpBehaviour
{
    private VRCPlayerApi targetPlayer;
    private VRCPlayerApi localPlayer;
    private SpriteRenderer spriteRenderer;
    private float heightOffset;
    private bool initialized;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        localPlayer = Networking.LocalPlayer;
    }

    public void Initialize(VRCPlayerApi player, Sprite sprite, float badgeHeight, float badgeScale)
    {
        targetPlayer = player;
        heightOffset = badgeHeight;
        transform.localScale = Vector3.one * badgeScale;

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = sprite;
        }

        if (localPlayer == null)
        {
            localPlayer = Networking.LocalPlayer;
        }

        initialized = targetPlayer != null && targetPlayer.IsValid();
        UpdateBadgeTransform();
    }

    public void ClearTarget()
    {
        initialized = false;
        targetPlayer = null;
    }

    public override void PostLateUpdate()
    {
        if (!initialized)
        {
            return;
        }

        if (targetPlayer == null || !targetPlayer.IsValid())
        {
            initialized = false;
            return;
        }

        UpdateBadgeTransform();
    }

    private void UpdateBadgeTransform()
    {
        VRCPlayerApi.TrackingData headData = targetPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
        transform.position = headData.position + Vector3.up * heightOffset;

        if (localPlayer != null && localPlayer.IsValid())
        {
            VRCPlayerApi.TrackingData localHeadData = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
            Vector3 lookDirection = transform.position - localHeadData.position;

            if (lookDirection.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
            }
        }
    }
}
