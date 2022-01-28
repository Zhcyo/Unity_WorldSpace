
using System;
using UnityEngine;

[Serializable]
public class HandMuscles
{
    public float spring = 10f;

    public float extraUpSpring;

    public float damper;

    public float squareDamper = 0.2f;

    public float maxHorizontalForce = 250f;

    public float maxVertialForce = 500f;

    public float grabSpring = 10f;

    public float grabExtraUpSpring;

    public float grabDamper;

    public float grabSquareDamper = 0.2f;

    public float grabMaxHorizontalForce = 250f;

    public float grabMaxVertialForce = 500f;

    public float maxLiftForce = 500f;

    public float maxPushForce = 200f;

    public float liftDampSqr = 0.1f;

    public float liftDamp = 0.1f;

    private readonly Human human;

    public readonly Ragdoll ragdoll;

    private readonly HumanMotion2 motion;

    public HandMuscles.TargetingMode targetingMode;

    public HandMuscles.TargetingMode grabTargetingMode = HandMuscles.TargetingMode.Ball;

    private HandMuscles.ScanMem leftMem = new HandMuscles.ScanMem();

    private HandMuscles.ScanMem rightMem = new HandMuscles.ScanMem();

    public float forwardMultiplier = 10f;

    public float armMass = 20f;

    public float bodyMass = 50f;

    public float maxForce = 300f;

    public float grabMaxForce = 450f;

    public float climbMaxForce = 800f;

    public float gravityForce = 100f;

    public float anisotrophy = 1f;

    public float maxStopForce = 150f;

    public float grabMaxStopForce = 500f;

    public float maxSpeed = 100f;

    public float onAxisAnisotrophy;

    public float offAxisAnisotrophy;

    public const float grabSnap = 0.3f;

    public const float targetHelperSnap = 0.3f;

    public const float targetHelperPull = 0.5f;

    public const float targetSnap = 0.2f;

    public const float targetPull = 0.5f;

    public const float regularSnap = 0.1f;

    public const float regularPull = 0.2f;

    private Collider[] colliders = new Collider[20];

    public HandMuscles(Human human, Ragdoll ragdoll, HumanMotion2 motion)
    {
        this.human = human;
        this.ragdoll = ragdoll;
        this.motion = motion;
    }

    private void AnimateHand(HumanSegment arm, HumanSegment forearm, HumanSegment hand, float phase, float tonus, bool right)
    {
        tonus = tonus * (50f * this.human.controls.walkSpeed);
        phase -= Mathf.Floor(phase);
        Vector3 vector3 = Quaternion.Euler(0f, this.human.controls.targetYawAngle, 0f) * Vector3.forward;
        Vector3 vector31 = Quaternion.Euler(0f, this.human.controls.targetYawAngle, 0f) * Vector3.right;
        if (!right)
        {
            vector31 = -vector31;
        }
        if (phase >= 0.5f)
        {
            HumanMotion2.AlignToVector(arm, arm.transform.up, -vector3 + (vector31 / 2f), 3f * tonus);
            HumanMotion2.AlignToVector(forearm, forearm.transform.up, vector3 + Vector3.down, 3f * tonus);
            return;
        }
        HumanMotion2.AlignToVector(arm, arm.transform.up, Vector3.down + (vector31 / 2f), 3f * tonus);
        HumanMotion2.AlignToVector(forearm, forearm.transform.up, (vector3 / 2f) - vector31, 3f * tonus);
    }

