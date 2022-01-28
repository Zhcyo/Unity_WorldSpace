using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IGrabbable
{
    void OnGrab();

    void OnRelease();
}
public interface IGrabbableWithInfo
{
    void OnGrab(GrabManager grabbedBy);

    void OnRelease(GrabManager releasedBy);
}

public class GrabManager : MonoBehaviour
{
    public List<GameObject> grabbedObjects = new List<GameObject>();

    private static Dictionary<GameObject, Vector3> grabStartPositions;

    private static List<GrabManager> all;

    private Human human;

    public bool hasGrabbed
    {
        get
        {
            return this.grabbedObjects.Count > 0;
        }
    }

    public Human Human
    {
        get
        {
            return this.human;
        }
    }

    static GrabManager()
    {
        GrabManager.grabStartPositions = new Dictionary<GameObject, Vector3>();
        GrabManager.all = new List<GrabManager>();
    }

    public GrabManager()
    {
    }

    private void CheckCarryEnd(GameObject grabObject)
    {
        bool flag = true;
        for (int i = 0; i < GrabManager.all.Count; i++)
        {
            flag &= !GrabManager.all[i].grabbedObjects.Contains(grabObject);
        }
        if (flag)
        {
            IGrabbable componentInParent = grabObject.GetComponentInParent<IGrabbable>();
            if (componentInParent != null)
            {
                componentInParent.OnRelease();
            }
            IGrabbableWithInfo grabbableWithInfo = grabObject.GetComponentInParent<IGrabbableWithInfo>();
            if (grabbableWithInfo != null)
            {
                grabbableWithInfo.OnRelease(this);
            }
            else
            {
            }
            if (grabObject != null && GrabManager.grabStartPositions.ContainsKey(grabObject))
            {
                Vector2 vector2 = (GrabManager.grabStartPositions[grabObject] - grabObject.transform.position);
                float _magnitude = vector2.magnitude;
                if (_magnitude > 0.1f)
                {
                    //StatsAndAchievements.AddCarry(this.human, _magnitude);
                }
            }
            GrabManager.grabStartPositions.Remove(grabObject);
            ////if (!CheatCodes.throwCheat)
            ////{
            ////    Human human = grabObject.GetComponentInParent<Human>();
            ////    if (human != null)
            ////    {
            ////        human.grabbedByHuman = null;
            ////    }
            ////}
        }
    }

    public void DistributeForce(Vector3 force)
    {
        for (int i = 0; i < this.grabbedObjects.Count; i++)
        {
            Rigidbody componentInParent = this.grabbedObjects[i].GetComponentInParent<Rigidbody>();
            if (componentInParent != null)
            {
                componentInParent.AddForce(force / (float)this.grabbedObjects.Count, 0);
            }
        }
    }

    public static Human GrabbedBy(GameObject item)
    {
        for (int i = 0; i < GrabManager.all.Count; i++)
        {
            if (GrabManager.all[i].IsGrabbed(item))
            {
                return GrabManager.all[i].human;
            }
        }
        return null;
    }

    public bool IsGrabbed(GameObject item)
    {
        for (int i = this.grabbedObjects.Count - 1; i >= 0; i--)
        {
            GameObject gameObject = this.grabbedObjects[i];
            if (gameObject != null)
            {
                for (Transform j = gameObject.transform; j != null; j = j.parent)
                {
                    if (j.gameObject== item)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public static bool IsGrabbedAny(GameObject item)
    {
        for (int i = 0; i < GrabManager.all.Count; i++)
        {
            if (GrabManager.all[i].IsGrabbed(item))
            {
                return true;
            }
        }
        return false;
    }

    public void ObjectGrabbed(GameObject grabObject)
    {
        bool flag = true;
        for (int i = 0; i < GrabManager.all.Count; i++)
        {
            flag &= !GrabManager.all[i].grabbedObjects.Contains(grabObject);
        }
        this.grabbedObjects.Add(grabObject);
        if (flag)
        {
            IGrabbable componentInParent = grabObject.GetComponentInParent<IGrabbable>();
            if (componentInParent != null)
            {
                componentInParent.OnGrab();
            }
            IGrabbableWithInfo grabbableWithInfo = grabObject.GetComponentInParent<IGrabbableWithInfo>();
            if (grabbableWithInfo != null)
            {
                grabbableWithInfo.OnGrab(this);
            }
            else
            {
            }
            GrabManager.grabStartPositions[grabObject] = grabObject.transform.position;
            Human human = grabObject.GetComponentInParent<Human>();
            if (human != null)
            {
                human.grabbedByHuman = this.human;
            }
        }
    }

    public void ObjectReleased(GameObject grabObject)
    {
        this.grabbedObjects.Remove(grabObject);
        this.CheckCarryEnd(grabObject);
    }

    private void OnDisable()
    {
        GrabManager.all.Remove(this);
    }

    private void OnEnable()
    {
        GrabManager.all.Add(this);
        this.human = base.GetComponentInParent<Human>();
    }

    public static void Release(GameObject item, float delay = 0f)
    {
        for (int i = 0; i < Human.all.Count; i++)
        {
            Human.all[i].ReleaseGrab(item, delay);
        }
    }

    internal void Reset()
    {
        GameObject[] array = this.grabbedObjects.ToArray();
        this.grabbedObjects.Clear();
        for (int i = 0; i < (int)array.Length; i++)
        {
            GameObject gameObject = array[0];
            if (gameObject != null)
            {
                this.CheckCarryEnd(gameObject);
            }
        }
    }
}

