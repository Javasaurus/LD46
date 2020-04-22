using System.Collections.Generic;
using UnityEngine;

public class ForestManager : MonoBehaviour
{
    public static ForestManager INSTANCE;
    public static ForestGenerator currentChunk;
    public ForestGenerator chunkPrefab;
    private ForestGenerator maxChunk;

    public List<ForestGenerator> existingChunks;
    public int chunkCount = 1;
    public int waveNr = 0;
    public int totalAmount = 1;

    public Transform front;
    public Transform back1;
    public Transform back2;
    public Transform back3;

    public void Awake()
    {
        Random.InitState(12080511);
        INSTANCE = this;
        existingChunks = new List<ForestGenerator>();
        ForestGenerator instance = GameObject.Instantiate(chunkPrefab) as ForestGenerator;
        instance.transform.position = Vector3.right * 0f;
        instance.transform.parent = transform;
        maxChunk = instance;

        chunkCount++;
        instance.InitChunk(true,0, 0,30);
        existingChunks.Add(instance);
        /*
        GenerateNext();
        */
    }

    public void GenerateNext()
    {
       /* 
        front.transform.localPosition = Vector3.zero;
        back1.transform.localPosition = Vector3.zero;
        back2.transform.localPosition = Vector3.zero;
        back3.transform.localPosition = Vector3.zero;
        */



        waveNr++;
        //delete the existing ones
        foreach (ForestGenerator chunk in existingChunks)
        {
            if (maxChunk != chunk)
            {
                chunk.UnLoad();
            }
        }
        existingChunks.Clear();
        existingChunks.Add(maxChunk);

        //TODO make this a level formula ...
        int amount = 3;

        //CREATE A NEW CHUNK WITH A NEW
        maxChunk.LockLeft();
        maxChunk.UnlockRight();
        for (int i = 0; i < amount; i++)
        {
            totalAmount += 1;
            ForestGenerator instance = GameObject.Instantiate(chunkPrefab) as ForestGenerator;
            instance.transform.position = maxChunk.transform.position + Vector3.right * 50f;
            instance.transform.parent = transform;
            maxChunk = instance;
            chunkCount++;
            instance.InitChunk(i == (amount - 1), totalAmount, waveNr,amount*30);
            existingChunks.Add(instance);
        }
        maxChunk.UnlockLeft();
        maxChunk.LockRight();


    }

    public void LateUpdate()
    {
        if (existingChunks.Count == 1)
        {
            maxChunk.LockLeft();
            maxChunk.LockRight();
        }
    }

}