    private Vector3 FindTarget(HandMuscles.ScanMem mem, Vector3 worldPos, out Collider targetCollider)
    {
        Vector3 _point;
        RaycastHit raycastHit = new RaycastHit();
        targetCollider = null;
        Vector3 vector3 = worldPos - mem.shoulder;
        Ray ray = new Ray(mem.shoulder, vector3.normalized);
        int num = Physics.OverlapCapsuleNonAlloc(ray.origin, worldPos, 0.5f, this.colliders, this.motion.grabLayers, (QueryTriggerInteraction)1);
        for (int i = 0; i < num; i++)
        {
            Collider collider = this.colliders[i];
            TargetHelper componentInChildren = collider.GetComponentInChildren<TargetHelper>();
            if (componentInChildren != null)
            {
                Vector3 _position = componentInChildren.transform.position - worldPos;
                _point = Math3d.ProjectPointOnLineSegment(ray.origin, worldPos, componentInChildren.transform.position) - componentInChildren.transform.position;
                float _magnitude = _point.magnitude;
                if (_magnitude >= 0.3f || (componentInChildren.transform.position - mem.hand).magnitude>=0.3f)
                {
                    worldPos = worldPos + (_position * Mathf.InverseLerp(0.5f, 0.3f, _magnitude));
                }
                else
                {
                    worldPos = componentInChildren.transform.position;
                    targetCollider = collider;
                }
                return worldPos;
            }
        }
        Vector3 vector31 = mem.hand + Vector3.ClampMagnitude(worldPos - mem.hand, 0.3f);
        targetCollider = null;
        Vector3 vector32 = vector31 - mem.pos;
        Ray ray1 = new Ray(mem.pos, vector32.normalized);
        Debug.DrawRay(ray1.origin, ray1.direction * vector32.magnitude, Color.yellow, 0.2f);
        float single = float.PositiveInfinity;
        Vector3 _point1 = vector31;
        for (float j = 0.05f; j <= 0.5f; j += 0.05f)
        {
         
            if (Physics.SphereCast(ray1, j, out raycastHit, vector32.magnitude, this.motion.grabLayers,(QueryTriggerInteraction)1))
                {
                _point = vector31 - raycastHit.point;
                float _magnitude1 = _point.magnitude;
                _magnitude1 = _magnitude1 + j / 10f;
                if (raycastHit.collider.tag == "Target")
                {
                    _magnitude1 /= 100f;
                }
                else if (j > 0.2f || Vector3.Dot((worldPos - mem.shoulder).normalized, (raycastHit.point - mem.shoulder).normalized) < 0.7f)
                {
                    goto Label0;
                }
                if (_magnitude1 < single)
                {
                    single = _magnitude1;
                    _point1 = raycastHit.point;
                    targetCollider = raycastHit.collider;
                }
            }
        Label0:;
        }
        if (targetCollider != null)
        {
            Vector3 vector33 = _point1 - vector31;
            _point = Math3d.ProjectPointOnLineSegment(ray1.origin, vector31, _point1) - _point1;
            float single1 = _point.magnitude;
            if (targetCollider.tag == "Target")
            {
                if (single1 >= 0.2f || (mem.hand - _point1).magnitude >= 0.5f)
                {
                    worldPos = vector31 + (vector33 * Mathf.InverseLerp(0.5f, 0.2f, single1));
                    targetCollider = null;
                }
                else
                {
                    worldPos = _point1;
                }
            }
            else if (single1 >= 0.1f || vector33.magnitude >= 0.1f)
            {
                worldPos = vector31 + (vector33 * Mathf.InverseLerp(0.2f, 0.1f, single1));
                targetCollider = null;
            }
            else
            {
                worldPos = _point1;
            }
        }
        mem.pos = vector31;
        return worldPos;
    }

    private void LiftBody(HumanSegment hand, Rigidbody body)
    {
        Vector3 _forward;
        if (this.human.GetComponent<GroundManager>().IsStanding(body.gameObject))
        {
            return;
        }
        if (body.tag == "NoLift")
        {
            return;
        }
        float single = 0.5f + 0.5f * Mathf.InverseLerp(0f, 100f, body.mass);
       
        Vector3 vector3 = (new Vector3(this.human.targetLiftDirection.x, 0, this.human.targetLiftDirection.z)  * this.maxPushForce).SetY(Mathf.Max(0f, this.human.targetLiftDirection.y) * this.maxLiftForce);
        Vector3 _position = hand.transform.position - body.worldCenterOfMass;
        float _magnitude = _position.magnitude;
        float single1 = single;
        float single2 = 1f;
        float single3 = 1f;
        Carryable component = body.GetComponent<Carryable>();
        if (component != null)
        {
            single1 *= component.liftForceMultiplier;
            single2 = component.forceHalfDistance;
            single3 = component.damping;
            if (single2 <= 0f)
            {
                throw new InvalidOperationException("halfdistance cant be 0 or less!");
            }
        }
        float single4 = single2 / (single2 + _magnitude);
        vector3 *= single1;
        vector3 *= single4;
        body.AddForce(vector3, 0);
        hand.rigidbody.AddForce(-vector3 * 0.5f, 0);
        this.ragdoll.partChest.rigidbody.AddForce(-vector3 * 0.5f, 0);
        body.AddTorque((-body.angularVelocity * this.liftDamp) * single3, (ForceMode)5);
        _position = body.angularVelocity;
        Vector3 _normalized = -_position.normalized;
        _position = body.angularVelocity;
        body.AddTorque(((_normalized * _position.sqrMagnitude) * this.liftDampSqr) * single3, (ForceMode)5);
        if (component != null && component.aiming != CarryableAiming.None)
        {
            Vector3 vector31 = this.human.targetLiftDirection;
            if (component.limitAlignToHorizontal)
            {
                vector31.y = 0f;
                vector31.Normalize();
            }
            if (component.aiming == CarryableAiming.ForwardAxis)
            {
                _forward = body.transform.forward;
            }
            else
            {
                _position = body.worldCenterOfMass - hand.transform.position;
                _forward = _position.normalized;
            }
            Vector3 vector32 = _forward;
            float single5 = component.aimSpring;
            float single6 = (component.aimTorque < float.PositiveInfinity ? component.aimTorque : single5);
            if (component.alwaysForward)
            {
                float single7 = Vector3.Dot(vector32, vector31);
                single7 = 0.5f + single7 / 2f;
                single6 *= Mathf.Pow(single7, component.aimAnglePower);
            }
            else
            {
                float single8 = Vector3.Dot(vector32, vector31);
                if (single8 < 0f)
                {
                    vector31 = -vector31;
                    single8 = -single8;
                }
                single6 *= Mathf.Pow(single8, component.aimAnglePower);
            }
            if (component.aimDistPower != 0f)
            {
                _position = body.worldCenterOfMass - hand.transform.position;
                single6 *= Mathf.Pow(_position.magnitude, component.aimDistPower);
            }
            HumanMotion2.AlignToVector(body, vector32, vector31, single5, single6);
        }
    }

