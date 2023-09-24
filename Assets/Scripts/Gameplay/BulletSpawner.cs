using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public BulletManager.BulletType bulletType = BulletManager.BulletType.Bullet1_Size1;
    public BulletSequence sequence;
    public GameObject muzzleFlash = null;

    public int rate = 1;
    private int timer = 0;
    public int speed = 10;

    public float startAngle = 0;
    public float endAngle = 0;
    public int radialNumber = 1;
    public float dAngle = 0; //change in angle
    
    public bool autoFireActive = false;
    private bool firing = false;
    private int frame = 0;

    public bool fireAtPlayer = false;
    public bool fireAtTarget = false;
    public GameObject target = null;
    public bool cleverShot = false;
    public bool homing = false;

    public void Shoot (int size)
    {
        if (size < 0) return;
        Vector2 primaryDirection = transform.up;

        if (fireAtPlayer || fireAtTarget)
        {
            Vector2 targetPosition = Vector2.zero;
            if (fireAtPlayer)
                targetPosition = GameManager.instance.playerOneCraft.transform.position;
            else if (fireAtTarget && target != null)
                targetPosition = target.transform.position;
            primaryDirection = targetPosition - (Vector2)transform.position;
            primaryDirection.Normalize();
        }
        if (firing || timer == 0) //shoot
        {
            float angle = startAngle;
            for (int a=0; a<radialNumber;a++)
            {
                Quaternion myRotation = Quaternion.AngleAxis(angle, Vector3.forward);
                Vector2 velocity = myRotation * transform.up * speed;
                BulletManager.BulletType bulletToShoot = bulletType + size;
                GameManager.instance.bulletManager.SpawnBullet(bulletToShoot, 
                                                               transform.position.x, 
                                                               transform.position.y, 
                                                               velocity.x, 
                                                               velocity.y, 
                                                               angle,dAngle,
                                                               homing);
                angle = angle + ((endAngle - startAngle) / (radialNumber - 1));
            }
            
        }
    }
    private void FixedUpdate()
    {
        timer++;
        if (timer >= rate)
        {
            timer = 0;
            /*if (muzzleFlash)
             * muzzleFlash.SetActive(false);*/
            if (autoFireActive)
            {
                firing = true;
                frame = 0;
            }
        }
        if (firing)
        {
            if (sequence.ShouldFire(frame))
                Shoot(1);
            frame++;
            if (frame > sequence.totalFrames)
                firing = false;
        }
    }

    public void Activate()
    {
        autoFireActive = true;
        timer = 0;
        frame = 0;
        firing = true;
    }

    public void Deactivate()
    {
        autoFireActive = false;
    }
}
[Serializable]
public class BulletSequence
{
    public List<int> emmitFrames = new List<int>();
    public int totalFrames;
    public bool ShouldFire(int currentFrame)
    {
        foreach(int frame in emmitFrames)
        {
            if (currentFrame == frame)
                return true;
        }
        return false;
    }
}



