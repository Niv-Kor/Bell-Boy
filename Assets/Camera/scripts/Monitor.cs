using UnityEngine;

public class Monitor : Singleton<Monitor>
{
    [Tooltip("The main camera of the game.")]
    [SerializeField] public Camera MainCamera;
}
