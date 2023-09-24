using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSystem : MonoBehaviour
{
    public static EffectSystem instance = null;
    public GameObject craftExplosionPrefab = null;
    public ParticleSystem craftParticlesPrefab = null;
    public ParticleSystem craftDebrisParticlesPrefab = null;

    void Start()
    {
        if (instance)
        {
            Debug.LogError("Trying to create more than one EffectSystem");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void CraftExplosion(Vector3 position)
    {
        Instantiate(craftExplosionPrefab, position, Quaternion.identity);
        Instantiate(craftDebrisParticlesPrefab, position, Quaternion.identity);
        Instantiate(craftExplosionPrefab, position, Quaternion.identity);
    }
}
