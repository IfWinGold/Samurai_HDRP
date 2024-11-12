using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FullScreenPass : ScriptableRendererFeature
{
    public Shader m_shader;
    public Material m_material;
    public FullScreen m_fullScreen;

    class FullScreenRenderPass : ScriptableRenderPass
    {
        ProfilingSampler profilingSampler = new ProfilingSampler("FullScreenGray");

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            throw new System.NotImplementedException();
        }
    }

    
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        throw new System.NotImplementedException();
    }

    FullScreenRenderPass m_scriptablePass = null;
    public override void Create()
    {
        if (m_shader == null)
        {
            m_shader = Shader.Find("Shader Graphs/FullScrenGray");
            return;
        }
        m_material = CoreUtils.CreateEngineMaterial(m_shader);
        m_fullScreen = VolumeManager.instance.stack.GetComponent<FullScreen>();

        if (!m_fullScreen)
            return;

    }
}
