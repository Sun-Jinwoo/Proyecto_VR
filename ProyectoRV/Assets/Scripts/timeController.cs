using System;
using UnityEngine;

public class Class1 : MonoBehaviour 
{
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    [Range(0.05f, 1f)]
    public float minTimeSacle = 0.15f;
    [Range(0.1f, 1f)]
    public float maxTimeSacle = 1f;

    public float movementMultiplier = 12f;
    public float movementThereshold = 0.001f;
    public float smoothSpeed = 3f;

    private Vector3 lastHeadPos;
    private Vector3 lastLeftHandPos;
    private Vector3 lastRightHandPos;

    void Start()
    {
        lastHeadPos = head.position;
        lastLeftHandPos = leftHand.position;
        lastRightHandPos = rightHand.position;

        Time.timeScale = minTimeSacle;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    void Update()
    {
        float headMovement = Vector3.Distance(head.position, lastHeadPos);
        float leftHandMovement = Vector3.Distance(leftHand.position, lastLeftHandPos);
        float rightHandMovement = Vector3.Distance(rightHand.position, lastRightHandPos);

        headMovement = Mathf.Max(0, headMovement - movementThereshold);
        leftHandMovement = Mathf.Max(0, leftHandMovement - movementThereshold);
        rightHandMovement = Mathf.Max(0, rightHandMovement - movementThereshold);

        float totalMovement = headMovement + leftHandMovement + rightHandMovement;
        float targetTimeScale = Mathf.Clamp(1f - (totalMovement * movementMultiplier), minTimeSacle, maxTimeSacle);

        Time.timeScale = Mathf.Lerp(Time.timeScale, targetTimeScale, Time.unscaledDeltaTime* smoothSpeed);

        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        lastHeadPos = head.position;
        lastLeftHandPos = leftHand.position;
        lastRightHandPos = rightHand.position;
    }


}