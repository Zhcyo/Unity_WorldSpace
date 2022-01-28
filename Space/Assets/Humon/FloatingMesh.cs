
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class FloatingMesh : MonoBehaviour
{
    [Tooltip("Index for each vert we will pass through when applying any movement")]
    public static int counter;

    private int index;

    [Tooltip("The collision surfaces that connect to the water surface")]
    public Collider hull;

    [Tooltip("The mesh connected to the hull , mesh that needs to float ")]
    public Mesh mesh;


    [Tooltip("The density of the object , effects the reaction away from the water surface")]
    public float density = 1000f;

    [Tooltip("Vector for the direction to ignore when appplying force to the hull ")]
    public Vector3 ignoreHydrodynamicForce;

    [Tooltip("The pressure in a particular direction")]
    public float pressureLinear = 20f;

    [Tooltip("The square of the pressure")]
    public float pressureSquare = 10f;

    [Tooltip("The amount of suction in a particular direction ")]
    public float suctionLinear = 20f;

    [Tooltip("The square of the suction")]
    public float suctionSquare = 10f;

    [Tooltip("How fast the power should fall off over the hull")]
    public float falloffPower = 1f;

    [Tooltip("How much to be effected by the wind")]
    public float bendWind = 0.1f;

    private Rigidbody body;

    private Vector3[] meshVertices;

    private int[] triangles;

    [Tooltip("Use this in order to show the prints coming from the script")]
    public bool showDebug;

    private FloatingMesh.FloatingVertex[] vertices;

    private Vector3 center;

    private Vector3 resultantMoment;

    private Vector3 resultantForce;

    private Vector3 resultantStaticForce;

    public float horizontalHydrostaticTreshold;

    public FloatingMesh()
    {
    }

    private void AddForceAtPosition(Vector3 force, Vector3 pos, bool isDynamic)
    {
        this.resultantMoment += Vector3.Cross(pos - this.center, force);
        this.resultantForce += force;
        if (!float.IsNaN(this.resultantMoment.x) && !float.IsNaN(this.resultantMoment.y) && !float.IsNaN(this.resultantMoment.z))
        {
            if (!isDynamic)
            {
                this.resultantStaticForce += force;
            }
            return;
        }
        if (this.showDebug)
        {
            Debug.Log(string.Concat(base.name, " resultantMoment is NaN "));
        }
        this.resultantMoment = Vector3.zero;
    }

    private void ApplyForces()
    {
        if (this.triangles == null || this.triangles.Length == 0)
        {
            this.Start();
        }
        if (this.body.IsSleeping())
        {
            return;
        }
        Profiler.BeginSample("TransfromVerts", this);
       // this.TransformVertices(base.transform.get_localToWorldMatrix());
        Profiler.EndSample();
        Profiler.BeginSample("ApplyTriangleForces", this);
        this.center = this.body.worldCenterOfMass;
        Vector3 _zero = Vector3.zero;
        Vector3 vector3 = _zero;
        this.resultantStaticForce = _zero;
        Vector3 vector31 = vector3;
        vector3 = vector31;
        this.resultantForce = vector31;
        this.resultantMoment = vector3;
        for (int i = 0; i < (int)this.triangles.Length; i += 3)
        {
            this.ApplyTriangleForces(this.vertices[this.triangles[i]], this.vertices[this.triangles[i + 1]], this.vertices[this.triangles[i + 2]]);
        }
        if (this.horizontalHydrostaticTreshold > 0f)
        {
            Vector3 vector32 = new Vector3(this.resultantStaticForce.x, 0f, this.resultantStaticForce.z);
            if (vector32.magnitude < this.horizontalHydrostaticTreshold)
            {
                this.resultantForce -= vector32;
            }
        }
        if (this.ignoreHydrodynamicForce != Vector3.zero)
        {
            Vector3 vector33 = base.transform.TransformVector(this.ignoreHydrodynamicForce);
            float single = Vector3.Dot(this.resultantForce, vector33.normalized);
            if (single < 0f)
            {
                this.resultantForce = this.resultantForce - (single * vector33);
            }
        }
        this.resultantForce = Vector3.ClampMagnitude(this.resultantForce, this.body.mass * 100f);
        this.resultantMoment = Vector3.ClampMagnitude(this.resultantMoment, this.body.mass * 100f);
        if (!float.IsNaN(this.resultantForce.x) && !float.IsNaN(this.resultantForce.y) && !float.IsNaN(this.resultantForce.z))
        {
            //this.body.AddForce(this.resultantForce * (float)FloatingMeshSyn.skipFrames);
        }
        else if (this.showDebug)
        {
            Debug.Log(string.Concat(base.name, " Force NaN "));
        }
        if (!float.IsNaN(this.resultantForce.x) && !float.IsNaN(this.resultantForce.y) && !float.IsNaN(this.resultantForce.z))
        {
            //this.body.AddTorque(this.resultantMoment * (float)FloatingMesh.skipFrames);
        }
        else if (this.showDebug)
        {
            Debug.Log(string.Concat(base.name, " Torque NaN "));
        }
        Profiler.EndSample();
    }

    private void ApplySumbergedTriangleForces(FloatingMesh.FloatingVertex t, FloatingMesh.FloatingVertex m, FloatingMesh.FloatingVertex l, Vector3 areaNormal)
    {
        if (m.depth == t.depth || t.depth == l.depth)
        {
            this.TrianglePointingDown(t, m, l, areaNormal);
            return;
        }
        if (m.depth == l.depth)
        {
            this.TrianglePointingUp(t, m, l, areaNormal);
            return;
        }
        float single = (m.depth - l.depth) / (t.depth - l.depth);
        FloatingMesh.FloatingVertex floatingVertex = new FloatingMesh.FloatingVertex()
        {
            pos = l.pos + (single * (t.pos - l.pos)),
            velocity = l.velocity + (single * (t.velocity - l.velocity)),
            depth = m.depth
        };
        FloatingMesh.FloatingVertex floatingVertex1 = floatingVertex;
        this.TrianglePointingUp(t, m, floatingVertex1, areaNormal);
        this.TrianglePointingDown(floatingVertex1, m, l, areaNormal);
    }

    private void ApplyTriangleForces(FloatingMesh.FloatingVertex a, FloatingMesh.FloatingVertex b, FloatingMesh.FloatingVertex c)
    {
        FloatingMesh.FloatingVertex floatingVertex;
        FloatingMesh.FloatingVertex floatingVertex1;
        FloatingMesh.FloatingVertex floatingVertex2;
        FloatingMesh.FloatingVertex floatingVertex3;
        if (a.depth <= 0f && b.depth <= 0f && c.depth <= 0f)
        {
            return;
        }
        Vector3 vector3 = -Vector3.Cross(a.pos - b.pos, c.pos - b.pos) * 0.5f;
        if (a.depth <= b.depth && a.depth <= c.depth)
        {
            floatingVertex = a;
            if (b.depth >= c.depth)
            {
                floatingVertex1 = c;
                floatingVertex2 = b;
            }
            else
            {
                floatingVertex1 = b;
                floatingVertex2 = c;
            }
        }
        else if (b.depth > a.depth || b.depth > c.depth)
        {
            floatingVertex = c;
            if (a.depth >= b.depth)
            {
                floatingVertex1 = b;
                floatingVertex2 = a;
            }
            else
            {
                floatingVertex1 = a;
                floatingVertex2 = b;
            }
        }
        else
        {
            floatingVertex = b;
            if (a.depth >= c.depth)
            {
                floatingVertex1 = c;
                floatingVertex2 = a;
            }
            else
            {
                floatingVertex1 = a;
                floatingVertex2 = c;
            }
        }
        if (a.depth >= 0f && b.depth >= 0f && c.depth >= 0f)
        {
            this.ApplySumbergedTriangleForces(floatingVertex, floatingVertex1, floatingVertex2, vector3);
            return;
        }
        if (floatingVertex1.depth <= 0f)
        {
            float single = -floatingVertex2.depth / (floatingVertex1.depth - floatingVertex2.depth);
            float single1 = -floatingVertex2.depth / (floatingVertex.depth - floatingVertex2.depth);
            floatingVertex3 = new FloatingMesh.FloatingVertex()
            {
                pos = floatingVertex2.pos + (single1 * (floatingVertex.pos - floatingVertex2.pos)),
                velocity = floatingVertex2.velocity + (single1 * (floatingVertex.velocity - floatingVertex2.velocity)),
                depth = 0f
            };
            FloatingMesh.FloatingVertex floatingVertex4 = floatingVertex3;
            floatingVertex3 = new FloatingMesh.FloatingVertex()
            {
                pos = floatingVertex2.pos + (single * (floatingVertex1.pos - floatingVertex2.pos)),
                velocity = floatingVertex2.velocity + (single * (floatingVertex1.velocity - floatingVertex2.velocity)),
                depth = 0f
            };
            this.ApplySumbergedTriangleForces(floatingVertex4, floatingVertex3, floatingVertex2, vector3);
            return;
        }
        float single2 = -floatingVertex.depth / (floatingVertex1.depth - floatingVertex.depth);
        float single3 = -floatingVertex.depth / (floatingVertex2.depth - floatingVertex.depth);
        floatingVertex3 = new FloatingMesh.FloatingVertex()
        {
            pos = floatingVertex.pos + (single3 * (floatingVertex2.pos - floatingVertex.pos)),
            velocity = floatingVertex.velocity + (single3 * (floatingVertex2.velocity - floatingVertex.velocity)),
            depth = 0f
        };
        FloatingMesh.FloatingVertex floatingVertex5 = floatingVertex3;
        floatingVertex3 = new FloatingMesh.FloatingVertex()
        {
            pos = floatingVertex.pos + (single2 * (floatingVertex1.pos - floatingVertex.pos)),
            velocity = floatingVertex.velocity + (single2 * (floatingVertex1.velocity - floatingVertex.velocity)),
            depth = 0f
        };
        FloatingMesh.FloatingVertex floatingVertex6 = floatingVertex3;
        this.ApplySumbergedTriangleForces(floatingVertex6, floatingVertex1, floatingVertex2, vector3);
        this.ApplySumbergedTriangleForces(floatingVertex5, floatingVertex6, floatingVertex2, vector3);
    }

    private void DrawTriangleGizmos(FloatingMesh.FloatingVertex t, FloatingMesh.FloatingVertex m, FloatingMesh.FloatingVertex l)
    {
    }

    private void FixedUpdate()
    {
        
        
        this.ApplyForces();
    }

    private void Hydrodynamic(FloatingMesh.FloatingVertex a, FloatingMesh.FloatingVertex b, FloatingMesh.FloatingVertex c, Vector3 areaNormal)
    {
        Vector3 vector3;
        if (this.body == null)
        {
            return;
        }
        float _magnitude = areaNormal.magnitude;
        Vector3 _normalized = areaNormal / _magnitude;
        _magnitude /= 3f;
        FloatingMesh.FloatingVertex floatingVertex = a;
        Vector3 vector31 = floatingVertex.velocity;
        float single = vector31.magnitude;
        if (single != 0f)
        {
            vector31 /= single;
        }
        float single1 = vector31.x * _normalized.x + vector31.y * _normalized.y + vector31.z * _normalized.z;
        float single2 = 0f;
        if (this.bendWind > 0f)
        {
            vector3 = _normalized - (vector31 * this.bendWind);
            _normalized = vector3.normalized;
        }
        single2 = (single1 <= 0f ? (this.suctionLinear * single + this.suctionSquare * single * single) * _magnitude * (this.falloffPower != 1f ? Mathf.Pow(-single1, this.falloffPower) : -single1) : -(this.pressureLinear * single + this.pressureSquare * single * single) * _magnitude * (this.falloffPower != 1f ? Mathf.Pow(single1, this.falloffPower) : single1));
        this.AddForceAtPosition(single2 * _normalized, floatingVertex.pos, true);
        FloatingMesh.FloatingVertex floatingVertex1 = b;
        Vector3 vector32 = floatingVertex1.velocity;
        float _magnitude1 = vector32.magnitude;
        if (_magnitude1 != 0f)
        {
            vector32 /= _magnitude1;
        }
        float single3 = vector32.x * _normalized.x + vector32.y * _normalized.y + vector32.z * _normalized.z;
        float single4 = 0f;
        if (this.bendWind > 0f)
        {
            vector3 = _normalized - (vector32 * this.bendWind);
            _normalized = vector3.normalized;
        }
        single4 = (single3 <= 0f ? (this.suctionLinear * _magnitude1 + this.suctionSquare * _magnitude1 * _magnitude1) * _magnitude * (this.falloffPower != 1f ? Mathf.Pow(-single3, this.falloffPower) : -single3) : -(this.pressureLinear * _magnitude1 + this.pressureSquare * _magnitude1 * _magnitude1) * _magnitude * (this.falloffPower != 1f ? Mathf.Pow(single3, this.falloffPower) : single3));
        this.AddForceAtPosition(single4 * _normalized, floatingVertex1.pos, true);
        FloatingMesh.FloatingVertex floatingVertex2 = c;
        Vector3 vector33 = floatingVertex2.velocity;
        float _magnitude2 = vector33.magnitude;
        if (_magnitude2 != 0f)
        {
            vector33 /= _magnitude2;
        }
        float single5 = vector33.x * _normalized.x + vector33.y * _normalized.y + vector33.z * _normalized.z;
        float single6 = 0f;
        if (this.bendWind > 0f)
        {
            vector3 = _normalized - (vector33 * this.bendWind);
            _normalized = vector3.normalized;
        }
        single6 = (single5 <= 0f ? (this.suctionLinear * _magnitude2 + this.suctionSquare * _magnitude2 * _magnitude2) * _magnitude * (this.falloffPower != 1f ? Mathf.Pow(-single5, this.falloffPower) : -single5) : -(this.pressureLinear * _magnitude2 + this.pressureSquare * _magnitude2 * _magnitude2) * _magnitude * (this.falloffPower != 1f ? Mathf.Pow(single5, this.falloffPower) : single5));
        this.AddForceAtPosition(single6 * _normalized, floatingVertex2.pos, true);
    }

    private void Hydrodynamic(FloatingMesh.FloatingVertex v, float surface, Vector3 normal)
    {
        if (this.showDebug)
        {
            Debug.Log(string.Concat(base.name, " Private Hydro dynamic "));
        }
        Vector3 vector3 = v.velocity;
        float _magnitude = vector3.magnitude;
        if (_magnitude != 0f)
        {
            vector3 /= _magnitude;
        }
        float single = vector3.x * normal.x + vector3.y * normal.y + vector3.z * normal.z;
        float single1 = 0f;
        if (this.bendWind > 0f)
        {
            Vector3 vector31 = normal - (vector3 * this.bendWind);
            normal = vector31.normalized;
        }
        single1 = (single <= 0f ? (this.suctionLinear * _magnitude + this.suctionSquare * _magnitude * _magnitude) * surface * (this.falloffPower != 1f ? Mathf.Pow(-single, this.falloffPower) : -single) : -(this.pressureLinear * _magnitude + this.pressureSquare * _magnitude * _magnitude) * surface * (this.falloffPower != 1f ? Mathf.Pow(single, this.falloffPower) : single));
        this.AddForceAtPosition(single1 * normal, v.pos, true);
    }

    private void Hydrostatic(FloatingMesh.FloatingVertex a, FloatingMesh.FloatingVertex b, FloatingMesh.FloatingVertex c, Vector3 areaNormal)
    {
        Vector3 vector3 = c.pos;
        Vector3 vector31 = (a.pos + b.pos) / 2f;
        Vector3 vector32 = a.pos;
        Vector3 vector33 = b.pos - a.pos;
        //vector33 = Mathf.ProjectPointOnLine(vector32,vector33.normalized,c.pos) - vector3;
        float _magnitude = vector33.magnitude;
        vector33 = a.pos - b.pos;
        float single = vector33.magnitude;
        float single1 = c.depth;
        float single2 = a.depth;
        float single3 = this.density * 9.81f * single * _magnitude * (single1 / 2f + (single2 - single1) / 3f);
        float single4 = this.density * 9.81f * single * _magnitude * _magnitude * (single1 / 3f + (single2 - single1) / 4f) / single3;
        if (single < 0.001f || _magnitude < 0.001f || single3 < 0.0001f)
        {
            return;
        }
        Vector3 _normalized = -single3 * areaNormal.normalized;
        Vector3 vector34 = vector3 + (((vector31 - vector3) * single4) / _magnitude);
        this.AddForceAtPosition(_normalized, vector34, false);
    }

    private void Start()
    {
        if (this.showDebug)
        {
            Debug.Log(string.Concat(base.name, " Started "));
        }
        int num = FloatingMesh.counter;
        FloatingMesh.counter = num + 1;
        this.index = num;
        this.body = base.GetComponent<Rigidbody>();
        if (this.body == null)
        {
            throw new InvalidOperationException("Floater needs a Rigidbody");
        }
        if (this.hull == null)
        {
            this.hull = base.GetComponentInChildren<Collider>();
        }
        if (this.hull == null)
        {
            throw new InvalidOperationException("MeshCollider");
        }
      
      
        if (this.mesh == null && this.hull is MeshCollider)
        {
            this.mesh = (this.hull as MeshCollider).sharedMesh;
        }
        this.triangles = this.mesh.triangles;
        this.meshVertices = this.mesh.vertices;
        List<Vector3> vector3s = new List<Vector3>();
        for (int i = 0; i < (int)this.triangles.Length; i++)
        {
            Vector3 vector3 = this.meshVertices[this.triangles[i]];
            int count = vector3s.IndexOf(vector3);
            if (count < 0)
            {
                count = vector3s.Count;
                vector3s.Add(vector3);
            }
            this.triangles[i] = count;
        }
        this.meshVertices = vector3s.ToArray();
        for (int j = 0; j < (int)this.meshVertices.Length; j++)
        {
            this.meshVertices[j] = base.transform.InverseTransformPoint(this.hull.transform.TransformPoint(this.meshVertices[j]));
        }
        this.vertices = new FloatingMesh.FloatingVertex[(int)this.meshVertices.Length];
    }

    private void TransformVertices(Matrix4x4 localToWorldMatrix)
    {
        float single = 0f;
       // Vector3 vector3 = Current.Sample(this.body.worldCenterOfMass, out single);
        for (int i = 0; i < (int)this.meshVertices.Length; i++)
        {
            Vector3 vector31 = localToWorldMatrix.MultiplyPoint3x4(this.meshVertices[i]);
            Vector3 _zero = Vector3.zero;
            if (this.body != null)
            {
                _zero = this.body.GetPointVelocity(vector31);
            }
            Vector3 _zero1 = Vector3.zero;
            float single1 = -1f;
          
            FloatingMesh.FloatingVertex[] floatingVertexArray = this.vertices;
            FloatingMesh.FloatingVertex floatingVertex = new FloatingMesh.FloatingVertex()
            {
                pos = vector31,
                velocity = _zero - _zero1,
                depth = single1
            };
            floatingVertexArray[i] = floatingVertex;
        }
    }

    private void TrianglePointingDown(FloatingMesh.FloatingVertex t, FloatingMesh.FloatingVertex m, FloatingMesh.FloatingVertex l, Vector3 areaNormal)
    {
        this.Hydrostatic(t, m, l, areaNormal);
        this.Hydrodynamic(m, l, t, areaNormal);
    }

    private void TrianglePointingUp(FloatingMesh.FloatingVertex t, FloatingMesh.FloatingVertex m, FloatingMesh.FloatingVertex l, Vector3 areaNormal)
    {
        this.Hydrostatic(m, l, t, areaNormal);
        this.Hydrodynamic(m, l, t, areaNormal);
    }

    private struct FloatingVertex
    {
        public Vector3 pos;

        public Vector3 velocity;

        public float depth;
    }
}