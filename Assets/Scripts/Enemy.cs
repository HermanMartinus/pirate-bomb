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

    public float jumpCooldown = 0.5f;
    public float attackCooldown = 2f;

    bool seen = false;
    bool chase = false;

    private void Awake()
    {
        m_Character = GetComponent<PlatformerCharacter2D>();
        player = GameObject.FindWithTag("Player").transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        DetectPlayer();

        if (!chase) return;

        if (Mathf.Abs(player.position.x - transform.position.x) > 1f)
        {
            float h = player.position.x > transform.position.x ? 0.5f : -0.5f;
            m_Character.Move(h, m_Jump);
        }
        else if(Mathf.Abs(player.position.y - transform.position.y) < 1f)
        {
            if (attackCooldown < 0)
            {
                m_Character.Attack();
                m_Character.Move(0, m_Jump);
                attackCooldown = 2f;
            }
        }
        m_Jump = false;

        if (player.position.y - transform.position.y > 0.5f && player.position.x - transform.position.x < 0.3f && jumpCooldown < 0)
        {
            m_Jump = true;
            jumpCooldown = 0.5f;
        }

        if (Mathf.Abs(player.position.y - transform.position.y) > 2f)
        {
            //chase = false;
            m_Character.Move(0, m_Jump);
            //seen = false;  
        }

        attackCooldown -= 1 * 0.02f;
        jumpCooldown -= 1 * 0.02f;

    }

    void DetectPlayer()
    {
        if (Mathf.Abs(player.position.x - transform.position.x) < distance && !seen)
        {
            if (Mathf.Abs(player.position.y - transform.position.y) < 1)
            {
                PlayerSeen();
            }
        }
    }

    void PlayerSeen()
    {
        seen = true;
        if (dialog)
        {
            Destroy(dialog);
        }
        dialog = Instantiate(dialogPrefab);
        Invoke("RemoveDialog", 1);
        dialog.transform.position = new Vector2(transform.position.x + 0.5f, transform.position.y + 0.5f);
        dialog.GetComponent<Animator>().SetTrigger("Interrogation");
    }

    public void RemoveDialog()
    {
        chase = true;
        Destroy(dialog);
    }

    public void Hit()
    {
        hitNumber--;
        if(hitNumber == 0)
        {
            m_Character.Die();
        }
        if(!seen)
            PlayerSeen();
    }
}

