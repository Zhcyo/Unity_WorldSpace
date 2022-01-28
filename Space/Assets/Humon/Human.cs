
using System;
using System.Collections.Generic;
using UnityEngine;

public class Human : HumanBase
{
    public static Human instance;

    public static List<Human> all;
    public bool spawning;

    public Vector3 targetDirection;

    public Vector3 targetLiftDirection;

    public bool jump;

    public bool disableInput;

   public Ragdoll ragdoll;

    public HumanControls controls;

    internal GroundManager groundManager;

    internal GrabManager grabManager;

    [NonSerialized]
    public HumanMotion2 motionControl2;

    public HumanState state;

    public bool onGround;

    public float groundAngle;

    public bool hasGrabbed;

    public bool isClimbing;

    public Human grabbedByHuman;

    public float wakeUpTime;

    internal float maxWakeUpTime = 2f;

    public float unconsciousTime;

    private float maxUnconsciousTime = 3f;

    private Vector3 grabStartPosition;

    [NonSerialized]
    public Rigidbody[] rigidbodies;

    private Vector3[] velocities;

    public float weight;

    public float mass;

    private Vector3 lastVelocity;

    private float totalHit;

    private float lastFrameHit;

    private float thisFrameHit;

    private float fallTimer;

    private float groundDelay;

    private float jumpDelay;

    private float slideTimer;

    private float[] groundAngles = new float[60];

    private int groundAnglesIdx;

    private float groundAnglesSum;

    private float lastGroundAngle;

    private uint evtScroll;



    private FixedJoint hook;

    public bool skipLimiting;

    private bool isFallSpeedInitialized;

    private bool isFallSpeedLimited;

    private bool overridenDrag;

    private void Start()
    {
        ragdoll.BindBall(this.transform);
        Initialize();
    }


    public Vector3 momentum
    {
        get
        {
            Vector3 _zero = Vector3.zero;
            for (int i = 0; i < rigidbodies.Length; i++)
            {
                Rigidbody rigidbody = rigidbodies[i];
                _zero = _zero + (rigidbody.velocity * rigidbody.mass);
            }
            return _zero;
        }
    }

    public Vector3 velocity
    {
        get
        {
            return this.momentum / this.mass;
        }
    }

    static Human()
    {
        Human.all = new List<Human>();
       
    }

    public Human()
    {
    }

    public void AddRandomTorque(float multiplier)
    {
        Vector3 _onUnitSphere = (UnityEngine.Random.onUnitSphere * 100f) * multiplier;
        for (int i = 0; i < rigidbodies.Length; i++)
        {
            this.rigidbodies[i].AddTorque(_onUnitSphere, (ForceMode)2);
        }
    }

    public Vector3 ControlVelocity(float maxVelocity, bool killHorizontal)
    {
        Vector3 vector3;
        Rigidbody[] rigidbodyArray = this.rigidbodies;
        Vector3 vector31 = this.velocity;
        Vector3 vector32 = vector31;
        vector32.y = 0f;
        vector3 = (!killHorizontal ? Vector3.ClampMagnitude(vector31, maxVelocity) - vector31 : Vector3.ClampMagnitude(vector31 - vector32, maxVelocity) - vector31);
        for (int i = 0; i < (int)rigidbodyArray.Length; i++)
        {
            Rigidbody rigidbody = rigidbodyArray[i];
            rigidbody.velocity = (rigidbody.velocity + vector3);
        }
        return vector31;
    }

