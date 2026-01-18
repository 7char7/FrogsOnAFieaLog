using UnityEngine;
using UnityEditor;

public static class ConfigureBloodParticle
{
    [MenuItem("Tools/Configure Blood Particle Effect")]
    public static void Configure()
    {
        string prefabPath = "Assets/Prefabs/BloodParticle.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        
        if (prefab == null)
        {
            Debug.LogError("BloodParticle prefab not found!");
            return;
        }
        
        GameObject instance = PrefabUtility.LoadPrefabContents(prefabPath);
        ParticleSystem ps = instance.GetComponent<ParticleSystem>();
        
        if (ps == null)
        {
            Debug.LogError("ParticleSystem component not found!");
            PrefabUtility.UnloadPrefabContents(instance);
            return;
        }
        
        var main = ps.main;
        main.loop = false;
        main.duration = 1f;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.3f, 0.6f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(2f, 4f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.1f);
        main.startColor = new Color(0.6f, 0.05f, 0.05f, 1f);
        main.gravityModifier = 0.5f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.playOnAwake = false;
        main.maxParticles = 50;
        
        var emission = ps.emission;
        emission.enabled = true;
        emission.rateOverTime = 0f;
        emission.SetBursts(new ParticleSystem.Burst[]
        {
            new ParticleSystem.Burst(0f, 10, 20, 1, 0.01f)
        });
        
        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 30f;
        shape.radius = 0.1f;
        shape.radiusThickness = 1f;
        shape.randomDirectionAmount = 0.2f;
        
        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] 
            { 
                new GradientColorKey(new Color(0.6f, 0.05f, 0.05f), 0f),
                new GradientColorKey(new Color(0.3f, 0.02f, 0.02f), 1f)
            },
            new GradientAlphaKey[] 
            { 
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);
        
        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        AnimationCurve sizeCurve = new AnimationCurve();
        sizeCurve.AddKey(0f, 1f);
        sizeCurve.AddKey(1f, 0.3f);
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);
        
        PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
        PrefabUtility.UnloadPrefabContents(instance);
        
        AssetDatabase.Refresh();
        Debug.Log("Blood particle effect configured successfully!");
    }
}

