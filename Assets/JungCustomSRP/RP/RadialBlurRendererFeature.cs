using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RadialBlurRendererFeature : ScriptableRendererFeature
{
    public Shader m_shader;        
    private Material m_materials;
    private RadialBlur m_volume;

    private RadialBlur m_radialBlurComponent;
    class CustomRenderPass : ScriptableRenderPass
    {
        ProfilingSampler m_ProfilingSampler = new ProfilingSampler("ColorBlit_Radial");
        private Material m_material;                
        private RadialBlur m_volume;
        private int SAMPLEING_COUNT = Shader.PropertyToID("_SampleingCount");        
        private int BLUR_POWER = Shader.PropertyToID("_BlurPower");
        private int DIRECTION = Shader.PropertyToID("_Direction");

        RenderTargetIdentifier source;
        private RenderTargetHandle destination;
        public CustomRenderPass(Material _targetMat,RadialBlur _volume)
        {
            this.m_material = _targetMat;                        
            m_volume = _volume;
        }
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            source = renderingData.cameraData.renderer.cameraColorTarget;
            cmd.GetTemporaryRT(destination.id, renderingData.cameraData.cameraTargetDescriptor);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {            
            var camera = renderingData.cameraData.camera;

            if (!m_volume.active) return;
            
            if (camera.targetTexture != null)
                return;
            if (camera.cameraType != CameraType.Game)
                return;

            if (m_material == null)
                return;

            CommandBuffer cmd = CommandBufferPool.Get();
            
            using (new UnityEngine.Rendering.ProfilingScope(cmd, m_ProfilingSampler))
            {
                cmd.Blit(source, destination.Identifier());
                m_material.SetFloat(SAMPLEING_COUNT, m_volume.m_sampleingCount.value);
                m_material.SetFloat(BLUR_POWER, m_volume.m_intensity.value);
                m_material.SetVector(DIRECTION, new Vector4(m_volume.m_direction.value.x,m_volume.m_direction.value.y,0f,0f));
                
                cmd.Blit(destination.Identifier(), source, m_material);                
            }
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();             
            CommandBufferPool.Release(cmd);
        }

        
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(destination.id);
        }        
    }

    private CustomRenderPass m_ScriptablePass;

    
    public override void Create()
    {
        if (m_shader == null)
        {
            m_shader = Shader.Find("ColorBlit_RadialBlur");            
            return;
        }
        m_materials = CoreUtils.CreateEngineMaterial(m_shader);
        m_volume = VolumeManager.instance.stack.GetComponent<RadialBlur>();

        if (!m_volume)
            return;

        m_ScriptablePass = new CustomRenderPass(m_materials, m_volume);
        m_ScriptablePass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {                
        renderer.EnqueuePass(m_ScriptablePass);
        
    }
    
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }

}


