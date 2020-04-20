using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class CharacterController : MonoBehaviour, Hittable
{
    private System.Random systemRand = new System.Random();

    public static CharacterController INSTANCE;
    public bool carryingBaby;
    public bool resting;
    private bool nearCheckpoint;

    public Baby BabyObject;
    public GameObject BabyGraphics;
    public Image babyTimerGraphics;
    public float maxBabyTimer = 10f;
    public float curBabyTimer = 0f;



    public float xInput;
    public bool attackInput;
    public bool jumpInput;
    public bool pickupInput;

    private bool blockMoving;

    public bool isBehindObject;
    public bool running;
    public float speed = 5f;
    public float jump = 10f;
    public LayerMask groundLayer;
    public LayerMask hideLayers;
    public bool isGrounded;

    private Animator anim;
    private Rigidbody2D rb;
    public SpriteRenderer sr;
    private BoxCollider2D bc;
    private CircleCollider2D cc;

    public AudioSource staffAudio;
    public AudioSource footstepAudio;


    private readonly float attackInterval = .15f;
    private float attackTimer;

    private readonly float pickupInterval = .75f;
    private float pickupTimer;


    public bool isHidden;

    public void OnEnable()
    {
        UnblockMovement();
    }
    // Start is called before the first frame update
    private void Awake()
    {
        INSTANCE = this;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        bc = GetComponent<BoxCollider2D>();
        cc = GetComponent<CircleCollider2D>();
        BabyObject.rb = BabyObject.GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        HintsController.INSTANCE?.ShowText("Escape the royal guards ! [F] to attack [DOWN ARROW] to pick up the Prince");
    }

    private void LateUpdate()
    {
        nearCheckpoint = ScanCheckpoints();
    }

    // Update is called once per frame
    private void Update()
    {
        if (resting)
        {
            return;
        }

        //for safety
        BabyGraphics.SetActive(carryingBaby);
        if (carryingBaby)
        {
            BabyObject.rb.velocity = Vector2.zero;
            BabyObject.gameObject.SetActive(!carryingBaby);
            curBabyTimer = Mathf.Lerp(curBabyTimer, 0, Time.deltaTime * 5f);
        }
        else
        {
            curBabyTimer += BabyObject.isGrounded ? Time.deltaTime : Time.deltaTime / 5f;
            if (curBabyTimer >= maxBabyTimer)
            {
                GameObject.FindObjectOfType<GameOverController>().SetGameOver();
            }
        }

        babyTimerGraphics.fillAmount = curBabyTimer / maxBabyTimer;

        isBehindObject = checkBehindObject();

        HandleInput();

        if (HandlePickup())
        {
            return;
        }

        HandleMovement();

        HandleAttack();
    }



    private void HandleInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        jumpInput = Input.GetKeyDown(KeyCode.Space);
        attackInput = Input.GetKeyDown(KeyCode.F);
        pickupInput = Input.GetKeyDown(KeyCode.DownArrow);
        Vector2 origin = bc.bounds.center - (Vector3.up * (bc.bounds.size.y / 2));
        bool tmpIsGrounded = rb.velocity.y == 0 && Physics2D.OverlapCircle(origin, 0.05f, groundLayer);

        if (!isGrounded && tmpIsGrounded)
        {
            PlayFootStep();
            PlayFootStep();

        }
        isGrounded = tmpIsGrounded;
    }

    private void HandleMovement()
    {
        rb.gravityScale = isGrounded ? 3 : rb.velocity.y <= 0 ? 6 : 3;

        if (blockMoving)
        {
            return;
        }
        running = xInput != 0;
        if (running)
        {
            sr.flipX = xInput < 0;
        }

        Vector2 movement = Vector2.zero;



        if (isGrounded)
        {
            if (running)
            {
                movement.x = speed * xInput;
            }

            if (jumpInput)
            {
                movement.y = jump;
            }


            rb.velocity = movement;
        }



        anim.SetBool("Running", running);
    }

    private bool HandleAttack()
    {
        if (attackTimer <= Time.time && attackInput)
        {
            blockMoving = true;
            attackTimer = Time.time + attackInterval;

            //check if a checkpoint is nearby?

            if (nearCheckpoint)
            {

                anim.SetInteger("AttackNr", 0);
            }
            else
            {
                staffAudio.pitch = .8f + ((float)systemRand.NextDouble() * .5f);
                staffAudio.PlayOneShot(staffAudio.clip);
                anim.SetInteger("AttackNr", isGrounded ? systemRand.Next(0, 3) : 2);
            }

            anim.SetTrigger("Attack");
            StartCoroutine(ResetMoveBlock(.45f));
            return true;
        }
        return false;
    }



    private bool HandlePickup()
    {
        if (isGrounded && pickupTimer <= Time.time && pickupInput)
        {
            blockMoving = true;
            pickupTimer = Time.time + pickupInterval;
            anim.SetTrigger("PickUp");
            rb.velocity = Vector2.zero;
            StartCoroutine(ResetMoveBlock(1.3f));

            return true;
        }
        return false;
    }

    private void ScanHittables()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 2);
        foreach (Collider2D collider in colliders)
        {
            if (collider != bc && collider != cc)
            {
                CheckPoint c = collider.GetComponent<CheckPoint>();
                if (c != null)
                {
                    int amount = GameObject.FindObjectsOfType<EnemyKnight>().Length;
                    if (amount == 0)
                    {
                        c.onHit((collider.transform.position - transform.position).normalized);
                    }
                }
                else
                {

                    collider.GetComponent<Hittable>()?.onHit((collider.transform.position - transform.position).normalized);
                }

            }
        }

    }

    private bool ScanCheckpoints()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 2);
        foreach (Collider2D collider in colliders)
        {
            if (collider != bc)
            {
                CheckPoint c = collider.GetComponent<CheckPoint>();
                if (c != null && c.knocks > 0)
                {
                    //check if there are still enemy's afoot
                    int amount = GameObject.FindObjectsOfType<EnemyKnight>().Length;
                    if (amount > 0)
                    {
                        HintsController.INSTANCE.ShowText("I won't open until all the guards are gone ! (" + amount + " remaining");
                        return false;
                    }
                    else
                    {
                        HintsController.INSTANCE.ShowText("A safehouse ! Knock 3 times on the door...");
                        return true;
                    }
                }


            }
        }
        return false;
    }

    private void ScanPickupItems()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 2);
        foreach (Collider2D collider in colliders)
        {
            collider.GetComponent<Baby>()?.PickUp(this);
        }
    }


    public void PlayFootStep()
    {
        footstepAudio.pitch = .8f + (float)systemRand.NextDouble() * .2f;
        footstepAudio.PlayOneShot(footstepAudio.clip);
    }


    private IEnumerator ResetMoveBlock( float time )
    {
        yield return new WaitForSeconds(time);
        UnblockMovement();
    }


    public void onHit( Vector2 direction )
    {
        if (carryingBaby)
        {
            carryingBaby = false;
            BabyGraphics.SetActive(false);
            BabyObject.gameObject.SetActive(true);
            BabyObject.rb.velocity = Vector3.zero;
            BabyObject.transform.parent = transform;
            BabyObject.transform.localPosition = Vector3.zero;
            BabyObject.rb.velocity = new Vector2(Random.Range(-4f, 4f), 10f);
            HintsController.INSTANCE.ShowText("Oh no !!!! Retrieve the baby !");
        }
        else
        {
            //do nothing?
        }
    }

    private void UnblockMovement()
    {
        blockMoving = false;
    }

    private bool checkBehindObject()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, .05f, hideLayers);
        return colliders != null && colliders.Length > 0;
    }

}
