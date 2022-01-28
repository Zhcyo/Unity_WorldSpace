using System;
using UnityEngine;

public class HumanMotion2 : MonoBehaviour
{
    private Human human;

    private Ragdoll ragdoll;

    public LayerMask grabLayers;

    public TorsoMuscles torso;

    public LegMuscles legs;

    public HandMuscles hands;

    public HumanMotion2()
    {
    }

    public static void AlignLook(HumanSegment segment, Quaternion targetRotation, float accelerationSpring, float damping)
    {
        float single = 0f;
        Vector3 vector3 = new Vector3();
        Quaternion quaternion = targetRotation * Quaternion.Inverse(segment.transform.rotation);
        quaternion.ToAngleAxis(out single, out vector3);
        if (single > 180f)
        {
            single -= 360f;
        }
        if (single < -180f)
        {
            single += 360f;
        }
        segment.rigidbody.AddTorque(((vector3 * single) * accelerationSpring) - (segment.rigidbody.angularVelocity * damping), (ForceMode)5);
    }

    public static void AlignToVector(Rigidbody body, Vector3 alignmentVector, Vector3 targetVector, float spring)
    {
        HumanMotion2.AlignToVector(body, alignmentVector, targetVector, spring, spring);
    }

    public static void AlignToVector(Rigidbody body, Vector3 alignmentVector, Vector3 targetVector, float spring, float maxTorque)
    {
        float single = 0.1f;
        Vector3 _angularVelocity = body.angularVelocity;
        Vector3 vector3 = Quaternion.AngleAxis(_angularVelocity.magnitude * 57.29578f * single, body.angularVelocity) * alignmentVector.normalized;
        Vector3 vector31 = Vector3.Cross(vector3.normalized, targetVector.normalized);
        Vector3 _normalized = vector31.normalized * Mathf.Asin(Mathf.Clamp01(vector31.magnitude));
        Vector3 vector32 = spring * _normalized;
        body.AddTorque(Vector3.ClampMagnitude(vector32, maxTorque), 0);
    }

    public static void AlignToVector(HumanSegment part, Vector3 alignmentVector, Vector3 targetVector, float spring)
    {
        HumanMotion2.AlignToVector(part.rigidbody, alignmentVector, targetVector, spring);
    }

    public void Initialize()
    {
        if (this.human != null)
        {
            return;
        }
        this.human = base.GetComponent<Human>();
        this.ragdoll = this.human.ragdoll;
        this.torso = new TorsoMuscles(this.human, this.ragdoll, this);
        this.legs = new LegMuscles(this.human, this.ragdoll, this);
        this.hands = new HandMuscles(this.human, this.ragdoll, this);
    }

    public void OnFixedUpdate()
    {
        this.hands.OnFixedUpdate();
        this.torso.OnFixedUpdate();
        this.legs.OnFixedUpdate(this.torso.feedbackForce);
    }
}