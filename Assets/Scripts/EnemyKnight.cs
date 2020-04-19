using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class EnemyKnight : MonoBehaviour, Hittable
{
    private System.Random rand = new System.Random();
    public int hp = 5;

    public float detectionRange = 10;
    public float speed = 5;
    public float arrivalRange = .75f;
    public Baby target;
    public LayerMask groundLayer;
    public bool isGrounded;

    private Color originalColor;
    private bool dead;
    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private BoxCollider2D bc;
    private AudioSource sfx;

    private readonly float wanderInterval = 2f;
    private float wanderTimer = 0;


    private static Sprite[] sprites1;
    private static Sprite[] sprites2;
    private static Sprite[] sprites3;
    // Start is called before the first frame update
    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        bc = GetComponent<BoxCollider2D>();
        sfx = GetComponent<AudioSource>();
        target = CharacterController.INSTANCE.BabyObject.GetComponent<Baby>();
        originalColor = sr.color;

        //find sprites for this one...
        if (sprites1 == null)
        {
            sprites1 = Resources.LoadAll<Sprite>("Sprites/Characters/Knights/Knight");
        }
        if (sprites2 == null)
        {
            sprites2 = Resources.LoadAll<Sprite>("Sprites/Characters/Knights/Knight_2");
        }
        if (sprites3 == null)
        {
            sprites3 = Resources.LoadAll<Sprite>("Sprites/Characters/Knights/Knight_3");
        }
        int randomInt = Random.Range(0, 3);
        switch (randomInt)
        {
            case 0:
                GetComponent<AnimationSync>().setSprites(sprites1);
                break;
            case 1:
                GetComponent<AnimationSync>().setSprites(sprites2);
                break;
            default:
                GetComponent<AnimationSync>().setSprites(sprites3);
                break;
        }

    }

    public void Update()
    {

        if (CharacterController.INSTANCE.resting)
        {
            return;
        }
        if (transform.position.y < -50f)
        {
            GameObject.Destroy(gameObject);
            return;
        }


        //kill if they fly too high
        if (rb.velocity.y > 0 && transform.position.y > 3.5f)
        {
            hp = 0;
            dead = true;
            bc.enabled = false;
            anim.enabled = false;
            rb.gravityScale = 3;
        }


        Vector2 origin = bc.bounds.center - (Vector3.up * (bc.bounds.size.y / 2));
        isGrounded = Physics2D.OverlapCircle(origin, 0.05f, groundLayer);


        if (rb.velocity.magnitude < .05f)
        {
            anim.SetBool("Running", false);
        }

        if (target != null && !target.parent.isBehindObject && sr.flipX == !target.parent.sr.flipX)
        {
            if (isGrounded)
            {
                ChaseTarget();
            }
        }
        else
        {
            if (target == null)
            {
                target = GameObject.FindObjectOfType<Baby>();
            }
            if (Time.time > wanderTimer)
            {
                wanderTimer = Time.time + wanderInterval;
                Wander();
                anim.SetBool("Attacking", false);
            }

        }


    }

    public void ChaseTarget()
    {

        bool running = false;
        bool attack = false;

        if (dead || !isGrounded || target == null)
        {
            return;
        }

        float dist = target.transform.position.x - transform.position.x;

        sr.flipX = dist < 0;
        float aDist = Mathf.Abs(dist);
        if (aDist < detectionRange)
        {

            if (aDist > arrivalRange)
            {
                running = true;
                attack = false;
                rb.velocity = new Vector2(Mathf.Sign(dist) * speed, 0);
            }
            else
            {
                running = false;
                attack = true;
                rb.velocity = Vector2.zero;
            }
        }

        anim.SetBool("Running", running);
        anim.SetBool("Attacking", attack);
    }

    public void Wander()
    {
        if (!isGrounded)
        {
            return;
        }

        float dir = Mathf.Sign(Random.Range(-1, 1));

        if (dir == 0)
        {
            sr.flipX = Random.Range(0f, 1f) > .5f;
            anim.SetBool("Running", false);
        }
        else
        {
            sr.flipX = dir < 0;
            rb.velocity = new Vector2(dir * speed, 0);
            anim.SetBool("Running", true);
        }
    }



    public void onHit( Vector2 direction )
    {
        hp--;
        rb.velocity = (Vector2.up + direction).normalized * Random.Range(15f, 0);
        if (hp <= 0)
        {
            dead = true;
            bc.enabled = false;
            anim.enabled = false;
            rb.gravityScale = 3;
        }
        else
        {
            sfx.pitch = .75f + (float)(rand.NextDouble() * .6f);
            sfx.PlayOneShot(sfx.clip);
            StartCoroutine(Flash());
        }
    }

    private void ScanHittables()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 4);
        foreach (Collider2D collider in colliders)
        {
            if (collider != bc)
            {
                if (collider.GetComponent<Baby>() != null || collider.GetComponent<CharacterController>() != null)
                {
                    collider.GetComponent<Hittable>()?.onHit((collider.transform.position - transform.position).normalized);
                }
            }
        }
    }

    private IEnumerator Flash()
    {
        sr.color = Color.yellow;
        yield return new WaitForSeconds(.2f);
        sr.color = originalColor;
    }

}