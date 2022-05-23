using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public GameObject tower, nozzle, cam,nozzleend;
    public GameObject bullet;
    public Material crystall;
    private Rigidbody rb;
    private float speed;
    private Vector2 mouse;
    private bool isMoving;
    public NetworkVariable<int> hp = new NetworkVariable<int>(100);

  //  public NetworkVariable<FixedString64Bytes> PlayerName = new NetworkVariable<FixedString64Bytes>(new FixedString64Bytes("Player"));


    [ServerRpc]
    void SetNickServerRpc(string nickname, ServerRpcParams rpcParams = default)
    {
      //  PlayerName.Value = new FixedString64Bytes(nickname);
    }
    [ServerRpc]
    void FireServerRpc(ServerRpcParams rpcParams = default) {
        GameObject bul = Instantiate(bullet,nozzleend.transform.position,nozzleend.transform.rotation);
        bul.GetComponent<NetworkObject>().Spawn();
        bul.GetComponent<Rigidbody>().AddRelativeForce(-120f,0,0,ForceMode.Impulse);
        Destroy(bul.gameObject, 2f);
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (!IsLocalPlayer)
        {
            cam.SetActive(false);
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
            isMoving = false;
            mouse.x = Input.GetAxis("Mouse X");
            mouse.y = Input.GetAxis("Mouse Y");
            if (Input.GetKey(KeyCode.W)) { if (speed == 5f) { speed = -5f; } if (speed > -10f) { speed -= 8f*Time.deltaTime; } isMoving = true; }
            if (Input.GetKey(KeyCode.S)) { if (speed == -5f) { speed = 5f; } if (speed < 10f) { speed += 8f * Time.deltaTime; } isMoving = true; }
            if (Input.GetKey(KeyCode.A)) { transform.Rotate(0, -1, 0); }
            if (Input.GetKey(KeyCode.D)) { transform.Rotate(0, 1, 0); }
            if (Input.GetKeyDown(KeyCode.Mouse0)) {
                FireServerRpc();
            }
            tower.transform.Rotate(0, mouse.x * 5f, 0);
            nozzle.transform.Rotate(0, 0, -mouse.y * 2f);

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


            ///DEBUG MODE
            if (Input.GetKeyDown(KeyCode.F1)) {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
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
        }
        else {
           // transform.position = Position.Value;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (IsServer) {
            if (collision.gameObject.name == "Shell(Clone)")
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
