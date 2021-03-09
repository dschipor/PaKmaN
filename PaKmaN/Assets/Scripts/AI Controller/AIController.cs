using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeController : MonoBehaviour
{
    
    public GameObject ghost;
    public GameObject decisionMaker;
    public GameObject red, pink, cyan, orange, pacman;
    public GameObject intersections;
    public float refreshTime = 0.25f;

    private DecisionTreeController treeController;
    private string treeName;
    private float timer;
    
    private bool isRed = false, isPink = false, isCyan = false, isOrange = false;
    private string defaultTarget;
    private List<string> outputStates;
    

    // Start is called before the first frame update
    void Start()
    {
        treeController = decisionMaker.GetComponent<DecisionTreeController>();
        //check what ghost is using the script
        if(String.Compare(ghost.name.ToString(), "red") == 0)
        {
            isRed = true;
            //treeName = treeController.redTree;
        }
        else if(String.Compare(ghost.name.ToString(), "pink") == 0)
        {
            isPink = true;
            //treeName = treeController.pinkTree;
        }
        else if (String.Compare(ghost.name.ToString(), "cyan") == 0)
        {
            isCyan = true;
            //treeName = treeController.cyanTree;
        }
        else if (String.Compare(ghost.name.ToString(), "orange") == 0)
        {
            isOrange = true;
            //treeName = treeController.orangeTree;
        }
        else
        {
            UnityEngine.Debug.Log("ERROR: ghost is unrecognized");
        }

        defaultTarget = ghost.name.ToString();

        // define possible output states
        outputStates = new List<string>(); 
        outputStates.Add(defaultTarget); // for staying inplace
        outputStates.Add(pacman.name.ToString());
        foreach (Transform t in intersections.transform)
        {
            string intersection = t.gameObject.name.ToString();
            outputStates.Add(intersection);
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if(timer >= refreshTime)
        {
            timer -= refreshTime;

            // Init the strings
            string frozen = "no", vulnerable = "no";
            string pacman_close = "no", pacman_nearest = "i0", pacman_target = "i0";
            string red_nearest = "i0", red_target = "red";
            string pink_nearest = "i0", pink_target = "pink";
            string cyan_nearest = "i0", cyan_target = "cyan";
            string orange_nearest = "i0";

            

            // query prerequisites            
            DecisionQuery query = new DecisionQuery();
            List<AttributeValue> querryValues = new List<AttributeValue>();

            if (isRed)
            {
                // populate states
                frozen = red.GetComponent<GhostController>().frozen.ToString();
                vulnerable = red.GetComponent<GhostController>().vulnerable.ToString();
                pacman_close = red.GetComponent<GhostController>().closeToPacman().ToString();
                pacman_nearest = pacman.GetComponent<PlayerController>().nearestIntersection().name.ToString();
                pacman_target = pacman.GetComponent<PlayerController>().getTarget().name.ToString();
                red_nearest = red.GetComponent<GhostController>().nearestIntersection().name.ToString();

                // populate querry values
                querryValues.Add(new AttributeValue("frozen", frozen, query));
                querryValues.Add(new AttributeValue("vulnerable", vulnerable, query));
                querryValues.Add(new AttributeValue("pacman_close", pacman_close, query));
                querryValues.Add(new AttributeValue("pacman_nearest", pacman_nearest, query));
                querryValues.Add(new AttributeValue("pacman_target", pacman_target, query));
                querryValues.Add(new AttributeValue("red_nearest", red_nearest, query));
            }
            else if (isPink)
            {
                // populate states
                frozen = pink.GetComponent<GhostController>().frozen.ToString();
                vulnerable = pink.GetComponent<GhostController>().vulnerable.ToString();
                pacman_close = pink.GetComponent<GhostController>().closeToPacman().ToString();
                pacman_nearest = pacman.GetComponent<PlayerController>().nearestIntersection().name.ToString();
                pacman_target = pacman.GetComponent<PlayerController>().getTarget().name.ToString();
                red_nearest = red.GetComponent<GhostController>().nearestIntersection().name.ToString();
                red_target = red.GetComponent<GhostController>().getTarget().name.ToString();
                pink_nearest = pink.GetComponent<GhostController>().nearestIntersection().name.ToString();

                // populate querry values
                querryValues.Add(new AttributeValue("frozen", frozen, query));
                querryValues.Add(new AttributeValue("vulnerable", vulnerable, query));
                querryValues.Add(new AttributeValue("pacman_close", pacman_close, query));
                querryValues.Add(new AttributeValue("pacman_nearest", pacman_nearest, query));
                querryValues.Add(new AttributeValue("pacman_target", pacman_target, query));
                querryValues.Add(new AttributeValue("red_nearest", red_nearest, query));
                querryValues.Add(new AttributeValue("red_target", red_target, query));
                querryValues.Add(new AttributeValue("pink_nearest", pink_nearest, query));
            }
            else if (isCyan)
            {
                // populate states
                frozen = cyan.GetComponent<GhostController>().frozen.ToString();
                vulnerable = cyan.GetComponent<GhostController>().vulnerable.ToString();
                pacman_close = cyan.GetComponent<GhostController>().closeToPacman().ToString();
                pacman_nearest = pacman.GetComponent<PlayerController>().nearestIntersection().name.ToString();
                pacman_target = pacman.GetComponent<PlayerController>().getTarget().name.ToString();
                //red_nearest = red.GetComponent<GhostController>().nearestIntersection().name.ToString();
                //red_target = red.GetComponent<GhostController>().getTarget().name.ToString();
                //pink_nearest = pink.GetComponent<GhostController>().nearestIntersection().name.ToString();
                pink_target = pink.GetComponent<GhostController>().getTarget().name.ToString();
                cyan_nearest = cyan.GetComponent<GhostController>().nearestIntersection().name.ToString();

                // populate querry values
                querryValues.Add(new AttributeValue("frozen", frozen, query));
                querryValues.Add(new AttributeValue("vulnerable", vulnerable, query));
                querryValues.Add(new AttributeValue("pacman_close", pacman_close, query));
                querryValues.Add(new AttributeValue("pacman_nearest", pacman_nearest, query));
                querryValues.Add(new AttributeValue("pacman_target", pacman_target, query));
                //querryValues.Add(new AttributeValue("red_nearest", red_nearest, query));
                //querryValues.Add(new AttributeValue("red_target", red_target, query));
                //querryValues.Add(new AttributeValue("pink_nearest", pink_nearest, query));
                querryValues.Add(new AttributeValue("pink_target", pink_target, query));
                querryValues.Add(new AttributeValue("cyan_nearest", cyan_nearest, query));
            }
            else if (isOrange)
            {
                // populate states
                frozen = orange.GetComponent<GhostController>().frozen.ToString();
                vulnerable = orange.GetComponent<GhostController>().vulnerable.ToString();
                pacman_close = orange.GetComponent<GhostController>().closeToPacman().ToString();
                pacman_nearest = pacman.GetComponent<PlayerController>().nearestIntersection().name.ToString();
                pacman_target = pacman.GetComponent<PlayerController>().getTarget().name.ToString();
                //red_nearest = red.GetComponent<GhostController>().nearestIntersection().name.ToString();
                //red_target = red.GetComponent<GhostController>().getTarget().name.ToString();
                //pink_nearest = pink.GetComponent<GhostController>().nearestIntersection().name.ToString();
                pink_target = pink.GetComponent<GhostController>().getTarget().name.ToString();
                //cyan_nearest = cyan.GetComponent<GhostController>().nearestIntersection().name.ToString();
                cyan_target = cyan.GetComponent<GhostController>().getTarget().name.ToString();
                orange_nearest = orange.GetComponent<GhostController>().nearestIntersection().name.ToString();

                // populate querry values
                querryValues.Add(new AttributeValue("frozen", frozen, query));
                querryValues.Add(new AttributeValue("vulnerable", vulnerable, query));
                querryValues.Add(new AttributeValue("pacman_close", pacman_close, query));
                querryValues.Add(new AttributeValue("pacman_nearest", pacman_nearest, query));
                querryValues.Add(new AttributeValue("pacman_target", pacman_target, query));
                //querryValues.Add(new AttributeValue("red_nearest", red_nearest, query));
                //querryValues.Add(new AttributeValue("red_target", red_target, query));
                //querryValues.Add(new AttributeValue("pink_nearest", pink_nearest, query));
                querryValues.Add(new AttributeValue("pink_target", pink_target, query));
                //querryValues.Add(new AttributeValue("cyan_nearest", cyan_nearest, query));
                querryValues.Add(new AttributeValue("cyan_target", cyan_target, query));
                querryValues.Add(new AttributeValue("orange_nearest", orange_nearest, query));
            }
            else
            {
                UnityEngine.Debug.Log("ERROR: ghost is unrecognized");
            }
            
            // get prediction result from tree
            string result = defaultTarget;
            bool decided = false;

            foreach(string state in outputStates)
            {
                query.Set(treeName, state, querryValues.ToArray());
                treeController.Decide(query);

                //check if decided update result
                if(query.Yes)
                {
                    UnityEngine.Debug.Log("TREE CONTROLLER: result found!");
                    result = state;
                    decided = true;
                }

                //UnityEngine.Debug.Log("TREE CONTROLLER: " + treeName + " " + treeController.PrintDecision(query) + "!");
            }

            // get target from prediction result
            GameObject newTarget = GameObject.Find(result);
            if(newTarget != null)
            {
                transform.position = newTarget.transform.position;

                string output = "";
                foreach(AttributeValue attribute in querryValues)
                {
                    output = output + attribute.name + "/" + attribute.value + "  ";
                }
                //UnityEngine.Debug.Log("TREE CONTROLLER OUTPUT: " + treeName +" DECIDED="+ decided + " QUERY=(" + output +") RESULT=" + result + "!");
            }
        }
    }
}
