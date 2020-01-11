using UnityEngine;

public class SkyboxRotator : MonoBehaviour
{
    [Tooltip("The speed of rotation.")]
    [SerializeField] private float speed;

    private static readonly string ROTATION_VARIABLE = "_Rotation";

    private void Update() {
        RenderSettings.skybox.SetFloat(ROTATION_VARIABLE, speed * Time.time);
    }
}