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

### ✅ Commit 4
- Implementación completa de la **mano del jugador **:
  - Las cartas se distribuyen centradas en la parte inferior.
  - Al soltar una carta fuera de un **CardSlot**, esta vuelve a su posición original con animación suave.
  - La mano se **reordena automáticamente** al soltar una carta en un **CardSlot**.
- Creación del **mazo (Deck)**:
  - Contador visible de cartas restantes.
  - Al hacer clic sobre el mazo, se extrae una carta.
  - La nueva carta se añade a la mano con **animación de arrastre**.

### ✅ Commit 5
- Implementación completa de la **mano del oponente (EnemyHand)** basado en **(PlayerHand)**:
  - Las cartas se distribuyen centradas en la parte superior.
- Creación del **mazo oponente (OpponentDeck)** basado en el mazo del jugador **(PlayerHand)**.
- Implementación del **campo de batalla** con 5 CardSlot para cada jugador.
- Implementación de **lógica de batalla (BattleManager)** (incompleto).
- Identificación de bugs:
  - Interacción del jugador con cartas del enemigo.
  - Interacción del jugador con slots del enemigo.

### ✅ Commit 6
- Avance en la **lógica de batalla (BattleManager)**:
	- Avance en la **IA**, robar y jugar cartas.
- Solución de **bugs**:
	- El jugador ya no puede interaccionar con las cartas eneminas.
	- El jugador ya no puede interaccionar con los CardSlot enemigos.
	
### ✅ Commit 7
- Avance en la **lógica de batalla (BattleManager)**:
	- Avance en la **IA**, ataque a cartas del jugador o a la propia vida del jugador.
- Avance en la **lógica de ataque** por parte del jugador:
	- Funcionalidad de click sobre carta para atacar (incompleto, ataca a jugador pero no a cartas).
- Interfaz:
	- Añadido display de **vida del jugador**.
	- Añadido **pila de descartes** tanto para jugador como IA.
- Solución de bugs:
	- Ya no se pueden poner cartas en los cardslots enemigos.
	- Ya se puede poner una carta en un cardslot en el que ha muerto otra carta.
	- La IA ya no puede atacar a las cartas de la pila de descartes.
	
### ✅ Commit 8 - "Final de proyecto"
- Completada **lógica de batalla (BattleManager)**:
	- IA completa, ataca al jugador/cartas, roba carta al inicio de turno y juega las cartas en mano.
	- El jugador ya puede atacar tanto al oponente (IA) como a las cartas que este tenga en juego con solo un click
	encima de la carta que desee usar para atacar. Esta carta atacará de forma aleatoria a una de las cartas enemigas.
- Interfaz:
	- Añadido **mensaje de Victoria/Derrota**. Dicho mensaje permite **volver a jugar o salir del juego** y aparece
	en el momento en el que la vida del jugador o oponente llega a 0.
Solución de bugs:
	- El jugador no podía añadir una carta en un CardSlot en el que había muerto una carta suya.
	- Los botones de Reset y Salir no ejecutaban su funcionalidad al clickar en ellos.
Identificación de bugs:
	- El jugador puede poner una carta en un CardSlot enemigo si en este CardSlot ha muerto una carta enemiga.
	- El mensaje de Victoria/Derrota se mantiene en la parte superior izquierda de la pantalla a pesar de tener valores de posicion
	establecidos.
---

## 🔧 Tecnologías utilizadas
- **Godot 4** con soporte para **.NET (C#)**
- Desarrollo 2D
- Sistema de eventos y físicas de Godot

---
