using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ShellParticleConfig : MonoBehaviour
{
    [SerializeField] private bool ejectLeft = false;
    
    private void Awake()
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        
        var main = ps.main;
        main.startLifetime = 0.8f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(2f, 4f);
        main.startSize = 0.05f;
        main.startColor = new Color(1f, 0.95f, 0f);
        main.gravityModifier = 1f;
        main.maxParticles = 50;
        main.loop = false;
        main.playOnAwake = false;
        
        var emission = ps.emission;
        emission.enabled = true;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0f, 3, 5, 1, 0.01f)
        });
        
        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 25f;
        shape.radius = 0.1f;
        shape.rotation = ejectLeft ? new Vector3(0, -90, 0) : new Vector3(0, 90, 0);
        
        var renderer = GetComponent<ParticleSystemRenderer>();
        if (renderer != null)
        {
            renderer.renderMode = ParticleSystemRenderMode.Billboard;
        }
    }
}
