using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int index; // index into the bullet pool
}
[Serializable]
public struct BulletData
{
    public float positionX;
    public float positionY;
    public float dX;
    public float dY;
    public float angle;
    public float dAngle;
    public int type;
    public bool active;
    public bool homing;
    

    public BulletData (float inX, 
        float inY, 
        float inDX,
        float inDY,
        float inAngle,
        float inDAngle,
        int inType,
        bool inActive,
        bool inHoming)

    {
        positionX = inX;
        positionY = inY;
        dX = inDX;
        dY = inDY;
        angle = inAngle;
        angle = inDAngle;
        type = inType;
        active= inActive;
        homing = inHoming;
    }
}
