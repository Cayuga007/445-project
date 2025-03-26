using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaterShader
{
    public class EnableFog : MonoBehaviour
    {
        public string PlayerTag;


        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(PlayerTag))
            {
                RenderSettings.fog = true;
                RenderSettings.fogMode = FogMode.Linear;
                RenderSettings.fogStartDistance = 0f;
                RenderSettings.fogEndDistance = 40f;
                RenderSettings.fogColor = new Color(94f / 255f, 94f / 255f, 94f / 255f);

                Debug.Log("Fog Activated");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(PlayerTag))
            {
                RenderSettings.fog = false;

                Debug.Log("Fog Deactivated");
            }
        }
    }
}


