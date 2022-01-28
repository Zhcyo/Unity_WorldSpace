using System;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public LayerMask collisionLayers;

    private GrabManager grabManager;

    private Human human;

    private Ragdoll ragdoll;

    private float ballRadius;

    private List<Collision> collisions = new List<Collision>();

    private List<Vector3> contacts = new List<Vector3>();

    public Vector3 lastImpulse;

    public Vector3 lastNonZeroImpulse;

    public float timeSinceLastNonzeroImpulse;

    public Ball()
    {
    }

    private void FixedUpdate()
    {
        this.ImpulseForce2();
        this.collisions.Clear();
        this.contacts.Clear();
    }

    private void HandleCollision(Collision collision)
    {
        RaycastHit raycastHit = new RaycastHit();
        Vector3 impulse = collision.impulse;
        if (impulse.y > 0f && this.human.onGround)
        {
            this.timeSinceLastNonzeroImpulse = Time.time;
        }
        Vector3 vector3 = this.human.controls.walkDirection;
        Debug.DrawRay(collision.contacts[0].point, impulse / 10f, Color.red, 0.5f);
        if (Vector3.Dot(impulse, vector3) >= 0f)
        {
            return;
        }
        float single = 0f;
        int num = 0;
        while (num < (int)collision.contacts.Length)
        {
            Vector3 _point = collision.contacts[num].point;
            Vector3 _up = (_point + (vector3 * 0.07f)) + (Vector3.up * 0.07f);
            Vector3 _up1 = (_point - (vector3 * 0.07f)) - (Vector3.up * 0.07f);
            Debug.DrawRay(_up, Vector3.down * 0.1f, Color.blue);
            if (Physics.Raycast(_up, Vector3.down, out  raycastHit, 0.1f, this.collisionLayers))
            {
                Debug.DrawRay(raycastHit.point, raycastHit.normal, Color.red);
            }
            Debug.DrawRay(_up1, vector3 * 0.1f, Color.blue);
            if (Physics.Raycast(_up1, vector3, out raycastHit, 0.1f, this.collisionLayers))
            {
                Debug.DrawRay(raycastHit.point, raycastHit.normal, Color.red);
            }
            if (!Physics.Raycast(_up, Vector3.down, out raycastHit, 0.1f, this.collisionLayers) || raycastHit.normal.y <= 0.7f || !Physics.Raycast(_up1, vector3,out raycastHit, 0.1f, this.collisionLayers) || raycastHit.normal.y >= 0.4f)
            {
                num++;
            }
            else
            {
                Debug.DrawLine(base.transform.position, collision.contacts[num].point, Color.red);
                single = 1.5f;
                break;
            }
        }
        if (this.human.ragdoll.partLeftHand.sensor.grabJoint != null && this.human.ragdoll.partRightHand.sensor.grabJoint != null)
        {
            float single1 = (this.human.ragdoll.partLeftHand.sensor.grabJoint != null ? Vector3.Dot(this.human.ragdoll.partLeftHand.transform.position - base.transform.position, vector3) : 0f);
            float single2 = (this.human.ragdoll.partRightHand.sensor.grabJoint != null ? Vector3.Dot(this.human.ragdoll.partRightHand.transform.position - base.transform.position, vector3) : 0f);
            single = Mathf.Max(single, (single1 + single2) / 2f);
        }
        if (single > 0f)
        {
            Vector3 vector31 = new Vector3(impulse.x,0,impulse.z); /*impulse. ZeroY();*/
            impulse = ((Vector3.up * vector31.magnitude) * single) - (vector31 / 2f);
            this.human.ragdoll.partBall.rigidbody.AddForce(impulse, (ForceMode)1);
            this.human.groundManager.DistributeForce(-impulse / Time.fixedDeltaTime, base.transform.position);
        }
    }

    private void ImpulseForce2()
    {
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.contacts.Length == 0)
        {
            return;
        }
        this.HandleCollision(collision);
        this.collisions.Add(collision);
        for (int i = 0; i < (int)collision.contacts.Length; i++)
        {
            this.contacts.Add(collision.contacts[i].point);
        }
    }

    public void OnCollisionStay(Collision collision)
    {
        if (collision.contacts.Length == 0)
        {
            return;
        }
        this.HandleCollision(collision);
        this.collisions.Add(collision);
        for (int i = 0; i < (int)collision.contacts.Length; i++)
        {
            this.contacts.Add(collision.contacts[i].point);
        }
    }

    private void OnEnable()
    {
        this.human = base.GetComponent<Human>();
        this.ragdoll = base.GetComponent<Ragdoll>();
        this.ballRadius = base.GetComponent<SphereCollider>().radius;
       this.grabManager = base.GetComponent<GrabManager>();
    }
}