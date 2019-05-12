using System.Collections;
using System.Collections.Generic;
using Prime31.TransitionKit;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets._2D;

public class Lives : MonoBehaviour
{
    public List<Animator> livesAnimators = new List<Animator>();

    int lives = 3;
    public PixelateTransition.PixelateFinalScaleEffect effect;

    public static Lives manager;
    GameObject player;


    // Start is called before the first frame update
    void Start()
    {
        if (manager == null)
            manager = this;

        player = GameObject.FindWithTag("Player");

        StartCoroutine(ReceiveLives());
    }

    IEnumerator ReceiveLives()
    {
        yield return new WaitForSeconds(0.2f);
        livesAnimators[0].enabled = true;
        yield return new WaitForSeconds(0.2f);
        livesAnimators[1].enabled = true;
        yield return new WaitForSeconds(0.2f);
        livesAnimators[2].enabled = true;
    }

    public bool isDead()
    {
        return !(lives > 0);
    }
 
    public void LoseLife()
    {
        if (isDead()) return;
        lives--;
        livesAnimators[lives].SetTrigger("LoseLife");
        if(lives == 0)
        {
            player.GetComponent<PlatformerCharacter2D>().Die();
            Invoke("Transition", 3);
           
        }
    }

    void Transition()
    {
        var enumValues = System.Enum.GetValues(typeof(PixelateTransition.PixelateFinalScaleEffect));


        var pixelater = new PixelateTransition()
        {
            nextScene = 0,
            finalScaleEffect = effect,
            duration = 1.0f
        };
        TransitionKit.instance.transitionWithDelegate(pixelater);
    }
}
