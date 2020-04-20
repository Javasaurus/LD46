using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ForestGenerator : MonoBehaviour
{

    private const int finalLevel = 12;
    private GameObject cam;

    public GameObject CheckPointPrefab;
    public GameObject EndPointPrefab;

    public GameObject rightBorder;
    public GameObject leftBorder;

    public ForestLayer[] forestLayers;
    public SpawnableLayer[] spawnableLayers;

    [HideInInspector]
    public bool doParallax = false;


    public int[] fibbonacci = new int[] { 0, 1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89 };


    public BoxCollider2D bc;
    // Start is called before the first frame update
    public void InitChunk( bool spawnCheckpoint, int waveNr )
    {

        UnlockLeft();
        UnlockRight();
        cam = Camera.main.gameObject;
        //  bc = GetComponent<BoxCollider2D>();
        foreach (ForestLayer layer in forestLayers)
        {
            GenerateTrees(layer);
        }

        foreach (SpawnableLayer layer in spawnableLayers)
        {
            if (spawnCheckpoint && !layer.allowInSpawnZone)
            {
                //TODO move it to the next or previous?
            }
            else
            {
                GenerateSpawnables(layer);
            }
        }

        // spawn a house if needed
        if (spawnCheckpoint)
        {

            GameObject checkPoint = waveNr == finalLevel ? GameObject.Instantiate(EndPointPrefab) : GameObject.Instantiate(CheckPointPrefab);
            checkPoint.transform.position = new Vector2(transform.position.x + (bc.bounds.size.x / 4), 4f);
            checkPoint.transform.parent = transform;
            checkPoint.GetComponent<CheckPoint>()?.EnsureDoorIsClear();
        }

    }

    private void GenerateTrees( ForestLayer layer )
    {
        layer.startpos = transform.position.x;
        layer.length = bc.bounds.size.x;

        for (float x = (transform.position.x - (bc.bounds.size.x / 2)); x < (transform.position.x + (bc.bounds.size.x / 2)); x += layer.interval)
        {

            float value = Mathf.PerlinNoise((transform.position.x + x) / 10, layer.layerY);

            if (value > 1 - layer.density)
            {
                GameObject instance = GameObject.Instantiate(layer.frontTreePrefabs[Random.Range(0, layer.frontTreePrefabs.Length)]);
                instance.transform.position = new Vector2(x + Random.Range(-layer.interval, layer.interval), layer.layerY + Random.Range(-.08f, .08f));
                instance.transform.localScale = new Vector3(Random.Range(.95f, 1.05f), Random.Range(.95f, 1.05f), 1);
                instance.transform.parent = layer.parent;
                BoxCollider2D tc = instance.GetComponent<BoxCollider2D>();
                SpriteRenderer tsr = instance.GetComponent<SpriteRenderer>();
                tsr.color = layer.layerColor;
                tsr.sortingOrder = layer.parallaxLayer;

                float ySize = tc.bounds.size.y / 2;


                if (!layer.keepTriggers)
                {
                    GameObject.Destroy(tc);
                }


            }
        }
    }

    private void GenerateSpawnables( SpawnableLayer layer )
    {
        ForestManager manager = ForestManager.INSTANCE;
        if (manager.chunkCount <= 2 || manager.chunkCount <= layer.minChunkCount)
        {
            return;
        }

        int expectedAmount = 8 + (3 * (manager.waveNr > fibbonacci.Length ? fibbonacci[fibbonacci.Length] : fibbonacci[manager.waveNr]));
        Debug.Log(manager.chunkCount + " - " + expectedAmount + " will be spawned");
        for (int i = 0; i < expectedAmount; i++)
        {
            //ignore the rarity factor for the jam
            //   if (1==1||Random.Range(0f, 1f) >= layer.rarity)
            //      {
            //spawn at a random spot 
            float x = transform.position.x + Random.Range(.05f, .95f) * bc.bounds.size.x;
            GameObject spawnable = GameObject.Instantiate(layer.prefab);
            spawnable.transform.position = new Vector2(x, layer.yValue);
            spawnable.transform.parent = transform;

            EnemyKnight enemy = spawnable.GetComponent<EnemyKnight>();
            if (enemy != null)
            {
                enemy.hp = 3 + manager.chunkCount / 2;
            }


            //        }
        }


    }

    private void Paralax( ForestLayer layer )
    {
        layer.startpos = transform.position.x;

        float temp = cam.transform.position.x * (1 - layer.parallaxEffect);
        float dist = (cam.transform.position.x * layer.parallaxEffect);

        layer.parent.position = new Vector3(transform.position.x - dist, layer.parent.position.y, layer.parent.position.z);

        if (temp > layer.startpos + layer.length)
        {
            layer.startpos += layer.length;
        }
        else if (temp < layer.startpos - layer.length)
        {
            layer.startpos -= layer.length;
        }


    }

    public void FixedUpdate()
    {
        if (!doParallax)
        {
            return;
        }

        foreach (ForestLayer layer in forestLayers)
        {
            Paralax(layer);
        }
    }

    public void OnTriggerEnter2D( Collider2D collision )
    {
        if (collision.GetComponent<CharacterController>())
        {
            ForestManager.currentChunk = this;
            doParallax = true;
        }
    }
    public void OnTriggerExit2D( Collider2D collision )
    {
        if (collision.GetComponent<CharacterController>())
        {
            doParallax = false;
        }
    }

    public void LockLeft()
    {
        leftBorder.SetActive(true);
    }

    public void LockRight()
    {
        rightBorder.SetActive(true);
    }
    public void UnlockLeft()
    {
        leftBorder.SetActive(false);
    }

    public void UnlockRight()
    {
        rightBorder.SetActive(false);
    }
}

[System.Serializable]
public class ForestLayer
{
    [HideInInspector]
    public float length, startpos;

    public int parallaxLayer;
    public float parallaxEffect;


    public float interval = 4f;
    public Color layerColor;
    public Transform parent;
    public float density = .27f;
    public GameObject[] frontTreePrefabs;
    public float layerY;
    public float layerOffsetX;
    public bool keepTriggers = true;
}

[System.Serializable]
public class SpawnableLayer
{
    public bool allowInSpawnZone = false;
    public float yValue;
    public GameObject prefab;
    public float rarity;
    public int minChunkCount;
}