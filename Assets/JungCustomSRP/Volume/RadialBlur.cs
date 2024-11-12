using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


[VolumeComponentMenu("JungCustomVolume/"+nameof(RadialBlur))]
public class RadialBlur : VolumeComponent
{
    public ClampedFloatParameter m_intensity= new(value:0f,min:0,max:1);
    public ClampedFloatParameter m_sampleingCount = new(value: 6f, min: 1, max: 64);
    public Vector2Parameter m_direction = new(value: new Vector2(0.5f,0.5f));



}
