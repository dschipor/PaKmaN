using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class DecisionController : MonoBehaviour
{
    public DecisionTreeController treeController;
    
    private List<AttributeValue> values; 
    DecisionQuery query;

    private float refreshTime = 2f; //check every refresh time what the target should be
    private float timer = 0.0f;

    private bool decided;
    private StringBuilder builder;
    private List<string> states;
    


    // Start is called before the first frame update
    void Start()
    {
        
        states = new List<string>();

        


        states.Add("Hungry");
        states.Add("Thirsty");
        states.Add("Lavatory");
        states.Add("Tired");
        states.Add("Comunicate");
        

        query = new DecisionQuery();
        

       

        decided = false;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
            

        if(timer >= refreshTime)
        {
            timer -= refreshTime;
            decided = false;
            

            values = new List<AttributeValue>();
            
            string s1 = getState();
            string s2 = getState();
            string s3 = getState();
            string s4 = getState();
            string s5 = getState();

            values.Add(new AttributeValue("Hungry", s1));
            values.Add(new AttributeValue("Thirsty", s2));
            values.Add(new AttributeValue("Lavatory", s3));
            values.Add(new AttributeValue("Tired", s4));
            values.Add(new AttributeValue("Communicate", s5));

            string result = "NONE";


            foreach (string s in states)
            {
                query.Set("InstinctSelector", s, values.ToArray());
                treeController.Decide(query);

                if (query.Yes)
                {
                    if (decided)
                    {
                        UnityEngine.Debug.Log("PRINT: WTF, DECIDED TWICE!");
                    }
                    else
                    {
                        decided = true;
                        result = s;
                    }  
                }
                //UnityEngine.Debug.Log("PRINT PARTIAL" + decided + " " + treeController.PrintDecision(query) + "!");
            }
            UnityEngine.Debug.Log("PRINT: DECIDED=" + decided + " RESULT=" + result
                + " " + s1 + "/" + s2 + "/" + s3 + "/" + s4 + "/" + s5 + "!");
        }
    }

    string getState()
    {
        string no = "No";
        string near = "Near";
        string far = "Far";

        int state = Random.Range(1, 100);

        if (state % 3 == 0) return no;
        else if (state % 3 == 1) return near;
        else if (state % 3 == 2) return far;
        else return no;
    }
}
