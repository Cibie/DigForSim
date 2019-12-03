using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class SpecialAgent : MonoBehaviour
{
    [HideInInspector]
    public List<string> objectives = new List<string>(); // this is structured like this: [objective1, timeAtObjective1, objective2, timeAtObjective2 ...]
    [HideInInspector]
    public float rest;// This is set as the time to rest at a certain target -> timeAtObjectiveN
    private GameObject target;
    private int obj = 0; //Keeps count of position in objectives for setting targets and rest time
    [HideInInspector]
    public NavMeshAgent agent;
    Transform agentCamera;

    //Variables for testing
    private float startTime;
    public float timeNeeded;
    public bool done = false;

    // Start is called before the first frame update
    private void OnEnable()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        agent.Warp(gameObject.transform.position);
        agentCamera = transform.GetChild(0);
        target = GameObject.Find(objectives[obj]);
        startTime = Time.time;
        if (target != null)
            agent.SetDestination(target.transform.position);
        rest = float.Parse(objectives[obj + 1]);
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending || target == null)
        {
            obj += 2;// Change next target position on the objectives array
            if (obj >= objectives.Count)// Check if going out of index with the array
                target = null;
            else
            {
                target = GameObject.Find(objectives[obj]);
                if (target != null)
                    agent.SetDestination(target.transform.position);
                rest = float.Parse(objectives[obj + 1]);
                agent.isStopped = true;//Changing the agent state
            }
        }
        if (target != null)
        {
            if (target.GetComponent<Collider>().GetType() == typeof(CapsuleCollider))
                agent.SetDestination(target.transform.position);
        }

        if (agent.isStopped) // rest countdown
            rest -= Time.deltaTime; 
        if (rest <= 0f) // when countdown ends the agent start moving again and the rest time is set again at 3 seconds
        {
            rest = 3f;
            agent.isStopped = false;
         }
        if (target == null && obj >= objectives.Count) // Here we check if the agent has still something to do, 
                                                        // and in case it has finished its work it will be destroyed.
                                                        //when there are no more agents in the scene the simulation will shut down
        {
            GameController.inst.totAgentsaux--;
            CameraControl.inst.Enable();
            agentCamera.gameObject.SetActive(false);
            GameController.inst.following = false;
            Destroy(gameObject);

        }

    }

    public void OnMouseDown()
    {
        if (!GameController.inst.following)
        {
            CameraControl.inst.Disable();
            agentCamera.gameObject.SetActive(true);
            GameController.inst.following = true;
        }
        else if (agentCamera.gameObject.activeSelf == true)
        {
            CameraControl.inst.Enable();
            agentCamera.gameObject.SetActive(false);
            GameController.inst.following = false;
        }
    }

    private void OnDestroy()
    {

    }
}
