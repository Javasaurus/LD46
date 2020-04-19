using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CheckPoint : MonoBehaviour, Hittable
{
    public AudioClip doorCreak;
    public AudioClip doorClose;
    public LayerMask stuffToClear;
    public int knocks = 3;
    private Animator anim;
    private bool used;

    private string[] Greetings = new string[]
    {
        "Come on in...Long live the king",
        "Hurry, before they catch us",
        "The king is dead, long live the king",
        "Need any supper?",
        "I can't believe they made a game about our misery",
        "I have a feeling the king hired more henchmen",
        "What, this isn't the prancing pony !",
        "Did Gandalf send you here?",
        "Oi mate, come on in and have some ale",
        "Karen ! This guy brought a pig in a blanket",
        "God bless you...",
        "A baby? I haven't seen you in years and you bring a baby !",
        "Ah ah ah, you didn't say the magic word",
        "I'm gonna make him a sandwich he can't refuse",
        "May the Force be with you",
        "There's no place like home",
        "You can't handle the truth!",
        "You never know what you're gonna get",
        "Oh, Prince, let's ask for the moon. We have the stars.",
        "It's alive! It's alive!",
        "As God is my witness, I'll never be hungry again",
        "Nobody puts Baby in a corner...",
        "Long live the king !",
        "You know nothing...",
        "Quickly, come in !",
        "Money talks: mine always says is goodbye",
        "Don’t spell part backwards. It’s a trap",
        "Don’t trust atoms, they make up everything",
        "Thanks for explaining the word “many” to me, it means a lot",
        "My King called me average — it’s so mean!",
        "It sure takes a lot of balls to golf the way I do",
        "I doubt, therefore I might be",
        "I, for one, like Roman numerals",
        "My wife and I were happy for 20 years. And then we met"
    };


    private AudioSource sfx;
    // Start is called before the first frame update
    private void Start()
    {
        anim = GetComponent<Animator>();
        sfx = GetComponent<AudioSource>();
    }

    public void EnsureDoorIsClear()
    {
        Vector3 center = GetComponent<BoxCollider2D>().bounds.center;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(center, .15f, stuffToClear);
        foreach (Collider2D collider in colliders)
        {
            GameObject.Destroy(collider.gameObject);
        }
    }

    private void PlayDoorCreak()
    {
        sfx.PlayOneShot(doorCreak);
    }

    private void PlayDoorClose()
    {
        sfx.PlayOneShot(doorClose);
    }


    public IEnumerator Activate()
    {
        CharacterController.INSTANCE.resting = true;
        HintsController.INSTANCE.ShowText(GetRandomString());
        Vector2 center = GetComponent<BoxCollider2D>().bounds.center;
        Transform charT = CharacterController.INSTANCE.transform;

        while (Vector2.Distance(charT.position, center) > .5f)
        {
            charT.position = Vector3.Lerp(charT.position, center, Time.deltaTime * 3f);
            yield return null;
        }
        charT.gameObject.SetActive(false);

        anim.SetTrigger("Close");

        Image overlay = GameObject.FindGameObjectWithTag("FadingOverlay")?.GetComponent<Image>();
        float a = 0;
        //fade in
        while (a < 1)
        {
            a += 0.01f;
            overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, a);
            yield return null;
        }
        Debug.Log("Waiting");
        //WAIT
        yield return new WaitForSeconds(3f);
        anim.SetTrigger("Open");
        //fade out
        a = 1;
        while (a >= 0)
        {
            a -= .01f;
            overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, a);
            yield return null;
        }

        charT.gameObject.SetActive(true);
        CharacterController.INSTANCE.resting = false;
        HintsController.INSTANCE.ShowText("The royal guards approach ! Let's make way to the next safe house...");
    }

    public void SpawnNextSection()
    {
        used = true;
        ForestManager.INSTANCE.GenerateNext();
    }

    public void onHit( Vector2 direction )
    {
        if (used)
        {
            return;
        }

        if (!CharacterController.INSTANCE.carryingBaby)
        {
            HintsController.INSTANCE.ShowText("You need the prince to enter a safehouse");
            return;
        }

        knocks--;
        sfx.PlayOneShot(sfx.clip);
        if (knocks == 0)
        {
            anim.SetTrigger("Open");
            StartCoroutine(Activate());
        }
    }


    private string GetRandomString()
    {
        return Greetings[Random.Range(0, Greetings.Length)];
    }


}
