using System.Collections.Generic;
using UnityEngine;

public class FloorDecorator : Singleton<FloorDecorator>
{
    [System.Serializable]
    private struct DecorationData
    {
        [Tooltip("The prefab of the decoration.")]
        [SerializeField] public GameObject Prefab;

        [Tooltip("The scale of the decoration (1 is original scale).")]
        [SerializeField] public float Scale;
    }

    [Header("Prefabs")]

    [Tooltip("A list of available decorations for the floors.")]
    [SerializeField] private List<DecorationData> decorations;

    [Tooltip("All different colors the floor carpets can randomly receive.")]
    [SerializeField] private List<Material> carpetColors;

    [Header("Chance")]

    [Tooltip("The total rate of decorations.")]
    [SerializeField] [Range(0f, 1f)] private float placementRate = .5f;

    [Tooltip("True for the placement rate to be completely random each time.")]
    [SerializeField] private bool randomRate = false;

    [Header("Debug")]

    [Tooltip("True to decorte the floors or false to leave them as they are.")]
    [SerializeField] private bool decorateFloors;

    [Tooltip("True to paint the floors' carpets or false to leave them " + 
             "with their default color.")]
    [SerializeField] private bool paintCarpets;

    private static readonly string DECORATIONS_PARENT_NAME = "Decorations";

    /// <summary>
    /// Decorate all of the bulding's floors.
    /// </summary>
    /// <param name="floors">An array of the building's floors</param>
    public void Decorate(Floor[] floors) {
        if (decorateFloors) PlaceDecorations(floors);
        if (paintCarpets && carpetColors.Count > 0) PaintCarpets(floors);
    }

    /// <summary>
    /// Paint the floors' carpets.
    /// </summary>
    /// <param name="floors">An array of the building's floors</param>
    private void PaintCarpets(Floor[] floors) {
        Material selectedMaterial, lastSelectedMaterial = null;
        int colorsAmount = carpetColors.Count;

        foreach (Floor floor in floors) {
            //skip lobby or roof
            if (floor.FloorNumber == 0 || floor.FloorNumber == floors.Length - 1) continue;
            MeshRenderer[] carpetRenderers = floor.Carpet.GetComponentsInChildren<MeshRenderer>();

            //select a distinct color
            do selectedMaterial = CollectionsUtil.SelectRandom(carpetColors);
            while (colorsAmount > 1 && selectedMaterial == lastSelectedMaterial);
            lastSelectedMaterial = selectedMaterial;

            //paint
            foreach (MeshRenderer renderer in carpetRenderers)
                renderer.material = selectedMaterial;
        }
    }

    /// <summary>
    /// Place decoration objects on the floor.
    /// </summary>
    /// <param name="floors">An array of the building's floors</param>
    private void PlaceDecorations(Floor[] floors) {
        foreach (Floor floor in floors) {
            GameObject decorationsParent = new GameObject(DECORATIONS_PARENT_NAME);
            decorationsParent.transform.SetParent(floor.transform);
            FloorPlanBlueprint floorPlan = floor.GetComponent<FloorPlanBlueprint>();

            foreach (Vector3 spot in floorPlan.DecorationSpots) {
                bool passedChance = false;

                if (randomRate) passedChance = RandomUtils.UnstableCondition();
                else passedChance = RandomUtils.UnstableCondition(placementRate);

                //instantiate a random decoration
                if (passedChance) {
                    DecorationData randomDecoration = CollectionsUtil.SelectRandom(decorations);
                    GameObject decoration = PlaceDecoration(randomDecoration, floor, spot);
                    decoration.transform.SetParent(decorationsParent.transform);
                }
            }
        }
    }

    /// <summary>
    /// Place an instance of a decoration at a specified point on the floor's indoor carpet.
    /// </summary>
    /// <param name="decorationData">The prefab of which to create an instance</param>
    /// <param name="floor">The floor on which to place the decoration</param>
    /// <param name="spot">A spot on the floor at which to locate the decoration</param>
    /// <returns>The created instance of the prefab.</returns>
    private GameObject PlaceDecoration(DecorationData decorationData, Floor floor, Vector3 spot) {
        GameObject prefab = decorationData.Prefab;
        GameObject instance = Instantiate(prefab);

        //resize and find correct location
        MeshRenderer meshRenderer = instance.GetComponent<MeshRenderer>();
        instance.transform.localScale *= decorationData.Scale;
        spot += floor.transform.position;
        spot.y = floor.IndoorHeight + meshRenderer.bounds.extents.y;
        instance.transform.position = spot;

        return instance;
    }
}