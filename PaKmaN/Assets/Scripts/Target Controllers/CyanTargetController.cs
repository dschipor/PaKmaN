using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Cryptography;
using UnityEngine;

public class CyanTargetController : MonoBehaviour
{
    private Vector2 originalPosition;

    public GameObject pacman;
    public GameObject red;
    public GameObject pink;
    public GameObject cyan;

    private bool frozen;
    private bool vulnerable;

    private GameObject intersections; //object containing all interections on the map
    private Transform intersectionsTransforms; //transform containing the children transforms of all intersections

    public float refreshTime; //check every refresh time what the target should be
    private float timer = 0.0f;

    public bool treeControlled;
    public GameObject treeControllerManager;
    private DecisionTreeController tree;
    public string treeName;
    private List<string> outputStates;

    public float maxAmbushRadius;
    public float minAmbushRadius;

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
        frozen = cyan.GetComponent<GhostController>().frozen;
        vulnerable = cyan.GetComponent<GhostController>().vulnerable;

        intersections = cyan.GetComponent<GhostController>().intersections;
        intersectionsTransforms = intersections.transform;

        timer = 0.0f;

        // get tree
        tree = treeControllerManager.GetComponent<DecisionTreeController>();

        // define possible output states
        outputStates = new List<string>();
        outputStates.Add("go_random");
        outputStates.Add("chase");
        outputStates.Add("simple_ambush");
        outputStates.Add("complex_ambush");
    }

    // Update is called once per frame
    void Update()
    {
        frozen = cyan.GetComponent<GhostController>().frozen;
        if (frozen)
        {
            transform.position = originalPosition;
            GameObject newTarget = cyan;
            pink.GetComponent<GhostController>().setTarget(newTarget);
        }
        else
        {
            vulnerable = cyan.GetComponent<GhostController>().vulnerable;

            // Update target and reset timer
            timer += Time.deltaTime;
            if (timer > refreshTime)
            {
                timer -= refreshTime;

                // check if controlled by decision tree
                if (treeControlled)
                {
                    GameObject ghost = cyan;

                    // querry prerequisites
                    DecisionQuery query = new DecisionQuery();
                    List<AttributeValue> queryValues = new List<AttributeValue>();

                    string frozen = ghost.GetComponent<GhostController>().frozen.ToString();
                    string is_vulnerable = ghost.GetComponent<GhostController>().vulnerable.ToString();
                    string pacman_close = ghost.GetComponent<GhostController>().closeToPacman().ToString();

                    queryValues.Add(new AttributeValue("frozen", frozen, query));
                    queryValues.Add(new AttributeValue("vulnerable", is_vulnerable, query));
                    queryValues.Add(new AttributeValue("pacman_close", pacman_close, query));

                    // get prediction
                    string result = "chase"; bool decided = false;
                    foreach (string possible_outcome in outputStates)
                    {
                        query.Set(treeName, possible_outcome, queryValues.ToArray());
                        tree.Decide(query);

                        if (query.Yes)
                        {
                            decided = true;
                            result = possible_outcome;
                        }

                        UnityEngine.Debug.Log("CYAN TREE: outcome=" + possible_outcome + " decided=" + decided);
                    }

                    // set new target
                    if (decided)
                    {
                        if (string.Compare(result, "go_random") == 0)
                        {
                            int randomNumber = Random.Range(0, intersectionsTransforms.childCount);
                            GameObject newTarget = intersectionsTransforms.GetChild(randomNumber).gameObject;
                            red.GetComponent<GhostController>().setTarget(newTarget);
                            transform.position = newTarget.transform.position;
                        }
                        else if (string.Compare(result, "chase") == 0)
                        {
                            GameObject newTarget = pacman;
                            ghost.GetComponent<GhostController>().setTarget(newTarget);
                            transform.position = newTarget.transform.position;
                        }
                        if (string.Compare(result, "simple_ambush") == 0)
                        {
                            GameObject newTarget = pacman.GetComponent<PlayerController>().getTarget();
                            ghost.GetComponent<GhostController>().setTarget(newTarget);
                            transform.position = newTarget.transform.position;
                        }
                        else
                        {
                            GameObject pacmanTarget = pacman.GetComponent<PlayerController>().getTarget();
                            // Search intersections close to pinkTarget that pacman isn't facing
                            foreach (Transform t in intersectionsTransforms)
                            {
                                float distance = Vector3.Distance(pacmanTarget.transform.position, t.position);
                                float newDistanceToPacman = Vector3.Distance(t.position, pacman.transform.position);
                                float distancePacmanToTarget = Vector3.Distance(pacman.transform.position, t.position);

                                if (distance > 2.0f && distance < 4.5f)
                                {
                                    Vector2 pacmanOrientation = t.position - pacman.transform.position;
                                    Vector2 ghostTargetOrientation = t.position - ghost.transform.position;
                                    Vector2 pacmanDirection = pacman.GetComponent<PlayerController>().getDirection();
                                    // Check if pacman and ghost face
                                    if (Vector2.Dot(pacmanOrientation, pacmanDirection) > 0 && Vector2.Dot(ghostTargetOrientation, pacmanDirection) > 0)
                                    {
                                        GameObject newTarget = t.gameObject;
                                        ghost.GetComponent<GhostController>().setTarget(newTarget);
                                        transform.position = newTarget.transform.position;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        /*GameObject pacmanTarget = pacman.GetComponent<PlayerController>().getTarget();
                        // Search intersections close to pinkTarget that pacman isn't facing
                        foreach (Transform t in intersectionsTransforms)
                        {
                            float distance = Vector3.Distance(pacmanTarget.transform.position, t.position);
                            float newDistanceToPacman = Vector3.Distance(t.position, pacman.transform.position);
                            float distancePacmanToTarget = Vector3.Distance(pacman.transform.position, t.position);

                            if (distance > 2.0f && distance < 4.5f)
                            {
                                Vector2 pacmanOrientation = t.position - pacman.transform.position;
                                Vector2 ghostTargetOrientation = t.position - ghost.transform.position;
                                Vector2 pacmanDirection = pacman.GetComponent<PlayerController>().getDirection();
                                // Check if pacman and ghost face
                                if (Vector2.Dot(pacmanOrientation, pacmanDirection) > 0 && Vector2.Dot(ghostTargetOrientation, pacmanDirection) > 0)
                                {
                                    GameObject newTarget = t.gameObject;
                                    ghost.GetComponent<GhostController>().setTarget(newTarget);
                                    transform.position = newTarget.transform.position;
                                }
                            }
                        }*/
                    }
                }
                else
                {
                    if (vulnerable)
                    {
                        bool closeToPacman = cyan.GetComponent<GhostController>().closeToPacman();
                        float distanceToCurrent = Vector3.Distance(cyan.transform.position, transform.position);

                        // Aimlessly running away and change target if close to pacman or previous destination
                        if (closeToPacman || distanceToCurrent <= 1.0f)
                        {

                            int randomNumber = Random.Range(0, intersectionsTransforms.childCount);

                            GameObject newTarget = intersectionsTransforms.GetChild(randomNumber).gameObject;
                            cyan.GetComponent<GhostController>().setTarget(newTarget);

                            transform.position = newTarget.transform.position;
                        }
                    }
                    else
                    {
                        bool closeToPacman = cyan.GetComponent<GhostController>().closeToPacman();
                        float distanceToPacman = Vector3.Distance(cyan.transform.position, pacman.transform.position);
                        float distanceToTarget = Vector3.Distance(cyan.transform.position, transform.position);
                        bool seesPacman = !Physics.Linecast(cyan.transform.position, pacman.transform.position);


                        // Follow pacman if very close to him
                        if (closeToPacman)
                        {
                            GameObject newTarget = pacman;
                            cyan.GetComponent<GhostController>().setTarget(newTarget);

                            transform.position = newTarget.transform.position;
                        }
                        // Ambush together with pink
                        else
                        {
                            GameObject pinkTarget = pink.GetComponent<GhostController>().getTarget();

                            // Search intersections close to pinkTarget that pacman isn't facing

                            foreach (Transform t in intersectionsTransforms)
                            {
                                float distance = Vector3.Distance(pinkTarget.transform.position, t.position);
                                float newDistanceToPacman = Vector3.Distance(t.position, pacman.transform.position);
                                float distancePacmanToTarget = Vector3.Distance(pacman.transform.position, t.position);

                                if (distance > minAmbushRadius && distance < maxAmbushRadius && newDistanceToPacman <= distanceToPacman
                                    && distance <= distancePacmanToTarget * 1.4)
                                {

                                    Vector2 pacmanOrientation = t.position - pacman.transform.position;
                                    Vector2 cyanTargetOrientation = t.position - cyan.transform.position;


                                    Vector2 pacmanDirection = pacman.GetComponent<PlayerController>().getDirection();

                                    // Check if pacman and ghost face
                                    if (Vector2.Dot(pacmanOrientation, pacmanDirection) > 0 && Vector2.Dot(cyanTargetOrientation, pacmanDirection) > 0)
                                    {
                                        GameObject newTarget = t.gameObject;
                                        cyan.GetComponent<GhostController>().setTarget(newTarget);

                                        transform.position = newTarget.transform.position;
                                    }
                                    //transform.position = t.position;
                                }
                            }
                        }

                    }
                }
            }
           
        }

    }
}

