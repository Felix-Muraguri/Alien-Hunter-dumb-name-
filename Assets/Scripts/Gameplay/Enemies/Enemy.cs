using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyData data;
    private EnemyPattern pattern;
    private EnemySections[] sections;

    private void Start()
    {
        sections = gameObject.GetComponentsInChildren<EnemySections>();
    }
    public void SetPattern(EnemyPattern inPattern)
    {
        pattern = inPattern;
    }

    private void FixedUpdate()
    {
        data.progressTimer++;

        pattern.Calculate(transform, data.progressTimer);
    }

    public void EnableState(string name)
    {
        foreach (EnemySections section in sections)
        {
            section.EnableState(name);
        }
            
    }

    public void DisableState(string name)
    {
        foreach (EnemySections section in sections)
        {
            section.DisableState(name);
        }
    }
}
[Serializable]
public struct EnemyData
{
    public float progressTimer;
    public float positionX;
    public float positionY;
    public int patternUID;
}
