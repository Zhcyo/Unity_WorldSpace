using System;
using UnityEngine;

[Serializable]
public class TorsoMuscles
{
    private readonly Human human;

    private readonly Ragdoll ragdoll;

    private readonly HumanMotion2 motion;

    public Vector3 feedbackForce;

    private float timeSinceUnconsious;

    private float timeSinceOffGround;

    private float idleAnimationPhase;

    private float idleAnimationDuration = 3f;

    public float predictTime = 0.001f;

    public float springSpeed = 0.01f;

    public float m = 1f;

    public float headOffset = 0.5f;

    public float headApparentMass = 1f;

    public float headMaxForceY = 40f;

    public float headMaxForceZ = 30f;

    public float chestOffset = 0.5f;

    public float chestApparentMass = 5f;

    public float chestMaxForceY = 20f;

    public float chestMaxForceZ = 20f;

    public float waistOffset = 0.5f;

    public float waistApparentMass = 10f;

    public float waistMaxForceY = 30f;

    public float waistMaxForceZ = 30f;

    public float hipsOffset = 0.5f;

    public float hipsApparentMass = 2f;

    public float hipsMaxForceY = 50f;

    public float hipsMaxForceZ = 50f;

    public float chestAngle = 1f;

    public float waistAngle = 0.7f;

    public float hipsAngle = 0.4f;

    public TorsoMuscles(Human human, Ragdoll ragdoll, HumanMotion2 motion)
    {
        this.human = human;
        this.ragdoll = ragdoll;
        this.motion = motion;
    }

    public Vector3 ApplyTorsoPose(float torsoTonus, float headTonus, float torsoBend, float lift)
    {
        lift = lift * (Mathf.Clamp01(this.timeSinceUnconsious / 3f) * Mathf.Clamp01(this.timeSinceOffGround * 0.2f + 0.8f));
        torsoTonus *= Mathf.Clamp01(this.timeSinceUnconsious);
        torsoTonus *= 2f;
        headTonus *= 2f;
        float single = this.human.weight * 0.8f * lift;
        float single1 = this.human.controls.targetPitchAngle;
        if (this.human.hasGrabbed)
        {
            single1 = (single1 + 80f) * 0.5f - 80f;
        }
        HumanMotion2.AlignLook(this.ragdoll.partHead, Quaternion.Euler(single1, this.human.controls.targetYawAngle, 0f), 2f * headTonus, 10f * headTonus);
        if (this.human.onGround || this.human.state == HumanState.Climb)
        {
            torsoBend *= 40f;
            HumanMotion2.AlignLook(this.ragdoll.partChest, Quaternion.Euler(this.human.controls.targetPitchAngle + torsoBend, this.human.controls.targetYawAngle, 0f), 2f * torsoTonus, 10f * torsoTonus);
            HumanMotion2.AlignLook(this.ragdoll.partWaist, Quaternion.Euler(this.human.controls.targetPitchAngle + torsoBend / 2f, this.human.controls.targetYawAngle, 0f), 1f * torsoTonus, 15f * torsoTonus);
            HumanMotion2.AlignLook(this.ragdoll.partHips, Quaternion.Euler(this.human.controls.targetPitchAngle, this.human.controls.targetYawAngle, 0f), 0.5f * torsoTonus, 20f * torsoTonus);
        }
        float single2 = 0f;
        if (this.human.targetDirection.y <= 0f)
        {
            single2 = -this.human.targetDirection.y;
        }
        else
        {
            single2 = this.human.targetDirection.y * 0.25f;
            if (this.human.onGround && this.human.ragdoll.partLeftHand.sensor.grabBody != null)
            {
                single2 *= 1.5f;
            }
            if (this.human.onGround && this.human.ragdoll.partRightHand.sensor.grabBody != null)
            {
                single2 *= 1.5f;
            }
        }
        Vector3 vector3 = Mathf.Lerp(0.2f, 0f, single2) * single * headTonus * Vector3.up;
        Vector3 vector31 = Mathf.Lerp(0.6f, 0f, single2) * single * torsoTonus * Vector3.up;
        Vector3 vector32 = Mathf.Lerp(0.2f, 0.5f, single2) * single * torsoTonus * Vector3.up;
        Vector3 vector33 = Mathf.Lerp(0f, 0.5f, single2) * single * torsoTonus * Vector3.up;
        if (this.human.controls.leftGrab)
        {
            this.UnblockArmBehindTheBack(this.ragdoll.partLeftHand, -1f);
        }
        if (this.human.controls.rightGrab)
        {
            this.UnblockArmBehindTheBack(this.ragdoll.partRightHand, 1f);
        }
        this.ragdoll.partHead.rigidbody.AddForce(vector3, 0);
        this.ragdoll.partChest.rigidbody.AddForce(vector31, 0);
        this.ragdoll.partWaist.rigidbody.AddForce(vector32, 0);
        this.ragdoll.partHips.rigidbody.AddForce(vector33, 0);
        this.StabilizeHorizontal(this.ragdoll.partHips.rigidbody, this.ragdoll.partBall.rigidbody, 1f * lift * Mathf.Lerp(1f, 0.25f, Mathf.Abs(single2)));
        this.StabilizeHorizontal(this.ragdoll.partHead.rigidbody, this.ragdoll.partBall.rigidbody, 0.2f * lift * Mathf.Lerp(1f, 0f, Mathf.Abs(single2)));
        return -(((vector3 + vector31) + vector32) + vector33);
    }

