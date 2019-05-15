using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets._2D;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private PlatformerCharacter2D m_Character;
    private bool m_Jump;

    [SerializeField] GameObject dialogPrefab;
    public float distance = 4f;
    Transform player;
    GameObject dialog;

    public int hitNumber = 2;

    float jumpCooldown = 0.5f;
    float attackCooldown = 2f;

    bool playerSeen = false;
    bool chase = false;

    Transform focussedBomb;

    public enum Pirate { captain, cucumber, whale, bigguy, bald };

    public Pirate pirate;

    private void Awake()
    {
        m_Character = GetComponent<PlatformerCharacter2D>();
        player = GameObject.FindWithTag("Player").transform;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        DetectPlayer();
        DetectBomb();

        if (!focussedBomb)
            PlayerChase();
        else if (pirate == Pirate.captain)
            RunAway();
        else
            BombChase();

        attackCooldown -= 1 * 0.02f;
        jumpCooldown -= 1 * 0.02f;

        if(focussedBomb && !focussedBomb.GetComponent<Bomb>().lit)
        {
            focussedBomb = null;
        }
    }

    void RunAway()
    {
        if (!focussedBomb || !chase) return;

        if (Vector2.Distance(focussedBomb.position, transform.position) < 10) // Move
        {
            if (Mathf.Abs(focussedBomb.position.y - transform.position.y) < 2f
                && Mathf.Abs(focussedBomb.position.x - transform.position.x) > m_Character.hitRadius)
            {
                float h = focussedBomb.position.x < transform.position.x ? 0.7f : -0.7f;
                m_Character.Move(h, m_Jump);
            }
            else
            {
                m_Character.Move(0, m_Jump);
            }
        }
    }

    void DetectBomb()
    {
        foreach (Bomb bomb in FindObjectsOfType<Bomb>())
        {
            if (Mathf.Abs(bomb.transform.position.x - transform.position.x) < distance && !focussedBomb)
            {
                if (Mathf.Abs(bomb.transform.position.y - transform.position.y) < 1)
                {
                    if(bomb.lit)
                        BombSeen(bomb.transform);
                }
            }
        }
    }

    void BombChase()
    {
        if (!focussedBomb || !chase) return;

        if (Vector2.Distance(focussedBomb.position, transform.position) > m_Character.hitRadius) // Move
        {
            if (Mathf.Abs(focussedBomb.position.y - transform.position.y) < 2f
                && Mathf.Abs(focussedBomb.position.x - transform.position.x) > m_Character.hitRadius)
            {
                float h = focussedBomb.position.x > transform.position.x ? 0.5f : -0.5f;
                m_Character.Move(h, m_Jump);
            }
            else
            {
                m_Character.Move(0, m_Jump);
            }
        }
        else
        {
            if (attackCooldown < 0)
            {
                m_Character.Attack(true);
                m_Character.Move(0, m_Jump);
                attackCooldown = 1f;

                if (pirate == Pirate.cucumber)
                    focussedBomb.GetComponent<Bomb>().BlowOut();
                if (pirate == Pirate.whale)
                    focussedBomb.GetComponent<Bomb>().Eat();
                else if (pirate != Pirate.captain)
                    chase = false;
                focussedBomb = null;
            }
        }
    }

    void DetectPlayer()
    {
        if (Mathf.Abs(player.position.x - transform.position.x) < distance && !playerSeen)
        {
            if (Mathf.Abs(player.position.y - transform.position.y) < 1)
            {
                PlayerSeen();
            }
        }
    }

    void PlayerChase()
    {
        if (!chase) return;

        // Move towards player
        if (Vector2.Distance(player.position, transform.position) > m_Character.hitRadius) // Move
        {
            if (Mathf.Abs(player.position.y - transform.position.y) < 2f
                && Mathf.Abs(player.position.x - transform.position.x) > m_Character.hitRadius)
            {
                float h = player.position.x > transform.position.x ? 0.5f : -0.5f;
                m_Character.Move(h, m_Jump);
            }
            else
            {
                m_Character.Move(0, m_Jump);
            }
        }
        else
        {
            if (attackCooldown < 0)
            {
                m_Character.Attack();
                m_Character.Move(0, m_Jump);
                attackCooldown = 2f;
            }
        }

        // Jump follow player
        m_Jump = false;
        if (player.position.y - transform.position.y > 0.5f
            && Vector2.Distance(player.position, transform.position) < 2
            && jumpCooldown < 0)
        {
            m_Jump = true;
            jumpCooldown = 0.5f;
        }
    }

    void PlayerSeen()
    {
        playerSeen = true;
        if (dialog)
        {
            Destroy(dialog);
        }
        dialog = Instantiate(dialogPrefab);
        Invoke("RemoveDialog", 1);
        dialog.transform.position = new Vector2(transform.position.x + 0.5f, transform.position.y + 0.5f);
        dialog.GetComponent<Animator>().SetTrigger("Interrogation");
        Invoke("StartChase", 1);
    }

    void BombSeen(Transform bomb)
    {
        focussedBomb = bomb;
        if (dialog)
        {
            Destroy(dialog);
        }
        dialog = Instantiate(dialogPrefab);
        Invoke("RemoveDialog", 1);
        dialog.transform.position = new Vector2(transform.position.x + 0.5f, transform.position.y + 0.5f);
        dialog.GetComponent<Animator>().SetTrigger("Exclamation");
        Invoke("StartChase", 1);
    }

    void StartChase()
    {
        chase = true;
    }

    public void RemoveDialog()
    {
        Destroy(dialog);
    }

    public void Hit()
    {
        hitNumber--;
        if(hitNumber == 0)
        {
            m_Character.Die();
        }
        //if(!seen)
            //PlayerSeen();
    }
}

