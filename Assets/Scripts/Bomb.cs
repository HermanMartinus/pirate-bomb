using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {

    public float radius = 2;
    public float power = 300;
    public float uplift = 0.1f;

    public float damageRadius = 2;

    public LayerMask whoIsEffected;

    public bool lit = true;
    static float shakeDuration = 0f;

    private float shakeMagnitude = 0.5f;

    private float dampingSpeed = 1f;

    Vector3 cameraInitialPosition;

    void Start() {
        cameraInitialPosition = Camera.main.transform.localPosition;
        StartCoroutine(Explode());
    }

    IEnumerator Explode() {

        yield return new WaitForSeconds(3);
        if (lit)
        {
            GetComponent<Animator>().SetTrigger("Explode");
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            ExplosiveForce();
            shakeDuration = 0.2f;
            CheckDamage();
            yield return new WaitForSeconds(0.7f);
            Destroy(gameObject);
        }
    }

    void ExplosiveForce() {
        Vector3 explosionPos = transform.position;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(explosionPos, radius);

        List<GameObject> hitPrev = new List<GameObject>();
        foreach (Collider2D hit in colliders)
        {
            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();

            if (rb != null && !hitPrev.Contains(hit.gameObject))
                Rigidbody2DExtension.AddExplosionForce(rb, power, explosionPos, radius, uplift);
            hitPrev.Add(hit.gameObject);
        }
    }

    void CheckDamage()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, damageRadius, whoIsEffected);

        List<GameObject> hitPrev = new List<GameObject>();
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject && !hitPrev.Contains(colliders[i].gameObject))
            {
               colliders[i].gameObject.SendMessage("Hit");
            }
            hitPrev.Add(colliders[i].gameObject);
        }
    }

    public void BlowOut()
    {
        StartCoroutine(WaitAndBlowOut());
    }

    IEnumerator WaitAndBlowOut()
    {
        yield return new WaitForSeconds(0.6f);
        GetComponent<Animator>().SetTrigger("TurnedOff");
        lit = false;
    }

    public void Eat()
    {
        Invoke("GetEaten", 0.8f);
    }

    void GetEaten()
    {
        Destroy(gameObject);
    }

    void Update()
    {
        if (shakeDuration > 0)
        {
            Camera.main.transform.localPosition = cameraInitialPosition + Random.insideUnitSphere * shakeMagnitude;

            shakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else
        {
            shakeDuration = 0f;
            Camera.main.transform.localPosition = cameraInitialPosition;
        }
    }
}