    public void OnFixedUpdate()
    {
        float single =this.human.controls.targetPitchAngle;
        float single1 =this.human.controls.targetYawAngle;
        float single2 = this.human.controls.leftExtend;
        float single3 =this.human.controls.rightExtend;
        bool flag =this.human.controls.leftGrab;
        bool flag1 = this .human.controls.rightGrab;
        bool flag2 = this.human.onGround;
        if ((this.ragdoll.partLeftHand.transform.position - this.ragdoll.partChest.transform.position).sqrMagnitude > 6f)
        {
            flag = false;
        }
        if ((this.ragdoll.partRightHand.transform.position - this.ragdoll.partChest.transform.position).sqrMagnitude > 6f)
        {
            flag1 = false;
        }
        Quaternion quaternion = Quaternion.Euler(single, single1, 0f);
        Quaternion quaternion1 = Quaternion.Euler(0f, single1, 0f);
        Vector3 _zero = Vector3.zero;
        Vector3 _position = Vector3.zero;
        float single4 = 0f;
        float single5 = 0f;
        if (single > 0f & flag2)
        {
            single5 = 0.4f * single / 90f;
        }
        HandMuscles.TargetingMode targetingMode = (this.ragdoll.partLeftHand.sensor.grabJoint != null ? this.grabTargetingMode : this.targetingMode);
        HandMuscles.TargetingMode targetingMode1 = (this.ragdoll.partRightHand.sensor.grabJoint != null ? this.grabTargetingMode : this.targetingMode);
        switch (targetingMode)
        {
            case HandMuscles.TargetingMode.Shoulder:
                {
                    _zero = this.ragdoll.partLeftArm.transform.position + (quaternion * new Vector3(0f, 0f, single2 * this.ragdoll.handLength));
                    break;
                }
            case HandMuscles.TargetingMode.Chest:
                {
                    _zero = (this.ragdoll.partChest.transform.position + (quaternion1 * new Vector3(-0.2f, 0.15f, 0f))) + (quaternion * new Vector3(0f, 0f, single2 * this.ragdoll.handLength));
                    break;
                }
            case HandMuscles.TargetingMode.Hips:
                {
                    if (single > 0f)
                    {
                        single4 = -0.3f * single / 90f;
                    }
                    _zero = (this.ragdoll.partHips.transform.position + (quaternion1 * new Vector3(-0.2f, 0.65f + single4, single5))) + (quaternion * new Vector3(0f, 0f, single2 * this.ragdoll.handLength));
                    break;
                }
            case HandMuscles.TargetingMode.Ball:
                {
                    if (single > 0f)
                    {
                        single4 = -0.2f * single / 90f;
                    }
                    if (this.ragdoll.partLeftHand.sensor.grabJoint != null)
                    {
                        single5 = (this.human.isClimbing ? -0.2f : 0f);
                    }
                    _zero = (this.ragdoll.partBall.transform.position+ (quaternion1 * new Vector3(-0.2f, 0.7f + single4, single5))) + (quaternion * new Vector3(0f, 0f, single2 * this.ragdoll.handLength));
                    break;
                }
        }
        switch (targetingMode1)
        {
            case HandMuscles.TargetingMode.Shoulder:
                {
                    _position = this.ragdoll.partRightArm.transform.position + (quaternion * new Vector3(0f, 0f, single3 * this.ragdoll.handLength));
                    break;
                }
            case HandMuscles.TargetingMode.Chest:
                {
                    _position = (this.ragdoll.partChest.transform.position + (quaternion1 * new Vector3(0.2f, 0.15f, 0f))) + (quaternion * new Vector3(0f, 0f, single3 * this.ragdoll.handLength));
                    break;
                }
            case HandMuscles.TargetingMode.Hips:
                {
                    if (single > 0f)
                    {
                        single4 = -0.3f * single / 90f;
                    }
                    _position = (this.ragdoll.partHips.transform.position + (quaternion1 * new Vector3(0.2f, 0.65f + single4, single5))) + (quaternion * new Vector3(0f, 0f, single3 * this.ragdoll.handLength));
                    break;
                }
            case HandMuscles.TargetingMode.Ball:
                {
                    if (single > 0f)
                    {
                        single4 = -0.2f * single / 90f;
                    }
                    if (this.ragdoll.partRightHand.sensor.grabJoint != null)
                    {
                        single5 = (this.human.isClimbing ? -0.2f : 0f);
                    }
                    _position = (this.ragdoll.partBall.transform.position + (quaternion1 * new Vector3(0.2f, 0.7f + single4, single5))) + (quaternion * new Vector3(0f, 0f, single3 * this.ragdoll.handLength));
                    break;
                }
        }
        this.ProcessHand(this.leftMem, this.ragdoll.partLeftArm, this.ragdoll.partLeftForearm, this.ragdoll.partLeftHand, _zero, single2, flag, this.motion.legs.legPhase + 0.5f, false);
        this.ProcessHand(this.rightMem, this.ragdoll.partRightArm, this.ragdoll.partRightForearm, this.ragdoll.partRightHand, _position, single3, flag1, this.motion.legs.legPhase, true);
    }

