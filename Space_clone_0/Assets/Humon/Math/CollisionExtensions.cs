
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class CollisionExtensions
{
    public static void Analyze(this Collision collision, out Vector3 pos, out float impulse, out float normalVelocity, out float tangentVelocity, out PhysicMaterial mat1, out PhysicMaterial mat2, out int id2, out float volume2, out float pitch2)
    {
        ContactPoint[] _contacts = collision.contacts;
        id2 = 2147483647;
        volume2 = 1f;
        pitch2 = 1f;
        object obj = null;
        PhysicMaterial physicMaterial = (PhysicMaterial)obj;
        mat2 = (PhysicMaterial)obj;
        mat1 = physicMaterial;
        normalVelocity = 0f;
        tangentVelocity = 0f;
        pos = collision.contacts[0].point;
        Collider _otherCollider = null;
        int num = 0;
        Vector3 _relativeVelocity = collision.relativeVelocity;
        for (int i = 0; i < (int)_contacts.Length; i++)
        {
            ContactPoint contactPoint = _contacts[i];
            float single = Vector3.Dot(contactPoint.normal, _relativeVelocity);
            Vector3 _normal = _relativeVelocity - (contactPoint.normal * single);
            float _magnitude = _normal.magnitude;
            if (single > normalVelocity)
            {
                normalVelocity = single;
                tangentVelocity = _magnitude;
                pos = contactPoint.point;
                mat1 = contactPoint.thisCollider.sharedMaterial;
                mat2 = contactPoint.otherCollider.sharedMaterial;
                _otherCollider = contactPoint.otherCollider;
            }
            num++;
        }
        impulse = collision.impulse.magnitude;
        if (_otherCollider != null)
        {
            //CollisionAudioSensor componentInParent = _otherCollider.GetComponentInParent<CollisionAudioSensor>();
            //if (componentInParent != null)
            //{
            //    id2 = componentInParent.id;
            //    volume2 = componentInParent.volume;
            //    pitch2 = componentInParent.pitch;
            //}
        }
    }

    public static Vector3 GetImpulse(this Collision collision)
    {
        Vector3 _impulse = collision.impulse;
        if (Vector3.Dot(_impulse, collision.contacts[0].normal) >= 0f)
        {
            return _impulse;
        }
        return -_impulse;
    }

    public static Vector3 GetNormalTangentVelocitiesAndImpulse(this Collision collision, Rigidbody thisBody)
    {
        Vector3 _normal;
        ContactPoint[] _contacts = collision.contacts;
        float single = 0f;
        float single1 = 0f;
        int num = 0;
        Vector3 _relativeVelocity = collision.relativeVelocity;
        for (int i = 0; i < (int)_contacts.Length; i++)
        {
            ContactPoint contactPoint = _contacts[i];
            float single2 = Vector3.Dot(contactPoint.normal, _relativeVelocity);
            _normal = _relativeVelocity - (contactPoint.normal* single2);
            float _magnitude = _normal.magnitude;
            if (single2 > single)
            {
                single = single2;
                single1 = _magnitude;
            }
            num++;
        }
        _normal = collision.impulse;
        return new Vector3(single, single1, _normal.magnitude);
    }

    public static Vector3 GetPoint(this Collision collision)
    {
        return collision.contacts[0].point;
    }
}