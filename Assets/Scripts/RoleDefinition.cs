using UdonSharp;
using UnityEngine;

/// <summary>
/// Define un rol visual seleccionable desde el Inspector.
/// Crea un GameObject por rol y asigna aquí su nombre lógico y sprite PNG.
/// </summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class RoleDefinition : UdonSharpBehaviour
{
    [Header("Role")]
    [Tooltip("Nombre interno del rol, por ejemplo Owner, VIP, DJ o Staff.")]
    public string roleName;

    [Tooltip("Sprite transparente que se mostrará encima de la cabeza del jugador.")]
    public Sprite roleSprite;
}
