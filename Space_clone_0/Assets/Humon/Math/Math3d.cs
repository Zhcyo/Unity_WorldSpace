using System;
using UnityEngine;

public static class Math3d
{
    public static Vector3 AddVectorLength(Vector3 vector, float size)
    {
        float single = Vector3.Magnitude(vector);
        single += size;
        return Vector3.Scale(Vector3.Normalize(vector), new Vector3(single, single, single));
    }

    public static float AngleVectorPlane(Vector3 vector, Vector3 normal)
    {
        float single = (float)Math.Acos((double)Vector3.Dot(vector, normal));
        return 1.57079637f - single;
    }

    public static bool AreLineSegmentsCrossing(Vector3 pointA1, Vector3 pointA2, Vector3 pointB1, Vector3 pointB2)
    {
        Vector3 vector3;
        Vector3 vector31;
        Vector3 vector32 = pointA2 - pointA1;
        Vector3 vector33 = pointB2 - pointB1;
        if (!Math3d.ClosestPointsOnTwoLines(out vector3, out vector31, pointA1, vector32.normalized, pointB1, vector33.normalized))
        {
            return false;
        }
        int num = Math3d.PointOnWhichSideOfLineSegment(pointA1, pointA2, vector3);
        int num1 = Math3d.PointOnWhichSideOfLineSegment(pointB1, pointB2, vector31);
        if (num == 0 && num1 == 0)
        {
            return true;
        }
        return false;
    }

    public static bool ClosestPointsOnLineSegment(out Vector3 closestPointLine, out Vector3 closestPointSegment, out float lineT, out float segmentT, Vector3 linePoint, Vector3 lineVec, Vector3 segmentPoint1, Vector3 segmentPoint2)
    {
        Vector3 vector3 = segmentPoint2 - segmentPoint1;
        closestPointLine = Vector3.zero;
        closestPointSegment = Vector3.zero;
        segmentT = 0f;
        lineT = 0f;
        float single = Vector3.Dot(lineVec, lineVec);
        float single1 = Vector3.Dot(lineVec, vector3);
        float single2 = Vector3.Dot(vector3, vector3);
        float single3 = single * single2 - single1 * single1;
        if (single3 == 0f)
        {
            return false;
        }
        Vector3 vector31 = linePoint - segmentPoint1;
        float single4 = Vector3.Dot(lineVec, vector31);
        float single5 = Vector3.Dot(vector3, vector31);
        float single6 = (single1 * single5 - single4 * single2) / single3;
        float single7 = (single * single5 - single4 * single1) / single3;
        lineT = single6;
        segmentT = Mathf.Clamp01(single7);
        closestPointLine = linePoint + (lineVec * single6);
        closestPointSegment = segmentPoint1 + (vector3 * segmentT);
        return true;
    }