    private void PlaceHand(HumanSegment arm, HumanSegment hand, Vector3 worldPos, bool active, bool grabbed, Rigidbody grabbedBody)
    {
        if (!active)
        {
            return;
        }
        Rigidbody rigidbody = hand.rigidbody;
        Vector3 vector3 = worldPos - rigidbody.worldCenterOfMass;
        Vector3 vector31 = new Vector3(0f, vector3.y, 0f);
        Vector3 _velocity = rigidbody.velocity - this.ragdoll.partBall.rigidbody.velocity;
        float single = this.armMass;
        float single1 = this.maxForce;
        if (grabbed)
        {
            if (grabbedBody == null)
            {
                single += this.bodyMass;
                single1 = Mathf.Lerp(this.grabMaxForce, this.climbMaxForce, (this.human.controls.targetPitchAngle - 50f) / 30f);
            }
            else
            {
                single += Mathf.Clamp(grabbedBody.mass / 2f, 0f, this.bodyMass);
                single1 = Mathf.Lerp(this.grabMaxForce, this.climbMaxForce, (this.human.controls.targetPitchAngle - 50f) / 30f);
            }
        }
        float single2 = single1 / single;
        int num = 600;
        Vector3 vector32 = (ConstantAccelerationControl.Solve(vector3, _velocity, single2, 0.1f) * single) + (Vector3.up * this.gravityForce);
        if (this.human.grabbedByHuman != null && this.human.grabbedByHuman.state == HumanState.Climb)
        {
            vector32 *= 1.7f;
            num *= 2;
        }
        if (!grabbed)
        {
            rigidbody.AddForce(vector32, 0);
            this.ragdoll.partHips.rigidbody.AddForce(-vector32, 0);
            return;
        }
        Vector3 vector33 = new Vector3(this.human.targetDirection.x, 0, this.human.targetDirection.z);// this.human.targetDirection.ZeroY();
        Vector3 _normalized = vector33.normalized;
        Vector3 vector34 = Mathf.Min(0f, Vector3.Dot(_normalized, vector32)) * _normalized;
        Vector3 vector35 = vector32 - vector34;
        Vector3 vector36 = new Vector3(0, vector32.y, 0);
        Vector3 vector37 = -vector32 * 0.25f;
        Vector3 vector38 = -vector32 * 0.75f;
        Vector3 vector39 = ((-vector32 * 0.1f) - (vector36 * 0.5f)) - (vector35 * 0.25f);
        Vector3 vector310 = (-vector36 * 0.2f) - (vector35 * 0.4f);
        if (grabbedBody != null)
        {
            Carryable component = grabbedBody.GetComponent<Carryable>();
            if (component != null)
            {
                vector37 *= component.handForceMultiplier;
                vector38 *= component.handForceMultiplier;
            }
        }
        float single3 = (this.human.state == HumanState.Climb ? Mathf.Clamp01((this.human.controls.targetPitchAngle - 10f) / 60f) : 1f);
        Vector3 vector311 = Vector3.Lerp(vector39, vector37, vector3.y + 0.5f) * single3;
        Vector3 vector312 = Vector3.Lerp(vector310, vector38, vector3.y + 0.5f) * single3;
        float single4 = Mathf.Abs(vector311.y + vector312.y);
        if (single4 > (float)num)
        {
            vector311 = vector311 * ((float)num / single4);
            vector312 = vector312 * ((float)num / single4);
        }
        this.ragdoll.partChest.rigidbody.AddForce(vector311, 0);
        this.ragdoll.partBall.rigidbody.AddForce(vector312, 0);
        rigidbody.AddForce(-vector311 - vector312, 0);
    }

