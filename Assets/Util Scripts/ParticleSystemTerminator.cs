using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleSystemTerminator : MonoBehaviour
{
    private ParticleSystem particles;

    private void Start() {
        this.particles = GetComponent<ParticleSystem>();
    }

    private void Update() {
        if (!particles.isPlaying) Destroy(this);
    }
}