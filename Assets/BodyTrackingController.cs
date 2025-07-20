using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

// Enum for ARKit/ARCore joint indices (expanded to include all joints from BodyTrackingController)
public enum JointIndices3D
{
    Hips = 0,
    Spine1 = 1,
    Neck1 = 20,
    Head = 21,
    LeftShoulder = 2,
    LeftUpperArm = 3,
    LeftLowerArm = 4,
    LeftHand = 5,
    RightShoulder = 6,
    RightUpperArm = 7,
    RightLowerArm = 8,
    RightHand = 9,
    LeftUpperLeg = 10,
    LeftLowerLeg = 11,
    LeftFoot = 12,
    RightUpperLeg = 13,
    RightLowerLeg = 14,
    RightFoot = 15
}

[RequireComponent(typeof(ARHumanBodyManager))]
public class HumanBodyTracking : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    [Tooltip("The ARHumanBodyManager which will produce frame events.")]
    private ARHumanBodyManager humanBodyManager;

    [SerializeField]
    [Tooltip("The Animator component of the rigged avatar.")]
    private Animator avatarAnimator;

    [Header("Smoothing")]
    [SerializeField]
    private float positionLerpSpeed = 5f;
    [SerializeField]
    private float rotationLerpSpeed = 5f;

    // Cache bone transforms for performance
    private Dictionary<HumanBodyBones, Transform> boneCache;

    // Mapping from AR joint indices to Unity HumanBodyBones
    private readonly Dictionary<JointIndices3D, HumanBodyBones> jointToBoneMap = new Dictionary<JointIndices3D, HumanBodyBones>
    {
        { JointIndices3D.Hips, HumanBodyBones.Hips },
        { JointIndices3D.Spine1, HumanBodyBones.Spine },
        { JointIndices3D.Neck1, HumanBodyBones.Neck },
        { JointIndices3D.Head, HumanBodyBones.Head },
        { JointIndices3D.LeftShoulder, HumanBodyBones.LeftShoulder },
        { JointIndices3D.LeftUpperArm, HumanBodyBones.LeftUpperArm },
        { JointIndices3D.LeftLowerArm, HumanBodyBones.LeftLowerArm },
        { JointIndices3D.LeftHand, HumanBodyBones.LeftHand },
        { JointIndices3D.RightShoulder, HumanBodyBones.RightShoulder },
        { JointIndices3D.RightUpperArm, HumanBodyBones.RightUpperArm },
        { JointIndices3D.RightLowerArm, HumanBodyBones.RightLowerArm },
        { JointIndices3D.RightHand, HumanBodyBones.RightHand },
        { JointIndices3D.LeftUpperLeg, HumanBodyBones.LeftUpperLeg },
        { JointIndices3D.LeftLowerLeg, HumanBodyBones.LeftLowerLeg },
        { JointIndices3D.LeftFoot, HumanBodyBones.LeftFoot },
        { JointIndices3D.RightUpperLeg, HumanBodyBones.RightUpperLeg },
        { JointIndices3D.RightLowerLeg, HumanBodyBones.RightLowerLeg },
        { JointIndices3D.RightFoot, HumanBodyBones.RightFoot }
    };

    void Awake()
    {
        // Initialize humanBodyManager
        humanBodyManager = GetComponent<ARHumanBodyManager>();
        if (humanBodyManager == null)
        {
            Debug.LogError("ARHumanBodyManager component is missing!", this);
            enabled = false;
            return;
        }

        // Validate avatarAnimator
        if (avatarAnimator == null)
        {
            Debug.LogError("Avatar Animator is not assigned!", this);
            enabled = false;
            return;
        }

        // Cache bone transforms
        boneCache = new Dictionary<HumanBodyBones, Transform>();
        foreach (var joint in jointToBoneMap)
        {
            Transform boneTransform = avatarAnimator.GetBoneTransform(joint.Value);
            if (boneTransform != null)
            {
                boneCache[joint.Value] = boneTransform;
            }
            else
            {
                Debug.LogWarning($"Bone {joint.Value} not found in Animator!", this);
            }
        }
    }

    void OnEnable()
    {
        if (humanBodyManager != null)
        {
            humanBodyManager.humanBodiesChanged += OnHumanBodiesChanged;
        }
    }

    void OnDisable()
    {
        if (humanBodyManager != null)
        {
            humanBodyManager.humanBodiesChanged -= OnHumanBodiesChanged;
        }
    }

    void OnHumanBodiesChanged(ARHumanBodiesChangedEventArgs eventArgs)
    {
        foreach (ARHumanBody humanBody in eventArgs.added)
        {
            UpdateBody(humanBody);
        }

        foreach (ARHumanBody humanBody in eventArgs.updated)
        {
            UpdateBody(humanBody);
        }
    }

    void UpdateBody(ARHumanBody body)
    {
        if (body == null || (body.trackingState != TrackingState.Tracking && body.trackingState != TrackingState.Limited) || !body.joints.IsCreated)
        {
            return;
        }

        NativeArray<XRHumanBodyJoint> joints = body.joints;

        // Update avatar's root transform to match hips
        if (joints[(int)JointIndices3D.Hips].tracked)
        {
            float positionLerp = 1f - Mathf.Exp(-positionLerpSpeed * Time.deltaTime);
            float rotationLerp = 1f - Mathf.Exp(-rotationLerpSpeed * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, joints[(int)JointIndices3D.Hips].anchorPose.position, positionLerp);
            transform.rotation = Quaternion.Slerp(transform.rotation, joints[(int)JointIndices3D.Hips].anchorPose.rotation, rotationLerp);
        }

        // Update each bone based on AR joint data
        foreach (var joint in jointToBoneMap)
        {
            MapJointToBone(joints, (int)joint.Key, joint.Value);
        }
    }

    void MapJointToBone(NativeArray<XRHumanBodyJoint> joints, int jointIndex, HumanBodyBones unityBone)
    {
        if (jointIndex < 0 || jointIndex >= joints.Length || !joints[jointIndex].tracked)
        {
            return;
        }

        if (!boneCache.TryGetValue(unityBone, out Transform bone))
        {
            return;
        }

        XRHumanBodyJoint arJoint = joints[jointIndex];
        Vector3 targetPosition = arJoint.anchorPose.position;
        Quaternion targetRotation = arJoint.anchorPose.rotation;

        // Apply smoothed transformations
        float positionLerp = 1f - Mathf.Exp(-positionLerpSpeed * Time.deltaTime);
        float rotationLerp = 1f - Mathf.Exp(-rotationLerpSpeed * Time.deltaTime);
        bone.position = Vector3.Lerp(bone.position, targetPosition, positionLerp);
        bone.rotation = Quaternion.Slerp(bone.rotation, targetRotation, rotationLerp);
    }
}