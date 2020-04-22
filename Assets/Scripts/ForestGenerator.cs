using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ForestGenerator : MonoBehaviour
{

    private const int finalLevel = 6;
    private GameObject cam;

    public GameObject CheckPointPrefab;
    public GameObject EndPointPrefab;

    public GameObject rightBorder;
    public GameObject leftBorder;

    public ForestLayer[] forestLayers;
    public SpawnableLayer[] spawnableLayers;

    public int[] fibbonacci = new int[] { 0, 2, 5, 8, 13, 21, 34, 55, 89 };

    private List<GameObject> objects;

    public BoxCollider2D bc;
    // Start is called before the first frame update
    public void InitChunk( bool spawnCheckpoint,int amount, int waveNr,float xOffset )
    {
        objects = new List<GameObject>();

        ForestManager manager = ForestManager.INSTANCE;
        UnlockLeft();
        UnlockRight();
        cam = Camera.main.gameObject;
        //  bc = GetComponent<BoxCollider2D>();
        foreach (ForestLayer layer in forestLayers)
        {
            switch (layer.parallaxLayer)
            {
                case 0:
                    layer.parent = manager.front;
                    break;
                case 1:
                    layer.parent = manager.back1;
                    break;
                case 2:
                    layer.parent = manager.back2;
                    break;
                default:
                    layer.parent = manager.back3;
                    break;
            }

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
            objects.Add(checkPoint);
            if (waveNr == finalLevel)
            {
                HintsController.INSTANCE?.ShowText("The castle is right up ahead ! Let's go !");
            }
            //  checkPoint.transform.parent = forestLayers[0].parent;

            checkPoint.transform.position = transform.position + (Vector3.right * 22.6f)+(Vector3.up*4.2f);
            
   

            checkPoint.GetComponent<CheckPoint>()?.EnsureDoorIsClear();
        }
        Camera.main.GetComponent<CameraFollow>().onCameraTranslate += Parallax;
    }

    private void GenerateTrees( ForestLayer layer )
    {
        layer.startpos = transform.position.x;
        layer.length = bc.bounds.size.x;

        for (float x = (transform.position.x - 2*(bc.bounds.size.x)); x < (transform.position.x + 2*(bc.bounds.size.x)); x += layer.interval)
        {

            float value = Mathf.PerlinNoise((transform.position.x + x) / 10, layer.layerY);

            if (value > 1 - layer.density)
            {
                GameObject instance = GameObject.Instantiate(layer.frontTreePrefabs[Random.Range(0, layer.frontTreePrefabs.Length)]);
                instance.transform.localPosition = new Vector2(x+Random.Range(-layer.interval, layer.interval), layer.layerY + Random.Range(-.18f, .18f));
                instance.transform.localScale = new Vector3(Random.Range(.95f, 1.05f), Random.Range(.95f, 1.05f), 1);
                instance.transform.parent = layer.parent;
                BoxCollider2D tc = instance.GetComponent<BoxCollider2D>();
                SpriteRenderer tsr = instance.GetComponent<SpriteRenderer>();
                tsr.color = layer.layerColor;
                tsr.sortingOrder = 3 - layer.parallaxLayer;
                objects.Add(instance);
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
        if (manager.chunkCount <= 1 || manager.chunkCount <= layer.minChunkCount)
        {
            return;
        }

        int expectedAmount = 8 + (3 * (manager.waveNr > fibbonacci.Length ? fibbonacci[fibbonacci.Length] : fibbonacci[manager.waveNr]));
        Debug.Log(manager.chunkCount + " - " + expectedAmount + " will be spawned");
        for (int i = 0; i < expectedAmount; i++)
        {
            //spawn at a random spot 
            float x = transform.position.x + Random.Range(.05f, .95f) * bc.bounds.size.x;
            GameObject spawnable = GameObject.Instantiate(layer.prefab);
            spawnable.transform.position = new Vector2(x, layer.yValue);
            spawnable.transform.parent = null;
            objects.Add(spawnable);
            EnemyKnight enemy = spawnable.GetComponent<EnemyKnight>();
            if (enemy != null)
            {
                enemy.hp = 3 + (manager.waveNr * 2);
            }


            //        }
        }


    }

    public void Parallax( float delta )
    {
        if (ForestManager.currentChunk == this)
        {
            foreach (ForestLayer layer in forestLayers)
            {
                ParallaxLayer(layer, delta);
            }
        }
    }

    private void ParallaxLayer( ForestLayer layer, float delta )
    {
        Vector3 newPos = layer.parent.localPosition;
        newPos.x += delta * layer.parallaxEffect;
        layer.parent.localPosition = newPos;
    }

    public void UnLoad()
    {
        foreach (GameObject go in objects)
        {
            GameObject.Destroy(go);
        }
        Destroy(gameObject);
    }

    public void OnTriggerEnter2D( Collider2D collision )
    {
        if (collision.GetComponent<CharacterController>())
        {
            ForestManager.currentChunk = this;
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