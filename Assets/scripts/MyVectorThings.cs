using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyVectorThings : MonoBehaviour
{
    public Vector3 ClampHorizonal(Vector3 inVector, float clampTo)
    {
        Vector3 horizontalVector = Vector3.ClampMagnitude(new Vector3(inVector.x, 0, inVector.z), clampTo);

        return new Vector3(horizontalVector.x, inVector.y, horizontalVector.z);


    }

    public Vector3 MakeHorizontal(Vector3 v)
    {
        return new Vector3(v.x, 0, v.z);
    }

    public float HorizontalAngle(Vector3 v1, Vector3 v2)
    {
        float angle = Vector3.Angle(MakeHorizontal(v1), MakeHorizontal(v2));
        return angle;
    }

    public float FloorAngle(Vector3 v)
    {
        float angle = 0f;
        angle = Vector3.Angle(Vector3.up, v);
        return angle;

    }

    public float FloorFacing(Vector3 v)
    {
        Vector3 hv = MakeHorizontal(v);
        float angle = Vector3.Angle(Vector3.forward, hv);
        return angle;

    }

    public Vector3 ForwarVector(Transform transform)
    {
        Vector3 outVector = transform.forward;
        outVector.y = 0;
        outVector.Normalize();
        return outVector;

    }


    public Vector3 RightVector(Transform transform)
    {
        Vector3 outVector = transform.right;
        outVector.y = 0;
        outVector.Normalize();
        return outVector;
    }




}
