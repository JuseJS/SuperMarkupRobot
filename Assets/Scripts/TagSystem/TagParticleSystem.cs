using UnityEngine;

public class TagParticleSystem : MonoBehaviour
{
    [SerializeField] private ParticleSystem successParticles;
    private static TagParticleSystem instance;
    public static TagParticleSystem Instance => instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void PlaySuccessEffect(Vector3 position)
    {
        ParticleSystem particles = Instantiate(successParticles, position, Quaternion.identity);
        particles.Play();
        Destroy(particles.gameObject, particles.main.duration);
    }
}