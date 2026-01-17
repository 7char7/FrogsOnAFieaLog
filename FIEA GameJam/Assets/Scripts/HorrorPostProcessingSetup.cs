using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HorrorPostProcessingSetup : MonoBehaviour
{
    [Header("Horror Post-Processing Settings")]
    [SerializeField] private Volume globalVolume;
    
    [Header("Vignette Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float vignetteIntensity = 0.45f;
    [Range(0f, 1f)]
    [SerializeField] private float vignetteSmoothness = 0.4f;
    
    [Header("Film Grain Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float filmGrainIntensity = 0.35f;
    
    [Header("Chromatic Aberration Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float chromaticAberrationIntensity = 0.25f;
    
    [Header("Color Adjustments")]
    [Range(-100f, 100f)]
    [SerializeField] private float postExposure = -0.5f;
    [Range(-100f, 100f)]
    [SerializeField] private float contrast = 15f;
    [Range(-100f, 100f)]
    [SerializeField] private float saturation = -30f;
    
    [Header("Shadows Midtones Highlights")]
    [SerializeField] private Color shadowsTint = new Color(0.6f, 0.7f, 0.8f, 1f);
    
    private void Start()
    {
        ApplyHorrorPostProcessing();
    }
    
    public void ApplyHorrorPostProcessing()
    {
        if (globalVolume == null)
        {
            Debug.LogError("Global Volume not assigned!");
            return;
        }
        
        VolumeProfile profile = globalVolume.profile;
        if (profile == null)
        {
            Debug.LogError("Volume Profile not found!");
            return;
        }
        
        SetupVignette(profile);
        SetupFilmGrain(profile);
        SetupChromaticAberration(profile);
        SetupColorAdjustments(profile);
        SetupShadowsMidtonesHighlights(profile);
        SetupTonemapping(profile);
        
        Debug.Log("Horror post-processing applied successfully!");
    }
    
    private void SetupVignette(VolumeProfile profile)
    {
        if (profile.TryGet<Vignette>(out var vignette))
        {
            vignette.active = true;
            vignette.intensity.value = vignetteIntensity;
            vignette.smoothness.value = vignetteSmoothness;
            vignette.color.value = Color.black;
        }
        else
        {
            vignette = profile.Add<Vignette>();
            vignette.active = true;
            vignette.intensity.Override(vignetteIntensity);
            vignette.smoothness.Override(vignetteSmoothness);
            vignette.color.Override(Color.black);
        }
    }
    
    private void SetupFilmGrain(VolumeProfile profile)
    {
        if (profile.TryGet<FilmGrain>(out var filmGrain))
        {
            filmGrain.active = true;
            filmGrain.intensity.value = filmGrainIntensity;
            filmGrain.type.value = FilmGrainLookup.Medium1;
            filmGrain.response.value = 0.8f;
        }
        else
        {
            filmGrain = profile.Add<FilmGrain>();
            filmGrain.active = true;
            filmGrain.intensity.Override(filmGrainIntensity);
            filmGrain.type.Override(FilmGrainLookup.Medium1);
            filmGrain.response.Override(0.8f);
        }
    }
    
    private void SetupChromaticAberration(VolumeProfile profile)
    {
        if (profile.TryGet<ChromaticAberration>(out var chromaticAberration))
        {
            chromaticAberration.active = true;
            chromaticAberration.intensity.value = chromaticAberrationIntensity;
        }
        else
        {
            chromaticAberration = profile.Add<ChromaticAberration>();
            chromaticAberration.active = true;
            chromaticAberration.intensity.Override(chromaticAberrationIntensity);
        }
    }
    
    private void SetupColorAdjustments(VolumeProfile profile)
    {
        if (profile.TryGet<ColorAdjustments>(out var colorAdjustments))
        {
            colorAdjustments.active = true;
            colorAdjustments.postExposure.value = postExposure;
            colorAdjustments.contrast.value = contrast;
            colorAdjustments.saturation.value = saturation;
            colorAdjustments.colorFilter.value = new Color(0.9f, 0.95f, 1f, 1f);
        }
        else
        {
            colorAdjustments = profile.Add<ColorAdjustments>();
            colorAdjustments.active = true;
            colorAdjustments.postExposure.Override(postExposure);
            colorAdjustments.contrast.Override(contrast);
            colorAdjustments.saturation.Override(saturation);
            colorAdjustments.colorFilter.Override(new Color(0.9f, 0.95f, 1f, 1f));
        }
    }
    
    private void SetupShadowsMidtonesHighlights(VolumeProfile profile)
    {
        if (profile.TryGet<ShadowsMidtonesHighlights>(out var smh))
        {
            smh.active = true;
            smh.shadows.value = new Vector4(shadowsTint.r, shadowsTint.g, shadowsTint.b, -0.1f);
            smh.midtones.value = new Vector4(0.9f, 0.9f, 1f, -0.05f);
            smh.highlights.value = new Vector4(1f, 1f, 1f, 0f);
        }
        else
        {
            smh = profile.Add<ShadowsMidtonesHighlights>();
            smh.active = true;
            smh.shadows.Override(new Vector4(shadowsTint.r, shadowsTint.g, shadowsTint.b, -0.1f));
            smh.midtones.Override(new Vector4(0.9f, 0.9f, 1f, -0.05f));
            smh.highlights.Override(new Vector4(1f, 1f, 1f, 0f));
        }
    }
    
    private void SetupTonemapping(VolumeProfile profile)
    {
        if (profile.TryGet<Tonemapping>(out var tonemapping))
        {
            tonemapping.active = true;
            tonemapping.mode.value = TonemappingMode.ACES;
        }
        else
        {
            tonemapping = profile.Add<Tonemapping>();
            tonemapping.active = true;
            tonemapping.mode.Override(TonemappingMode.ACES);
        }
    }
}
