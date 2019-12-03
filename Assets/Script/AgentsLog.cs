using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class AgentsLog : MonoBehaviour
{
    //[HideInInspector]
    public int ID;
    //[HideInInspector]
    public string area = "";
    // Start is called before the first frame update
    private bool ok;
    private SpecialAgent spec;
    void Start()
    {
        //Each Agent generate a proper Log file
        PrintLog();
        InvokeRepeating("PrintLog", GameController.inst.logInvokeTime, GameController.inst.logInvokeTime);
        spec = gameObject.GetComponent<SpecialAgent>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void PrintLog()
    {
        StreamWriter writer = new StreamWriter("./AgentsLog/Output" + ID + ".txt", true);
        var line = "Agent: " + gameObject.transform.name + " Position: " + transform.position + " in " + area + " Time: " + GameController.inst.time;
        GameController.inst.endingTime = GameController.inst.time;
        writer.WriteLine(line);
        writer.Close();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetType() == typeof(MeshCollider))
        {
            area = other.gameObject.transform.name;
        }
    }
}
