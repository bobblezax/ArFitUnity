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
        rightLowerArm = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
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

        float beat = Time.time * 3f; // Faster tempo for dance (120 BPM approximation)

        // Hips: sway side to side, slight bounce, and rotation
        hips.position = hipsInitialPos + Vector3.up * Mathf.Abs(Mathf.Sin(beat)) * 0.15f;
        hips.localRotation = hipsBindRot * Quaternion.Euler(
            8f * Mathf.Sin(beat * 0.5f),  // Slight tilt
            15f * Mathf.Sin(beat),        // Side sway rotation
            10f * Mathf.Cos(beat * 0.75f) // Hip twist
        );

        // Spine: groovy bend and twist
        if (spine)
            spine.localRotation = spineBindRot * Quaternion.Euler(
                12f * Mathf.Sin(beat * 0.8f), // Forward/back bend
                20f * Mathf.Cos(beat * 0.6f), // Twist
                10f * Mathf.Sin(beat)         // Side lean
            );

        // Head: bob and nod to the beat
        if (head)
            head.localRotation = headBindRot * Quaternion.Euler(
                15f * Mathf.Sin(beat * 1.2f), // Nod
                25f * Mathf.Cos(beat),        // Turn
                10f * Mathf.Sin(beat * 0.9f)  // Tilt
            );

        // Left Upper Arm: swing and raise
        if (leftUpperArm)
            leftUpperArm.localRotation = leftArmBindRot * Quaternion.Euler(
                60f * Mathf.Sin(beat),        // Swing forward/back
                30f * Mathf.Cos(beat * 0.8f), // Raise/lower
                20f * Mathf.Sin(beat * 0.7f)  // Rotate
            );

        // Right Upper Arm: opposite phase for dynamic dance
        if (rightUpperArm)
            rightUpperArm.localRotation = rightArmBindRot * Quaternion.Euler(
                60f * Mathf.Sin(beat + Mathf.PI),     // Swing forward/back
                30f * Mathf.Cos(beat * 0.8f + Mathf.PI), // Raise/lower
                20f * Mathf.Sin(beat * 0.7f + Mathf.PI)  // Rotate
            );

        // Left Lower Arm: elbow bend with flair
        if (leftLowerArm)
            leftLowerArm.localRotation = leftLowerArmBindRot * Quaternion.Euler(
                70f * Mathf.Abs(Mathf.Sin(beat * 1.1f)), // Elbow bend
                0,
                15f * Mathf.Sin(beat * 0.9f)            // Slight wrist twist
            );

        // Right Lower Arm: elbow bend with flair
        if (rightLowerArm)
            rightLowerArm.localRotation = rightLowerArmBindRot * Quaternion.Euler(
                70f * Mathf.Abs(Mathf.Sin(beat * 1.1f + Mathf.PI)), // Elbow bend
                0,
                15f * Mathf.Sin(beat * 0.9f + Mathf.PI)            // Slight wrist twist
            );

        // Left Upper Leg: step and sway
        if (leftUpperLeg)
            leftUpperLeg.localRotation = leftLegBindRot * Quaternion.Euler(
                40f * Mathf.Sin(beat + Mathf.PI), // Step forward/back
                15f * Mathf.Cos(beat * 0.8f),     // Side sway
                10f * Mathf.Sin(beat * 0.7f)      // Rotation
            );

        // Right Upper Leg: opposite phase step
        if (rightUpperLeg)
            rightUpperLeg.localRotation = rightLegBindRot * Quaternion.Euler(
                40f * Mathf.Sin(beat),            // Step forward/back
                15f * Mathf.Cos(beat * 0.8f + Mathf.PI), // Side sway
                10f * Mathf.Sin(beat * 0.7f + Mathf.PI)  // Rotation
            );

        // Left Lower Leg: knee bend for stepping
        if (leftLowerLeg)
            leftLowerLeg.localRotation = leftLowerLegBindRot * Quaternion.Euler(
                60f * Mathf.Abs(Mathf.Sin(beat * 1.2f)), // Knee bend
                0,
                0
            );

        // Right Lower Leg: knee bend for stepping
        if (rightLowerLeg)
            rightLowerLeg.localRotation = rightLowerLegBindRot * Quaternion.Euler(
                60f * Mathf.Abs(Mathf.Sin(beat * 1.2f + Mathf.PI)), // Knee bend
                0,
                0
            );
    }
}