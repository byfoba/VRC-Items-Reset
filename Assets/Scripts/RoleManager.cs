using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

/// <summary>
/// Instancia badges locales para jugadores cuyo displayName coincide con la configuración.
/// </summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class RoleManager : UdonSharpBehaviour
{
    [Header("Prefab")]
    [Tooltip("Prefab local con SpriteRenderer y RoleBillboard. No necesita networking.")]
    [SerializeField] private GameObject badgePrefab;

    [Header("Player Role Assignments")]
    [Tooltip("DisplayName exacto del jugador. Debe tener la misma longitud que Player Roles.")]
    [SerializeField] private string[] playerNames;

    [Tooltip("Rol asignado a cada jugador. Debe tener la misma longitud que Player Names.")]
    [SerializeField] private RoleDefinition[] playerRoles;

    [Header("Badge Transform")]
    [Tooltip("Altura extra sobre el tracking Head del jugador.")]
    [SerializeField] private float badgeHeight = 0.35f;

    [Tooltip("Escala uniforme aplicada al prefab del badge.")]
    [SerializeField] private float badgeScale = 0.25f;

    [Header("Capacity")]
    [Tooltip("Cantidad máxima de badges locales activos que este manager administrará.")]
    [SerializeField] private int maxBadgeSlots = 100;

    private VRCPlayerApi[] trackedPlayers;
    private GameObject[] badgeObjects;
    private RoleBillboard[] badgeBillboards;
    private bool initialized;

    private void Start()
    {
        if (maxBadgeSlots < 1)
        {
            maxBadgeSlots = 1;
        }

        trackedPlayers = new VRCPlayerApi[maxBadgeSlots];
        badgeObjects = new GameObject[maxBadgeSlots];
        badgeBillboards = new RoleBillboard[maxBadgeSlots];
        initialized = true;

        int playerCount = VRCPlayerApi.GetPlayerCount();
        VRCPlayerApi[] players = new VRCPlayerApi[playerCount];
        VRCPlayerApi.GetPlayers(players);

        for (int i = 0; i < players.Length; i++)
        {
            TryCreateBadgeForPlayer(players[i]);
        }
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (!initialized)
        {
            return;
        }

        TryCreateBadgeForPlayer(player);
    }

    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        if (!initialized)
        {
            return;
        }

        RemoveBadgeForPlayer(player);
    }

    private void TryCreateBadgeForPlayer(VRCPlayerApi player)
    {
        if (player == null || !player.IsValid())
        {
            return;
        }

        if (badgePrefab == null)
        {
            return;
        }

        if (FindSlotForPlayer(player) >= 0)
        {
            return;
        }

        RoleDefinition role = GetRoleForDisplayName(player.displayName);
        if (role == null || role.roleSprite == null)
        {
            return;
        }

        int slot = FindFreeSlot();
        if (slot < 0)
        {
            return;
        }

        GameObject badgeObject = VRCInstantiate(badgePrefab);
        if (badgeObject == null)
        {
            return;
        }

        RoleBillboard billboard = (RoleBillboard)badgeObject.GetComponent(typeof(RoleBillboard));
        if (billboard == null)
        {
            Object.Destroy(badgeObject);
            return;
        }

        trackedPlayers[slot] = player;
        badgeObjects[slot] = badgeObject;
        badgeBillboards[slot] = billboard;
        billboard.Initialize(player, role.roleSprite, badgeHeight, badgeScale);
        badgeObject.SetActive(true);
    }

    private RoleDefinition GetRoleForDisplayName(string displayName)
    {
        if (playerNames == null || playerRoles == null)
        {
            return null;
        }

        int count = playerNames.Length;
        if (playerRoles.Length < count)
        {
            count = playerRoles.Length;
        }

        for (int i = 0; i < count; i++)
        {
            if (playerNames[i] == displayName)
            {
                return playerRoles[i];
            }
        }

        return null;
    }

    private int FindSlotForPlayer(VRCPlayerApi player)
    {
        for (int i = 0; i < trackedPlayers.Length; i++)
        {
            if (trackedPlayers[i] == player)
            {
                return i;
            }
        }

        return -1;
    }

    private int FindFreeSlot()
    {
        for (int i = 0; i < trackedPlayers.Length; i++)
        {
            if (trackedPlayers[i] == null)
            {
                return i;
            }
        }

        return -1;
    }

    private void RemoveBadgeForPlayer(VRCPlayerApi player)
    {
        int slot = FindSlotForPlayer(player);
        if (slot < 0)
        {
            return;
        }

        if (badgeBillboards[slot] != null)
        {
            badgeBillboards[slot].ClearTarget();
        }

        if (badgeObjects[slot] != null)
        {
            Object.Destroy(badgeObjects[slot]);
        }

        trackedPlayers[slot] = null;
        badgeObjects[slot] = null;
        badgeBillboards[slot] = null;
    }
}
