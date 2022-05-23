using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class cameralobby : MonoBehaviour
{
    public GameObject manager;
    public string ip="127.0.0.1", nickname;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnGUI()
    {
        GUI.Box(new Rect(Screen.width * 0.5f - 100, Screen.height * 0.5f, 30, 30), "IP");
        ip = GUI.TextField(new Rect(Screen.width*0.5f-70,Screen.height*0.5f,200,30),ip);
        GUI.Box(new Rect(Screen.width * 0.5f - 100, Screen.height * 0.5f+30, 30, 30), "Ник");
        nickname = GUI.TextField(new Rect(Screen.width*0.5f-70,Screen.height*0.5f+30,200,30),nickname);
        if (GUI.Button(new Rect(Screen.width * 0.5f - 100, Screen.height * 0.5f + 60, 100, 30), "Хост")) {
            NetworkManager.Singleton.StartHost();
            gameObject.SetActive(false);
        }
        if (GUI.Button(new Rect(Screen.width * 0.5f, Screen.height * 0.5f + 60, 100, 30), "Клиент")) {
            manager.GetComponent<UnityTransport>().ConnectionData.Address= ip;
            NetworkManager.Singleton.StartClient();
            gameObject.SetActive(false);
        }

        if(GUI.Button(new Rect(0,0,100,20),"Графика бе(")){QualitySettings.SetQualityLevel(2,true); }
        if(GUI.Button(new Rect(0,20,100,20),"Графика норм")){QualitySettings.SetQualityLevel(1,true); }
        if(GUI.Button(new Rect(0,40,100,20),"Графика вау)")){QualitySettings.SetQualityLevel(0,true); }
    }
}
