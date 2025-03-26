using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaterShader
{
    public class WaterRippleController : MonoBehaviour
    {
        public Material WaterBufferMaterial;
        public Material WaterMaterial;
        public float delta = 1f;
        public float rippleSize = 0.01f;
        public float dampening = 0.01f;

        private RenderTexture BufferTexture;

        void Start()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;

            BufferTexture = new RenderTexture(1080, 1080, 0, RenderTextureFormat.ARGBFloat);
            BufferTexture.Create();

            WaterBufferMaterial.SetFloat("_dampening", dampening);
            WaterBufferMaterial.SetFloat("_rippleSize", rippleSize);
            WaterBufferMaterial.SetFloat("_delta", delta);

            WaterBufferMaterial.SetVector("_mousePos", new Vector4(10000f, 10000f, 0f, 0f));
        }


        void Update()
        {
            //float fps = 1f / Time.deltaTime;
            //float temp2 = 1.4f / (fps / 60f);
            //temp2 = Mathf.Clamp(temp2, 0.5f, 1.39f);
            //Debug.Log("temp: " + temp2);
            //WaterBufferMaterial.SetFloat("_delta", temp2);

            // Set Buffer Tex to the current state of feedbackBuffer
            WaterBufferMaterial.SetTexture("_BufferTex", BufferTexture);

            // Render with feedback buffer as both the source and target
            RenderTexture temp = RenderTexture.GetTemporary(BufferTexture.width, BufferTexture.height, 0, RenderTextureFormat.ARGBFloat);
            Graphics.Blit(BufferTexture, temp, WaterBufferMaterial);
            Graphics.Blit(temp, BufferTexture);
            RenderTexture.ReleaseTemporary(temp);

            //set the feedbacK buffer texture to the water material
            WaterMaterial.SetTexture("_BufferTex", BufferTexture);

        }
    }
}


