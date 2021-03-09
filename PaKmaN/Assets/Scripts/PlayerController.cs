using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    public int speed = 3;//how fast PacMan can travel
    public int score = 0;
    public int livesLeft = 2;//how many extras lives pacman has left

    public Text scoreText;
    public Image life1;
    public Image life2;

    private Vector2 direction;
    private bool alive = true;

    // limits of movement
    public int minX = -8, maxX = 8, minY = -4, maxY = 14;

    public GameObject intersections; // Object containing all interections on the map
    private Transform intersectionsTransforms; // Transform containing the children transforms of all intersections
    private GameObject target; // Intersection where player is headed
    public float closeDistance; // Distance that is considered close
    public float refreshTime; // Time until target updates
    private float timer = 0.0f;

    Rigidbody2D rb2d;
    Animator anim;

    // Use this for initialization
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        intersectionsTransforms = intersections.transform;
        target = transform.gameObject;
        timer = 0.0f;

        scoreText.text = "" + 0;
    }

    void Update()
    {
        if (alive)
        {
            anim.SetFloat("currentSpeed", rb2d.velocity.magnitude);
            if (Input.GetAxis("Horizontal") < 0)
            {
                direction = Vector2.left;
            }
            if (Input.GetAxis("Horizontal") > 0)
            {
                direction = Vector2.right;
            }
            if (Input.GetAxis("Vertical") < 0)
            {
                direction = Vector2.down;
            }
            if (Input.GetAxis("Vertical") > 0)
            {
                direction = Vector2.up;
            }
            rb2d.velocity = direction * speed;
            transform.up = direction;
            if (rb2d.velocity.x == 0)
            {
                transform.position = new Vector2(Mathf.Round(transform.position.x), transform.position.y);
            }
            if (rb2d.velocity.y == 0)
            {
                transform.position = new Vector2(transform.position.x, Mathf.Round(transform.position.y));
            }

            //update target after refresh time
            timer += Time.deltaTime;
            if (timer > refreshTime)
            {
                timer -= refreshTime;
                float minDistance = closeDistance;
                foreach (Transform t in intersectionsTransforms)
                {
                    float distance = Vector3.Distance(t.position, transform.position);

                    // Check if intersection is nearby
                    if (distance <= minDistance)
                    { 
                        Vector2 orientation = t.transform.position - transform.position;
                        // Check if faced towards intersection
                        if (Vector2.Dot(orientation, direction) > 0)
                        { 
                            // Check if path between pacman and intersection is free
                            if (!Physics.Linecast(transform.position, target.transform.position))
                            {
                                target = t.gameObject;
                                minDistance = distance;
                            }
                        }
                    }
                }
            }

        }
        else
        {
            target = transform.gameObject;
        }
    }

    public GameObject nearestIntersection()
    {
        GameObject output = transform.gameObject;
        float minDistance = closeDistance;

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

    public void addPoints(int points)
    {
        score += points;
        scoreText.text = "" + score;
    }

    public void setAlive(bool isAlive)
    {
        alive = isAlive;
        anim.SetBool("alive", alive);
        rb2d.velocity = Vector2.zero;
    }

    public void setLivesLeft(int lives)
    {
        livesLeft = lives;
        life1.enabled = livesLeft >= 1;
        life2.enabled = livesLeft >= 2;
    }

    public Vector2 getDirection()
    {
        return direction;
    }

    public GameObject getTarget()
    {
        return target;
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
        if (minX < 0)
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

        else if (y > 5)
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
