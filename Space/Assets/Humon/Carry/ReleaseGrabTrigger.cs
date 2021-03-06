using System;
using UnityEngine;

public class ReleaseGrabTrigger : MonoBehaviour
{
    public ReleaseGrabTrigger()
    {
    }

    public void OnTriggerEnter(Collider other)
    {
        CollisionSensor component = other.GetComponent<CollisionSensor>();
        if (component != null)
        {
            component.BlockGrab(this);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        CollisionSensor component = other.GetComponent<CollisionSensor>();
        if (component != null)
        {
            component.UnblockBlockGrab();
        }
    }
}