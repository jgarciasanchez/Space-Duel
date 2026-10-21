using UnityEngine;

namespace FLFlight {
    /// <summary>
    /// Stats for each selectable ship. Used both by the selection screen and the match.
    /// </summary>
    [System.Serializable]
    public struct ShipStats {
        public string name;
        public int damage;
        public int life;
        public float speedMultiplier;

        public ShipStats (string name, int damage, int life, float speedMultiplier) {
            this.name = name;
            this.damage = damage;
            this.life = life;
            this.speedMultiplier = speedMultiplier;
        }
    }

    public static class ShipCatalog {
        public const int MaxDamage = 30;
        public const int MaxLife = 100;
        public const float MaxSpeedMultiplier = 1.25f;

        public static readonly ShipStats[] Ships = {
            new ShipStats ("nave1", 25, 100, 0.85f), // pesada: mas dano y vida
            new ShipStats ("nave2", 18, 70, 1.25f), // ligera: mas rapida
        };

        public static ShipStats Get (int index) {
            return Ships[Mathf.Clamp (index, 0, Ships.Length - 1)];
        }
    }

    /// <summary>
    /// Carries the ships chosen in the selection screen over to the Game scene.
    /// When the Game scene is opened directly, nothing is selected and the
    /// values configured on each ship in the Inspector are used.
    /// </summary>
    public static class MatchSettings {
        private static readonly int[] selectedShips = { -1, -1 };

        public static void SelectShip (int playerNumber, int shipIndex) {
            selectedShips[playerNumber - 1] = shipIndex;
        }

        public static bool TryGetShip (int playerNumber, out ShipStats stats) {
            int index = selectedShips[playerNumber - 1];
            stats = index >= 0 ? ShipCatalog.Get (index) : default (ShipStats);
            return index >= 0;
        }
    }
}