    private void ProcessHand(HandMuscles.ScanMem mem, HumanSegment arm, HumanSegment forearm, HumanSegment hand, Vector3 worldPos, float extend, bool grab, float animationPhase, bool right)
    {
        double num = 0.1 + (double)(0.14f * Mathf.Abs(this.human.controls.targetPitchAngle - mem.grabAngle) / 80f);
        double num1 = num * 2;
        //if (CheatCodes.climbCheat)
        //{
        //    double num2 = num / 4;
        //    num = num2;
        //    num1 = num2;
        //}
        if (grab && !hand.sensor.grab)
        {
            if ((double)mem.grabTime <= num)
            {
                grab = false;
            }
            else
            {
                mem.pos = arm.transform.position;
            }
        }
        if (!hand.sensor.grab || grab)
        {
            mem.grabTime += Time.fixedDeltaTime;
        }
        else
        {
            mem.grabTime = 0f;
            mem.grabAngle = this.human.controls.targetPitchAngle;
        }
        hand.sensor.grab = (double)mem.grabTime > num1 & grab;
        if (extend <= 0.2f)
        {
            hand.sensor.grabFilter = null;
            if (this.human.state == HumanState.Walk)
            {
                this.AnimateHand(arm, forearm, hand, animationPhase, 1f, right);
                return;
            }
            if (this.human.state == HumanState.FreeFall)
            {
                Vector3 vector3 = this.human.targetDirection;
                vector3.y = 0f;
                HumanMotion2.AlignToVector(arm, arm.transform.up, -vector3, 2f);
                HumanMotion2.AlignToVector(forearm, forearm.transform.up, vector3, 2f);
                return;
            }
            Vector3 vector31 = this.human.targetDirection;
            vector31.y = 0f;
            HumanMotion2.AlignToVector(arm, arm.transform.up, -vector31, 20f);
            HumanMotion2.AlignToVector(forearm, forearm.transform.up, vector31, 20f);
            return;
        }
        hand.sensor.targetPosition = worldPos;
        mem.shoulder = arm.transform.position;
        mem.hand = hand.transform.position;
        if (hand.sensor.grabJoint == null)
        {
            worldPos = this.FindTarget(mem, worldPos, out hand.sensor.grabFilter);
        }
        this.PlaceHand(arm, hand, worldPos, true, hand.sensor.grabJoint != null, hand.sensor.grabBody);
        if (hand.sensor.grabBody != null)
        {
            this.LiftBody(hand, hand.sensor.grabBody);
        }
        hand.sensor.grabPosition = worldPos;
    }

    private class ScanMem
    {
        public Vector3 pos;

        public Vector3 shoulder;

        public Vector3 hand;

        public float grabTime;

        public float grabAngle;

        public ScanMem()
        {
        }
    }

    public enum TargetingMode
    {
        Shoulder,
        Chest,
        Hips,
        Ball
    }
}