    public static bool ClosestPointsOnTwoLines(out Vector3 closestPointLine1, out Vector3 closestPointLine2, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
    {
        closestPointLine1 = Vector3.zero;
        closestPointLine2 = Vector3.zero;
        float single = Vector3.Dot(lineVec1, lineVec1);
        float single1 = Vector3.Dot(lineVec1, lineVec2);
        float single2 = Vector3.Dot(lineVec2, lineVec2);
        float single3 = single * single2 - single1 * single1;
        if (single3 == 0f)
        {
            return false;
        }
        Vector3 vector3 = linePoint1 - linePoint2;
        float single4 = Vector3.Dot(lineVec1, vector3);
        float single5 = Vector3.Dot(lineVec2, vector3);
        float single6 = (single1 * single5 - single4 * single2) / single3;
        float single7 = (single * single5 - single4 * single1) / single3;
        closestPointLine1 = linePoint1 + (lineVec1 * single6);
        closestPointLine2 = linePoint2 + (lineVec2 * single7);
        return true;
    }

    public static bool ClosestPointsOnTwoLines(out float s, out float t, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
    {
        float single = 0f;
        float single1 = single;
        s = single;
        t = single1;
        float single2 = Vector3.Dot(lineVec1, lineVec1);
        float single3 = Vector3.Dot(lineVec1, lineVec2);
        float single4 = Vector3.Dot(lineVec2, lineVec2);
        float single5 = single2 * single4 - single3 * single3;
        if (single5 == 0f)
        {
            return false;
        }
        Vector3 vector3 = linePoint1 - linePoint2;
        float single6 = Vector3.Dot(lineVec1, vector3);
        float single7 = Vector3.Dot(lineVec2, vector3);
        s = (single3 * single7 - single6 * single4) / single5;
        t = (single2 * single7 - single6 * single3) / single5;
        return true;
    }

    public static float DotProductAngle(Vector3 vec1, Vector3 vec2)
    {
        double num = (double)Vector3.Dot(vec1, vec2);
        if (num < -1)
        {
            num = -1;
        }
        if (num > 1)
        {
            num = 1;
        }
        return (float)Math.Acos(num);
    }

    public static Vector3 GetForwardVector(Quaternion q)
    {
        return q * Vector3.forward;
    }

    public static Vector3 GetRightVector(Quaternion q)
    {
        return q * Vector3.right;
    }

    public static Vector3 GetUpVector(Quaternion q)
    {
        return q * Vector3.up;
    }

    public static bool IsLineInRectangle(Vector3 linePoint1, Vector3 linePoint2, Vector3 rectA, Vector3 rectB, Vector3 rectC, Vector3 rectD)
    {
        bool flag = false;
        bool flag1 = Math3d.IsPointInRectangle(linePoint1, rectA, rectC, rectB, rectD);
        if (!flag1)
        {
            flag = Math3d.IsPointInRectangle(linePoint2, rectA, rectC, rectB, rectD);
        }
        if (flag1 || flag)
        {
            return true;
        }
        if (Math3d.AreLineSegmentsCrossing(linePoint1, linePoint2, rectA, rectB) | Math3d.AreLineSegmentsCrossing(linePoint1, linePoint2, rectB, rectC) | Math3d.AreLineSegmentsCrossing(linePoint1, linePoint2, rectC, rectD) | Math3d.AreLineSegmentsCrossing(linePoint1, linePoint2, rectD, rectA))
        {
            return true;
        }
        return false;
    }

    public static bool IsPointInRectangle(Vector3 point, Vector3 rectA, Vector3 rectC, Vector3 rectB, Vector3 rectD)
    {
        Vector3 vector3 = rectC - rectA;
        float _magnitude = -(vector3.magnitude / 2f);
        vector3 = Math3d.AddVectorLength(vector3, _magnitude);
        Vector3 vector31 = rectA + vector3;
        Vector3 vector32 = rectB - rectA;
        float single = vector32.magnitude / 2f;
        Vector3 vector33 = rectD - rectA;
        float _magnitude1 = vector33.magnitude / 2f;
        vector3 = Math3d.ProjectPointOnLine(vector31, vector32.normalized, point) - point;
        float single1 = vector3.magnitude;
        vector3 = Math3d.ProjectPointOnLine(vector31, vector33.normalized, point) - point;
        if (vector3.magnitude <= single && single1 <= _magnitude1)
        {
            return true;
        }
        return false;
    }

    public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
    {
        intersection = Vector3.zero;
        Vector3 vector3 = linePoint2 - linePoint1;
        Vector3 vector31 = Vector3.Cross(lineVec1, lineVec2);
        Vector3 vector32 = Vector3.Cross(vector3, lineVec2);
        float single = Vector3.Dot(vector3, vector31);
        if (single >= 1E-05f || single <= -1E-05f)
        {
            return false;
        }
        float single1 = Vector3.Dot(vector32, vector31) / vector31.sqrMagnitude;
        if (single1 < 0f || single1 > 1f)
        {
            return false;
        }
        intersection = linePoint1 + (lineVec1 * single1);
        return true;
    }

    public static bool LineLineIntersection(out float tLine1, out float tLine2, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
    {
        tLine1 = float.PositiveInfinity;
        tLine2 = float.PositiveInfinity;
        Vector3 vector3 = linePoint2 - linePoint1;
        Vector3 vector31 = Vector3.Cross(lineVec1, lineVec2);
        Vector3 vector32 = Vector3.Cross(vector3, lineVec2);
        Vector3 vector33 = Vector3.Cross(vector3, lineVec1);
        float single = Vector3.Dot(vector3, vector31);
        if (single >= 1E-05f || single <= -1E-05f)
        {
            return false;
        }
        tLine1 = Vector3.Dot(vector32, vector31) / vector31.sqrMagnitude;
        tLine2 = Vector3.Dot(vector33, vector31) / vector31.sqrMagnitude;
        return true;
    }

    public static bool LinePlaneIntersection(out Vector3 intersection, Vector3 linePoint, Vector3 lineVec, Vector3 planeNormal, Vector3 planePoint)
    {
        intersection = Vector3.zero;
        float single = Vector3.Dot(planePoint - linePoint, planeNormal);
        float single1 = Vector3.Dot(lineVec, planeNormal);
        if (single1 == 0f)
        {
            return false;
        }
        Vector3 vector3 = Math3d.SetVectorLength(lineVec, single / single1);
        intersection = linePoint + vector3;
        return true;
    }

    public static void LookRotationExtended(ref GameObject gameObjectInOut, Vector3 alignWithVector, Vector3 alignWithNormal, Vector3 customForward, Vector3 customUp)
    {
        Quaternion quaternion = Quaternion.LookRotation(alignWithVector, alignWithNormal);
        Quaternion quaternion1 = Quaternion.LookRotation(customForward, customUp);
        gameObjectInOut.transform.rotation=(quaternion * Quaternion.Inverse(quaternion1));
    }

    public static float MouseDistanceToCircle(Vector3 point, float radius)
    {
        Camera camera;
        camera = (Camera.current == null ? Camera.main : Camera.current);
        Vector3 vector3 = new Vector3(Event.current.mousePosition.x, (float)camera.pixelHeight - Event.current.mousePosition.y, 0f);
        Vector3 screenPoint = camera.WorldToScreenPoint(point);
        screenPoint = new Vector3(screenPoint.x, screenPoint.y, 0f);
        return (screenPoint - vector3).magnitude - radius;
    }

    public static float MouseDistanceToLine(Vector3 linePoint1, Vector3 linePoint2)
    {
        Camera camera;
        camera = (Camera.current == null ? Camera.main : Camera.current);
        Vector3 vector3 = new Vector3(Event.current.mousePosition.x, (float)camera.pixelHeight - Event.current.mousePosition.y, 0f);
        Vector3 screenPoint = camera.WorldToScreenPoint(linePoint1);
        Vector3 screenPoint1 = camera.WorldToScreenPoint(linePoint2);
        Vector3 vector31 = Math3d.ProjectPointOnLineSegment(screenPoint, screenPoint1, vector3);
        vector31 = new Vector3(vector31.x, vector31.y, 0f);
        return (vector31 - vector3).magnitude;
    }

    public static void PlaneFrom3Points(out Vector3 planeNormal, out Vector3 planePoint, Vector3 pointA, Vector3 pointB, Vector3 pointC)
    {
        Vector3 vector3;
        planeNormal = Vector3.zero;
        planePoint = Vector3.zero ;
        Vector3 vector31 = pointB - pointA;
        Vector3 vector32 = pointC - pointA;
        planeNormal = Vector3.Normalize(Vector3.Cross(vector31, vector32));
        Vector3 vector33 = pointA + (vector31 / 2f);
        Vector3 vector34 = pointA + (vector32 / 2f);
        Vector3 vector35 = pointC - vector33;
        Vector3 vector36 = pointB - vector34;
        Math3d.ClosestPointsOnTwoLines(out planePoint, out vector3, vector33, vector35, vector34, vector36);
    }

    public static bool PlanePlaneIntersection(out Vector3 linePoint, out Vector3 lineVec, Vector3 plane1Normal, Vector3 plane1Position, Vector3 plane2Normal, Vector3 plane2Position)
    {
        linePoint = Vector3.zero ;
        lineVec = Vector3.zero ;
        lineVec = Vector3.Cross(plane1Normal, plane2Normal);
        Vector3 vector3 = Vector3.Cross(plane2Normal, lineVec);
        float single = Vector3.Dot(plane1Normal, vector3);
        if (Mathf.Abs(single) <= 0.006f)
        {
            return false;
        }
        Vector3 vector31 = plane1Position - plane2Position;
        float single1 = Vector3.Dot(plane1Normal, vector31) / single;
        linePoint = plane2Position + (single1 * vector3);
        return true;
    }

    public static int PointOnWhichSideOfLineSegment(Vector3 linePoint1, Vector3 linePoint2, Vector3 point)
    {
        Vector3 vector3 = linePoint2 - linePoint1;
        Vector3 vector31 = point - linePoint1;
        if (Vector3.Dot(vector31, vector3) <= 0f)
        {
            return 1;
        }
        if (vector31.magnitude <= vector3.magnitude)
        {
            return 0;
        }
        return 2;
    }

    public static Vector3 PositionFromMatrix(Matrix4x4 m)
    {
        Vector4 column = m.GetColumn(3);
        return new Vector3(column.x, column.y, column.z);
    }

    public static void PreciseAlign(ref GameObject gameObjectInOut, Vector3 alignWithVector, Vector3 alignWithNormal, Vector3 alignWithPosition, Vector3 triangleForward, Vector3 triangleNormal, Vector3 trianglePosition)
    {
        Math3d.LookRotationExtended(ref gameObjectInOut, alignWithVector, alignWithNormal, triangleForward, triangleNormal);
        Vector3 vector3 = gameObjectInOut.transform.TransformPoint(trianglePosition);
        Vector3 vector31 = alignWithPosition - vector3;
        gameObjectInOut.transform.Translate(vector31, 0);
    }

    public static Vector3 ProjectPointOnLine(Vector3 linePoint, Vector3 lineVec, Vector3 point)
    {
        float single = Vector3.Dot(point - linePoint, lineVec);
        return linePoint + (lineVec * single);
    }

    public static Vector3 ProjectPointOnLine(out float t, Vector3 linePoint, Vector3 lineVec, Vector3 point)
    {
        Vector3 vector3 = point - linePoint;
        t = Vector3.Dot(vector3, lineVec);
        return linePoint + (lineVec * t);
    }

    public static Vector3 ProjectPointOnLineSegment(Vector3 linePoint1, Vector3 linePoint2, Vector3 point)
    {
        Vector3 vector3 = linePoint2 - linePoint1;
        Vector3 vector31 = Math3d.ProjectPointOnLine(linePoint1, vector3.normalized, point);
        int num = Math3d.PointOnWhichSideOfLineSegment(linePoint1, linePoint2, vector31);
        if (num == 0)
        {
            return vector31;
        }
        if (num == 1)
        {
            return linePoint1;
        }
        if (num == 2)
        {
            return linePoint2;
        }
        return Vector3.zero;
    }

    public static Vector3 ProjectPointOnPlane(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
    {
        float single = Math3d.SignedDistancePlanePoint(planeNormal, planePoint, point) * -1f;
        return point + Math3d.SetVectorLength(planeNormal, single);
    }

    public static Vector3 ProjectVectorOnPlane(Vector3 planeNormal, Vector3 vector)
    {
        return vector - (Vector3.Dot(vector, planeNormal) * planeNormal);
    }

    public static Quaternion QuaternionFromMatrix(Matrix4x4 m)
    {
        return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
    }

    public static Vector3 SetVectorLength(Vector3 vector, float size)
    {
        return Vector3.Normalize(vector) * size;
    }

    public static float SignedDistancePlanePoint(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
    {
        return Vector3.Dot(planeNormal, point - planePoint);
    }

    public static float SignedDotProduct(Vector3 vectorA, Vector3 vectorB, Vector3 normal)
    {
        return Vector3.Dot(Vector3.Cross(normal, vectorA), vectorB);
    }

    public static float SignedVectorAngle(Vector3 referenceVector, Vector3 otherVector, Vector3 normal)
    {
        Vector3 vector3 = Vector3.Cross(normal, referenceVector);
        return Vector3.Angle(referenceVector, otherVector) * Mathf.Sign(Vector3.Dot(vector3, otherVector));
    }

    public static Quaternion SubtractRotation(Quaternion B, Quaternion A)
    {
        return Quaternion.Inverse(A) * B;
    }

    private static void VectorsToTransform(ref GameObject gameObjectInOut, Vector3 positionVector, Vector3 directionVector, Vector3 normalVector)
    {
        gameObjectInOut.transform.position=(positionVector);
        gameObjectInOut.transform.rotation=(Quaternion.LookRotation(directionVector, normalVector));
    }
}