# TODO - Overhead Role Badges VRChat

## Plan
- [x] Revisar instrucciones del repo y estado inicial.
- [x] Verificar APIs oficiales necesarias para VRChat Worlds SDK 3.10.x/UdonSharp.
- [x] Diseñar arquitectura simple con `RoleDefinition`, `RoleManager` y `RoleBillboard`.
- [x] Implementar scripts compatibles con Unity 2022.3/UdonSharp sin Reflection ni paquetes externos.
- [x] Revisar diff y ejecutar verificaciones programáticas disponibles.
- [x] Commit + PR.

## Review
- Se agregó `RoleDefinition` para configurar roles y sprites PNG desde el Inspector.
- Se agregó `RoleManager` para asignar roles por `displayName`, crear badges locales al entrar jugadores y destruirlos al salir.
- Se agregó `RoleBillboard` para seguir `TrackingDataType.Head` en `PostLateUpdate` y orientar el sprite hacia la cabeza local.
- Se documentó la configuración recomendada del prefab, roles y manager en Unity.
- Verificaciones ejecutadas: revisión estática de bloques de control de excepciones y patrones/APIs prohibidas en scripts nuevos.
