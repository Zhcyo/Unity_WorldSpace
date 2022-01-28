using System;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    public HumanSegment partHead;

    public HumanSegment partChest;

    public HumanSegment partWaist;

    public HumanSegment partHips;

    public HumanSegment partLeftArm;

    public HumanSegment partLeftForearm;

    public HumanSegment partLeftHand;

    public HumanSegment partLeftThigh;

    public HumanSegment partLeftLeg;

    public HumanSegment partLeftFoot;

    public HumanSegment partRightArm;

    public HumanSegment partRightForearm;

    public HumanSegment partRightHand;

    public HumanSegment partRightThigh;

    public HumanSegment partRightLeg;

    public HumanSegment partRightFoot;

    public HumanSegment partBall;

    public Transform[] bones;

    public float handLength;

    private bool initialized;

    public Ragdoll()
    {
    }
    private void Start()
    {
   
    }
    private void AddAntistretch(HumanSegment seg1, HumanSegment seg2)
    {
        ConfigurableJoint configurableJoint = seg1.rigidbody.gameObject.AddComponent<ConfigurableJoint>();
        int num = 1;
        ConfigurableJointMotion configurableJointMotion = (ConfigurableJointMotion)num;
        configurableJoint.zMotion = ((ConfigurableJointMotion)num);
        ConfigurableJointMotion configurableJointMotion1 = configurableJointMotion;
        ConfigurableJointMotion configurableJointMotion2 = configurableJointMotion1;
        configurableJoint.yMotion = (configurableJointMotion1);
        configurableJoint.xMotion = (configurableJointMotion2);
        SoftJointLimit softJointLimit = new SoftJointLimit();
        Vector3 _position = seg1.transform.position - seg2.transform.position;
        softJointLimit.limit = (_position.magnitude);
        configurableJoint.linearLimit = (softJointLimit);
        configurableJoint.autoConfigureConnectedAnchor = (false);
        configurableJoint.anchor = (Vector3.zero);
        configurableJoint.connectedBody = (seg2.rigidbody);
        configurableJoint.connectedAnchor = (Vector3.zero);
    }

    public void AllowHandBallRotation(bool allow)
    {
        ConfigurableJointMotion configurableJointMotion = (ConfigurableJointMotion)(allow ? 2 : 0);
        ConfigurableJoint component = this.partLeftHand.rigidbody.GetComponent<ConfigurableJoint>();
        component.angularXMotion = (configurableJointMotion);
        component.angularYMotion = (configurableJointMotion);
        component.angularZMotion = (configurableJointMotion);
        ConfigurableJoint configurableJoint = this.partRightHand.rigidbody.GetComponent<ConfigurableJoint>();
        configurableJoint.angularXMotion = (configurableJointMotion);
        configurableJoint.angularYMotion = (configurableJointMotion);
        configurableJoint.angularZMotion = (configurableJointMotion);
    }

    private void Awake()
    {
        if (this.initialized)
        {
            return;
        }
        this.initialized = true;
        this.CollectSegments();
        this.SetupColliders();
        Vector3 _position = this.partLeftArm.transform.position - this.partLeftForearm.transform.position;
        float _magnitude = _position.magnitude;
        _position = this.partLeftForearm.transform.position - this.partLeftHand.transform.position;
        this.handLength = _magnitude + _position.magnitude;
    }

    public void BindBall(Transform ballTransform)
    {
        this.partBall = this.BindSegmanet(ballTransform);
        SpringJoint component = this.partBall.rigidbody.GetComponent<SpringJoint>();
        component.autoConfigureConnectedAnchor = false;
        component.connectedAnchor = (this.partHips.transform.InverseTransformPoint(base.transform.position + (Vector3.up * ((this.partBall.collider as SphereCollider).radius + component.maxDistance))));
        component.connectedBody = (this.partHips.rigidbody);
    }

    private HumanSegment BindSegmanet(Transform transform)
    {
        HumanSegment humanSegment = new HumanSegment();
        humanSegment.transform = transform;
        humanSegment.collider = humanSegment.transform.GetComponent<Collider>();
        humanSegment.rigidbody = humanSegment.transform.GetComponent<Rigidbody>();
        humanSegment.startupRotation = humanSegment.transform.localRotation;
        humanSegment.sensor = humanSegment.transform.GetComponent<CollisionSensor>();
        humanSegment.bindPose = humanSegment.transform.worldToLocalMatrix * base.transform.localToWorldMatrix;

        return humanSegment;
    }

    private void CollectSegments()
    {
        Dictionary<string, Transform> strs = new Dictionary<string, Transform>();
        Transform[] componentsInChildren = GetComponentsInChildren<Transform>();
        for (int i = 0; i < componentsInChildren.Length; i++)
        {
            strs.Add(componentsInChildren[i].name.ToLower(), componentsInChildren[i]);
            //strs[componentsInChildren[i].name.ToLower()] = componentsInChildren[i];
        }
        this.partHead = this.FindSegment(strs, "head");
        this.partChest = this.FindSegment(strs, "chest");
        this.partWaist = this.FindSegment(strs, "waist");
        this.partHips = this.FindSegment(strs, "hips");
        this.partLeftArm = this.FindSegment(strs, "leftArm");
        this.partLeftForearm = this.FindSegment(strs, "leftForearm");
        this.partLeftHand = this.FindSegment(strs, "leftHand");
        this.partLeftThigh = this.FindSegment(strs, "leftThigh");
        this.partLeftLeg = this.FindSegment(strs, "leftLeg");
        this.partLeftFoot = this.FindSegment(strs, "leftFoot");
        this.partRightArm = this.FindSegment(strs, "rightArm");
        this.partRightForearm = this.FindSegment(strs, "rightForearm");
        this.partRightHand = this.FindSegment(strs, "rightHand");
        this.partRightThigh = this.FindSegment(strs, "rightThigh");
        this.partRightLeg = this.FindSegment(strs, "rightLeg");
        this.partRightFoot = this.FindSegment(strs, "rightFoot");
        this.SetupHeadComponents(this.partHead);
        this.SetupBodyComponents(this.partChest);
        this.SetupBodyComponents(this.partWaist);
        this.SetupBodyComponents(this.partHips);
        this.SetupLimbComponents(this.partLeftArm);
        this.SetupLimbComponents(this.partLeftForearm);
        this.SetupLimbComponents(this.partLeftThigh);
        this.SetupLimbComponents(this.partLeftLeg);
        this.SetupLimbComponents(this.partRightArm);
        this.SetupLimbComponents(this.partRightForearm);
        this.SetupLimbComponents(this.partRightThigh);
        this.SetupLimbComponents(this.partRightLeg);
        this.SetupFootComponents(this.partLeftFoot);
        this.SetupFootComponents(this.partRightFoot);
        this.SetupHandComponents(this.partLeftHand);
        this.SetupHandComponents(this.partRightHand);
        this.partLeftHand.sensor.otherSide = this.partRightHand.sensor;
        this.partRightHand.sensor.otherSide = this.partLeftHand.sensor;
        this.AddAntistretch(this.partLeftHand, this.partChest);
        this.AddAntistretch(this.partRightHand, this.partChest);
        this.AddAntistretch(this.partLeftFoot, this.partHips);
        this.AddAntistretch(this.partRightFoot, this.partHips);
    }

    private HumanSegment FindSegment(Dictionary<string, Transform> children, string name)
    {
        return this.BindSegmanet(children[name.ToLower()]);
    }

    public void Lock()
    {
        Rigidbody[] componentsInChildren = base.GetComponentsInChildren<Rigidbody>();
        for (int i = 0; i < (int)componentsInChildren.Length; i++)
        {
            componentsInChildren[i].isKinematic = (true);
        }
    }

    public void ReleaseHeavyArms()
    {
        Rigidbody rigidbody = this.partLeftHand.rigidbody;
        Rigidbody rigidbody1 = this.partLeftForearm.rigidbody;
        float single = 5f;
        float single1 = single;
        this.partLeftArm.rigidbody.mass = (single);
        float single2 = single1;
        float single3 = single2;
        rigidbody1.mass = (single2);
        rigidbody.mass = (single3);
        Rigidbody rigidbody2 = this.partRightHand.rigidbody;
        Rigidbody rigidbody3 = this.partRightForearm.rigidbody;
        float single4 = 5f;
        single1 = single4;
        this.partRightArm.rigidbody.mass = (single4);
        float single5 = single1;
        single3 = single5;
        rigidbody3.mass = (single5);
        rigidbody2.mass = (single3);
    }

    private void SetupBodyComponents(HumanSegment part)
    {
        part.sensor = part.transform.gameObject.AddComponent<CollisionSensor>();
        //       part.transform.gameObject.AddComponent<CollisionAudioSensor>();
        part.sensor.knockdown = 1f;
    }

    private void SetupColliders()
    {
        Physics.IgnoreCollision(this.partChest.collider, this.partHead.collider);
        Physics.IgnoreCollision(this.partChest.collider, this.partLeftArm.collider);
        Physics.IgnoreCollision(this.partChest.collider, this.partLeftForearm.collider);
        Physics.IgnoreCollision(this.partChest.collider, this.partRightArm.collider);
        Physics.IgnoreCollision(this.partChest.collider, this.partRightForearm.collider);
        Physics.IgnoreCollision(this.partChest.collider, this.partWaist.collider);
        Physics.IgnoreCollision(this.partHips.collider, this.partChest.collider);
        Physics.IgnoreCollision(this.partHips.collider, this.partWaist.collider);
        Physics.IgnoreCollision(this.partHips.collider, this.partLeftThigh.collider);
        Physics.IgnoreCollision(this.partHips.collider, this.partLeftLeg.collider);
        Physics.IgnoreCollision(this.partHips.collider, this.partLeftFoot.collider);
        Physics.IgnoreCollision(this.partHips.collider, this.partRightThigh.collider);
        Physics.IgnoreCollision(this.partHips.collider, this.partRightLeg.collider);
        Physics.IgnoreCollision(this.partHips.collider, this.partRightFoot.collider);
        Physics.IgnoreCollision(this.partLeftForearm.collider, this.partLeftArm.collider);
        Physics.IgnoreCollision(this.partLeftForearm.collider, this.partLeftHand.collider);
        Physics.IgnoreCollision(this.partLeftArm.collider, this.partLeftHand.collider);
        Physics.IgnoreCollision(this.partRightForearm.collider, this.partRightArm.collider);
        Physics.IgnoreCollision(this.partRightForearm.collider, this.partRightHand.collider);
        Physics.IgnoreCollision(this.partRightArm.collider, this.partRightHand.collider);
        Physics.IgnoreCollision(this.partLeftThigh.collider, this.partLeftLeg.collider);
        Physics.IgnoreCollision(this.partRightThigh.collider, this.partRightLeg.collider);
    }

    private void SetupFootComponents(HumanSegment part)
    {
        part.sensor = part.transform.gameObject.AddComponent<CollisionSensor>();
        // part.transform.get_gameObject().AddComponent<FootCollisionAudioSensor>();
        part.sensor.groundCheck = true;
    }

    private void SetupHandComponents(HumanSegment part)
    {
        part.sensor = part.transform.gameObject.AddComponent<CollisionSensor>();
        // part.transform.get_gameObject().AddComponent<CollisionAudioSensor>();
    }

    private void SetupHeadComponents(HumanSegment part)
    {
        part.sensor = part.transform.gameObject.AddComponent<CollisionSensor>();
        // part.transform.get_gameObject().AddComponent<CollisionAudioSensor>();
        part.sensor.knockdown = 2f;
    }

    private void SetupLimbComponents(HumanSegment part)
    {
        part.sensor = part.transform.gameObject.AddComponent<CollisionSensor>();
    }

    internal void StretchHandsLegs(Vector3 direction, Vector3 right, int force)
    {
        this.partLeftHand.rigidbody.AddForce(((direction - right) * (float)force) / 2f, 0);
        this.partRightHand.rigidbody.AddForce(((direction + right) * (float)force) / 2f, 0);
        this.partLeftFoot.rigidbody.AddForce(((-direction - right) * (float)force) / 2f, 0);
        this.partRightFoot.rigidbody.AddForce(((-direction + right) * (float)force) / 2f, 0);
        this.partLeftForearm.rigidbody.AddForce(((direction - right) * (float)force) / 2f, 0);
        this.partRightForearm.rigidbody.AddForce(((direction + right) * (float)force) / 2f, 0);
        this.partLeftLeg.rigidbody.AddForce(((-direction - right) * (float)force) / 2f, 0);
        this.partRightLeg.rigidbody.AddForce(((-direction + right) * (float)force) / 2f, 0);
    }

    public void ToggleHeavyArms(bool left, bool right)
    {
        float single;
        float single1;
        if (left)
        {
            Rigidbody rigidbody = this.partLeftHand.rigidbody;
            Rigidbody rigidbody1 = this.partLeftForearm.rigidbody;
            float single2 = 20f;
            single1 = single2;
            this.partLeftArm.rigidbody.mass = (single2);
            float single3 = single1;
            single = single3;
            rigidbody1.mass = (single3);
            rigidbody.mass = (single);
        }
        if (right)
        {
            Rigidbody rigidbody2 = this.partRightHand.rigidbody;
            Rigidbody rigidbody3 = this.partRightForearm.rigidbody;
            float single4 = 20f;
            single1 = single4;
            this.partRightArm.rigidbody.mass = (single4);
            float single5 = single1;
            single = single5;
            rigidbody3.mass = (single5);
            rigidbody2.mass = (single);
        }
    }
}