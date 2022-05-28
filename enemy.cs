using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class enemy : NetworkBehaviour
{
    public bool isTurret;
    public Material deadmat;
    public GameObject nozzle, tower;
    public GameObject[] nozzleend;
    public GameObject hpbar;
    public GameObject bullet,explosion;
    public GameObject target;
    public Vector3 targetpoint;
    public int agressiontimer;
    public int firerate = 5;
    public bool isParallelNozzles=true;
    public float needgetdown=1f;
    public bool DEBUGisLinesShowing=false;
    public float tankspeed;
    private float speed;
    private bool isMoving, isManeuver;
    private Rigidbody rb;
    private RaycastHit hit;
    private Vector3 buftrgt;
    private float mindelta;
    private int staytimer;
    private MeshRenderer m;
    public NetworkVariable<int> hp = new NetworkVariable<int>(100);
    private int localhp,maxhp;
    private int localbulletcounter;
    [ServerRpc]
    void FireServerRpc(ServerRpcParams rpcParams = default)
    {
        int i;
        if(isParallelNozzles){
            for(i=0;i<nozzleend.Length;++i){
            GameObject bul = Instantiate(bullet, nozzleend[i].transform.position, nozzleend[i].transform.rotation);
            bul.GetComponent<NetworkObject>().Spawn();
            bul.GetComponent<Rigidbody>().AddRelativeForce(-120f, 0, 0, ForceMode.Impulse);
            Destroy(bul.gameObject, 2f);
            }
        }else{
            i=localbulletcounter%nozzleend.Length;
            GameObject bul = Instantiate(bullet, nozzleend[i].transform.position, nozzleend[i].transform.rotation);
            bul.GetComponent<NetworkObject>().Spawn();
            bul.GetComponent<Rigidbody>().AddRelativeForce(-120f, 0, 0, ForceMode.Impulse);
            Destroy(bul.gameObject, 2f);
            ++localbulletcounter;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        maxhp=hp.Value;
       // localhp=hp.Value;
    }
    void AllChildBlack(Transform trans){
                if(trans.TryGetComponent<MeshRenderer>(out m)){
                 m.material=deadmat;
                 //   m.material.SetColor("Color",Color.black);
                 //   print(m.material.GetColor("Color"));
                }
            for(int i=0;i<trans.childCount;++i){
                AllChildBlack(trans.GetChild(i));
            }
    }
    // Update is called once per frame
    void Update()
    {
        if (hp.Value < 0) {
            rb.isKinematic = false;
            GameObject exp =Instantiate(explosion,transform.position,transform.rotation);
            Destroy(exp.transform.GetChild(0).gameObject,0.3f);
            Destroy(exp,3f);
            AllChildBlack(transform);
            if(isTurret)transform.Translate(0, 2, 0);
            rb.AddRelativeForce(Random.Range(-30,30), 500f, Random.Range(-30,30), ForceMode.Impulse);
            hpbar.SetActive(false);
            enabled = false;
        }
        if (IsServer) {
            if (agressiontimer > 0) {
                if (target)
                {
                    tower.transform.LookAt(new Vector3(target.transform.position.x,tower.transform.position.y,target.transform.position.z));
                    tower.transform.Rotate(0,180,0);
                    nozzle.transform.LookAt(target.transform.position);
                    nozzle.transform.Rotate(0,180,0);
                    if (agressiontimer % (1000/firerate)==0) {
                        FireServerRpc();
                    }
                    if(!isTurret){
                        if(!isManeuver){
                            for(int i=0;i<5;++i){
                                if(Vector3.Distance(transform.position,target.transform.position)<30&&Random.Range(0,3)==0)
                                    {
                                        buftrgt=new Vector3(transform.position.x+Random.Range(-15f,15f),transform.position.y-needgetdown,transform.position.z+Random.Range(-15f,15f));
                                    }
                                else
                                    {
                                        buftrgt=new Vector3(target.transform.position.x+Random.Range(-15f,15f),target.transform.position.y-needgetdown,target.transform.position.z+Random.Range(-15f,15f));
                                    }
                              if(DEBUGisLinesShowing)  Debug.DrawRay(transform.position+new Vector3(0,1f,0),buftrgt-transform.position,Color.white,4f);
                                Physics.Raycast(transform.position+new Vector3(0,1f,0), buftrgt-transform.position, out hit);
                                if(!hit.transform||hit.transform.name=="пол")
                                {
                                    targetpoint=buftrgt;
                                    isManeuver=true;
                                    Debug.DrawRay(transform.position+new Vector3(0,1f,0),buftrgt-transform.position,Color.cyan,20f);
                                    break;
                                }
                            }
                        }else if(Vector3.Distance(targetpoint,transform.position)>1.5f){
                            Debug.DrawRay(targetpoint,Vector3.up,Color.cyan);
                            transform.LookAt(new Vector3(targetpoint.x,transform.position.y,targetpoint.z));
                            transform.Rotate(0,180,0);
                            if(speed>-10f){speed-=3f*Time.deltaTime;isMoving=true;}
                            if(rb.velocity.magnitude<10f){rb.AddRelativeForce(0,0,speed*tankspeed*Time.deltaTime);}
                            if(mindelta-Vector3.Distance(transform.position, targetpoint)<0.1f){++staytimer;}else{staytimer=0;}
                            if(staytimer==500){isManeuver=false;staytimer=0;}
                            mindelta=Vector3.Distance(transform.position, targetpoint);
                        }else{
                            isManeuver=false;
                        }
                            if(!isMoving){
                                if (speed < -5f) { speed += 5f * Time.deltaTime;  }
                                if (speed > 5f) { speed -= 5f * Time.deltaTime;  }
                                if (speed > 0f && speed < 5f) { speed = 0f; }
                                if (speed < 0f && speed > -5f) { speed = 0f; }
                            }
                            isMoving=false;
                    }
                }
                --agressiontimer; 
            } else
            {
                for (int i = 0; i < GameObject.FindGameObjectsWithTag("Player").Length; ++i)
                {
                    if (Random.Range(0, 2) == 0 && Vector3.Distance(transform.position, GameObject.FindGameObjectsWithTag("Player")[i].transform.position) < 100f)
                    {
                        Debug.DrawRay(transform.position, GameObject.FindGameObjectsWithTag("Player")[i].transform.position - transform.position, Color.cyan);
                        Physics.Raycast(nozzleend[0].transform.position, GameObject.FindGameObjectsWithTag("Player")[i].transform.position - nozzleend[0].transform.position, out hit);
                        if (hit.collider&&hit.collider.gameObject == GameObject.FindGameObjectsWithTag("Player")[i]) 
                        {
                            target = GameObject.FindGameObjectsWithTag("Player")[i];
                            agressiontimer = 1000;
                        }
                    }
                }
            }
        }
        if(localhp!=hp.Value){
            localhp=hp.Value;
            hpbar.GetComponent<TextMesh>().text=localhp+"/"+maxhp;
            hpbar.transform.GetChild(0).transform.localScale=new Vector3(10.23f/maxhp*localhp,1f,1.38f);
        }
    }
    private void OnDrawGizmos()
    {
        if (target) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(target.transform.position,1f);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (IsServer)
        {
            if (collision.gameObject.name == "Shell(Clone)")
            {
                hp.Value -= 15;
            }
        }
    }
}
