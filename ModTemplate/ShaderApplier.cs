using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OuterWildsPixelShader
{
    class ShaderApplier : MonoBehaviour
    {
        RenderTexture cameraTexture;
        RenderTexture pixelTexture;
        ComputeShader computeShader;
        bool running;
       
        public void Ready(RenderTexture cameraTexture, RenderTexture pixelTexture, ComputeShader computeShader)
        {
            this.cameraTexture = cameraTexture;
            this.pixelTexture = pixelTexture;
            this.computeShader = computeShader;
            running = true;
        }
        private void Update()
        {
            if (!running || Keyboard.current["h"].IsPressed()) { return; }
            computeShader.SetTexture(0, "cameraTexture", cameraTexture);
            computeShader.SetTexture(0, "pixelTexture", pixelTexture);
            computeShader.SetFloat("scaleFactor", MainModBehaviour.Instance.ScaleFactor);
            computeShader.SetBool("noBlend", MainModBehaviour.Instance.NoBlend);
            computeShader.Dispatch(0, 2000 / (int)Math.Min(MainModBehaviour.Instance.ScaleFactor * MainModBehaviour.Instance.ScaleFactor, 8 * 8), 
                1200 / (int)Math.Min(MainModBehaviour.Instance.ScaleFactor * MainModBehaviour.Instance.ScaleFactor, 8 * 8), 1);
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (!running)
            {
                return;
            }
            if (Keyboard.current["h"].IsPressed())
            {
                Graphics.Blit(cameraTexture, destination);
            }
            else
            {
                Graphics.Blit(pixelTexture, destination);
            }

        }
    }
}
