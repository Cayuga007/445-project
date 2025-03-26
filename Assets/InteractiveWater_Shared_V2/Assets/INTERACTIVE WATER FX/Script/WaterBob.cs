using UnityEngine;

namespace WaterShader
{
    public class WaterBob : MonoBehaviour
    {
        public enum RotationAxis
        {
            X,
            Y,
            Z
        }

        [Header("Water Bob Settings")]
        public float bobFrequency = 0.2f;
        public float bobAmplitude = 0.15f;
        public float waveSpeed = 4;
        public float rotationAmplitude = 1.0f;


        public RotationAxis rotationAxis = RotationAxis.Z;

        private Vector3 startPos;
        private Quaternion startRotation;
        private float time;

        void Start()
        {
            startPos = transform.position;
            startRotation = transform.rotation;
        }

        void Update()
        {
            time += Time.deltaTime * waveSpeed;

            float wave = Mathf.Sin(time * bobFrequency) * bobAmplitude;
            transform.position = startPos + new Vector3(0, wave, 0);

            float rotationValue = Mathf.Sin(time * bobFrequency * 2) * rotationAmplitude;

            Quaternion rotation = Quaternion.identity;

            switch (rotationAxis)
            {
                case RotationAxis.X:
                    rotation = Quaternion.Euler(rotationValue, 0, 0);
                    break;
                case RotationAxis.Y:
                    rotation = Quaternion.Euler(0, rotationValue, 0);
                    break;
                case RotationAxis.Z:
                    rotation = Quaternion.Euler(0, 0, rotationValue);
                    break;
            }

            transform.rotation = startRotation * rotation;
        }
    }
}
