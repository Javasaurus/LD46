using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Enemy : MonoBehaviour, Hittable
{
    private System.Random rand = new System.Random();
    public int hp = 3;
    public float speed = 5;
    public float arrivalRange = .75f;
    public Transform target;
    public LayerMask groundLayer;
    public bool isGrounded;

    private Color originalColor;

    private bool dead;
    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private AudioSource sfx;
    private BoxCollider2D bc;
    // Start is called before the first frame update
    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        bc = GetComponent<BoxCollider2D>();
        sfx = GetComponent<AudioSource>();
        target = CharacterController.INSTANCE.BabyObject.transform;
        originalColor = sr.color;
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

        Vector2 origin = bc.bounds.center - (Vector3.up * (bc.bounds.size.y / 2));
        isGrounded = Physics2D.OverlapCircle(origin, 0.05f, groundLayer);

        ChaseTarget();
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

        if (Mathf.Abs(dist) > arrivalRange)
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

        anim.SetBool("Running", running);
        anim.SetBool("Attacking", attack);
    }

    public void onHit( Vector2 direction )
    {
        hp--;
        rb.velocity = (Vector2.up + direction).normalized * Random.Range(5, 15);
        if (hp <= 0)
        {
            dead = true;
            bc.enabled = false;
            anim.enabled = false;
            rb.gravityScale = 3;
        }
        else
        {
            sfx.pitch = .85f + (float)(rand.NextDouble() * .5f);
            sfx.PlayOneShot(sfx.clip);
            StartCoroutine(Flash());
        }
    }


    private void ScanHittables()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 2);
        foreach (Collider2D collider in colliders)
        {
            if (collider != bc && !collider.GetComponent<EnemyEntity>())
            {
                collider.GetComponent<Hittable>()?.onHit((collider.transform.position - transform.position).normalized);
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
