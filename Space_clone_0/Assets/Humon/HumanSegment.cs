using System;
using UnityEngine;
[System.Serializable]
public class HumanSegment
{
    public Transform transform;

    public Collider collider;

    public Rigidbody rigidbody;

    public Quaternion startupRotation;

    public CollisionSensor sensor;

    public Transform skeleton;

    public HumanSegment parent;

    public Matrix4x4 bindPose;
    public HumanSegment(){}
    public HumanSegment(Transform transform, Collider collider,Rigidbody rigidbody, Quaternion startupRotation, CollisionSensor sensor,Matrix4x4 bindPose)
    {
        this.transform = transform;
        this.collider = collider;
        this.rigidbody = rigidbody;
        this.startupRotation = startupRotation;
        this.sensor = sensor;
        this.bindPose = bindPose;
    }
}