    private void FixedUpdate()
    {
        if (this.thisFrameHit + this.lastFrameHit > 30f)
        {
            this.MakeUnconscious();
            this.ReleaseGrab(3f);
        }
        this.lastFrameHit = this.thisFrameHit;
        this.thisFrameHit = 0f;
        this.jumpDelay -= Time.fixedDeltaTime;
        this.groundDelay -= Time.fixedDeltaTime;
        if (!this.disableInput)
        {
            this.ProcessInput();
        }
        this.LimitFallSpeed();
        Quaternion quaternion = Quaternion.Euler(this.controls.targetPitchAngle, this.controls.targetYawAngle, 0f);
        this.targetDirection = quaternion * Vector3.forward;
        this.targetLiftDirection = Quaternion.Euler(Mathf.Clamp(this.controls.targetPitchAngle, -70f, 80f), this.controls.targetYawAngle, 0f) * Vector3.forward;


        this.groundAngle = 90f;
        this.groundAngle = Mathf.Min(this.groundAngle, this.ragdoll.partBall.sensor.groundAngle);
        this.groundAngle = Mathf.Min(this.groundAngle, this.ragdoll.partLeftFoot.sensor.groundAngle);
        this.groundAngle = Mathf.Min(this.groundAngle, this.ragdoll.partRightFoot.sensor.groundAngle);
        bool flag1 = this.hasGrabbed;
       this.onGround = (this.groundDelay > 0f ? false : this.groundManager.onGround);
       this.hasGrabbed = this.grabManager.hasGrabbed;
        float single = 90f;
        float single1 = single;
        this.ragdoll.partRightFoot.sensor.groundAngle = single;
        float single2 = single1;
        single1 = single2;
        this.ragdoll.partLeftFoot.sensor.groundAngle = single2;
        this.ragdoll.partBall.sensor.groundAngle = single1;
        if (this.hasGrabbed && base.transform.position.y < this.grabStartPosition.y)
        {
            this.grabStartPosition = base.transform.position;
        }
        if (!this.hasGrabbed || base.transform.position.y - this.grabStartPosition.y <= 0.5f)
        {
            this.isClimbing = false;
        }
        else
        {
            this.isClimbing = true;
        }
        if (flag1 != this.hasGrabbed && this.hasGrabbed)
        {
            this.grabStartPosition = base.transform.position;
        }
        if (this.state != HumanState.Spawning)
        {
            this.spawning = false;
        }
        else
        {
            this.spawning = true;
            if (this.onGround)
            {
                this.MakeUnconscious();
            }
        }
        this.ProcessUnconscious();
        if (this.state != HumanState.Dead && this.state != HumanState.Unconscious && this.state != HumanState.Spawning)
        {
            this.ProcessFall();
            if (!this.onGround)
            {
                if (this.ragdoll.partLeftHand.sensor.grabObject != null || this.ragdoll.partRightHand.sensor.grabObject != null)
                {
                    this.state = HumanState.Climb;
                }
            }
            else if (this.controls.jump && this.jumpDelay <= 0f)
            {
                this.state = HumanState.Jump;
                this.jump = true;
                this.jumpDelay = 0.5f;
                this.groundDelay = 0.2f;
            }
            else if (this.controls.walkSpeed <= 0f)
            {
                this.state = HumanState.Idle;
            }
            else
            {
                this.state = HumanState.Walk;
            }
        }
        if (this.skipLimiting)
        {
            this.skipLimiting = false;
            return;
        }
        for (int i = 0; i < rigidbodies.Length; i++)
        {
            Vector3 vector3 = this.velocities[i];
            Vector3 _velocity = this.rigidbodies[i].velocity;
            Vector3 vector31 = _velocity - vector3;
            if (Vector3.Dot(vector3, vector31) < 0f)
            {
                Vector3 _normalized = vector3.normalized;
                float _magnitude = vector3.magnitude;
                float single3 = Mathf.Clamp(-Vector3.Dot(_normalized, vector31), 0f, _magnitude);
                vector31 = vector31 + (_normalized * single3);
            }
            float _deltaTime = 1000f * Time.deltaTime;
            if (vector31.magnitude > _deltaTime)
            {
                Vector3 vector32 = Vector3.ClampMagnitude(vector31, _deltaTime);
                _velocity = _velocity - (vector31 - vector32);
                this.rigidbodies[i].velocity = (_velocity);
            }
            this.velocities[i] = _velocity;
        }
    }

    internal void Hide()
    {
        this.SetPosition(new Vector3(0f, 500f, 0f));
        this.hook = this.ragdoll.partHead.rigidbody.gameObject.AddComponent<FixedJoint>();
    }

    public void Initialize()
    {
        this.ragdoll = base.GetComponentInChildren<Ragdoll>();
        this.motionControl2.Initialize();
        //HumanHead humanHead = this.ragdoll.partHead.transform.get_gameObject().AddComponent<HumanHead>();
        //humanHead.humanAudio = base.GetComponentInChildren<HumanAudio>();
        //componentInChildren.get_transform().SetParent(humanHead.get_transform(), false);
        this.InitializeBodies();
    }

