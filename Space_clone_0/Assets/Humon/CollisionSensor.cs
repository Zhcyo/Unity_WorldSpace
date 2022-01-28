using System;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSensor : MonoBehaviour
{
    private const float JOINT_BREAK_FORCE = 20000f;

    public CollisionSensor otherSide;

    public bool forwardCollisionAudio = true;

    private GrabManager grabManager;

    private GroundManager groundManager;

    private float handToHandClimb = 0.2f;

    public float knockdown;

    public bool groundCheck;

    public bool grab;

    public Vector3 grabPosition;

    public Vector3 targetPosition;

    private float grabPrecision = 0.1f;

    public Collider grabFilter;

    public bool onGround;

    public Transform groundObject;

    public ConfigurableJoint grabJoint;

    public Rigidbody grabBody;

    public GameObject grabObject;

    public Action<GameObject, Vector3, PhysicMaterial, Vector3> onCollideTap;

    public Action<GameObject, Vector3, PhysicMaterial, Vector3> onGrabTap;

    public Action offGrabTap;

    public Action<CollisionSensor, Collision> onGroundTap2;

    public Action<CollisionSensor> offGroundTap2;

    public Action<CollisionSensor, Collision> onStayTap2;

    private List<Collider> activeCollider = new List<Collider>();

    private List<Collider> ativeGrounded = new List<Collider>();

    private float blockGrab;

    private Transform thisTransform;

    private Rigidbody thisBody;

    [NonSerialized]
    public Human human;

    private Vector3 entryTangentVelocityImpulse;

    private Vector3 normalTangentVelocityImpulse;

    public float groundAngle;

    private ReleaseGrabTrigger blockGrabTrigger;

    public CollisionSensor()
    {
    }

    public void BlockGrab(ReleaseGrabTrigger releaseGrabTrigger)
    {
        this.blockGrabTrigger = releaseGrabTrigger;
        this.ReleaseGrab(0f);
    }

    private void FixedUpdate()
    {
        if (!this.grab && this.grabJoint != null)
        {
            this.ReleaseGrab(0f);
        }
        this.blockGrab -= Time.fixedDeltaTime;
        if (this.grabFilter == null)
        {
            return;
        }
        if (this.grab && this.grabJoint == null && this.blockGrab <= 0f)
        {
            Vector3 _position = base.transform.position;
            Vector3 vector3 = this.grabPosition - _position;
            float _radius = base.GetComponent<SphereCollider>().radius;
            Collider collider = this.grabFilter;
            Transform component = collider.GetComponent<Transform>();
            Rigidbody componentInParent = collider.GetComponentInParent<Rigidbody>();
            IGrabbable grabbable = component.GetComponentInParent<IGrabbable>();
            if (grabbable != null)
            {
                this.grabObject = (grabbable as MonoBehaviour).gameObject;
            }
            if (this.grabBody == null)
            {
                this.grabObject = collider.gameObject;
            }
            else
            {
                this.grabObject = this.grabBody.gameObject;
            }

            this.grabJoint = base.gameObject.AddComponent<ConfigurableJoint>();
            this.grabJoint.autoConfigureConnectedAnchor = (false);
            this.grabJoint.anchor = (base.transform.InverseTransformPoint(_position + (vector3.normalized * _radius)));
            if (!componentInParent)
            {
                this.grabJoint.connectedAnchor=(this.grabPosition);
            }
            else
            {
                this.grabJoint.connectedBody=(componentInParent);
                this.grabJoint.connectedAnchor=(componentInParent.transform.InverseTransformPoint(this.grabPosition));
            }
            this.grabJoint.xMotion=(0);
            this.grabJoint.yMotion=(0);
            this.grabJoint.zMotion=(0);
            this.grabJoint.angularXMotion=(0);
            this.grabJoint.angularYMotion=(0);
            this.grabJoint.angularZMotion=(0);
            this.grabJoint.breakForce=(20000f);
            this.grabJoint.breakTorque=(20000f);
            this.grabJoint.enablePreprocessing=(false);
            this.grabBody = componentInParent;
            this.grabManager.ObjectGrabbed(this.grabObject);
            if (this.onGrabTap != null)
            {
                this.onGrabTap(base.gameObject, this.grabPosition, collider.sharedMaterial, this.normalTangentVelocityImpulse);
            }
        }
    }

    private float GetSurfaceAngle(ContactPoint contact, Vector3 direction)
    {
        return Vector3.Angle(contact.normal, direction);
    }

    private void GrabCheck(Transform collisionTransform, Rigidbody collisionRigidbody, Collider collider, ContactPoint[] contacts)
    {
        if (this.grabFilter != null)
        {
            return;
        }
        if (this.blockGrab > 0f)
        {
            return;
        }
        if (this.grabJoint == null &&this.blockGrabTrigger == null && collisionTransform.tag!= "NoGrab")
        {
            if ((this.targetPosition - contacts[0].point).magnitude > 0.5f)
            {
                return;
            }
            bool componentInParent = collider.GetComponentInParent<Human>() != null;
            if (this.human.onGround && collider.tag!= "Target" && !componentInParent)
            {
                bool flag = collider.GetComponentInParent<Rigidbody>();
                Vector3 _position = base.transform.position - this.targetPosition;
                Vector3 _normal = contacts[0].normal;
                if (Vector3.Dot(_normal, _position.normalized) < (flag ? 0.4f : 0.7f))
                {
                    return;
                }
                if (Vector3.Dot(_normal, _position) < (flag ? 0.05f : 0.2f))
                {
                    return;
                }
            }
            IGrabbable grabbable = collisionTransform.GetComponentInParent<IGrabbable>();
            if (grabbable != null)
            {
                this.grabObject = (grabbable as MonoBehaviour).gameObject;
            }
            else if (this.grabBody == null)
            {
                this.grabObject = collider.gameObject;
            }
            else
            {
                this.grabObject = this.grabBody.gameObject;
            }
            if ( this.human.state == HumanState.Climb && this.otherSide.grabObject == this.grabObject && base.transform.position.y > this.otherSide.transform.position.y + this.handToHandClimb)
            {
                return;
            }
            this.grabJoint = base.gameObject.AddComponent<ConfigurableJoint>();
            if (collisionRigidbody)
            {
                this.grabJoint.connectedBody=(collisionRigidbody);
            }
            this.grabJoint.xMotion=(0);
            this.grabJoint.yMotion=(0);
            this.grabJoint.zMotion=(0);
            this.grabJoint.angularXMotion=(0);
            this.grabJoint.angularYMotion=(0);
            this.grabJoint.angularZMotion=(0);
            this.grabJoint.breakForce=(20000f);
            this.grabJoint.breakTorque=(20000f);
            this.grabJoint.enablePreprocessing=(false);
            this.grabBody = collisionRigidbody;
            this.grabManager.ObjectGrabbed(this.grabObject);
            if (this.onGrabTap != null)
            {
                this.onGrabTap(base.gameObject, contacts[0].point, collider.sharedMaterial, this.normalTangentVelocityImpulse);
            }
        }
    }

    private void GroundCheck(Collision collision)
    {
        Vector3 impulse = collision.impulse;
        float single = Vector3.Angle(impulse.normalized, Vector3.up);
        if (single < this.groundAngle)
        {
            this.groundAngle = single;
        }
        ContactPoint[] _contacts = collision.contacts;
        float single1 = 90f;
        for (int i = 0; i < (int)_contacts.Length; i++)
        {
            ContactPoint contactPoint = _contacts[i];
            float surfaceAngle = this.GetSurfaceAngle(contactPoint, Vector3.up);
            if (surfaceAngle < 60f)
            {
               this.groundManager.ReportSurfaceAngle(surfaceAngle);
                if (surfaceAngle < single1)
                {
                    single1 = surfaceAngle;
                }
            }
        }
        if (single1 < 60f)
        {
            if (single1 < this.groundAngle)
            {
                this.groundAngle = single1;
            }
            if (!this.onGround && this.onGroundTap2 != null)
            {
                this.onGroundTap2(this, collision);
            }
            this.onGround = true;
            this.groundObject = collision.transform;
       
           this.groundManager.ObjectEnter(this.groundObject.gameObject);
        }
    }

    private void HandleCollision(Collision collision, bool enter)
    {
        if (collision.contacts.Length == 0)
        {
            return;
        }
        if (this.thisTransform != null)
        {
            Transform _transform = collision.transform;
            if (_transform.root != this.thisTransform.root)
            {
                Rigidbody _rigidbody = collision.rigidbody;
                Collider _collider = collision.collider;
                ContactPoint[] _contacts = collision.contacts;
                if (_contacts.Length == 0)
                {
                    return;
                }
                if (this.grab && (this.grabFilter == null || this.grabFilter == _collider))
                {
                    this.GrabCheck(_transform, _rigidbody, _collider, _contacts);
                }
                if (this.groundCheck)
                {
                    this.GroundCheck(collision);
                }
                if (enter && this.onCollideTap != null)
                {
                    this.onCollideTap(base.gameObject, collision.contacts[0].point, collision.collider.sharedMaterial, this.normalTangentVelocityImpulse);
                }
            }
        }
    }

    public bool IsGrabbed(GameObject go)
    {
        if (this.grabObject == null)
        {
            return false;
        }
        for (Transform i = this.grabObject.transform; i != null; i = i.parent)
        {
            if (i.gameObject == go)
            {
                return true;
            }
        }
        return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.contacts.Length == 0)
        {
            return;
        }
        Vector3 normalTangentVelocitiesAndImpulse = collision.GetNormalTangentVelocitiesAndImpulse(this.thisBody);
        Vector3 vector3 = normalTangentVelocitiesAndImpulse;
        this.normalTangentVelocityImpulse = normalTangentVelocitiesAndImpulse;
        this.entryTangentVelocityImpulse = vector3;
        this.HandleCollision(collision, true);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (this.groundCheck && collision.transform.root != this.thisTransform.root)
        {
            if (this.onGround && this.offGroundTap2 != null)
            {
                this.offGroundTap2(this);
            }
            this.onGround = false;
            this.groundObject = null;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.contacts.Length == 0)
        {
            return;
        }
      this.normalTangentVelocityImpulse = collision.GetNormalTangentVelocitiesAndImpulse(this.thisBody);
        this.HandleCollision(collision, false);
        if (this.onStayTap2 != null)
        {
            this.onStayTap2(this, collision);
        }
    }

    private void OnEnable()
    {
        this.thisTransform = base.transform;
        this.thisBody = base.GetComponent<Rigidbody>();
        this.grabManager = base.GetComponentInParent<GrabManager>();
        this.groundManager = base.GetComponentInParent<GroundManager>();
        this.human = base.GetComponentInParent<Human>();
    }

    private void OnJointBreak(float breakForce)
    {
        if (breakForce >= 20000f)
        {
            Debug.LogError(string.Concat("Joint break force: ", breakForce.ToString()));
            this.ReleaseGrab(0f);
        }
    }

    public void ReleaseGrab(float blockTime = 0f)
    {
        if (this.grabJoint != null)
        {
            if (this.grabObject != null)
            {
                this.grabManager.ObjectReleased(this.grabObject);
            }
            Destroy(this.grabJoint);
            this.grabJoint = null;
            this.grabBody = null;
            this.grabObject = null;
            if (this.offGrabTap != null)
            {
                this.offGrabTap();
            }
        }
        this.blockGrab = Mathf.Max(this.blockGrab, blockTime);
    }

    private bool SurfaceWithinAngle(ContactPoint contact, Vector3 direction, float angle)
    {
        bool flag = false;
        if (Vector3.Angle(contact.normal, direction) <= angle)
        {
            flag = true;
        }
        return flag;
    }

    public void UnblockBlockGrab()
    {
        this.blockGrabTrigger = null;
    }
}