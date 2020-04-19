using UnityEngine;

public class Baby : MonoBehaviour, Hittable, Pickup
{
    public CharacterController parent;

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
    }

    public void PickUp( CharacterController c )
    {
        c.carryingBaby = true;
        c.BabyGraphics.SetActive(true);
        c.BabyObject.gameObject.SetActive(false);
        c.BabyObject.rb.velocity = Vector2.zero;
        c.BabyObject.transform.parent = transform;
        c.BabyObject.transform.localPosition = Vector2.zero;
        Debug.Log("baby picked up");
    }
}