    private void InitializeBodies()
    {
        this.rigidbodies = base.GetComponentsInChildren<Rigidbody>();
        this.velocities = new Vector3[(int)this.rigidbodies.Length];
        this.mass = 0f;
        for (int i = 0; i < (int)this.rigidbodies.Length; i++)
        {
            Rigidbody rigidbody = this.rigidbodies[i];
            if (rigidbody != null)
            {
                rigidbody.maxAngularVelocity = (10f);
                this.mass += rigidbody.mass;
            }
        }
        this.weight = this.mass * 9.81f;
    }

    public Vector3 KillHorizontalVelocity()
    {
        Rigidbody[] rigidbodyArray = this.rigidbodies;
        Vector3 vector3 = this.velocity;
        Vector3 vector31 = vector3;
        vector31.y = 0f;
        vector3 -= vector31;
        for (int i = 0; i < (int)rigidbodyArray.Length; i++)
        {
            Rigidbody rigidbody = rigidbodyArray[i];
            rigidbody.velocity = (rigidbody.velocity + -vector31);
        }
        return vector3;
    }

    private void LimitFallSpeed()
    {
        bool flag = true;
        if (this.isFallSpeedLimited != flag || !this.isFallSpeedInitialized)
        {
            this.isFallSpeedInitialized = true;
            this.isFallSpeedLimited = flag;
            if (flag)
            {
                this.SetDrag(0.1f, false);
                return;
            }
            this.SetDrag(0.05f, false);
        }
    }

    public Vector3 LimitHorizontalVelocity(float max)
    {
        Rigidbody[] rigidbodyArray = this.rigidbodies;
        Vector3 vector3 = this.velocity;
        Vector3 vector31 = vector3;
        vector31.y = 0f;
        if (vector31.magnitude < max)
        {
            return vector3;
        }
        vector31 -= Vector3.ClampMagnitude(vector31, max);
        vector3 -= vector31;
        for (int i = 0; i < (int)rigidbodyArray.Length; i++)
        {
            Rigidbody rigidbody = rigidbodyArray[i];
            rigidbody.velocity = (rigidbody.velocity + -vector31);
        }
        return vector3;
    }

    public void MakeUnconscious(float time)
    {
        this.unconsciousTime = time;
        this.state = HumanState.Unconscious;
    }

    public void MakeUnconscious()
    {
        this.unconsciousTime = this.maxUnconsciousTime;
        this.state = HumanState.Unconscious;
    }

    private void OnDisable()
    {
        Human.all.Remove(this);
    }

    private void OnEnable()
    {
        Human.all.Add(this);
        Human.instance = this;
        this.grabManager = base.GetComponent<GrabManager>();
       this.groundManager = base.GetComponent<GroundManager>();
        this.motionControl2 = base.GetComponent<HumanMotion2>();
        this.controls = base.GetComponentInParent<HumanControls>();
    }



    private void ProcessFall()
    {
        this.PushGroundAngle();
        bool flag = false;
        if (this.groundAnglesSum / (float)((int)this.groundAngles.Length) > 45f)
        {
            flag = true;
            this.slideTimer = 0f;
            this.onGround = false;
            this.state = HumanState.Slide;
        }
        else if (this.state == HumanState.Slide && this.groundAnglesSum / (float)((int)this.groundAngles.Length) < 37f && this.ragdoll.partBall.rigidbody.velocity.y > -1f)
        {
            this.slideTimer += Time.fixedDeltaTime;
            if (this.slideTimer < 0.003f)
            {
                this.onGround = false;
            }
        }
        if (this.onGround || flag)
        {
            this.fallTimer = 0f;
        }
        else
        {
            if (this.fallTimer < 5f)
            {
                this.fallTimer += Time.deltaTime;
            }
            if (this.state == HumanState.Climb)
            {
                this.fallTimer = 0f;
            }
            if (this.fallTimer > 3f)
            {
                this.state = HumanState.FreeFall;
                return;
            }
            if (this.fallTimer > 1f)
            {
                this.state = HumanState.Fall;
                return;
            }
        }
    }

