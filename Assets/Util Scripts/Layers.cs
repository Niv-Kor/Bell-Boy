using UnityEngine;

namespace Constants
{
    public static class Layers
    {
        //names
        public static readonly string NAME_DEFAULT = "Default";
        public static readonly string NAME_TRANSPARENT_FX = "TransparentFX";
        public static readonly string NAME_IGNORE_RAYCAST = "Ignore Raycast";
        public static readonly string NAME_WATER = "Water";
        public static readonly string NAME_UI = "UI";
        public static readonly string NAME_PERSON = "Person";
        public static readonly string NAME_GROUND = "Ground";
        public static readonly string NAME_ELEVATOR = "Elevator";

        //masks
        public static readonly LayerMask DEFAULT = LayerMask.GetMask(NAME_DEFAULT);
        public static readonly LayerMask TRANSPARENT_FX = LayerMask.GetMask(NAME_TRANSPARENT_FX);
        public static readonly LayerMask IGNORE_RAYCAST = LayerMask.GetMask(NAME_IGNORE_RAYCAST);
        public static readonly LayerMask WATER = LayerMask.GetMask(NAME_WATER);
        public static readonly LayerMask UI = LayerMask.GetMask(NAME_UI);
        public static readonly LayerMask PERSON = LayerMask.GetMask(NAME_PERSON);
        public static readonly LayerMask GROUND = LayerMask.GetMask(NAME_GROUND);
        public static readonly LayerMask ELEVATOR = LayerMask.GetMask(NAME_ELEVATOR);

        /// <summary>
        /// Check if a certain layer is contained in a layer mask.
        /// </summary>
        /// <param name="layer">The layer to check (as is 'gameObject.layer')</param>
        /// <param name="mask">A mask that may contain the layer</param>
        /// <returns>True if the mask contains the layer.</returns>
        public static bool ContainedInMask(int layer, LayerMask mask) {
            return (mask & 1 << layer) == 1 << layer;
        }
    }
}