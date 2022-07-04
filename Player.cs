using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public GameObject tower, nozzle, cam,aviacam;
    public GameObject[] nozzleend;
    public GameObject[] avianozzleend;
    public GameObject[] rocketnozzleend;
    public GameObject bullet,rocket;
    public GameObject tank,avia;
    public Material crystall;
    private Rigidbody rb;
    private float speed;
    private Vector2 mouse;
    private bool isMoving;
    public bool isAviaLocal;
    public NetworkVariable<int> hp = new NetworkVariable<int>(100);
    public NetworkVariable<bool> isAvia=new NetworkVariable<bool>();

  //  public NetworkVariable<FixedString64Bytes> PlayerName = new NetworkVariable<FixedString64Bytes>(new FixedString64Bytes("Player"));


    [ServerRpc]
    void SetNickServerRpc(string nickname, ServerRpcParams rpcParams = default)
    {
      //  PlayerName.Value = new FixedString64Bytes(nickname);
    }
    [ServerRpc]
    void SetAviaServerRpc(bool isAvia, ServerRpcParams rpcParams = default)
    {
      this.isAvia.Value=isAvia;
    }
    [ServerRpc]
    void FireServerRpc(bool isRocket,ServerRpcParams rpcParams = default)
    {
        if (!isRocket)
        {
            if (!isAvia.Value)
            {
                for (int i = 0; i < nozzleend.Length; ++i)
                {
                    GameObject bul = Instantiate(bullet, nozzleend[i].transform.position, nozzleend[i].transform.rotation);
                    bul.GetComponent<NetworkObject>().Spawn();
                    bul.GetComponent<Rigidbody>().AddRelativeForce(-120f, 0, 0, ForceMode.Impulse);
                    Destroy(bul.gameObject, 2f);
                }
            }
            else
            {
                for (int i = 0; i < avianozzleend.Length; ++i)
                {
                    GameObject bul = Instantiate(bullet, avianozzleend[i].transform.position, avianozzleend[i].transform.rotation);
                    bul.GetComponent<NetworkObject>().Spawn();
                    bul.GetComponent<Rigidbody>().AddRelativeForce(-120f, 0, 0, ForceMode.Impulse);
                    Destroy(bul.gameObject, 2f);
                }

            }
        }
        else
        {
            for (int i = 0; i < rocketnozzleend.Length; ++i)
            {
                GameObject bul = Instantiate(rocket, rocketnozzleend[i].transform.position, rocketnozzleend[i].transform.rotation);
                bul.GetComponent<NetworkObject>().Spawn();
                bul.GetComponent<Rigidbody>().AddRelativeForce(-110f, 0, 0, ForceMode.Impulse);
                Destroy(bul.gameObject, 3f);
            }

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (!IsLocalPlayer)
        {
            cam.SetActive(false);
            aviacam.SetActive(false);
        }
        else 
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            //SetNickServerRpc(GameObject.Find("Main Camera").GetComponent<cameralobby>().nickname);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsLocalPlayer)
        {
            if(!isAvia.Value){
            isMoving = false;
            mouse.x = Input.GetAxis("Mouse X");
            mouse.y = Input.GetAxis("Mouse Y");
            if (Input.GetKey(KeyCode.W)) { if (speed == 5f||Mathf.Abs(speed)<0.0001f) { speed = -5f; } if (speed > -10f) { speed -= 8f*Time.deltaTime; } isMoving = true; }
            if (Input.GetKey(KeyCode.S)) { if (speed == -5f||Mathf.Abs(speed)<0.0001f) { speed = 5f; } if (speed < 10f) { speed += 8f * Time.deltaTime; } isMoving = true; }
            if (Input.GetKey(KeyCode.A)) { transform.Rotate(0, -1, 0); }
            if (Input.GetKey(KeyCode.D)) { transform.Rotate(0, 1, 0); }
            if (Input.GetKeyDown(KeyCode.Mouse0)) {
                FireServerRpc(false);
            }
            tower.transform.Rotate(0, mouse.x * 5f, 0);
            nozzle.transform.Rotate(0, 0, -mouse.y * 2f);
            if(nozzle.transform.localRotation.z<-0.17f){nozzle.transform.localRotation=new Quaternion(nozzle.transform.localRotation.x,nozzle.transform.localRotation.y,-0.17f,nozzle.transform.localRotation.w);}
            if(nozzle.transform.localRotation.z>0.03f){nozzle.transform.localRotation=new Quaternion(nozzle.transform.localRotation.x,nozzle.transform.localRotation.y,0.03f,nozzle.transform.localRotation.w);}

            if (!isMoving)
            {
                if (speed < -5f) { speed += 7f * Time.deltaTime;  }
                if (speed > 5f) { speed -= 7f * Time.deltaTime;  }
                if (speed > 0f && speed < 5f) { speed = 0f; }
                if (speed < 0f && speed > -5f) { speed = 0f; }
            }
            rb.AddRelativeForce(speed*Time.deltaTime*12000f, 0, 0);
            if (rb.velocity.magnitude > 10f&&rb.velocity.magnitude<30f) {
                cam.GetComponent<Camera>().fieldOfView = 60 + rb.velocity.magnitude * 0.5f;
            }
            if (rb.velocity.magnitude < 10f) { cam.GetComponent<Camera>().fieldOfView = 60; }
            // SubmitPositionRequestServerRpc(transform.position);
            }
            else{
                isMoving=false;
                mouse.x = Input.GetAxis("Mouse X");
                mouse.y = Input.GetAxis("Mouse Y");
                transform.Rotate(0,mouse.x*3f,mouse.y*3f);
                if (Input.GetKey(KeyCode.W)) { if (speed > -20f) { speed -= 8f*Time.deltaTime; } isMoving = true; }
                if (Input.GetKey(KeyCode.S)) { if (speed < -1f) { speed += 8f * Time.deltaTime; } isMoving = true; }
                if (Input.GetKey(KeyCode.A)) { transform.Rotate(-2, 0, 0); }
                if (Input.GetKey(KeyCode.D)) { transform.Rotate(2, 0, 0); }
                if (Input.GetKeyDown(KeyCode.Mouse0)) {
                FireServerRpc(false);
                }
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    FireServerRpc(true);
                }
                if (!isMoving)
                {
                if (speed < -5f) { speed += 7f * Time.deltaTime;  }}
                transform.Translate(speed*0.1f,0,0);
                    aviacam.GetComponent<Camera>().fieldOfView = 60 - speed;
            }
            if(isAvia.Value!=isAviaLocal){
                isAviaLocal=isAvia.Value;
                tank.SetActive(!isAviaLocal);
                avia.SetActive(isAviaLocal);
                rb.useGravity=!isAviaLocal;
                rb.freezeRotation=isAviaLocal;
                if(isAviaLocal){
                    transform.position=transform.position+ new Vector3(0,-transform.position.y+50,0);
                    speed=-1f;
                }
                }


            ///DEBUG MODE
            if (Input.GetKeyDown(KeyCode.F1)) {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                NetworkManager.Singleton.Shutdown();
                Application.LoadLevel(0);
            }
            if (Input.GetKeyDown(KeyCode.F2)) {
                transform.GetChild(0).GetComponent<MeshRenderer>().material = crystall;
                tower.transform.GetChild(0).GetComponent<MeshRenderer>().material = crystall;
                nozzle.transform.GetChild(0).GetComponent<MeshRenderer>().material = crystall;
            }
            if (Input.GetKeyDown(KeyCode.F3)) {
                transform.rotation = Quaternion.identity;
            }
            if (Input.GetKeyDown(KeyCode.F4)) {
                transform.position = new Vector3();
            }
            if (Input.GetKeyDown(KeyCode.F5))
            {
                rb.isKinematic = true;
            }
            if (Input.GetKeyDown(KeyCode.F6)) {
                rb.isKinematic = false;
            }
            if (Input.GetKeyDown(KeyCode.F7)) {
                SetAviaServerRpc(true);
            }
            if (Input.GetKeyDown(KeyCode.F8)) {
                SetAviaServerRpc(false);
            }
        }
        else {
            if(isAvia.Value!=isAviaLocal){
                isAviaLocal=isAvia.Value;
                tank.SetActive(!isAviaLocal);
                avia.SetActive(isAviaLocal);
                }
           // transform.position = Position.Value;
        }


        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (IsServer) {
            if (collision.gameObject.name == "Shell(Clone)" || collision.gameObject.CompareTag("Shell"))
            {
                hp.Value -= 15;
            }
        }
    }
    private void OnGUI()
    {
        if (IsLocalPlayer)
        {
            GUI.Box(new Rect(Screen.width - 80, Screen.height - 40, 80, 40), "" + speed);
            GUI.Box(new Rect(Screen.width - 80, Screen.height - 80, 80, 40), "" + rb.velocity.magnitude);
            GUI.Box(new Rect(Screen.width - 80, Screen.height - 120, 80, 40), "" + hp.Value);
        }
    }
}