    private Vector3 ClimbAnimation()
    {
        float single = Mathf.Clamp01((this.human.controls.targetPitchAngle - 10f) / 60f);
        int num = ((this.ragdoll.partLeftHand.sensor.grabJoint != null || this.human.controls.leftGrab) && (this.ragdoll.partRightHand.sensor.grabJoint != null || this.human.controls.rightGrab) ? 1 : 0);
        return this.ApplyTorsoPose((float)num, 1f, 0f, Mathf.Lerp(0.2f, 1f, single));
    }

    private Vector3 FallAnimation()
    {
        return this.ApplyTorsoPose(0.5f, 1f, 0f, 0.3f);
    }

    private Vector3 FreeFallAnimation()
    {
        this.human.AddRandomTorque(0.01f);
        float single = Mathf.Sin(Time.time * 3f) * 0.1f;
        float single1 = this.human.weight;
        this.ragdoll.partHips.rigidbody.AddForce((-Vector3.up * single1) * single, 0);
        this.ragdoll.partChest.rigidbody.AddForce((-Vector3.up * single1) * -single, 0);
        return Vector3.zero;
    }

    private Vector3 IdleAnimation()
    {
        this.idleAnimationPhase = MathUtils.Wrap(this.idleAnimationPhase + Time.deltaTime  / this.idleAnimationDuration, 1f);
        float single = Mathf.Lerp(1f, -0.5f, Mathf.Sin(this.idleAnimationPhase * 3.14159274f * 2f) / 2f + 0.5f);
        return this.ApplyTorsoPose(1f, 1f, single, 1f);
    }

    private Vector3 JumpAnimation()
    {
        return this.ApplyTorsoPose(1f, 1f, 0f, 1f);
    }

    public void OnFixedUpdate()
    {
        this.timeSinceUnconsious += Time.fixedDeltaTime;
        this.timeSinceOffGround += Time.fixedDeltaTime;
        if (!this.human.onGround)
        {
            this.timeSinceOffGround = 0f;
        }
        this.feedbackForce = Vector3.zero;
        HumanState humanState = this.human.state;
        if (humanState == HumanState.Fall && this.human.grabbedByHuman != null)
        {
            humanState = HumanState.Idle;
        }
        switch (humanState)
        {
            case HumanState.Idle:
            case HumanState.Slide:
                {
                    this.feedbackForce = this.IdleAnimation();
                    return;
                }
            case HumanState.Walk:
                {
                    this.feedbackForce = this.StandAnimation();
                    return;
                }
            case HumanState.Climb:
                {
                    this.feedbackForce = this.ClimbAnimation();
                    return;
                }
            case HumanState.Jump:
                {
                    this.feedbackForce = this.JumpAnimation();
                    return;
                }
            case HumanState.Fall:
                {
                    this.feedbackForce = this.FallAnimation();
                    return;
                }
            case HumanState.FreeFall:
                {
                    this.feedbackForce = this.FreeFallAnimation();
                    return;
                }
            case HumanState.Unconscious:
                {
                    this.timeSinceUnconsious = 0f;
                    return;
                }
            case HumanState.Dead:
            case HumanState.Spawning:
                {
                    return;
                }
            default:
                {
                    return;
                }
        }
    }

    private void StabilizeHorizontal(Rigidbody top, Rigidbody bottom, float multiplier)
    {
        float single = 3f;
        Vector3 _position = ((bottom.position+ (bottom.velocity * Time.fixedDeltaTime)) - top.position) - ((top.velocity * Time.fixedDeltaTime) * single);
        _position.y = 0f;
        Vector3 _mass = (_position * top.mass) / Time.fixedDeltaTime;
        _mass *= multiplier;
        top.AddForce(_mass, 0);
        bottom.AddForce(-_mass, 0);
    }

    private Vector3 StandAnimation()
    {
        return this.ApplyTorsoPose(1f, 1f, 0f, 1f);
    }

    private void UnblockArmBehindTheBack(HumanSegment hand, float direction)
    {
        Bounds _bounds = this.ragdoll.partHead.collider.bounds;
        Vector3 _center = _bounds.center;
        Vector3 vector3 = hand.collider.bounds.center;
        Vector3 vector31 = this.ragdoll.partChest.transform.InverseTransformVector(vector3 - _center);
        float single = Mathf.InverseLerp(0f, -0.1f, vector31.z) * Mathf.InverseLerp(0.1f, -0.1f, vector31.x * direction) * Mathf.InverseLerp(-0.3f, -0.1f, vector31.y);
        if (single > 0f)
        {
            this.ragdoll.partHead.rigidbody.AddForce((-direction * single * this.ragdoll.partChest.transform.right) * 200f, 0);
            hand.rigidbody.AddForce((direction * single * this.ragdoll.partChest.transform.right) * 200f, 0);
        }
    }
}