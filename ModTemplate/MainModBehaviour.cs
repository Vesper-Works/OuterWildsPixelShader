using OWML.ModHelper;
using OWML.Common;
using System;
using UnityEngine;
using OWML.ModHelper.Events;

namespace OuterWildsPixelShader
{
    public class MainModBehaviour : ModBehaviour
    {
        RenderTexture cameraTexture;
        RenderTexture pixelTexture;
        ComputeShader computeShader;
        Camera PixelCamera;
        public float ScaleFactor;
        public bool NoBlend;
        Camera NormalCamera { get => Camera.main; }
        public static MainModBehaviour Instance { get; set; }
        private void Start()
        {
            Instance = this;
      
            ModHelper.Console.WriteLine("Skipping splash screen...");
            var titleScreenAnimation = FindObjectOfType<TitleScreenAnimation>();
            titleScreenAnimation.SetValue("_fadeDuration", 0);
            titleScreenAnimation.SetValue("_gamepadSplash", false);
            titleScreenAnimation.SetValue("_introPan", false);
            titleScreenAnimation.Invoke("FadeInTitleLogo");
            ModHelper.Console.WriteLine("Done!");

            ModHelper.Events.Scenes.OnCompleteSceneChange += OnCompleteSceneChange;

            Textures();
            ComputeShader();
            CameraWork();
        }

        private void OnCompleteSceneChange(OWScene oldScene, OWScene newScene)
        {
            CameraWork();
        }

        public override void Configure(IModConfig config)
        {
            NoBlend = !config.GetSettingsValue<bool>("Blend");
            ScaleFactor = config.GetSettingsValue<float>("Pixel scale");         
        }

        private void Textures()
        {
            ModHelper.Console.WriteLine("Creating textures...");
            pixelTexture = new RenderTexture(1920, 1080, 0);
            pixelTexture.enableRandomWrite = true;
            pixelTexture.filterMode = FilterMode.Point;
            pixelTexture.Create();

            cameraTexture = new RenderTexture(1920, 1080, 0);
            cameraTexture.enableRandomWrite = true;
            cameraTexture.filterMode = FilterMode.Point;
            cameraTexture.Create();
            ModHelper.Console.WriteLine("Done!");
        }

        private void ComputeShader()
        {
            ModHelper.Console.WriteLine("Loading compute shader...");

            var shaderbundle = ModHelper.Assets.LoadBundle("pixelshader");

            computeShader = shaderbundle.LoadAsset<ComputeShader>("PixelShader");
            ModHelper.Console.WriteLine("Done!");
        }

        private void CameraWork()
        {
            if (NormalCamera == null) { return; }

            GameObject textureCameraGO = new GameObject();
            PixelCamera = textureCameraGO.AddComponent<Camera>();
            textureCameraGO.AddComponent<OWCamera>();

            textureCameraGO.transform.SetParent(NormalCamera.transform);
            textureCameraGO.transform.position = Vector3.zero;
            textureCameraGO.transform.rotation = Quaternion.identity;
            PixelCamera.CopyFrom(NormalCamera);
            PixelCamera.cullingMask = 1 << 5;
            NormalCamera.targetTexture = cameraTexture;
            ModHelper.Console.WriteLine("Done!");

            AddACIIRenderingToCamera(textureCameraGO);

        }
        private void AddACIIRenderingToCamera(GameObject camera)
        {
            camera.AddComponent<ShaderApplier>().Ready(cameraTexture, pixelTexture, computeShader);
        }
    }
}

