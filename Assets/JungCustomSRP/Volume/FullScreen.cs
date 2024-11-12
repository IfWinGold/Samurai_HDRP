using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


[VolumeComponentMenu("JungCustomVolume/" + nameof(FullScreen))]
public class FullScreen : VolumeComponent
{
    public BoolParameter m_grayActive = new(value: false);
}
