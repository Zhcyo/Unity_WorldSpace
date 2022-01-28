using System;
using UnityEngine;

public class RagdollBoneEnvelope : MonoBehaviour
{
    public float innerThickness;

    public float outerThickness;

    public float power;

    public float innerWeight;

    public float outerWeight;

    public Vector3 centerOffset;

    public float lengthMultiplier;

    [NonSerialized]
    public Vector3 start;

    [NonSerialized]
    public Vector3 direction;

    [NonSerialized]
    public float height;

    [NonSerialized]
    public float radius;

    public float innerRadius
    {
        get
        {
            float single = this.radius - (this.innerThickness != 0f ? this.innerThickness : 0.125f);
            if (single <= 0f)
            {
                return 0f;
            }
            return single;
        }
    }

    public float outerRadius
    {
        get
        {
            return this.radius + (this.outerThickness != 0f ? this.outerThickness : 0.125f);
        }
    }

    public RagdollBoneEnvelope()
    {
    }

    public void ReadCollider()
    {
        Collider component = base.GetComponent<Collider>();
        if (component is SphereCollider)
        {
            SphereCollider sphereCollider = component as SphereCollider;
            this.start = sphereCollider.center + this.centerOffset;
            this.direction = Vector3.zero;
            this.height = 0f;
            this.radius = sphereCollider.radius;
            return;
        }
        if (component is CapsuleCollider)
        {
            CapsuleCollider capsuleCollider = component as CapsuleCollider;
            this.direction = new Vector3(1f, 0f, 0f);
            if (capsuleCollider.direction == 1)
            {
                this.direction = new Vector3(0f, 1f, 0f);
            }
            else if (capsuleCollider.direction == 2)
            {
                this.direction = new Vector3(0f, 0f, 1f);
            }
            this.height = Math.Max(0f, capsuleCollider.height - 2f * capsuleCollider.radius);
            if (this.lengthMultiplier != 0f)
            {
                this.height *= this.lengthMultiplier;
            }
            this.start = (capsuleCollider.center - ((this.direction * this.height) / 2f)) + this.centerOffset;
            this.radius = capsuleCollider.radius;
        }
    }
}