using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public float interactionDistance = 3f; // ��󽻻�����
    public Transform holdPoint; // ץȡ����ʱ�Ĺ��ص�
    private GameObject heldObject = null;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    [Header("Layer Settings")]
    public LayerMask interactableLayer; // �ɽ�������Ĳ㼶

    [Header("Input Settings")]
    public OVRInput.Button pickupButton = OVRInput.Button.One; // Ĭ������ A ��

    [Header("Hand Transform")]
    public Transform handTransform; // �������ߵ���㷽�򣨿��ÿ��������ֲ�λ�ã�

    void Update()
    {
        HandleInteraction();
    }

    private void HandleInteraction()
    {
        if (OVRInput.GetDown(pickupButton))
        {
            if (heldObject == null)
            {
                TryPickupObject();
            }
            else
            {
                PlaceObjectDown();
            }
        }
    }

    private void TryPickupObject()
    {
        Ray ray = new Ray(handTransform.position, handTransform.forward);
        Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.green);

        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactableLayer))
        {
            if (hit.collider.CompareTag("Interactable"))
            {
                heldObject = hit.collider.gameObject;

                originalPosition = heldObject.transform.position;
                originalRotation = heldObject.transform.rotation;

                Rigidbody rb = heldObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;
                }

                heldObject.transform.position = holdPoint.position;
                heldObject.transform.rotation = holdPoint.rotation;
                heldObject.transform.SetParent(holdPoint);
            }
        }
    }

    private void PlaceObjectDown()
    {
        if (heldObject != null)
        {
            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }

            heldObject.transform.SetParent(null);
            heldObject.transform.position = originalPosition;
            heldObject.transform.rotation = originalRotation;

            heldObject = null;
        }
    }
}
