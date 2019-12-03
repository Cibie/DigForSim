using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.AI;
using System.IO;
using UnityEditor;
using System;
using System.Linq;
using System.Globalization;

public class GameController : MonoBehaviour
{
    public GameObject wAgent;
    public GameObject specAgent;
    public GameObject objective;
    public GameObject terrain;
    Vector3 terrainSize;
    Vector3 terrainCenter;

    AgentsLog persScript;
    private string[] filePaths = Directory.GetFiles("./AgentsLog");
    public float totAgents, totAgentsaux;
    private float wanderingAgentsNumber;
    private int checkPercentage;
    private int numberOfCurrentAgents;
    private bool RoundCeiling = false;
    private int counter = 0;
    private int nLines;
    public float simulationStart;
    private Dictionary<String, Color> colors = new Dictionary<String, Color>();
    public NavMeshSurface[] surfaces;
    private StreamWriter agentLister;
    string[] datas;
    string[] datas2;
    string filepath = "./ConfigFiles/";

    [HideInInspector]
    public bool following = false;

    [HideInInspector]
    public float time = 0f;

    [HideInInspector]
    public float endingTime = 0f;

    [HideInInspector]
    public float logInvokeTime;

    public static GameController inst;

    void Awake()
    {
        if (inst != null)
            return;
        inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        simulationStart = Time.time;
        filepath = filepath + SceneController.inst.filename;
        if (File.Exists("AgentList.txt"))
            File.WriteAllText("AgentList.txt", string.Empty);
        agentLister = new StreamWriter("AgentList.txt", true);


        //Creating Color Dictionary:

        colors["blue"] = Color.blue;
        colors["red"] = Color.red;
        colors["yellow"] = Color.yellow;
        colors["green"] = Color.green;
        colors["cyan"] = Color.cyan;
        colors["black"] = Color.black;
        colors["white"] = Color.white;
        colors["magenta"] = Color.magenta;
        colors["grey"] = Color.grey;

        //Creating the NavMesh
        for (int i = 0; i < surfaces.Length; i++)
            surfaces[i].BuildNavMesh();
        //In those line The program skip the first line of the config files and creates the given objective for the simulations
        for (int i = 0; i < filePaths.Length; i++)
        {
            File.Delete(filePaths[i]);
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true; //Locking the cursor on the foreground of the screen and making it visible 
        var terr = terrain.GetComponent<Terrain>();
        terrainSize = terr.terrainData.size;                            // Getting boundaries of the terrain pasesed
        terrainCenter = terrain.GetComponent<Collider>().bounds.center; // to the GameController
        string[] filelines = File.ReadAllLines(filepath);               
        logInvokeTime = float.Parse(filelines[1].Replace(".",",")); // Getting the Time interval from the config file
        var line = filelines[4].Split(' ');
        for (int i = 0; i < line.Length; i += 2) // Starting to take information from the Config file
        {
            if (line[i] != "") // Getting objects to insert in the scene
            {
                GameObject obj;
                obj = Instantiate(objective) as GameObject;
                obj.name = line[i]; // elem i is the object
                Vector3 sP = GameObject.Find(line[i + 1]).transform.position; // elem i+1 is the location of which the program
                                                                              // has to take coordinates for creating the obj
                sP.y += 2;
                obj.transform.position = sP;
            }
        }

        //In those line The program skip description lines and take the total number of agents from the fifth line of the file
        line = filelines[7].Split(' ');
        wanderingAgentsNumber = int.Parse(line[1]);
        //Spawn a certain number of each type of agents


        for (int i = 5; i < filelines.Length; i++) // Here manage the creation of Agents
        {
            GameObject person = null;
            if (i >= 13)// This manage the creation of special Agents
            {
                CreateSpecialAgent(person, filelines[i]);
                totAgents++;
            }
            else if (i >= 8 && i <= 10 && wanderingAgentsNumber != 0) //this manage creation of all Wandering agents
            {
                line = filelines[i].Split(' ');
                if (!RoundCeiling)// Here we alternate rounding up and rounding down for managing cases of values as 7,5
                {// number of current agents is the number of agents of the current type
                    numberOfCurrentAgents = (int)Mathf.Floor((wanderingAgentsNumber / 100) * float.Parse(line[1]));
                    RoundCeiling = true;
                }
                else
                {
                    numberOfCurrentAgents = (int)Mathf.Ceil((wanderingAgentsNumber / 100) * float.Parse(line[1]));
                    RoundCeiling = false;
                }
                checkPercentage += int.Parse(line[1]);
                for (int j = 0; j < numberOfCurrentAgents; j++)
                {
                    CreateWanderingAgents(person, filelines[i]);
                }
            }
        }
        if (checkPercentage != 100 && wanderingAgentsNumber != 0)// If the Sum of all percentages it's more than 100%
        {                                                        // application is automatically quitted
            print("Check percentages, their sum is different from 100");
            QuitApp();
        }
        totAgentsaux = totAgents;
        agentLister.Close(); 

    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Escape) || totAgentsaux == 0)  // The "Esc" button on the keyboard allow the user to stop the execution and quit the application or all special agents are destroyed
            QuitApp();
        //checkPercentage = 0;    
    }

    public static Vector3 GetRandomPoint(Vector3 center, float maxDistance)
    {
        // Get Random Point inside Sphere which position is center, radius is maxDistance
        Vector3 randomPos = Random.insideUnitSphere * maxDistance + center;
        int walkMask = 1 << NavMesh.GetAreaFromName("Walkable");//Returns the area index for a named NavMesh area type.
        NavMeshHit hit; // NavMesh Sampling Info Container            
        // from randomPos find a nearest point on NavMesh surface in range of maxDistance
        NavMesh.SamplePosition(randomPos, out hit, maxDistance, walkMask);
        return hit.position;
    }

    public int GetCounter()
    {
        return counter;
    }

    private void QuitApp()// this function create a single log file that merge all the agents log file
    {
        //This if have to be uncommented if working on the editor and commented if building the executable file
        if(Application.isEditor)
            EditorApplication.isPlaying = false; // stop the editor
        //Mergin all the Log file in a single one
        string[] outputs = Directory.GetFiles("./SimulationLogs");
        nLines = (int)Mathf.Floor(time / logInvokeTime);
        for (int i = 0; i < outputs.Length; i++)
        {
            File.Delete(outputs[i]);
        }
        StreamWriter writer = new StreamWriter("./SimulationLogs/Output.txt", true);
        writer.WriteLine("TotalNumberOfAgents: " + totAgents + " EndingTime: " + endingTime);
        for (int i = 0; i <= nLines; i++) // <= because we have one additional line for each output file, that is the one of the initial position
        {
            for (int j = 0; j < (int)totAgents; j++) // If the file is shorter than the others the program will
            {                                        // look for the line and will manage the exception doing nothing
                datas = File.ReadAllLines("./AgentsLog/Output" + j + ".txt");
                try
                {
                    writer.WriteLine(datas[i]);
                }
                catch
                {
                }
            }
        }
        writer.Close();
        Application.Quit(); //stop the application when launched by executable file
    }

    public void CreateSpecialAgent(GameObject person, string fileline)
    {
        var line = fileline.Split(' ');
        agentLister.WriteLine(line[0]);
        Vector3 spawnPos = GameObject.Find(line[1]).transform.position;
        person = Instantiate(specAgent, spawnPos, transform.rotation) as GameObject;
        person.name = line[0];
        var script = person.GetComponent<SpecialAgent>();
        var logF = person.GetComponent<AgentsLog>();
        var renderer = person.GetComponent<Renderer>();
        renderer.material.color = colors[line[2].ToLower()];
        script.agent.speed = float.Parse(line[3].Replace(".",","));
        logF.area = line[1];
        for (int k = 4; k < line.Length; k++)
        {
            script.objectives.Add(line[k]);
        }
        persScript = person.GetComponent<AgentsLog>();
        persScript.ID = counter; // set ID of each agent with an incremental number

        counter++;
    }

    private void CreateWanderingAgents(GameObject person, string fileline)
    {
        var line = fileline.Split(' ');
        Vector3 spawnPos = GetRandomPoint(terrainCenter, (terrainSize.x / 2) - 10);
        person = Instantiate(wAgent, spawnPos, transform.rotation) as GameObject;
        var script = person.GetComponent<WanderingAgents>();
        script.wanderRadius = float.Parse(line[2]); // set radius of the zone
        script.wanderTimer = float.Parse(line[3]); // set time passed in that zone
        script.agent.speed = float.Parse(line[4].Replace(".",",")); // set agent speed movement
    }

    private void OnApplicationQuit()
    {
        QuitApp();
    }

}
