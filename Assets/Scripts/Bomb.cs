using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {

  public float radius = 100;
  public float power = 100;
  public float uplift = 100;

  void Start() {
    StartCoroutine(Explode());
  }

  IEnumerator Explode() {

    yield return new WaitForSeconds(3);
    GetComponent<Animator>().SetTrigger("Explode");
    GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
    ExplosiveForce();
    yield return new WaitForSeconds(0.7f);
    Destroy(gameObject);
  }

  void ExplosiveForce() {
    Vector3 explosionPos = transform.position;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(explosionPos, radius);
        foreach (Collider2D hit in colliders)
        {
            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();

            if (rb != null)
                Rigidbody2DExtension.AddExplosionForce(rb, power, explosionPos, radius, uplift);
        }
  }
}

public static class Rigidbody2DExtension
{
    public static void AddExplosionForce(this Rigidbody2D body, float explosionForce, Vector3 explosionPosition, float explosionRadius)
    {
        var dir = (body.transform.position - explosionPosition);
        float wearoff = 1 - (dir.magnitude / explosionRadius);
        body.AddForce(dir.normalized * explosionForce * wearoff);
    }
 
    public static void AddExplosionForce(this Rigidbody2D body, float explosionForce, Vector3 explosionPosition, float explosionRadius, float upliftModifier)
    {
        var dir = (body.transform.position - explosionPosition);
        float wearoff = 1 - (dir.magnitude / explosionRadius);
        Vector3 baseForce = dir.normalized * explosionForce * wearoff;
        body.AddForce(baseForce);
 
        float upliftWearoff = 1 - upliftModifier / explosionRadius;
        Vector3 upliftForce = Vector2.up * explosionForce * upliftWearoff;
        body.AddForce(upliftForce);
    }
}