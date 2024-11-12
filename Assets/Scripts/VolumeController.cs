using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class VolumeController : MonoBehaviour
{
    public static VolumeController instance = null;

    [SerializeField] private VolumeProfile volume;
    [SerializeField] private bool bfullscreenGray = false;
    [SerializeField] private Material matFullScreenGray;
    private RadialBlur radialBlur;
    public float blurIntensity { get => radialBlur.m_intensity.value; set => radialBlur.m_intensity.value = value; }
    public float blurSampleingCount { get => radialBlur.m_sampleingCount.value; set => radialBlur.m_sampleingCount.value = value; }
    public Vector2 blurDirection { get => radialBlur.m_direction.value; set => radialBlur.m_direction.value = value; }
    private void Awake()
    {
        if (instance == null)
            instance = this;

        volume.TryGet(out radialBlur);

    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SetActiveFullScreenGray(true);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            SetActiveFullScreenGray(false);
    }

    public void SetActiveFullScreenGray(bool _active)
    {
        if(_active)
        {
            matFullScreenGray.SetInt("_Active",1);
        }
        else
        {
            matFullScreenGray.SetInt("_Active",0);
        }       
     }
}
