# Cardlords

**Cardlords** es un juego de cartas por turnos desarrollado con **Godot 4** y **C#**, que combina mecánicas de estrategia y juego de cartas coleccionables en un entorno visual 2D.

---

## 🧾 Historial de desarrollo

### ✅ Commit 1
- Creación del **escenario principal en 2D**.
- Implementación del **nodo de carta**, incluyendo:
  - Imagen representativa.
  - Colisión para interacción.
- Desarrollo de script inicial en **GDScript** para implementar el sistema de **arrastrar cartas (drag & drop)**.

### ✅ Commit 2
- Migración del código de **GDScript a C#**.
- Implementación de **efectos de hover** al pasar el ratón sobre una carta.
- Identificación de bugs:
  - Selección múltiple no deseada.
  - Múltiples cartas arrastradas simultáneamente.
  - Desplazamiento incorrecto de las cartas al hacer drag.
- Estos errores serán corregidos en versiones futuras.

### ✅ Commit 3  
- Corrección de los bugs detectados anteriormente:  
  - Se evita que varias cartas puedan ser seleccionadas o arrastradas simultáneamente.  
  - Las cartas ya no se salen de los bordes de la pantalla al hacer drag.  
- Creación de los **CardSlots** (espacios para soltar cartas):  
  - Cada slot puede recibir una sola carta.  
  - Al soltar una carta sobre un slot vacío, esta se alinea al centro del mismo.  
  - Se desactiva la colisión de la carta para evitar que vuelva a ser arrastrada.  
- Integración completa entre cartas y slots.  
- Añadidos comentarios simples en el código para facilitar la comprensión y mantenimiento.  

---

## 🔧 Tecnologías utilizadas
- **Godot 4** con soporte para **.NET (C#)**
- Desarrollo 2D
- Sistema de eventos y físicas de Godot

---
