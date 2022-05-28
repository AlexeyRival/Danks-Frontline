using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class generator : NetworkBehaviour
{
    public GameObject[] fragaments,pointsofinterest;
    public float step;
    public int iterx=50,itery=50;
    private bool isStarted;
    public NetworkVariable<int> seed = new NetworkVariable<int>(0);
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
        if(IsServer&&seed.Value==0){
            seed.Value=Random.Range(1,int.MaxValue);
        }
        if(seed.Value!=0&&!isStarted){
            Random.seed=seed.Value;
            for(int x=0;x<iterx;++x){
                for(int y=0;y<itery;++y){
                    Instantiate(fragaments[Random.Range(0,fragaments.Length)],transform.position,transform.rotation).transform.Rotate(0,90*Random.Range(0,4),0);
                    transform.Translate(step,0,0);
                }
                transform.Translate(-step*iterx,0,step);
            }
            isStarted=true;
        }
    }
}
