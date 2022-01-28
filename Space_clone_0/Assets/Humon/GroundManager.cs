using System;
using System.Collections.Generic;
using UnityEngine;

public class GroundManager : MonoBehaviour
{
    public List<GameObject> groundObjects = new List<GameObject>();

    private List<Rigidbody> groundRigids = new List<Rigidbody>();

    private static List<GroundManager> all;

    private static Dictionary<GroundVehicle, Vector3> vehicleStartPositions;

    private static Dictionary<FloatingMesh, Vector3> shipStartPositions;

    private List<GameObject> removedObjects = new List<GameObject>();

    public float surfaceAngle;

    public Vector3 groudSpeed
    {
        get
        {
            Vector3 _zero = Vector3.zero;
            for (int i = 0; i < this.groundRigids.Count; i++)
            {
                Rigidbody item = this.groundRigids[i];
                if (item != null)
                {
                    Vector3 _velocity = item.velocity;
                    if (Mathf.Abs(_zero.x) < Mathf.Abs(_velocity.x))
                    {
                        _zero.x = _velocity.x;
                    }
                    if (Mathf.Abs(_zero.y) < Mathf.Abs(_velocity.y))
                    {
                        _zero.y = _velocity.y;
                    }
                    if (Mathf.Abs(_zero.z) < Mathf.Abs(_velocity.z))
                    {
                        _zero.z = _velocity.z;
                    }
                }
            }
            return _zero;
        }
    }

    public bool onGround
    {
        get
        {
            return this.groundObjects.Count > 0;
        }
    }

    static GroundManager()
    {
        GroundManager.all = new List<GroundManager>();
        GroundManager.vehicleStartPositions = new Dictionary<GroundVehicle, Vector3>();
        GroundManager.shipStartPositions = new Dictionary<FloatingMesh, Vector3>();
    }

    public GroundManager()
    {
    }

    private static void CheckDriveEnd<T>(GameObject groundObject, Dictionary<T, Vector3> startPositions)
    where T : MonoBehaviour
    {
        T componentInParent = groundObject.GetComponentInParent<T>();
        if (componentInParent != null && startPositions.ContainsKey(componentInParent))
        {
            bool flag = true;
            for (int i = 0; i < GroundManager.all.Count; i++)
            {
                int num = 0;
                while (num < GroundManager.all[i].groundObjects.Count)
                {
                    GameObject item = GroundManager.all[i].groundObjects[num];
                    if (item == null || !item.transform.IsChildOf(componentInParent.transform))
                    {
                        num++;
                    }
                    else
                    {
                        flag = false;
                        break;
                    }
                }
            }
            if (flag)
            {
                Vector2 vector2 = (startPositions[componentInParent] - groundObject.transform.position);
                float _magnitude = vector2.magnitude;
                if (_magnitude > 0f) { 
            
                //    if (typeof(T) != typeof(FloatingMesh))
                //    {
                //        StatsAndAchievements.AddDrive(_magnitude);
                //    }
                //    else
                //    {
                //        StatsAndAchievements.AddShip(_magnitude);
                //    }
                }
                startPositions.Remove(componentInParent);
            }
        }
    }

    private void CheckDriveUpdate<T>(Dictionary<T, Vector3> startPositions)
    where T : MonoBehaviour
    {
        for (int i = 0; i < this.groundObjects.Count; i++)
        {
            GameObject item = this.groundObjects[i];
            if (item != null)
            {
                T componentInParent = item.GetComponentInParent<T>();
                if (componentInParent != null && startPositions.ContainsKey(componentInParent))
                {
                    Vector2 vector2 = (startPositions[componentInParent] - item.transform.position);
                    float _magnitude = vector2.magnitude;
                    if (_magnitude > 5f)
                    {
                        //if (typeof(T) != typeof(FloatingMesh))
                        //{
                        //    StatsAndAchievements.AddDrive(_magnitude);
                        //}
                        //else
                        //{
                        //    StatsAndAchievements.AddShip(_magnitude);
                        //}
                        startPositions[componentInParent] = item.transform.position;
                    }
                }
            }
        }
    }

    internal void DecaySurfaceAngle()
    {
        this.surfaceAngle = Mathf.Min(90f, this.surfaceAngle + 90f * Time.fixedDeltaTime);
    }

