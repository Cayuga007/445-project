using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MirrorEffect : MonoBehaviour
{
    public Material blankMaterial; // The default blank mirror material
    public Material faceMaterial;  // The material with the MC's face
    public Renderer mirrorRenderer; // Drag your mirror object here

    public float flashDuration = 3f;
    private bool isFlashing = false;

    void Start()
    {
        if (mirrorRenderer == null)
            mirrorRenderer = GetComponent<Renderer>();

        mirrorRenderer.material = blankMaterial;
    }

    void Update()
    {
        // Press "E" to trigger the mirror flash effect
        if (Input.GetKeyDown(KeyCode.E))
        {
            TriggerMirrorFlash();
        }
    }

    public void TriggerMirrorFlash()
    {
        if (!isFlashing)
        {
            StartCoroutine(FlashMirror());
        }
    }

    IEnumerator FlashMirror()
    {
        isFlashing = true;
        mirrorRenderer.material = faceMaterial;
        yield return new WaitForSeconds(flashDuration);
        mirrorRenderer.material = blankMaterial;
        isFlashing = false;
    }
}