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
