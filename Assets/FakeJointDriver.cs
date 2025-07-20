using UnityEngine;

public class FakeJointDriver : MonoBehaviour
{
    public Animator animator;

    private Transform hips, spine, head, 
        leftUpperArm, leftLowerArm, rightUpperArm, rightLowerArm,
        leftUpperLeg, leftLowerLeg, rightUpperLeg, rightLowerLeg;

    private Quaternion hipsBindRot, spineBindRot, headBindRot, 
        leftArmBindRot, rightArmBindRot, leftLowerArmBindRot, rightLowerArmBindRot,
        leftLegBindRot, rightLegBindRot, leftLowerLegBindRot, rightLowerLegBindRot;

    private Vector3 hipsInitialPos;

    void Start()
    {
        // Get bone transforms
        hips = animator.GetBoneTransform(HumanBodyBones.Hips);
        spine = animator.GetBoneTransform(HumanBodyBones.Spine);
        head = animator.GetBoneTransform(HumanBodyBones.Head);
        leftUpperArm = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        rightUpperArm = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
        leftLowerArm = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
        rightLowerArm = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
        leftUpperLeg = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
        rightUpperLeg = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
        leftLowerLeg = animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
        rightLowerLeg = animator.GetBoneTransform(HumanBodyBones.RightLowerLeg);

        // Store bind pose rotations
        if (hips) hipsBindRot = hips.localRotation;
        if (spine) spineBindRot = spine.localRotation;
        if (head) headBindRot = head.localRotation;
        if (leftUpperArm) leftArmBindRot = leftUpperArm.localRotation;
        if (rightUpperArm) rightArmBindRot = rightUpperArm.localRotation;
        if (leftLowerArm) leftLowerArmBindRot = leftLowerArm.localRotation;
        if (rightLowerArm) rightLowerArmBindRot = rightLowerArm.localRotation;
        if (leftUpperLeg) leftLegBindRot = leftUpperLeg.localRotation;
        if (rightUpperLeg) rightLegBindRot = rightUpperLeg.localRotation;
        if (leftLowerLeg) leftLowerLegBindRot = leftLowerLeg.localRotation;
        if (rightLowerLeg) rightLowerLegBindRot = rightLowerLeg.localRotation;

        if (hips) hipsInitialPos = hips.position;
    }

    void Update()
    {
        if (hips == null) return;

        // Hips: up/down and slight tilt
        hips.position = hipsInitialPos + Vector3.up * Mathf.Sin(Time.time * 2f) * 0.1f;
        hips.localRotation = hipsBindRot * Quaternion.Euler(
            5f * Mathf.Sin(Time.time * 1.5f), // Tilt forward/back
            10f * Mathf.Sin(Time.time * 1.3f), // Rotate left/right
            5f * Mathf.Cos(Time.time * 1.7f)  // Lean side to side
        );

        // Spine: bend forward/back, twist, and side lean
        if (spine)
            spine.localRotation = spineBindRot * Quaternion.Euler(
                15f * Mathf.Sin(Time.time * 1.2f), // Forward/back
                20f * Mathf.Sin(Time.time * 1.0f), // Twist
                10f * Mathf.Cos(Time.time * 1.4f)  // Side lean
            );

        // Head: nod, turn, and tilt
        if (head)
            head.localRotation = headBindRot * Quaternion.Euler(
                20f * Mathf.Sin(Time.time * 1.5f), // Nod
                30f * Mathf.Sin(Time.time * 1.3f), // Turn
                15f * Mathf.Cos(Time.time * 1.6f)  // Tilt
            );

        // Left Upper Arm: swing forward/back, up/down, and rotate
        if (leftUpperArm)
            leftUpperArm.localRotation = leftArmBindRot * Quaternion.Euler(
                45f * Mathf.Sin(Time.time * 2f),      // Swing forward/back
                20f * Mathf.Cos(Time.time * 1.8f),    // Up/down
                15f * Mathf.Sin(Time.time * 1.6f)     // Rotate
            );

        // Right Upper Arm: opposite phase for natural look
        if (rightUpperArm)
            rightUpperArm.localRotation = rightArmBindRot * Quaternion.Euler(
                45f * Mathf.Sin(Time.time * 2f + Mathf.PI), // Swing forward/back
                20f * Mathf.Cos(Time.time * 1.8f + Mathf.PI), // Up/down
                15f * Mathf.Sin(Time.time * 1.6f + Mathf.PI)  // Rotate
            );

        // Left Lower Arm: elbow bend
        if (leftLowerArm)
            leftLowerArm.localRotation = leftLowerArmBindRot * Quaternion.Euler(
                60f * Mathf.Abs(Mathf.Sin(Time.time * 2.2f)), // Elbow bend
                0,
                0
            );

        // Right Lower Arm: elbow bend
        if (rightLowerArm)
            rightLowerArm.localRotation = rightLowerArmBindRot * Quaternion.Euler(
                60f * Mathf.Abs(Mathf.Sin(Time.time * 2.2f + Mathf.PI)), // Elbow bend
                0,
                0
            );

        // Left Upper Leg: swing forward/back, slight abduction, and rotation
        if (leftUpperLeg)
            leftUpperLeg.localRotation = leftLegBindRot * Quaternion.Euler(
                35f * Mathf.Sin(Time.time * 2f + Mathf.PI), // Swing forward/back
                10f * Mathf.Cos(Time.time * 1.9f),         // Abduction/adduction
                10f * Mathf.Sin(Time.time * 1.7f)          // Rotation
            );

        // Right Upper Leg: opposite phase
        if (rightUpperLeg)
            rightUpperLeg.localRotation = rightLegBindRot * Quaternion.Euler(
                35f * Mathf.Sin(Time.time * 2f),          // Swing forward/back
                10f * Mathf.Cos(Time.time * 1.9f + Mathf.PI), // Abduction/adduction
                10f * Mathf.Sin(Time.time * 1.7f + Mathf.PI)  // Rotation
            );

        // Left Lower Leg: knee bend
        if (leftLowerLeg)
            leftLowerLeg.localRotation = leftLowerLegBindRot * Quaternion.Euler(
                50f * Mathf.Abs(Mathf.Sin(Time.time * 2.1f)), // Knee bend
                0,
                0
            );

        // Right Lower Leg: knee bend
        if (rightLowerLeg)
            rightLowerLeg.localRotation = rightLowerLegBindRot * Quaternion.Euler(
                50f * Mathf.Abs(Mathf.Sin(Time.time * 2.1f + Mathf.PI)), // Knee bend
                0,
                0
            );
    }
}