    public void DistributeForce(Vector3 force, Vector3 pos)
    {
        for (int i = 0; i < this.groundRigids.Count; i++)
        {
            Rigidbody item = this.groundRigids[i];
            if (item != null)
            {
                item.AddForceAtPosition(Vector3.ClampMagnitude(force / (float)this.groundRigids.Count, item.mass / Time.fixedDeltaTime * 10f), pos, 0);
            }
        }
    }

    public bool IsStanding(GameObject item)
    {
        for (int i = this.groundObjects.Count - 1; i >= 0; i--)
        {
            GameObject gameObject = this.groundObjects[i];
            if (gameObject != null)
            {
                for (Transform j = gameObject.transform; j != null; j = j.parent)
                {
                    if (j.gameObject == item)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public static bool IsStandingAny(GameObject item)
    {
        for (int i = 0; i < GroundManager.all.Count; i++)
        {
            if (GroundManager.all[i].IsStanding(item))
            {
                return true;
            }
        }
        return false;
    }

    public void ObjectEnter(GameObject groundObject)
    {
        if (this.groundObjects.Contains(groundObject))
        {
            return;
        }
        this.removedObjects.Remove(groundObject);
        this.groundObjects.Add(groundObject);
        this.groundRigids.Add(groundObject.GetComponentInParent<Rigidbody>());
        FloatingMesh componentInParent = groundObject.GetComponentInParent<FloatingMesh>();
        if (componentInParent != null && !GroundManager.shipStartPositions.ContainsKey(componentInParent))
        {
            GroundManager.shipStartPositions[componentInParent] = groundObject.transform.position;
        }
        GroundVehicle _position = groundObject.GetComponentInParent<GroundVehicle>();
        if (_position != null && !GroundManager.vehicleStartPositions.ContainsKey(_position))
        {
            GroundManager.vehicleStartPositions[_position] = groundObject.transform.position;
        }
    }

    private void OnDisable()
    {
        GroundManager.all.Remove(this);
    }

    private void OnEnable()
    {
        GroundManager.all.Add(this);
    }

    public void PostFixedUpdate()
    {
        for (int i = 0; i < this.removedObjects.Count; i++)
        {
            if (GroundManager.shipStartPositions.Count > 0)
            {
                GroundManager.CheckDriveEnd<FloatingMesh>(this.removedObjects[i], GroundManager.shipStartPositions);
            }
            if (GroundManager.vehicleStartPositions.Count > 0)
            {
                GroundManager.CheckDriveEnd<GroundVehicle>(this.removedObjects[i], GroundManager.vehicleStartPositions);
            }
        }
        List<GameObject> gameObjects = this.removedObjects;
        this.removedObjects = this.groundObjects;
        this.groundObjects = gameObjects;
        this.groundObjects.Clear();
        this.groundRigids.Clear();
    }

    public void ReportSurfaceAngle(float surfaceAngle)
    {
        this.surfaceAngle = Mathf.Min(surfaceAngle, this.surfaceAngle);
    }

    internal void Reset()
    {
        GameObject[] array = this.groundObjects.ToArray();
        this.groundObjects.Clear();
        this.groundRigids.Clear();
        for (int i = 0; i < (int)array.Length; i++)
        {
            GameObject gameObject = array[0];
            if (gameObject != null)
            {
                GroundManager.CheckDriveEnd<FloatingMesh>(gameObject, GroundManager.shipStartPositions);
                GroundManager.CheckDriveEnd<GroundVehicle>(gameObject, GroundManager.vehicleStartPositions);
            }
        }
    }

    internal static void ResetOnSceneUnload()
    {
        foreach (GroundManager groundManager in GroundManager.all)
        {
            groundManager.groundRigids.Clear();
            groundManager.groundObjects.Clear();
        }
        GroundManager.vehicleStartPositions.Clear();
        GroundManager.shipStartPositions.Clear();
    }

    public void Update()
    {
        if (GroundManager.shipStartPositions.Count > 0)
        {
            this.CheckDriveUpdate<FloatingMesh>(GroundManager.shipStartPositions);
        }
        if (GroundManager.vehicleStartPositions.Count > 0)
        {
            this.CheckDriveUpdate<GroundVehicle>(GroundManager.vehicleStartPositions);
        }
    }
}