using System;
using UnityEngine;

[Serializable]
public class Packet
{
    public int action;
    public Vector3 platformLeft;
    public Vector3 platformRight;
    public Vector3 ballPos;
}
