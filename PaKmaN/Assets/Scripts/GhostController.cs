using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class GhostController : MonoBehaviour
{
    public Color vulnerableColor = Color.blue;
    public int points = 400;

    private Rigidbody2D rb2d;
    private CircleCollider2D cc2d;

    // limits of movement
    public int minX = -8, maxX = 8, minY = -4, maxY = 14;

    public Vector2 originalPosition;
    private Color originalColor;
    private SpriteRenderer sr;
    public AudioSource ghostEatenSound;
    public GameObject pacman;
    public bool frozen = false;
    public bool vulnerable = false;
    private bool eaten = false;

    public GameObject intersections; //object containing all interections on the map
    private Transform intersectionsTransforms; //transform containing the children transforms of all intersections
    public GameObject target; //where is ghost headed

    public float closeDistance;// distance that is considered close

    // Use this for initialization
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        cc2d = GetComponent<CircleCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        originalPosition = transform.position;
        originalColor = sr.color;
        originalColor = sr.color;

        intersectionsTransforms = intersections.transform;
        target = transform.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (!vulnerable)
        {
            foreach (Transform t in GetComponentsInChildren<Transform>())
            {
                if (t != transform)
                {
                    if (pacman) t.up = (pacman.transform.position - transform.position).normalized;
                    else t.up = transform.position;
                }
            }
        }
        else
        {
            foreach (Transform t in GetComponentsInChildren<Transform>())
            {
                if (t != transform)
                {
                    if (pacman) t.up = (pacman.transform.position - transform.position).normalized * -1;
                    else t.up = transform.position;
                }
            }
        }

    }

    public void setVulnerable(bool isVulnerable)
    {
        vulnerable = isVulnerable;
        if (vulnerable)
        {
            sr.color = vulnerableColor;
        }
        else
        {
            sr.color = originalColor;
        }
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            if (vulnerable)
            {
                coll.gameObject.GetComponent<PlayerController>().addPoints(points);
                setEaten(true);
                reset();
                freeze(true);
            }
            else
            {
                GameManager.pacmanKilled();
            }
        }
    }

    public void reset()
    {
        transform.position = originalPosition;
        freeze(false);
    }

    public void freeze(bool freeze)
    {
        frozen = freeze;  
    }

    public void setEaten(bool isEaten)
    {
        eaten = isEaten;
        if (eaten)
        {
            sr.color = new Color(0, 0, 0, 0);
            ghostEatenSound.Play();
            setVulnerable(false);
        }
        else
        {
            sr.color = originalColor;
            vulnerable = false;
        }
    }

    public void blink()
    {
        if (sr.color == originalColor)
        {
            sr.color = vulnerableColor;
        }
        else if (sr.color == vulnerableColor)
        {
            sr.color = originalColor;
        }
    }

    public GameObject nearestIntersection()
    {
        GameObject output = transform.gameObject;
        float minDistance = 100.0f;

        foreach (Transform t in intersectionsTransforms)
        {
            float distance = Vector3.Distance(t.position, transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                output = t.gameObject;
            }
        }
        return output;
    }

    public bool closeToPacman()
    {
        float distanceToPacman = Vector3.Distance(transform.position, pacman.transform.position);
        
        if(distanceToPacman < closeDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void setTarget(GameObject newTarget)
    {
        target = newTarget;
    }

    public GameObject getTarget()
    {
        return target;
    }

    public GameObject goUp()
    {
        GameObject res = pacman;

        float minDist = 100.0f; Transform partialRes = pacman.transform;
        foreach(Transform t in intersectionsTransforms)
        {
            float distY = t.position.y - transform.position.y;
            if(distY > 0 && distY < minDist)
            {
                partialRes = t;
                minDist = distY;
            }
        }

        float distPacman = pacman.transform.position.y - transform.position.y;
        if(distPacman > 0 && distPacman < minDist)
        {
            res = pacman;
        }
        else
        {
            res = partialRes.gameObject;
        }
        return res;
    }

    public GameObject goDown()
    {
        GameObject res = pacman;

        float minDist = 100.0f; Transform partialRes = pacman.transform;
        foreach (Transform t in intersectionsTransforms)
        {
            float distY = transform.position.y - t.position.y;
            if (distY > 0 && distY < minDist)
            {
                partialRes = t;
                minDist = distY;
            }
        }

        float distPacman = transform.position.y - pacman.transform.position.y;
        if (distPacman > 0 && distPacman < minDist)
        {
            res = pacman;
        }
        else
        {
            res = partialRes.gameObject;
        }
        return res;
    }

    public GameObject goRight()
    {
        GameObject res = pacman;

        float minDist = 100.0f; Transform partialRes = pacman.transform;
        foreach (Transform t in intersectionsTransforms)
        {
            float distY = t.position.x - transform.position.x;
            if (distY > 0 && distY < minDist)
            {
                partialRes = t;
                minDist = distY;
            }
        }

        float distPacman = pacman.transform.position.x - transform.position.x;
        if (distPacman > 0 && distPacman < minDist)
        {
            res = pacman;
        }
        else
        {
            res = partialRes.gameObject;
        }
        return res;
    }

    public string getDirection()
    {
        float distX = target.transform.position.x - transform.position.x;
        float distY = target.transform.position.y - transform.position.y;


        if (Mathf.Abs(distX) < Mathf.Abs(distY))
        {
            if (distY > 0)
            {
                return "1";
            }
            else
            {
                return "2";
            }
        }
        else
        {
            if (distX > 0)
            {
                return "3";
            }
            else
            {
                return "4";
            }
        }
    }



    public GameObject goLeft()
    {
        GameObject res = pacman;

        float minDist = 100.0f; Transform partialRes = pacman.transform;
        foreach (Transform t in intersectionsTransforms)
        {
            float distY = transform.position.x - t.position.x;
            if (distY > 0 && distY < minDist)
            {
                partialRes = t;
                minDist = distY;
            }
        }

        float distPacman = transform.position.x - pacman.transform.position.x;
        if (distPacman > 0 && distPacman < minDist)
        {
            res = pacman;
        }
        else
        {
            res = partialRes.gameObject;
        }
        return res;
    }

    public int getX()
    {
        int res = Mathf.CeilToInt(transform.position.x);
        res = Mathf.Max(res, minX);
        res = Mathf.Min(res, maxX);

        return res;
    }
    public int getTargetX()
    {
        int res = Mathf.CeilToInt(target.transform.position.x);
        res = Mathf.Max(res, minX);
        res = Mathf.Min(res, maxX);

        return res;
    }
    public int getY()
    {
        int res = Mathf.CeilToInt(transform.position.y);
        res = Mathf.Max(res, minY);
        res = Mathf.Min(res, maxY);

        return res;
    }
    public int getTargetY()
    {
        int res = Mathf.CeilToInt(target.transform.position.y);
        res = Mathf.Max(res, minY);
        res = Mathf.Min(res, maxY);

        return res;
    }

    public float xToPositive(int x)
    {
        if(minX < 0)
        {
            return x - minX;
        }
        else
        {
            return x;
        }
    }

    public float yToPositive(int y)
    {
        if (minY < 0)
        {
            return y - minY;
        }
        else
        {
            return y;
        }
    }

    public float xAdjustBack(int x)
    {
        if (minX < 0)
        {
            return x + minX;
        }
        else
        {
            return x;
        }
    }

    public float yAdjustBack(int y)
    {
        if (minY < 0)
        {
            return y + minY;
        }
        else
        {
            return y;
        }
    }

    public float getPositiveX()
    {
        return xToPositive(getX());
    }

    public float getPositiveTargetX()
    {
        return xToPositive(getTargetX());
    }

    public float getPositiveY()
    {
        return yToPositive(getY());
    }

    public float getPositiveTargetY()
    {
        return yToPositive(getTargetY());
    }

    public int getLine()
    {
        float y = transform.position.y;
        
        if (y > 11)
        {
            return 1;
        }
        
        else if(y > 5)
        {
            return 2;
        }
        else if (y > -1)
        {
            return 3;
        }
        else
        {
            return 4;
        }
    }
    public int getColumn()
    {
        float x = transform.position.x;

        if (x < -4)
        {
            return 1;
        }
        else if (x < 0)
        {
            return 2;
        }
        else if (x < 5)
        {
            return 3;
        }
        else
        {
            return 4;
        }
    }



}
