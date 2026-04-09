# TODO - Auto reset de items VRChat

## Plan
- [x] Definir especificación del comportamiento (timeout 10 min, ownership, sincronización).
- [x] Implementar script UdonSharp reutilizable para items.
- [x] Documentar pasos de configuración en Unity/VRChat SDK.
- [x] Verificar estructura de archivos y revisar cambios finales.
- [x] Commit + PR.

## Review
- Se implementó `NetworkedItemAutoReset` con timeout configurable y reset ejecutado únicamente por owner.
- Se documentó setup mínimo para Unity 2022.3.22f1 + VRChat SDK Worlds 3.10.2.
- Se confirmó que el repo ahora contiene script + guía de uso.
