using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.Collections;

[RequireComponent(typeof(ARHumanBodyManager))]
public class BodyTrackingController : MonoBehaviour
{
    [Header("References")]
    public Animator avatarAnimator; // Your rigged model's Animator

    ARHumanBodyManager humanBodyManager;

    [Header("Smoothing")]
    public float positionLerpSpeed = 5f;
    public float rotationLerpSpeed = 5f;

    // === ARKit joint index mapping ===
    const int HIPS = 0;
    const int SPINE1 = 1;
    const int NECK1 = 20;
    const int HEAD = 21;

    const int LEFT_SHOULDER = 2;
    const int LEFT_UPPER_ARM = 3;
    const int LEFT_LOWER_ARM = 4;
    const int LEFT_HAND = 5;

    const int RIGHT_SHOULDER = 6;
    const int RIGHT_UPPER_ARM = 7;
    const int RIGHT_LOWER_ARM = 8;
    const int RIGHT_HAND = 9;

    const int LEFT_UPPER_LEG = 10;
    const int LEFT_LOWER_LEG = 11;
    const int LEFT_FOOT = 12;

    const int RIGHT_UPPER_LEG = 13;
    const int RIGHT_LOWER_LEG = 14;
    const int RIGHT_FOOT = 15;

    void Awake()
    {
        humanBodyManager = GetComponent<ARHumanBodyManager>();
    }

    void OnEnable()
    {
        humanBodyManager.humanBodiesChanged += OnHumanBodiesChanged;
    }

    void OnDisable()
    {
        humanBodyManager.humanBodiesChanged -= OnHumanBodiesChanged;
    }

    void OnHumanBodiesChanged(ARHumanBodiesChangedEventArgs args)
    {
        foreach (var body in args.added)
        {
            UpdateAvatar(body);
        }

        foreach (var body in args.updated)
        {
            UpdateAvatar(body);
        }
    }

    void UpdateAvatar(ARHumanBody body)
    {
        if (body == null || body.trackingState != TrackingState.Tracking)
            return;

        NativeArray<XRHumanBodyJoint> joints = body.joints;

        // Hips and spine
        MapJointToBone(joints, HIPS, HumanBodyBones.Hips);
        MapJointToBone(joints, SPINE1, HumanBodyBones.Spine);

        // Neck and head
        MapJointToBone(joints, NECK1, HumanBodyBones.Neck);
        MapJointToBone(joints, HEAD, HumanBodyBones.Head);

        // Left arm
        MapJointToBone(joints, LEFT_SHOULDER, HumanBodyBones.LeftShoulder);
        MapJointToBone(joints, LEFT_UPPER_ARM, HumanBodyBones.LeftUpperArm);
        MapJointToBone(joints, LEFT_LOWER_ARM, HumanBodyBones.LeftLowerArm);
        MapJointToBone(joints, LEFT_HAND, HumanBodyBones.LeftHand);

        // Right arm
        MapJointToBone(joints, RIGHT_SHOULDER, HumanBodyBones.RightShoulder);
        MapJointToBone(joints, RIGHT_UPPER_ARM, HumanBodyBones.RightUpperArm);
        MapJointToBone(joints, RIGHT_LOWER_ARM, HumanBodyBones.RightLowerArm);
        MapJointToBone(joints, RIGHT_HAND, HumanBodyBones.RightHand);

        // Left leg
        MapJointToBone(joints, LEFT_UPPER_LEG, HumanBodyBones.LeftUpperLeg);
        MapJointToBone(joints, LEFT_LOWER_LEG, HumanBodyBones.LeftLowerLeg);
        MapJointToBone(joints, LEFT_FOOT, HumanBodyBones.LeftFoot);

        // Right leg
        MapJointToBone(joints, RIGHT_UPPER_LEG, HumanBodyBones.RightUpperLeg);
        MapJointToBone(joints, RIGHT_LOWER_LEG, HumanBodyBones.RightLowerLeg);
        MapJointToBone(joints, RIGHT_FOOT, HumanBodyBones.RightFoot);
    }

    void MapJointToBone(NativeArray<XRHumanBodyJoint> joints, int jointIndex, HumanBodyBones unityBone)
    {
        if (jointIndex < 0 || jointIndex >= joints.Length)
            return;

        XRHumanBodyJoint arJoint = joints[jointIndex];

        if (!arJoint.tracked)
            return;

        Transform bone = avatarAnimator.GetBoneTransform(unityBone);

        if (bone == null)
            return;

        Vector3 targetPosition = arJoint.anchorPose.position;
        Quaternion targetRotation = arJoint.anchorPose.rotation;

        // Smooth position/rotation (world space)
        bone.position = Vector3.Lerp(bone.position, targetPosition, Time.deltaTime * positionLerpSpeed);
        bone.rotation = Quaternion.Slerp(bone.rotation, targetRotation, Time.deltaTime * rotationLerpSpeed);
    }
}
