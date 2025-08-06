<p align="center">
  <img src="https://github.com/manchego666/Tiendara/blob/master/Docs/Logo.png" alt="Tiendara Logo" width="400"/>
</p>

# Infraestructura del proyecto – Tiendara

Este documento describe la arquitectura lógica y estructural del proyecto **Tiendara**, una aplicación de punto de venta moderna creada con .NET MAUI.

---

## 🧩 Arquitectura general

El proyecto sigue una **estructura por capas** para separar la lógica de negocio, los datos y la interfaz gráfica. Aunque no se aplica un MVVM estricto, se mantiene una clara organización:

📁 Tiendara/
├── 📁 CapaVisual/ → Formularios, páginas XAML, interfaces del usuario
├── 📁 CapaLogica/ → Controladores, procesos, validaciones
├── 📁 CapaDatos/ → Clases entidad (Producto, Venta, Empleado...)
├── 📁 Docs/ → Documentación y materiales gráficos
├── 📁 Diagramas/ → Diagramas UML, casos de uso, clases
└── Archivos base MAUI (App.xaml, MauiProgram.cs, etc.)

---

## 🧠 Lógica principal del sistema

Las funciones clave se dividen en módulos:

- **Inventario:** Registro, modificación y eliminación de productos.
- **Ventas:** Registro de ventas, historial, y corte de caja.
- **Empleados:** Registro, pagos y roles.
- **Proveedores:** Recepción de productos y seguimiento.
- **Usuarios:** Autenticación e identificación de tipo de usuario.
- **VIP:** Funciones especiales como escáner, chat y mapa.

---

## 🧾 Clases base (CapaDatos)

```csharp
//csharp
// Ejemplo de clases principales en CapaDatos
class Producto { int ID; string Nombre; decimal Precio; int Stock; }
class Venta { int ID; DateTime Fecha; List<Producto> ProductosVendidos; decimal Total; }
class Empleado { int ID; string Nombre; string Rol; decimal Sueldo; }
class Proveedor { int ID; string Nombre; string Telefono; string ProductoSuministrado; }
class Usuario { int ID; string Correo; string Tipo; string ClaveAcceso; }
class CorteCaja { int ID; DateTime Fecha; decimal TotalVentas; decimal SaldoInicial; }
```
---
💾 Persistencia de datos
Por ahora:
Base de datos SQL Server local o SQLite.

Datos persistentes en el dispositivo.

Futuro:
Sincronización remota o multiusuario.

Posibilidad de usar Azure, Firebase o servidores propios.
---
📲 Plataforma objetivo
Android como principal (soporte desde celulares gama media).

PC compatible en modo depuración o escritorio extendido.

Futuro: versión web para paneles administrativos (opcional).
---
🔐 Seguridad y acceso
Control por tipo de usuario (Dueño, Empleado, Cliente)

Acceso VIP desbloquea ciertas pantallas.

Control antispam en el sistema de chat.

En versiones futuras: validación por huella, tokens VIP, y cifrado de base de datos.
---
🧠 Inteligencia y soporte
Sistema modular para añadir IA en estadísticas o chat.

Control de tiendas visibles en mapa (estado abierto/cerrado).

Interfaz intuitiva y bonita para aumentar la usabilidad.

© 2025 ZorroDev – Todos los derechos reservados.
