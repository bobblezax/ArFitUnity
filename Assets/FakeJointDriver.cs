using UnityEngine;

public class FakeJointDriver : MonoBehaviour
{
    public Animator animator;

    private Transform hips, spine, head, leftUpperArm, rightUpperArm, leftUpperLeg, rightUpperLeg;

    private Quaternion hipsBindRot, spineBindRot, headBindRot, leftArmBindRot, rightArmBindRot, leftLegBindRot, rightLegBindRot;

    private Vector3 hipsInitialPos;

    void Start()
    {
        hips = animator.GetBoneTransform(HumanBodyBones.Hips);
        spine = animator.GetBoneTransform(HumanBodyBones.Spine);
        head = animator.GetBoneTransform(HumanBodyBones.Head);
        leftUpperArm = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        rightUpperArm = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
        leftUpperLeg = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
        rightUpperLeg = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);

        // Store bind pose rotations
        if (hips) hipsBindRot = hips.localRotation;
        if (spine) spineBindRot = spine.localRotation;
        if (head) headBindRot = head.localRotation;
        if (leftUpperArm) leftArmBindRot = leftUpperArm.localRotation;
        if (rightUpperArm) rightArmBindRot = rightUpperArm.localRotation;
        if (leftUpperLeg) leftLegBindRot = leftUpperLeg.localRotation;
        if (rightUpperLeg) rightLegBindRot = rightUpperLeg.localRotation;

        if (hips) hipsInitialPos = hips.position;
    }

    void Update()
    {
        if (hips == null) return;

        // Bob hips up/down
        hips.position = hipsInitialPos + Vector3.up * Mathf.Sin(Time.time * 2f) * 0.05f;

        if (spine)
            spine.localRotation = spineBindRot * Quaternion.Euler(
                0,
                20f * Mathf.Sin(Time.time * 1.2f),
                0
            );

        if (head)
            head.localRotation = headBindRot * Quaternion.Euler(
                15f * Mathf.Sin(Time.time * 1.5f),
                0,
                0
            );

        if (leftUpperArm)
            leftUpperArm.localRotation = leftArmBindRot * Quaternion.Euler(
                30f * Mathf.Sin(Time.time * 2f),
                0,
                0
            );

        if (rightUpperArm)
            rightUpperArm.localRotation = rightArmBindRot * Quaternion.Euler(
                30f * Mathf.Sin(Time.time * 2f + Mathf.PI),
                0,
                0
            );

        if (leftUpperLeg)
            leftUpperLeg.localRotation = leftLegBindRot * Quaternion.Euler(
                25f * Mathf.Sin(Time.time * 2f + Mathf.PI),
                0,
                0
            );

        if (rightUpperLeg)
            rightUpperLeg.localRotation = rightLegBindRot * Quaternion.Euler(
                25f * Mathf.Sin(Time.time * 2f),
                0,
                0
            );
    }
}
