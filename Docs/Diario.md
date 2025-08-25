<p align="center">
  <img src="https://github.com/manchego666/Tiendara/blob/master/Docs/Logo.png" alt="Tiendara Logo" width="400"/>
</p>

# Diario – 2025-08-21

## Hecho
- Separado MediaServer y App (MAUI) con rutas reales.
- Subida de avatar y logo -> guarda `AvatarPath` y `LogoPath` en SQL.
- Perfil: refresco inmediato de imagen (cache-buster).
- PerfilNegocio: visor de logo implementado.

## Cambios clave
- `PerfilPage`: usa `IFotoApi` y `BackendConfig`.
- `PerfilNegocioPage`: `VerLogoAsync`, handlers Editar*.
- `NegocioServiceSql`: columnas `LogoPath`.

## Pendiente
- Pantallas de edición (datos/temas).
- UX minimal para Inventario/Ventas.

## Enlaces
- Commit: <hash>
- Issue: #123
