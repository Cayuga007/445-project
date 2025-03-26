using UnityEngine;

namespace WaterShader
{
    [RequireComponent(typeof(Rigidbody))]
    public class WaterFloatObject : MonoBehaviour
    {
        public float depthBeforeSubmersion = 1f; 
        public float displacementAmount = 3f; 
        public float waterDrag = 0.5f; 
        public float waterAngularDrag = 0.5f; 
        public Transform waterPlane; 
        public WaterRippleController waterController;

        private Rigidbody rb;
        private float waterHeight;
        float waterHeightOffset;

        bool objectDropped = false;
        bool objectMove = false;

        Vector3 prevPos = Vector3.zero;
        Vector3 currentPos = Vector3.zero;

        void Start()
        {
            waterHeightOffset = 2.0f;

            rb = GetComponent<Rigidbody>();
            currentPos = this.transform.position;
            prevPos = currentPos;
        }

        void FixedUpdate()
        {
            rb.AddForce(Physics.gravity, ForceMode.Acceleration);

            waterHeight = GetWaterHeightAtPosition(transform.position);

            if (transform.position.y < waterHeight)
            {
                float submersionDepth = waterHeight - transform.position.y;

                float displacementMultiplier = Mathf.Clamp01(submersionDepth / depthBeforeSubmersion) * displacementAmount;
                rb.AddForce(Vector3.up * Mathf.Abs(Physics.gravity.y) * displacementMultiplier, ForceMode.Acceleration);

                rb.AddForce(-rb.velocity * waterDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
                rb.AddTorque(-rb.angularVelocity * waterAngularDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
            }
            
        }

        private float GetWaterHeightAtPosition(Vector3 position)
        {
            float waveHeight = Shader.GetGlobalFloat("_WaveHeight");
            float waveFrequency = Shader.GetGlobalFloat("_WaveFrequency");
            float waveSpeed = Shader.GetGlobalFloat("_WaveSpeed");

            float xWave = Mathf.Sin(position.x * waveFrequency + Time.time * waveSpeed);
            float zWave = Mathf.Cos(position.z * waveFrequency + Time.time * waveSpeed);
            return waterPlane.position.y + (xWave * zWave * waveHeight);
        }

        void Update(){

            currentPos = this.transform.position;

            //if object dropped, create ripples
            if ( currentPos.y <= (waterHeight) && !objectDropped)
            {
                objectDropped = true;
                SetFloatingObjectPos(currentPos);
            }
            else
            {
                objectDropped = false;
            }


            //if object moved, create ripples
            var currentPosXZ = new Vector3(currentPos.x, 0f, currentPos.z);
            var prevPosXZ = new Vector3(prevPos.x, 0f, prevPos.z);
            if (currentPosXZ != prevPosXZ && objectDropped)
            {
                objectMove = true;
                SetFloatingObjectPos(currentPos);
                prevPos = currentPos;
                //Debug.Log("object moved.. " + localPosNormalized);

            }
            else
            {
                objectMove = false;
            }

            if(!(objectDropped || objectMove))
                waterController.WaterBufferMaterial.SetVector("_mousePos", new Vector4(10000f, 10000f, 0f, 0f));
           

        }

        void SetFloatingObjectPos(Vector3 pos)
        {
            Vector3 localPos = waterPlane.InverseTransformPoint(pos);
            Vector3 localScale = waterPlane.localScale;
            Vector4 localPosNormalized = new Vector4(1.0f - (localPos.x / 10f + 0.5f), 1.0f - (localPos.z / 10f + 0.5f), 0f, 0f);
            waterController.WaterBufferMaterial.SetVector("_mousePos", localPosNormalized);
        }
        
    }
}

