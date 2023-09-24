using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySections : MonoBehaviour
{
    public List<EnemyState> states = new List<EnemyState>();
    public void EnableState(string name)
    {
        foreach(EnemyState state in states)
        {
            if (state.stateName == name)
                state.Enable();
        }
    }

    public void DisableState(string name)
    {
        foreach (EnemyState state in states)
        {
            if (state.stateName == name)
                state.Disable();
        }
    }

}
