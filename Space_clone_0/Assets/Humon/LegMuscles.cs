using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LegMuscles
{

    private readonly Human human;

    private readonly Ragdoll ragdoll;

    private readonly HumanMotion2 motion;

    private PhysicMaterial ballMaterial;

    private PhysicMaterial footMaterial;

    private float ballFriction;

    private float footFriction;

    private float ballRadius;

    private float stepToAlignOverrideDuration = 0.5f;

    private float stepToAlignOverride;

    public float legPhase;

    private float forwardImpulse;

    private float upImpulse;

    private int framesToApplyJumpImpulse;

    public LegMuscles(Human human, Ragdoll ragdoll, HumanMotion2 motion)
    {
        this.human = human;
        this.ragdoll = ragdoll;
        this.motion = motion;
        this.ballRadius = (ragdoll.partBall.collider as SphereCollider).radius;
        this.ballMaterial = ragdoll.partBall.collider.material;
        this.footMaterial = ragdoll.partLeftFoot.collider.material;
        this.ballFriction = this.ballMaterial.staticFriction;
        this.footFriction = this.footMaterial.staticFriction;
    }

    private void AddWalkForce()
    {
        Vector3 vector3 = this.human.controls.walkDirection * 300f;
        this.ragdoll.partBall.rigidbody.AddForce(vector3, 0);
        if (this.human.onGround)
        {
            this.human.groundManager.DistributeForce(-vector3, this.ragdoll.partBall.rigidbody.position);
            return;
        }
        if (this.human.hasGrabbed)
        {
            this.human.grabManager.DistributeForce(-vector3 * 0.5f);
        }
    }

    private Vector3 AnimateLeg(HumanSegment thigh, HumanSegment leg, HumanSegment foot, float phase, Vector3 torsoFeedback, float tonus)
    {
        tonus *= 20f;
        phase -= Mathf.Floor(phase);
        if (phase < 0.2f)
        {
            HumanMotion2.AlignToVector(thigh, thigh.transform.up, this.human.controls.walkDirection + Vector3.down, 3f * tonus);
            HumanMotion2.AlignToVector(leg, thigh.transform.up, -this.human.controls.walkDirection - Vector3.up, tonus);
            Vector3 _up = Vector3.up * 20f;
            foot.rigidbody.AddForce(_up, 0);
            return -_up;
        }
        if (phase >= 0.5f)
        {
            if (phase < 0.7f)
            {
                Vector3 vector3 = torsoFeedback * 0.2f;
                foot.rigidbody.AddForce(vector3, 0);
                HumanMotion2.AlignToVector(thigh, thigh.transform.up, this.human.controls.walkDirection + Vector3.down, tonus);
                HumanMotion2.AlignToVector(leg, thigh.transform.up, Vector3.down, tonus);
                return -vector3;
            }
            if (phase < 0.9f)
            {
                Vector3 vector31 = torsoFeedback * 0.2f;
                foot.rigidbody.AddForce(vector31, 0);
                HumanMotion2.AlignToVector(thigh, thigh.transform.up, -this.human.controls.walkDirection + Vector3.down, tonus);
                HumanMotion2.AlignToVector(leg, thigh.transform.up, -this.human.controls.walkDirection + Vector3.down, tonus);
                return -vector31;
            }
            HumanMotion2.AlignToVector(thigh, thigh.transform.up, -this.human.controls.walkDirection + Vector3.down, tonus);
            HumanMotion2.AlignToVector(leg, thigh.transform.up, -this.human.controls.walkDirection, tonus);
        }
        else
        {
            HumanMotion2.AlignToVector(thigh, thigh.transform.up, this.human.controls.walkDirection, 2f * tonus);
            HumanMotion2.AlignToVector(leg, thigh.transform.up, this.human.controls.walkDirection, 3f * tonus);
        }
        return Vector3.zero;
    }

    private void ApplyJumpImpulses()
    {
        float single = 1f;
        for (int i = 0; i < this.human.groundManager.groundObjects.Count; i++)
        {
            if (this.human.grabManager.IsGrabbed(this.human.groundManager.groundObjects[i]))
            {
                single = 0.75f;
            }
        }
        Vector3 _up = (Vector3.up * this.upImpulse) * single;
        Vector3 _normalized = (this.human.controls.walkDirection.normalized * this.forwardImpulse) * single;
        this.ragdoll.partHead.rigidbody.AddForce((_up * 0.1f) + (_normalized * 0.1f), (ForceMode)1);
        this.ragdoll.partChest.rigidbody.AddForce((_up * 0.1f) + (_normalized * 0.1f), (ForceMode)1);
        this.ragdoll.partWaist.rigidbody.AddForce((_up * 0.1f) + (_normalized * 0.1f), (ForceMode)1);
        this.ragdoll.partHips.rigidbody.AddForce((_up * 0.1f) + (_normalized * 0.1f), (ForceMode)1);
        this.ragdoll.partBall.rigidbody.AddForce((_up * 0.1f) + (_normalized * 0.1f), (ForceMode)1);
        this.ragdoll.partLeftThigh.rigidbody.AddForce((_up * 0.05f) + (_normalized * 0.05f), (ForceMode)1);
        this.ragdoll.partRightThigh.rigidbody.AddForce((_up * 0.05f) + (_normalized * 0.05f), (ForceMode)1);
        this.ragdoll.partLeftLeg.rigidbody.AddForce((_up * 0.05f) + (_normalized * 0.05f), (ForceMode)1);
        this.ragdoll.partRightLeg.rigidbody.AddForce((_up * 0.05f) + (_normalized * 0.05f), (ForceMode)1);
        this.ragdoll.partLeftFoot.rigidbody.AddForce((_up * 0.05f) + (_normalized * 0.05f), (ForceMode)1);
        this.ragdoll.partRightFoot.rigidbody.AddForce((_up * 0.05f) + (_normalized * 0.05f), (ForceMode)1);
        this.ragdoll.partLeftArm.rigidbody.AddForce((_up * 0.05f) + (_normalized * 0.05f), (ForceMode)1);
        this.ragdoll.partRightArm.rigidbody.AddForce((_up * 0.05f) + (_normalized * 0.05f), (ForceMode)1);
        this.ragdoll.partLeftForearm.rigidbody.AddForce((_up * 0.05f) + (_normalized * 0.05f), (ForceMode)1);
        this.ragdoll.partRightForearm.rigidbody.AddForce((_up * 0.05f) + (_normalized * 0.05f), (ForceMode)1);
        this.human.groundManager.DistributeForce(-_up / Time.fixedDeltaTime, this.ragdoll.partBall.rigidbody.position);
    }

    private void JumpAnimation(Vector3 torsoFeedback)
    {
        this.ragdoll.partHips.rigidbody.AddForce(torsoFeedback, 0);
        if (!this.human.jump)
        {
            int num = this.framesToApplyJumpImpulse;
            this.framesToApplyJumpImpulse = num - 1;
            if (num > 0)
            {
                this.ApplyJumpImpulses();
            }
            int num1 = 3;
            int num2 = 500;
            float single = this.human.controls.unsmoothedWalkSpeed * (float)num1 * this.human.mass;
            Vector3 vector3 = this.human.momentum;
            float single1 = Vector3.Dot(this.human.controls.walkDirection.normalized, vector3);
            float single2 = Mathf.Clamp((single - single1) / Time.fixedDeltaTime, 0f, (float)num2);
            this.ragdoll.partChest.rigidbody.AddForce(single2 * this.human.controls.walkDirection.normalized, 0);
            return;
        }
        float single3 = 0.75f;
        int num3 = 2;
        Vector3 _gravity = Physics.gravity;
        float single4 = Mathf.Sqrt(2f * single3 / _gravity.magnitude);
        float single5 = Mathf.Clamp(this.human.groundManager.groudSpeed.y, 0f, 100f);
        single5 = Mathf.Pow(single5, 1.2f);
        _gravity = Physics.gravity;
        float _magnitude = (single4 + single5 / _gravity.magnitude) * this.human.weight;
        float single6 = this.human.controls.unsmoothedWalkSpeed * ((float)num3 + single5 / 2f) * this.human.mass;
        Vector3 vector31 = this.human.momentum;
        float single7 = Vector3.Dot(this.human.controls.walkDirection.normalized, vector31);
        if (single7 < 0f)
        {
            single7 = 0f;
        }
        this.upImpulse = _magnitude - vector31.y;
        if (this.upImpulse < 0f)
        {
            this.upImpulse = 0f;
        }
        this.forwardImpulse = single6 - single7;
        if (this.forwardImpulse < 0f)
        {
            this.forwardImpulse = 0f;
        }
        this.framesToApplyJumpImpulse = 1;
        if (this.human.onGround || Time.time - this.human.GetComponent<Ball>().timeSinceLastNonzeroImpulse < 0.2f)
        {
            this.upImpulse /= (float)this.framesToApplyJumpImpulse;
            this.forwardImpulse /= (float)this.framesToApplyJumpImpulse;
            this.ApplyJumpImpulses();
            this.framesToApplyJumpImpulse--;
        }
        this.human.skipLimiting = true;
        this.human.jump = false;
    }

    private void NoAnimation(Vector3 torsoFeedback)
    {
        if (this.human.state == HumanState.Fall && this.human.grabbedByHuman != null)
        {
            return;
        }
        this.ragdoll.partHips.rigidbody.AddForce(torsoFeedback, 0);
    }

    public void OnFixedUpdate(Vector3 torsoFeedback)
    {
        float single;
        this.stepToAlignOverride -= Time.fixedDeltaTime;
        float single1 = (this.human.state == HumanState.Slide ? 0f : this.ballFriction);
        if (single1 != this.ballMaterial.staticFriction)
        {
            PhysicMaterial physicMaterial = this.ballMaterial;
            float single2 = single1;
            float single3 = single2;
            this.ballMaterial.dynamicFriction = (single2);
            physicMaterial.staticFriction = (single3);
            PhysicMaterial physicMaterial1 = this.footMaterial;
            PhysicMaterial physicMaterial2 = this.footMaterial;
            single = (this.human.state == HumanState.Slide ? 0f : this.footFriction);
            single3 = single;
            physicMaterial2.dynamicFriction = (single);
            physicMaterial1.staticFriction = (single3);
            this.ragdoll.partBall.collider.sharedMaterial = (this.ballMaterial);
            this.ragdoll.partLeftFoot.collider.sharedMaterial = (this.footMaterial);
            this.ragdoll.partRightFoot.collider.sharedMaterial = (this.footMaterial);
        }
        switch (this.human.state)
        {
            case HumanState.Idle:
                {
                  
                    if (Vector2.Angle((new Vector2(0f, 1f)).Rotate(-this.human.controls.targetYawAngle * 0.0174532924f), this.ragdoll.partHips.transform.forward.To2D()) > (float)((this.human.controls.leftGrab || this.human.controls.rightGrab ? 75 : 90)))
                    {
                        this.stepToAlignOverride = this.stepToAlignOverrideDuration;
                    }
                    if (this.stepToAlignOverride > 0f)
                    {
                        this.RunAnimation(torsoFeedback, 0.5f);
                        return;
                    }
                    this.StandAnimation(torsoFeedback, 1f);
                    return;
                }
            case HumanState.Walk:
                {
                    this.RunAnimation(torsoFeedback, 1f);
                    return;
                }
            case HumanState.Climb:
                {
                    if (this.human.controls.walkSpeed > 0f)
                    {
                        this.RunAnimation(torsoFeedback, 0f);
                        return;
                    }
                    this.StandAnimation(torsoFeedback, 0f);
                    return;
                }
            case HumanState.Jump:
                {
                    this.JumpAnimation(torsoFeedback);
                    return;
                }
            case HumanState.Slide:
                {
                    this.StandAnimation(torsoFeedback, 1f);
                    return;
                }
            case HumanState.Fall:
                {
                    this.NoAnimation(torsoFeedback);
                    return;
                }
            case HumanState.FreeFall:
                {
                    this.NoAnimation(torsoFeedback);
                    return;
                }
            case HumanState.Unconscious:
                {
                    this.NoAnimation(torsoFeedback);
                    return;
                }
            case HumanState.Dead:
                {
                    this.NoAnimation(torsoFeedback);
                    return;
                }
            case HumanState.Spawning:
                {
                    this.NoAnimation(torsoFeedback);
                    return;
                }
            default:
                {
                    return;
                }
        }
    }
   

    private void RotateBall()
    {
        float single = (this.human.state == HumanState.Walk ? 2.5f : 1.2f);
        Vector3 vector3 = new Vector3(this.human.controls.walkDirection.z, 0f, -this.human.controls.walkDirection.x);
        this.ragdoll.partBall.rigidbody.angularVelocity = (single / this.ballRadius * vector3);
        Rigidbody rigidbody = this.ragdoll.partBall.rigidbody;
        Vector3 _angularVelocity = this.ragdoll.partBall.rigidbody.angularVelocity;
        rigidbody.maxAngularVelocity = (_angularVelocity.magnitude);
    }

    private void RunAnimation(Vector3 torsoFeedback, float tonus)
    {
        this.legPhase = Time.realtimeSinceStartup * 1.5f;
        torsoFeedback += this.AnimateLeg(this.ragdoll.partLeftThigh, this.ragdoll.partLeftLeg, this.ragdoll.partLeftFoot, this.legPhase, torsoFeedback, tonus);
        torsoFeedback += this.AnimateLeg(this.ragdoll.partRightThigh, this.ragdoll.partRightLeg, this.ragdoll.partRightFoot, this.legPhase + 0.5f, torsoFeedback, tonus);
        this.ragdoll.partBall.rigidbody.AddForce(torsoFeedback, 0);
        this.RotateBall();
        this.AddWalkForce();
    }

    private void StandAnimation(Vector3 torsoFeedback, float tonus)
    {
        HumanMotion2.AlignToVector(this.ragdoll.partLeftThigh, -this.ragdoll.partLeftThigh.transform.up, Vector3.up, 10f * tonus);
        HumanMotion2.AlignToVector(this.ragdoll.partRightThigh, -this.ragdoll.partRightThigh.transform.up, Vector3.up, 10f * tonus);
        HumanMotion2.AlignToVector(this.ragdoll.partLeftLeg, -this.ragdoll.partLeftLeg.transform.up, Vector3.up, 10f * tonus);
        HumanMotion2.AlignToVector(this.ragdoll.partRightLeg, -this.ragdoll.partRightLeg.transform.up, Vector3.up, 10f * tonus);
        this.ragdoll.partBall.rigidbody.AddForce(torsoFeedback * 0.2f, 0);
        this.ragdoll.partLeftFoot.rigidbody.AddForce(torsoFeedback * 0.4f, 0);
        this.ragdoll.partRightFoot.rigidbody.AddForce(torsoFeedback * 0.4f, 0);
        this.ragdoll.partBall.rigidbody.angularVelocity = (Vector3.zero);
    }
}