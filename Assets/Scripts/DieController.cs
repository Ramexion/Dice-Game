
using System;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class DiceController :MonoBehaviour
{
    [Header("Face Configuration")]
    [SerializeField]
    private List<DiceFace> faces = new List<DiceFace>();
    [SerializeField] private AudioSource Diesound;

    [Header("Physics")]
    [SerializeField] private float upwardForce = 4f;
    [SerializeField] private float torqueStrength = 12f;

    private Rigidbody rb;

    public Rigidbody Body => rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.isKinematic = true;
    }

    public void Roll()
    {
        rb.isKinematic = false;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.WakeUp();

        Vector3 randomTorque = new Vector3(
            UnityEngine.Random.Range(-torqueStrength, torqueStrength),
            UnityEngine.Random.Range(-torqueStrength, torqueStrength),
            UnityEngine.Random.Range(-torqueStrength, torqueStrength)
        );

        Vector3 randomForce = new Vector3(
            UnityEngine.Random.Range(-1.5f, 1.5f),
            upwardForce,
            UnityEngine.Random.Range(-1.5f, 1.5f)
        );

        rb.AddForce(randomForce, ForceMode.Impulse);

        rb.AddTorque(randomTorque, ForceMode.Impulse);
        Diesound.Play();
    }

    public bool IsSettled()
    {
        return rb.linearVelocity.magnitude < 0.05f &&
               rb.angularVelocity.magnitude < 0.05f;
    }

    public void StopPhysics()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.isKinematic = true;
    }

    public int GetTopFaceValue()
    {
        float highestDot = -Mathf.Infinity;
        int topValue = 0;

        foreach (DiceFace face in faces)
        {
            float dot = Vector3.Dot(
                face.faceNormal.up,
                Vector3.up
            );

            if (dot > highestDot)
            {
                highestDot = dot;
                topValue = face.value;
            }
        }
        Debug.Log("Detected Top Face: " + topValue);


        return topValue;
    }

    public Quaternion GetTargetRotation(int targetValue)
    {
        foreach (DiceFace face in faces)
        {
            if (face.value == targetValue &&
                face.faceNormal != null)
            {
                Quaternion correction =
                    Quaternion.FromToRotation(
                        face.faceNormal.up,
                        Vector3.up
                    );

                return correction * transform.rotation;
            }
        }

        Debug.LogError(
            "Face value " + targetValue +
            " not configured on " + gameObject.name
        );

        return transform.rotation;
    }

    public void SetRotation(Quaternion rotation)
    {
        transform.rotation = rotation;
    }

    private void OnValidate()
    {
        if (faces == null)
            return;

        foreach (DiceFace face in faces)
        {
            if (face.value < 1 || face.value > 6)
                Debug.LogWarning("Dice face values must be 1-6.");
        }
    }
}
[Serializable]
public class DiceFace
{
    public int value;
    public Transform faceNormal;
}

