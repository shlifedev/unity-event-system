using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using LD.Framework;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using Random = UnityEngine.Random;

public class CubeGenerator : MonoBehaviour, 
    IEventListener<CubeCreateMessage>,
    IEventListener<CubeMergeMessage>
{
    public GameObject CubePrefab;
 
    private float cooltime = 0;
    public float nextTime = 0.5f;

    private List<GameObject> cubes = new();
    private void Update()
    {
       
        
        cooltime -= Time.deltaTime;
        if (cooltime <= 0)
        {
            cooltime = nextTime;
            EventBus.Broadcast(new CubeCreateMessage(Vector3.one * Random.Range(0.01f, 0.5f), false));
        }
    }

    void OnEnable()
    {
        EventBus.Register(this);
    }

    void OnDisable()
    {
        EventBus.Unregister(this);
    } 

    public void OnEventRaised(CubeCreateMessage eventMessageArgs)
    { 
        var obj = GameObject.Instantiate(CubePrefab);
        obj.transform.position = this.transform.position;
        obj.transform.localScale = eventMessageArgs.Scale;
        if (!eventMessageArgs.Merged)
            cubes.Add(obj);
        else
        {
            obj.transform.position += new Vector3(0, 2, 0);
            obj.GetComponent<Rigidbody>().AddForce(new Vector3(0, 200, 0), ForceMode.Impulse);
        }
    }

    public void OnEventRaised(CubeMergeMessage eventMessageArgs)
    {  
        cubes.ForEach(x=>GameObject.DestroyImmediate(x));
        cubes.Clear();

        EventBus.Broadcast(new CubeCreateMessage(new Vector3(2, 2, 2), true));

    }
 
}
