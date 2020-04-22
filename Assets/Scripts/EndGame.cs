using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGame : MonoBehaviour
{
    public GameObject endEffect;

    public void OnTriggerEnter2D( Collider2D collision )
    {

        CharacterController player = collision.GetComponent<CharacterController>();
        if (player != null)
        {
            //check if there are still enemy's afoot
            int amount = GameObject.FindObjectsOfType<EnemyEntity>().Length;
            if (amount > 0)
            {
                HintsController.INSTANCE.ShowText("I won't open until all the guards are gone ! (" + amount + " remaining");
            }
            else
            {
                HintsController.INSTANCE.ShowText("You made it !");
                StartCoroutine(Finish());
            }
        }
    }

    private IEnumerator Finish()
    {
        endEffect.SetActive(true);
        AudioSource bgm = GetComponent<AudioSource>();
        bgm?.PlayOneShot(bgm.clip);
        CharacterController.INSTANCE.gameObject.SetActive(false);
        yield return new WaitForSeconds(3f);
        Image overlay = GameObject.FindGameObjectWithTag("FadingOverlay")?.GetComponent<Image>();
        float a = 0;
        //fade in
        while (a < 1)
        {
            a += 0.01f;
            overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, a);
            yield return null;
        }
        SceneManager.LoadScene(3);
    }
}
