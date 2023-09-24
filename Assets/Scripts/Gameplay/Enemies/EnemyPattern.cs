using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyPattern : MonoBehaviour
{
    public List<EnemyStep> steps = new List<EnemyStep>();
    public Enemy enemyPrefab;
    private Enemy spawnedEnemy;
    private int UID;
    public bool stayOnLast = true;
    private int currentStateIndex = 0;
    private int previousStateIndex = -1;
    [MenuItem("GameObject/Alien Hunter/EnemyPattern",false,10)]
    static void CreateEnemyPatternObject(MenuCommand menuCommand)
    {
        Helpers helper = (Helpers)Resources.Load("Helper");
        if (helper != null)
        {
            GameObject go = new GameObject("EnemyPattern" + helper.nextFreePatternID);
            EnemyPattern pattern = go.AddComponent<EnemyPattern>();
            pattern.UID = helper.nextFreePatternID;
            helper.nextFreePatternID++;
            //Register creation with undo system
            Undo.RegisterCompleteObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }
        else Debug.LogError("Could not find Helper");
    }

    public void Spawn()
    {
        if (spawnedEnemy == null)
        {
            spawnedEnemy = Instantiate(enemyPrefab, transform.position, transform.rotation).GetComponent<Enemy>();
            spawnedEnemy.SetPattern(this);
        }
    }

    //Code in charge of calculating StateIndex
    public Vector2 CalculatePosition(float progressTimer) 
    {
        currentStateIndex = WhichStep(progressTimer);
        if (currentStateIndex < 0) return spawnedEnemy.transform.position;
        EnemyStep step = steps[currentStateIndex];
        float stepTime = progressTimer - StartTime(currentStateIndex);
        Vector3 startPos = EndPosition(currentStateIndex - 1);
        return step.CalculatePosition(startPos, stepTime);
    }

    public void Calculate(Transform enemyTransform, float progressTimer)
    {
        Vector3 pos = CalculatePosition(progressTimer);
        Quaternion rot = CalculateRotation(progressTimer);

        enemyTransform.position = pos;
        enemyTransform.rotation = rot;

        if (currentStateIndex != previousStateIndex) //state has changed
        {
            if (previousStateIndex>=0)
            {
                //Call deactivate states
                EnemyStep prevStep = steps[previousStateIndex];
                prevStep.FireDeactivateStates(spawnedEnemy);
            }
            if (currentStateIndex>=0)
            {
                //call activate states
                EnemyStep currStep = steps[currentStateIndex];
                currStep.FireActivateStates(spawnedEnemy);
            }
            previousStateIndex = currentStateIndex;
        }

    }

    public Quaternion CalculateRotation(float progressTimer)
    {
        return Quaternion.identity;
    }

    int WhichStep(float timer)
    {
        float timeToCheck = timer;
        for (int s = 0; s < steps.Count; s++)
        {
            if (timeToCheck < steps[s].TimeToComplete())
            return s;
            timeToCheck -= steps[s].TimeToComplete();
        }
        if (stayOnLast)
        return steps.Count - 1;
        return -1;
    }

    public float StartTime(int step)
    {
        if (step <= 0) return 0;
        float result = 0;
        for (int s=0; s<step;s++)
        {
            result += steps[s].TimeToComplete();
        }
        return result;
    }

    public Vector3 EndPosition(int stepIndex)
    {
        Vector3 result = transform.position;
        if (stepIndex>=0)
        {
            for (int s=0; s<=stepIndex; s++)
            {
                result = steps[s].EndPosition(result);
            }
        }
        return result;
    }
}
