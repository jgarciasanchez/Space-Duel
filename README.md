# Space Duel

*A university project — 3D local multiplayer space combat game built in Unity.*

[Español ↓](#es)

## English

**Space Duel** is a 3D local-multiplayer space combat game made as a university coursework project. Two players pick a ship, fly through an asteroid field, and fight until one of them runs out of health.

### Gameplay

- **2-player local versus** — Player 1 and Player 2 fly simultaneously on the same screen/setup, each with independent controls (keyboard, mouse, or gamepad axes).
- **Ship selection screen** — before the match, each player picks between two ships with different stats (damage, health, speed).
- **Newtonian-ish flight model** — ships are driven by `Rigidbody` forces/torques (pitch, yaw, roll, strafe, throttle) rather than simple transform movement.
- **Combat** — ships fire projectiles that damage the opposing player on hit.
- **Asteroid field** — the arena is populated by randomly scattered/spawned asteroids (box or sphere distribution, random scale and rotation).
- **Pickups** — destroying "Miner" objects grants the shooter bonus damage, health, and a temporary buff duration.
- **Pause menu & game-over screen** — both players can pause, and the match ends with a win message once a player's health hits zero.

### Tech

- **Engine:** Unity `2019.3.14f1`
- **Language:** C#
- **Scenes:** `Interface` (main menu) → `Seleccion` (ship selection) → `Game` (match)
- Large binary assets (models, some textures) are tracked with **Git LFS**.

### Project structure

```
Assets/
  Scripts/
    Ship/      Flight physics, player input, bullets
    Camera/    Camera rig(s)
    UI/        Menus, HUD, ship selection
    Asteroids/ Random asteroid field spawner
    Utils/     Misc helpers (buff spawner, smoothing)
  Scenes/      Interface, Seleccion, Game
  Modelos/     3D models (ships, asteroids, characters)
  Materials/   Materials & textures
  Sound/       SFX and music
  UI/          UI textures/sprites
```

### Running it

1. Install **Unity Hub** and Unity **2019.3.14f1** (or a nearby 2019.3.x version).
2. Clone the repo (make sure [Git LFS](https://git-lfs.com/) is installed first, so binary assets download correctly):
   ```bash
   git lfs install
   git clone <repo-url>
   ```
3. Open the project folder from Unity Hub.
4. Open the `Interface` scene and press Play.

### Status

This was built as a learning exercise to practice Unity physics-based flight, local multiplayer input handling, and basic gameplay systems (combat, pickups, UI flow). It's shared as-is for portfolio/reference purposes rather than actively maintained.

---

<a name="es"></a>
## Español

**Space Duel** es un juego de combate espacial 3D en modo local para dos jugadores, hecho como proyecto de una asignatura universitaria. Dos jugadores eligen una nave, vuelan por un campo de asteroides y combaten hasta que a uno se le agota la vida.

### Jugabilidad

- **Versus local a 2 jugadores** — el Jugador 1 y el Jugador 2 vuelan simultáneamente en la misma pantalla, cada uno con sus propios controles (teclado, ratón o ejes de mando).
- **Pantalla de selección de nave** — antes de la partida, cada jugador elige entre dos naves con estadísticas distintas (daño, vida, velocidad).
- **Modelo de vuelo inercial** — las naves se mueven mediante fuerzas y torques aplicados al `Rigidbody` (cabeceo, guiñada, alabeo, desplazamiento lateral y aceleración), no con movimiento directo del transform.
- **Combate** — las naves disparan proyectiles que dañan al jugador contrario al impactar.
- **Campo de asteroides** — la arena se puebla con asteroides generados aleatoriamente (distribución en caja o esfera, con escala y rotación aleatorias).
- **Power-ups** — destruir objetos "Miner" otorga a quien dispara bonificaciones de daño, vida y una duración temporal de mejora.
- **Menú de pausa y pantalla de fin de partida** — ambos jugadores pueden pausar, y la partida termina mostrando un mensaje de victoria cuando la vida de un jugador llega a cero.

### Tecnología

- **Motor:** Unity `2019.3.14f1`
- **Lenguaje:** C#
- **Escenas:** `Interface` (menú principal) → `Seleccion` (selección de nave) → `Game` (partida)
- Los assets binarios grandes (modelos, algunas texturas) se gestionan con **Git LFS**.

### Estructura del proyecto

```
Assets/
  Scripts/
    Ship/      Física de vuelo, input de jugador, balas
    Camera/    Rig(s) de cámara
    UI/        Menús, HUD, selección de nave
    Asteroids/ Generador aleatorio del campo de asteroides
    Utils/     Utilidades varias (spawner de buffs, suavizado)
  Scenes/      Interface, Seleccion, Game
  Modelos/     Modelos 3D (naves, asteroides, personajes)
  Materials/   Materiales y texturas
  Sound/       Efectos de sonido y música
  UI/          Texturas/sprites de interfaz
```

### Cómo ejecutarlo

1. Instala **Unity Hub** y Unity **2019.3.14f1** (o una versión cercana de 2019.3.x).
2. Clona el repositorio (asegúrate de tener [Git LFS](https://git-lfs.com/) instalado antes, para que los assets binarios se descarguen correctamente):
   ```bash
   git lfs install
   git clone <repo-url>
   ```
3. Abre la carpeta del proyecto desde Unity Hub.
4. Abre la escena `Interface` y pulsa Play.

### Estado

Este proyecto se hizo como ejercicio de aprendizaje para practicar vuelo basado en física en Unity, manejo de input local para varios jugadores y sistemas básicos de jugabilidad (combate, power-ups, flujo de UI). Se comparte tal cual, con fines de portafolio/referencia, sin mantenimiento activo.
