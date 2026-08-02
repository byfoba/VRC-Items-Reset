# Auto reset de items en VRChat (UdonSharp)

Este repo incluye un `UdonSharpBehaviour` para que objetos como palillos, comida o lapiceras vuelvan automáticamente a su posición original tras un período de inactividad.

## Archivo principal
- `Assets/Scripts/NetworkedItemAutoReset.cs`

## Qué hace
- Guarda posición/rotación inicial al iniciar la escena.
- Controla inactividad (por defecto: **600 segundos = 10 minutos**).
- Si el objeto no está siendo sostenido y vence el timeout:
  - el **owner** hace el reset,
  - aplica `TeleportTo(...)` si hay `VRCObjectSync` para mantener sincronización.
- Registra interacción cuando alguien:
  - usa `Interact`,
  - levanta el objeto (`OnPickup`),
  - suelta el objeto (`OnDrop`).

## Configuración recomendada en Unity (VRChat SDK 3.10.2)
1. Asegúrate de usar **UdonSharp**.
2. En cada item que quieras auto-resetear, agrega:
   - `VRC Pickup`
   - `VRC Object Sync` (muy recomendado para sync visual correcto)
   - `Rigidbody` (si el objeto usa física)
   - `NetworkedItemAutoReset` (este script)
3. En el script, deja `Inactivity Seconds` en `600` (o cámbialo).
4. Sube el mundo y prueba con 2 clientes para validar sync.

## Notas
- El reset lo ejecuta siempre el owner actual del objeto.
- Al interactuar/pickear, el script intenta tomar ownership para mantener coherencia del timer.
- Si prefieres reset manual por botón, puedes llamar `DoResetNow()` desde otro UdonBehaviour.

# Overhead Role Badges en VRChat (UdonSharp)

Este repo también incluye una solución local para mostrar badges PNG sobre la cabeza de jugadores concretos usando `SpriteRenderer`.

## Archivos
- `Assets/Scripts/RoleDefinition.cs`: define el nombre lógico del rol y su sprite.
- `Assets/Scripts/RoleManager.cs`: compara `VRCPlayerApi.displayName`, instancia badges y limpia al salir jugadores.
- `Assets/Scripts/RoleBillboard.cs`: mantiene cada badge sobre `TrackingDataType.Head` y orientado hacia la vista local.

## Decisiones técnicas
- Se usa `OnPlayerJoined`/`OnPlayerLeft` para evitar búsquedas continuas de jugadores.
- En `Start` se llama una sola vez a `VRCPlayerApi.GetPlayers(...)` para cubrir jugadores ya presentes al cargar el behaviour.
- Los badges son locales y no sincronizados; cada cliente instancia su propia copia visual, evitando tráfico de red.
- VRChat no expone acceso directo a la cámara del jugador en Udon. Por eso el billboard usa la posición de la cabeza del jugador local obtenida con `GetTrackingData(Head)`, que es la alternativa documentada para orientación/vista en Udon.
- El seguimiento se hace en `PostLateUpdate`, evento recomendado para posiciones de tracking actualizadas después de IK.

## Configuración recomendada en Unity
1. Crea un prefab de badge con:
   - `SpriteRenderer`.
   - `RoleBillboard`.
   - Sin componentes de networking.
2. Crea un GameObject por cada rol y agrega `RoleDefinition`:
   - `roleName`: por ejemplo `Owner`, `VIP`, `Pool Master`, `DJ`, `Staff`.
   - `roleSprite`: sprite PNG transparente importado como Sprite.
3. Crea un GameObject manager en la escena y agrega `RoleManager`.
4. En `RoleManager` asigna:
   - `Badge Prefab`.
   - `Player Names`: display names exactos, por ejemplo `Turco`, `Yuki`, `Matias`.
   - `Player Roles`: referencias a los `RoleDefinition` correspondientes en el mismo orden.
   - `Badge Height`, `Badge Scale` y `Max Badge Slots`.
5. Para agregar jugadores o cambiar roles, modifica únicamente el Inspector.

## Notas de compatibilidad
- Pensado para Unity 2022.3.x, VRChat Worlds SDK 3.10.x y UdonSharp.
- No usa Reflection, `FindObjectOfType`, APIs obsoletas ni paquetes externos.
- Compatible con PC y Quest si los sprites/materiales usados también lo son.
