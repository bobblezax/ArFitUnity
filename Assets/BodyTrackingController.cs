using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

// Enum for ARKit/ARCore joint indices
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
public class BodyTrackingController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    [Tooltip("The ARHumanBodyManager which will produce frame events.")]
    private ARHumanBodyManager humanBodyManager;

    [SerializeField]
    [Tooltip("The ARCameraManager to control camera facing direction.")]
    private ARCameraManager cameraManager;

    [SerializeField]
    [Tooltip("The Animator component of the rigged avatar.")]
    private Animator avatarAnimator;

    [Header("Smoothing")]
    [SerializeField]
    private float positionLerpSpeed = 5f;
    [SerializeField]
    private float rotationLerpSpeed = 5f;

    [Header("Front Camera Adjustments")]
    [SerializeField]
    [Tooltip("Apply mirroring correction for front-facing camera (flips left-right).")]
    private bool applyMirroringCorrection = true;

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

    private bool isCameraInitialized;

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

        // Validate cameraManager
        if (cameraManager == null)
        {
            cameraManager = FindObjectOfType<ARCameraManager>();
            if (cameraManager == null)
            {
                Debug.LogError("ARCameraManager not found in scene!", this);
                enabled = false;
                return;
            }
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

    void Start()
    {
        // Start coroutine to ensure front-facing camera is set
        StartCoroutine(TrySetFrontFacingCamera());
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
        StopAllCoroutines();
    }

    IEnumerator TrySetFrontFacingCamera()
    {
        // Wait until AR subsystem is initialized
        while (cameraManager != null && cameraManager.subsystem == null)
        {
            Debug.Log("Waiting for ARCameraManager subsystem to initialize...");
            yield return new WaitForSeconds(0.1f);
        }

        if (cameraManager == null || cameraManager.subsystem == null)
        {
            Debug.LogError("ARCameraManager or subsystem not available. Cannot set front-facing camera.", this);
            isCameraInitialized = false;
            yield break;
        }

        // Check if front-facing camera is set
        if (cameraManager.currentFacingDirection != CameraFacingDirection.User)
        {
            cameraManager.requestedFacingDirection = CameraFacingDirection.User;
            Debug.Log("Requested front-facing camera.");
        }
        else
        {
            Debug.Log("Front-facing camera already set.");
        }

        // Verify the current facing direction
        yield return new WaitForSeconds(0.5f); // Wait for subsystem to apply change
        if (cameraManager.currentFacingDirection != CameraFacingDirection.User)
        {
            Debug.LogWarning($"Failed to set front-facing camera. Current facing: {cameraManager.currentFacingDirection}. Device may not support front-facing body tracking.", this);
        }
        else
        {
            Debug.Log("Front-facing camera successfully set.");
        }

        isCameraInitialized = true;
    }

    void OnHumanBodiesChanged(ARHumanBodiesChangedEventArgs eventArgs)
    {
        if (!isCameraInitialized)
        {
            return; // Skip updates until camera is initialized
        }

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
            Debug.Log("No valid body tracking data.");
            return;
        }

        NativeArray<XRHumanBodyJoint> joints = body.joints;

        // Update avatar's root transform to match hips
        if (joints[(int)JointIndices3D.Hips].tracked)
        {
            float positionLerp = 1f - Mathf.Exp(-positionLerpSpeed * Time.deltaTime);
            float rotationLerp = 1f - Mathf.Exp(-rotationLerpSpeed * Time.deltaTime);
            Vector3 targetPosition = joints[(int)JointIndices3D.Hips].anchorPose.position;
            Quaternion targetRotation = joints[(int)JointIndices3D.Hips].anchorPose.rotation;

            // Apply mirroring correction for front-facing camera
            if (applyMirroringCorrection && cameraManager.currentFacingDirection == CameraFacingDirection.User)
            {
                targetPosition.x = -targetPosition.x; // Mirror horizontally
                targetRotation = Quaternion.Euler(targetRotation.eulerAngles.x, 180f - targetRotation.eulerAngles.y, targetRotation.eulerAngles.z); // Flip Y rotation
            }

            transform.position = Vector3.Lerp(transform.position, targetPosition, positionLerp);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationLerp);
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

        // Apply mirroring correction for front-facing camera
        if (applyMirroringCorrection && cameraManager.currentFacingDirection == CameraFacingDirection.User)
        {
            targetPosition.x = -targetPosition.x; // Mirror horizontally
            targetRotation = Quaternion.Euler(targetRotation.eulerAngles.x, 180f - targetRotation.eulerAngles.y, targetRotation.eulerAngles.z); // Flip Y rotation
        }

        // Apply smoothed transformations
        float positionLerp = 1f - Mathf.Exp(-positionLerpSpeed * Time.deltaTime);
        float rotationLerp = 1f - Mathf.Exp(-rotationLerpSpeed * Time.deltaTime);
        bone.position = Vector3.Lerp(bone.position, targetPosition, positionLerp);
        bone.rotation = Quaternion.Slerp(bone.rotation, targetRotation, rotationLerp);
    }
}