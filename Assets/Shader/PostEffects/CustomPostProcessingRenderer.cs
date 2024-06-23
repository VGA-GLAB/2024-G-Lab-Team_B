using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable]
[VolumeComponentMenu("Custom/CustomPostProcessingEffect")]
public class CustomPostProcessingEffect : VolumeComponent, IPostProcessComponent
{
    public BoolParameter isEnabled = new BoolParameter(false);

    public bool IsActive() => isEnabled.value;

    public bool IsTileCompatible() => false;
}

public class CustomPostProcessingRenderer : ScriptableRendererFeature
{
    class CustomPostProcessingPass : ScriptableRenderPass
    {
        private Material _material;
        private RenderTargetIdentifier _source;
        private RenderTargetHandle _tempTexture;

        public CustomPostProcessingPass(Material material)
        {
            this._material = material;
            _tempTexture.Init("_TempTexture");
        }

        public void Setup(RenderTargetIdentifier source)
        {
            this._source = source;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // 描画処理を記述
            CommandBuffer cmd = CommandBufferPool.Get("CustomPostProcessing");

            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            cmd.GetTemporaryRT(_tempTexture.id, opaqueDesc);

            Blit(cmd, _source, _tempTexture.Identifier(), _material, 0);
            Blit(cmd, _tempTexture.Identifier(), _source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(_tempTexture.id);
        }
    }

    [SerializeField] private Shader _shader;
    private Material _material;
    private CustomPostProcessingPass _postProcessingPass;

    public override void Create()
    {
        if (_shader == null)
        {
            Debug.LogError("Missing shader for CustomPostProcessingEffect");
            return;
        }
        _material = CoreUtils.CreateEngineMaterial(_shader);
        _postProcessingPass = new CustomPostProcessingPass(_material)
        {
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        var cameraColorTarget = renderer.cameraColorTarget;
        _postProcessingPass.Setup(cameraColorTarget);
        renderer.EnqueuePass(_postProcessingPass);
    }
    
    public void Dispose()
    {
        CoreUtils.Destroy(_material);
    }
}
