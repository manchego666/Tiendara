<p align="center">
  <img src="https://github.com/manchego666/Tiendara/blob/master/Docs/Logo.png" alt="Tiendara Logo" width="400"/>
</p>

# ✅ TODO – Proyecto Tiendara

Este documento describe las tareas actuales y futuras para el desarrollo de **Tiendara**, así como las áreas de trabajo principales.

---

## 🟢 Funciones básicas (versión gratuita)

- [x] Crear clase `Producto` con ID, nombre, precio y stock.
- [x] Crear clase `Empleado` con nombre, sueldo y rol.
- [x] Crear clase `Venta` con productos vendidos, total y fecha.
- [x] Crear clase `Proveedor` con nombre y productos suministrados.
- [ ] Pantalla para registrar nuevos productos.
- [ ] Pantalla para registrar ventas.
- [ ] Módulo de corte de caja y cálculo automático.
- [ ] Gestión básica de empleados (alta, baja, pagos).
- [ ] Registro y edición de proveedores.
- [ ] Alerta de stock bajo.
- [ ] Sistema de sesión básica (sin validación aún).

---

## 💎 Funciones VIP (acceso mediante suscripción)

- [ ] Activador de modo VIP (token, pago manual o validación).
- [ ] Escaneo de código de barras con cámara.
- [ ] Chat entre cliente y tienda (interno, con antispam).
- [ ] Mapa de tiendas registradas (abiertas o cerradas).
- [ ] Reportes avanzados (ventas por día, por producto, por usuario).
- [ ] Validación de empleados con huella (si el celular lo soporta).
- [ ] Panel de control extendido con gráficas y filtros.
- [ ] Gestión multiusuario (sincronización de usuarios VIP).

---

## 🖼️ Interfaz gráfica (UI)

- [x] Diseño de `MainPage.xaml` con botones principales.
- [ ] Diseño de páginas secundarias (Ventas, Inventario, Configuración).
- [ ] Animaciones y efectos visuales simples.
- [ ] Tema claro con soporte para modo oscuro (a futuro).
- [ ] Compatibilidad con diferentes resoluciones móviles.

---

## 🗃️ Base de datos

- [ ] Implementar SQLite o SQL Server local.
- [ ] Crear tablas para cada entidad (`Producto`, `Venta`, etc.)
- [ ] Guardado y carga de datos al iniciar la app.
- [ ] Relación entre ventas y productos vendidos.
- [ ] Registro de historial (empleados, ventas, cortes).

---

## 🧪 Pruebas y validaciones

- [ ] Probar en múltiples resoluciones Android.
- [ ] Pruebas manuales de cada módulo al completarlo.
- [ ] Revisión de rendimiento.
- [ ] Validaciones en campos (no dejar texto vacío, rangos válidos, etc.)

---

## 🧱 Arquitectura y organización

- [x] Separar clases en `CapaDatos`, `CapaLogica` y `CapaVisual`.
- [ ] Crear controladores para lógica (Ej. `ControladorVentas`)
- [ ] Organizar navegación entre páginas desde `AppShell.xaml`.
- [ ] Documentar estructura y crear `infraestructura.md`.

---

## 📄 Documentación y GitHub

- [x] `README.md`
- [x] `VISION.md`
- [x] `ROADMAP.md`
- [x] `infraestructura.md`
- [x] Logo en carpeta `Docs/`
- [ ] Crear imágenes UML en `Diagramas/`
- [ ] Añadir documentación de clases si cambia su estructura.

---

<p align="center"><b>© 2025 ZorroDev – Todos los derechos reservados.</b></p>