    private void ProcessInput()
    {

        if (this.controls.unconscious)
        {
            this.MakeUnconscious();
        }
        if (this.motionControl2.enabled)
        {
           this.motionControl2.OnFixedUpdate();
        }
    }

    private void ProcessUnconscious()
    {
        if (this.state == HumanState.Unconscious)
        {
            this.unconsciousTime -= Time.fixedDeltaTime;
            if (this.unconsciousTime <= 0f)
            {
                this.state = HumanState.Fall;
                this.wakeUpTime = this.maxWakeUpTime;
                this.unconsciousTime = 0f;
            }
        }
        if (this.wakeUpTime > 0f)
        {
            this.wakeUpTime -= Time.fixedDeltaTime;
            if (this.wakeUpTime <= 0f)
            {
                this.wakeUpTime = 0f;
            }
        }
    }

    private void PushGroundAngle()
    {
        float single = (!this.onGround || this.groundAngle >= 80f ? this.lastGroundAngle : this.groundAngle);
        this.lastGroundAngle = single;
        this.groundAnglesSum -= this.groundAngles[this.groundAnglesIdx];
        this.groundAnglesSum += single;
        this.groundAngles[this.groundAnglesIdx] = single;
        this.groundAnglesIdx = (this.groundAnglesIdx + 1) % (int)this.groundAngles.Length;
    }

    internal void ReceiveHit(Vector3 impulse)
    {
        this.thisFrameHit = Mathf.Max(this.thisFrameHit, impulse.magnitude);
    }

    public void ReleaseGrab(float blockTime = 0f)
    {
        this.ragdoll.partLeftHand.sensor.ReleaseGrab(blockTime);
        this.ragdoll.partRightHand.sensor.ReleaseGrab(blockTime);
    }

    public void ReleaseGrab(GameObject item, float blockTime = 0f)
    {
        if (this.ragdoll.partLeftHand.sensor.IsGrabbed(item))
        {
            this.ragdoll.partLeftHand.sensor.ReleaseGrab(blockTime);
        }
        if (this.ragdoll.partRightHand.sensor.IsGrabbed(item))
        {
            this.ragdoll.partRightHand.sensor.ReleaseGrab(blockTime);
        }
    }

    public void Reset()
    {
        this.groundManager.Reset();
        this.grabManager.Reset();
        for (int i = 0; i < (int)this.groundAngles.Length; i++)
        {
            this.groundAngles[i] = 0f;
        }
        this.groundAnglesSum = 0f;
    }

    public void ResetDrag()
    {
        this.overridenDrag = false;
        this.isFallSpeedInitialized = false;
        this.LimitFallSpeed();
    }

    private void Scroll(Vector3 scroll)
    {

        Transform _transform = base.transform;
        _transform.position = (_transform.position + scroll);


        //  this.player.cameraController.Scroll(scroll);


    }


    public void SetDrag(float drag, bool external = true)
    {
        if (!external && this.overridenDrag)
        {
            return;
        }
        this.overridenDrag = external;
        for (int i = 0; i < (int)this.rigidbodies.Length; i++)
        {
            this.rigidbodies[i].drag = (drag);
        }
    }

    public void SetPosition(Vector3 spawnPos)
    {

        Vector3 vector3 = spawnPos - base.transform.position;
        this.Scroll(vector3);
    }

    internal void Show()
    {

        this.SetPosition(new Vector3(0f, 50f, 0f));
    }

    public void SpawnAt(Vector3 pos)
    {
        this.state = HumanState.Spawning;
        Vector3 vector3 = this.KillHorizontalVelocity();
        int num = 2;


        Vector3 vector31 = (pos - (vector3 * (float)num)) - (((Physics.gravity * (float)num) * (float)num) / 2f);
        this.SetPosition(vector31);
        if (vector3.magnitude < 5f)
        {
            this.AddRandomTorque(1f);
        }
        this.Reset();
    }

    public void SpawnAt(Transform spawnPoint, Vector3 offset)
    {
        this.SpawnAt(offset + spawnPoint.position);
    }
}



public enum HumanState
{
    Idle,
    Walk,
    Climb,
    Jump,
    Slide,
    Fall,
    FreeFall,
    Unconscious,
    Dead,
    Spawning
}