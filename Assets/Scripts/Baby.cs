using System.Collections;
using UnityEngine;

public class Baby : MonoBehaviour, Hittable, Pickup
{
    public CharacterController parent;
    public GameObject babyTrap;
    public bool canBePickedUp = true;
    public LayerMask groundLayer;
    public bool isGrounded;
    [HideInInspector]
    public Rigidbody2D rb;


    private AudioSource audio;
    private readonly float minInterval = .75f;
    private readonly float maxInterval = 1.3f;
    private float cryTimer;
    private System.Random rand;
    public void Awake()
    {
        rand = new System.Random();
        rb = GetComponent<Rigidbody2D>();
        audio = GetComponent<AudioSource>();
    }

    public void onHit( Vector2 direction )
    {
        rb.velocity = new Vector2(Random.Range(-4f, 4f), 10f);
    }

    public void Update()
    {
        if (Time.time > cryTimer && !audio.isPlaying && !CharacterController.INSTANCE.resting)
        {
            cryTimer = Time.time + (minInterval + (float)rand.NextDouble() * (maxInterval - minInterval));
            audio.pitch = 1f + (float)rand.NextDouble() * (.5f);
            audio.PlayOneShot(audio.clip);
        }

        isGrounded = rb.velocity.y <= 0 && Physics2D.OverlapCircle(transform.position, 0.2f, groundLayer);

        if (!CharacterController.INSTANCE.carryingBaby && rb.velocity.y < 0)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, .05f);
            foreach (Collider2D col in colliders)
            {
                CharacterController c = col.GetComponent<CharacterController>();
                if (c != null)
                {
                    PickUp(c);
                    return;
                }
            }
        }

    }

    public void PickUp( CharacterController c )
    {
        if (!canBePickedUp)
        {
            return;
        }

        c.carryingBaby = true;
        c.BabyGraphics.SetActive(true);
        c.BabyObject.gameObject.SetActive(false);
        c.BabyObject.rb.velocity = Vector2.zero;
        c.BabyObject.transform.parent = transform;
        c.BabyObject.transform.localPosition = Vector2.zero;
    }

    public void Trap( float duration )
    {
        StartCoroutine(SetBabyTrap(duration));
    }


    private IEnumerator SetBabyTrap( float duration )
    {
        canBePickedUp = false;
        babyTrap.SetActive(true);
        yield return new WaitForSeconds(duration);
        babyTrap.SetActive(false);
        canBePickedUp = true;
    }
}
