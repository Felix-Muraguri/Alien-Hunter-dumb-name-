using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Craft : MonoBehaviour
{
    public CraftData craftData = new CraftData();
    Vector3 newPosition = new Vector3();
    public int playerIndex;

    public GameObject Thruster1;
    public GameObject Thruster2;

    public GameObject LeftFlame;
    public GameObject RightFlame;
    public GameObject FrontFlame;

    public Option[] options = new Option[4];
    public GameObject[] optionMarkersL1 = new GameObject[1];
    public GameObject[] optionMarkersL2 = new GameObject[2];
    public GameObject[] optionMarkersL3 = new GameObject[3];
    public GameObject[] optionMarkersL4 = new GameObject[4];

    public Beam beam = null;

    public GameObject bombPrefab = null;

    public CraftConfiguration config;
    Animator animator;
    int leftBoolID;
    int rightBoolID;
    int layerMask = 0;

    public BulletSpawner[] bulletSpawner = new BulletSpawner[5];

    bool alive = true;
    bool invulnerable = true;
    int invulnerableTimer = 120;
    const int INVULNERABLELENGTH = 120;

    SpriteRenderer spriteRenderer = null;

    private void Start()
    {
        animator = GetComponent<Animator>();
        Debug.Assert(animator);
        rightBoolID = Animator.StringToHash("Left");
        rightBoolID = Animator.StringToHash("Right");
        spriteRenderer = GetComponent<SpriteRenderer>();
        Debug.Assert(spriteRenderer);
        layerMask = ~LayerMask.GetMask("Player Bullets") & ~LayerMask.GetMask("Player Bombs") & ~LayerMask.GetMask("Player");
        craftData.beamCharge = (char)20;
    }

    public void SetInvulnerable()
    {
        invulnerable = true;
        invulnerableTimer = INVULNERABLELENGTH;
    }

    private void FixedUpdate()
    {
        if (invulnerable)
        {
            if (invulnerableTimer % 12 < 6)
                spriteRenderer.material.SetColor("_SolidColor", Color.black);
            else
                spriteRenderer.material.SetColor("_SolidColor", Color.white);
            invulnerableTimer--;
            if (invulnerableTimer <= 0)
            {
                invulnerable = false;
                spriteRenderer.material.SetColor("_SolidColor", Color.black);
            }
        }
        //Hit detection
        int maxColliders = 10;
        Collider[] hits = new Collider[maxColliders];
        Vector2 halfSize = new Vector2(3f, 4f);
        int noOfHits = Physics.OverlapBoxNonAlloc(transform.position, halfSize, hits, Quaternion.identity, layerMask);
        if (noOfHits>0)
        {
            Hit();
        }

        //Movement
        if (InputManager.instance && alive)
        {
            craftData.positionX += InputManager.instance.playerState[0].movement.x * config.Speed;
            craftData.positionY += InputManager.instance.playerState[0].movement.y * config.Speed;
            newPosition.x = (int)craftData.positionX;
            newPosition.y = (int)craftData.positionY;
            gameObject.transform.position = newPosition;

            if (InputManager.instance.playerState[0].up)
            {
                Thruster1.SetActive(true);
                Thruster2.SetActive(true);
            }
            else
            {
                Thruster1.SetActive(false);
                Thruster2.SetActive(false);
            }
            if (InputManager.instance.playerState[0].down)
            {
                FrontFlame.SetActive(true);
            }
            else
            {
                FrontFlame.SetActive(false);
            }
            if (InputManager.instance.playerState[0].left)
            {
                RightFlame.SetActive(true);
                animator.SetBool(leftBoolID, true);
            }
            else
            {
                RightFlame.SetActive(false);
                animator.SetBool(leftBoolID, false);
            }
            if (InputManager.instance.playerState[0].right)
            {
                LeftFlame.SetActive(true);
                animator.SetBool(rightBoolID, true);
            }
            else
            {
                LeftFlame.SetActive(false);
                animator.SetBool(rightBoolID, false);
            }
            
            //shooting bullets
            if (InputManager.instance.playerState[playerIndex].shoot)
            {
                ShotConfiguration shotConfig = config.shotLevel[craftData.shotPower];
                for (int s=0; s<5;s++)
                {
                    bulletSpawner[s].Shoot(shotConfig.spawnerSizes[s]);
                }

                //options
                for (int o=0;o<craftData.noOfEnabledOptions; o++)
                {
                    if (options[o])
                        options[o].Shoot();
                }
                
            }

            //Options Button
            if (!InputManager.instance.playerPrevState[playerIndex].options &&
                InputManager.instance.playerState[playerIndex].options)
            {
                craftData.optionsLayout++;
                if (craftData.optionsLayout > 3)
                    craftData.optionsLayout = (char)0;
                SetOptionsLayout(craftData.optionsLayout);
            }

            //BEAM
            if(InputManager.instance.playerState[playerIndex].beam)
            {
                beam.Fire();
            }

            //Bomb
            if(!InputManager.instance.playerPrevState[playerIndex].bomb &&
                InputManager.instance.playerState[playerIndex].bomb)
            {
                FireBomb();
            }
        }
    }

    public void Hit()
    {
        if (!invulnerable)
            Explode();
    }
    public void Explode()
    {
        alive = false;
        StartCoroutine(Exploding());
    }

    IEnumerator Exploding()
    {
        Color col = Color.white;
        for (float redness = 0; redness <=1; redness+=0.3f)
        {
            col.g = 1 - redness;
            col.b = 1 - redness;
            spriteRenderer.color = col;
            yield return new WaitForSeconds(0.1f);
        }
        EffectSystem.instance.CraftExplosion(transform.position);
        Destroy(gameObject);
        GameManager.instance.playerOneCraft = null;
        yield return null;
    }

    public void AddOption()
    {
        if (craftData.noOfEnabledOptions<4)
        {
            options[craftData.noOfEnabledOptions].gameObject.SetActive(true);
            craftData.noOfEnabledOptions++;
        }
    }
    public void SetOptionsLayout(int layoutIndex)
    {
        Debug.Assert(layoutIndex < 4);
        for (int o=0;o<4;o++)
        {
            switch(layoutIndex)
            {
                case 0:
                    options[o].gameObject.transform.position = optionMarkersL1[o].transform.position;
                    options[o].gameObject.transform.rotation = optionMarkersL1[o].transform.rotation;
                    break;
                case 1:
                    options[o].gameObject.transform.position = optionMarkersL2[o].transform.position;
                    options[o].gameObject.transform.rotation = optionMarkersL2[o].transform.rotation;
                    break;
                case 2:
                    options[o].gameObject.transform.position = optionMarkersL3[o].transform.position;
                    options[o].gameObject.transform.rotation = optionMarkersL3[o].transform.rotation;
                    break;
                case 3:
                    options[o].gameObject.transform.position = optionMarkersL4[o].transform.position;
                    options[o].gameObject.transform.rotation = optionMarkersL4[o].transform.rotation;
                    break;
            }
        }
    }

    public void IncreaseBeamStrength()
    {
        if (craftData.beamPower<5)
        {
            craftData.beamPower++;
            UpdateBeam();
        }
    }
    public void UpdateBeam()
    {
        beam.beamWidth = (craftData.beamPower + 2) * 8f;
    }
    void FireBomb()
    {
        Vector3 pos = transform.position;
        pos.y += 100;
        Instantiate(bombPrefab, pos, Quaternion.identity);
    }
}

public class CraftData
{
    public float positionX;
    public float positionY;
    public char shotPower;

    public char noOfEnabledOptions;
    public char optionsLayout;

    public bool beamFiring;
    public char beamPower;  //power settings and width
    public char beamCharge; //max charge (upgradable)
    public char beamTimer;  //current charge level (how much of beam is left)
}

