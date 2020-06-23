using UnityEngine;

namespace FLFlight {
    public static class SmoothDamp {

        public static Quaternion DampS (Quaternion a, Quaternion b, float lambda, float dt) {
            return Quaternion.Slerp (a, b, 1 - Mathf.Exp (-lambda * dt));
        }
    }
}