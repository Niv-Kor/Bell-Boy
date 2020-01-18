using UnityEngine;

public class StoreyBuilder : Singleton<StoreyBuilder>
{
    [Header("Prefabs")]

    [Tooltip("The prefab of the roof, above the last floor.")]
    [SerializeField] private GameObject roofPrefab;

    [Tooltip("The prefab of the floor that should be builded as a building block.")]
    [SerializeField] private GameObject floorPrefab;

    [Tooltip("The prefab of the lobby, below the first floor.")]
    [SerializeField] private GameObject lobbyPrefab;

    [Header("Building Instructions")]

    [Tooltip("Amount of storeys to build.")]
    [SerializeField] private int storeys = 3;

    [Tooltip("The position of the first floor block.")]
    [SerializeField] private Vector3 buildingPosition;

    [Tooltip("The rotation of the first floor block.")]
    [SerializeField] private Vector3 buildingRotation;

    private static readonly string FLOORS_PARENT_NAME = "Storeys";

    private GameObject storeysParent;
    private float[,] storeysHeights;

    public Floor[] Storeys { get; private set; }

    private void Awake() {
        this.storeysParent = new GameObject(FLOORS_PARENT_NAME);
        storeysParent.transform.SetParent(transform);
        this.storeysHeights = new float[storeys + 2, 2];
        Storeys = Build(out float buildingHeight);

        //set the maximum height of the camera based on the building's height
        VerticalCamSlider camSlider = Monitor.Instance.MainCamera.GetComponent<VerticalCamSlider>();
        float roofHeight = Storeys[Storeys.Length - 1].GetComponent<Floor>().Volume.y;
        camSlider.MaxHeight = buildingHeight + roofHeight * .35f;

        //decorate floors
        FloorDecorator.Instance.Decorate(Storeys);
    }

    /// <summary>
    /// Build the building's storeys on top of each other.
    /// </summary>
    /// <param name="height">The total height of the building</param>
    /// <returns>An array of the storeys.</returns>
    private Floor[] Build(out float height) {
        GameObject[] floorObjects = new GameObject[storeys + 2];
        Quaternion rotation = Quaternion.Euler(buildingRotation);
        height = 0;

        for (int i = 0; i < floorObjects.Length; i++) {
            GameObject prefab;
            string objName, floorName;
            storeysHeights[i, 0] = height;

            //build the lobby
            if (i == 0) {
                prefab = lobbyPrefab;
                objName = "Lobby";
                floorName = "L";
            }
            //build the roof
            else if (i == floorObjects.Length - 1) {
                prefab = roofPrefab;
                objName = "Roof";
                floorName = "R";
            }
            //build a floor block
            else {
                prefab = floorPrefab;
                string suffix;

                switch (i) {
                    case 1: suffix = "st"; break;
                    case 2: suffix = "nd"; break;
                    case 3: suffix = "rd"; break;
                    default: suffix = "th"; break;
                }

                objName = i + suffix + " Floor";
                floorName = "" + i;
            }

            //create an instance of the prefab
            floorObjects[i] = Instantiate(prefab, storeysParent.transform);
            floorObjects[i].name = objName;
            Floor floorComponent = floorObjects[i].GetComponent<Floor>();
            Vector3 floorScale = floorComponent.Volume;
            float yPos = buildingPosition.y + height;
            Vector3 position = new Vector3(buildingPosition.x, yPos, buildingPosition.z);
            floorComponent.FloorName = floorName;
            floorComponent.FloorNumber = i;
            height += floorScale.y;
            storeysHeights[i, 1] = height;

            //set floor position and rotation
            floorComponent.UpdateLocation(position);
            floorObjects[i].transform.rotation = rotation; 
        }

        //convert to array of Floor components
        Floor[] floors = new Floor[floorObjects.Length];
        for (int i = 0; i < floorObjects.Length; i++)
            floors[i] = floorObjects[i].GetComponent<Floor>();

        return floors;
    }

    /// <summary>
    /// Get a random floor number from the building.
    /// </summary>
    /// <param name="includeLobby">True to include the lobby as an option</param>
    /// <param name="includeRoof">True to include the roof as an option</param>
    /// <returns>A random floor number.</returns>
    public int GetRandomFloorNumber(bool includeLobby, bool includeRoof) {
        int startIndex = includeLobby ? 0 : 1;
        int endIndex = includeRoof ? Storeys.Length : Storeys.Length - 1;
        return Random.Range(startIndex, endIndex);
    }

    /// <summary>
    /// Get a random floor from the building.
    /// </summary>
    /// <param name="includeLobby">True to include the lobby as an option</param>
    /// <param name="includeRoof">True to include the roof as an option</param>
    /// <returns>A random floor.</returns>
    public Floor GetRandomFloor(bool includeLobby, bool includeRoof) {
        return Storeys[GetRandomFloorNumber(includeLobby, includeRoof)].GetComponent<Floor>();
    }

    /// <summary>
    /// Get the number of the floor that a given height value is within its range.
    /// </summary>
    /// <param name="height">The height to check</param>
    /// <returns>
    /// The corresponding floor's number.
    /// If the value is negative, return the lobby floor number;
    /// If the value is too high, return the roof floor number.
    /// </returns>
    public int GetFloorNumberByHeight(float height) {
        for (int i = 0; i < storeysHeights.Length; i++)
            if (height >= storeysHeights[i, 0] && height <= storeysHeights[i, 1]) return i;

        return -1;
    }

    /// <summary>
    /// Get the floor that a given height value is within its range.
    /// </summary>
    /// <param name="height">The height to check</param>
    /// <returns>
    /// The corresponding floor.
    /// If the value is negative, return the lobby floor;
    /// If the value is too high, return the roof floor.
    /// </returns>
    public Floor GetFloorByHeight(float height) {
        return Storeys[GetFloorNumberByHeight(height)].GetComponent<Floor>();
    }
}