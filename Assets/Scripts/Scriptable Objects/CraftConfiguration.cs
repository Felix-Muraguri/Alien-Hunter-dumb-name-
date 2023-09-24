using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="CraftConfig", menuName ="Alien Hunter/Craft Configuration")]
public class CraftConfiguration : ScriptableObject
{
    public const int MAX_SHOT_POWER = 10;
    public float Speed;
    public float bulletStrength;
    public float beamStrength;
    public Sprite craftSprite;
    public ShotConfiguration[] shotLevel = new ShotConfiguration[MAX_SHOT_POWER];
}
[Serializable]
public class ShotConfiguration
{
    public int[] spawnerSizes = new int[5];
}
