using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System.IO;
using System;

public class DataCollector : MonoBehaviour
{
    public GameObject pacman;
    public GameObject red;
    public GameObject pink;
    public GameObject cyan;
    public GameObject orange;

    public bool recordRed, recordPink, recordCyan, recordOrange;

    public string sqlLiteDbName = "DecisionTreeData.db";
    public string dataFolder = "DecisionTrees";

    public float refreshTime;
    private float timer;

    private IDbConnection dbcon;
    private IDbCommand dbcm;
    private IDataReader dbr; 

    // Start is called before the first frame update
    void Start()
    {
        timer = 0.0f;

        string path = GetConnectionString();
        UnityEngine.Debug.Log("DATABASE: " + path + "!");
        dbcon = new SqliteConnection(path);
        dbcon.Open();
        dbcm = dbcon.CreateCommand();   
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > refreshTime)
        {
            timer -= refreshTime;

            bool red_vulnerable = red.GetComponent<GhostController>().vulnerable;
            bool pink_vulnerable = pink.GetComponent<GhostController>().vulnerable;
            bool cyan_vulnerable = cyan.GetComponent<GhostController>().vulnerable;
            bool orange_vulnerable = orange.GetComponent<GhostController>().vulnerable;

            //collect 
            /*if (recordRed && !red_vulnerable)
            {
                redRecord(pacman, red, dbcm, dbr);
            }
            if(recordPink && !pink_vulnerable)
            {
                pinkRecord(pacman, red, pink, dbcm, dbr);
            }
            if (recordCyan && !cyan_vulnerable)
            {
                cyanRecord(pacman, red, pink, cyan, dbcm, dbr);
            }
            if (recordOrange && !orange_vulnerable)
            {
                orangeRecord(pacman, red, pink, cyan, orange, dbcm, dbr);
            }
            */
            //collect special
            if (recordRed)
            {
                specialRecord(pacman, red, "red_special", dbcm, dbr);
            }
            if (recordPink)
            {
                specialRecord(pacman, pink, "pink_special", dbcm, dbr);
            }
            if (recordCyan)
            {
                specialRecord(pacman, cyan, "cyan_special", dbcm, dbr);
            }
            if (recordOrange)
            {
                specialRecord(pacman, orange, "orange_special", dbcm, dbr);
            }

        }

    }
    public void redRecord(GameObject pacman, GameObject red, IDbCommand manager, IDataReader reader)
    {
        
        // red 1
        string vulnerable = red.GetComponent<GhostController>().vulnerable.ToString();
        string pacman_close = red.GetComponent<GhostController>().closeToPacman().ToString();
        string pacman_nearest = getState(pacman.GetComponent<PlayerController>().nearestIntersection().name.ToString());
        string pacman_target = getState(pacman.GetComponent<PlayerController>().getTarget().name.ToString());
        string red_nearest = getState(red.GetComponent<GhostController>().nearestIntersection().name.ToString());
        string red_target = getState(red.GetComponent<GhostController>().getTarget().name.ToString());

        string red_dbquery = "INSERT INTO red_1 VALUES( '" + vulnerable + "' , '" + pacman_close +
            "' , '" + pacman_nearest + "' , '" + pacman_target + "' , '" + red_nearest + "' , '" + red_target + "' )";

        manager.CommandText = red_dbquery;
        reader = manager.ExecuteReader();
        reader.Close();

        //red 2
        vulnerable = red.GetComponent<GhostController>().vulnerable.ToString();
        string pacman_x = pacman.transform.position.x.ToString();
        string pacman_y = pacman.transform.position.y.ToString();
        string red_x = red.transform.position.x.ToString();
        string red_y = red.transform.position.y.ToString();
        red_target = getState(red.GetComponent<GhostController>().getTarget().name.ToString());

        red_dbquery = "INSERT INTO red_2 VALUES( '" + vulnerable + "' , '" + pacman_x +
            "' , '" + pacman_y + "' , '" + red_x + "' , '" + red_y + "' , '" + red_target + "' )";
        manager.CommandText = red_dbquery;
        reader = manager.ExecuteReader();
        reader.Close();

        //red 3
        red_dbquery = "INSERT INTO red_3 VALUES( '" + vulnerable + "' , '" + pacman_x +
            "' , '" + pacman_y + "' , '" + red_x + "' , '" + red_y + "' , '" + red_target + "' )";
        
        manager.CommandText = red_dbquery;
        reader = manager.ExecuteReader();
        reader.Close();


        //red 4
        red_dbquery = "INSERT INTO red_4 VALUES( '" + vulnerable + "' , '" + pacman_x +
            "' , '" + pacman_y + "' , '" + red_x + "' , '" + red_y + "' , '" + red_target + "' )";

        manager.CommandText = red_dbquery;
        reader = manager.ExecuteReader();
        reader.Close();

        //red 5
        string direction = red.GetComponent<GhostController>().getDirection().ToString();

        red_dbquery = "INSERT INTO red_5 VALUES( '" + vulnerable + "' , '" + pacman_x +
            "' , '" + pacman_y + "' , '" + red_x + "' , '" + red_y + "' , '" + direction + "' )";

        manager.CommandText = red_dbquery;
        reader = manager.ExecuteReader();
        reader.Close();

        //red 6
        vulnerable = red.GetComponent<GhostController>().vulnerable.ToString();
        string red_row = red.GetComponent<GhostController>().getLine().ToString();
        string red_column = red.GetComponent<GhostController>().getColumn().ToString();
        direction = red.GetComponent<GhostController>().getDirection().ToString();

        red_dbquery = "INSERT INTO red_6 VALUES( '" + vulnerable + "' , '" + pacman_x +
            "' , '" + pacman_y + "' , '" + red_row + "' , '" + red_column + "' , '" + direction + "' )";

        manager.CommandText = red_dbquery;
        reader = manager.ExecuteReader();
        reader.Close();
    }

    public void specialRecord(GameObject pacman, GameObject ghost, string table_name, IDbCommand manager, IDataReader reader)
    {
        string frozen = ghost.GetComponent<GhostController>().frozen.ToString();
        string vulnerable = ghost.GetComponent<GhostController>().frozen.ToString();
        bool vul = ghost.GetComponent<GhostController>().vulnerable;
        string pacman_close = ghost.GetComponent<GhostController>().closeToPacman().ToString();
        string action = "1";
        if (!vul)
        {
            if (String.Compare(ghost.name, "red") == 0)
                action = "2";
            else if (String.Compare(ghost.name, "pink") == 0)
                action = "3";
            else action = "4";
        }
        string ghost_dbquery = "INSERT INTO " + table_name + " VALUES( '" + frozen + "' , '" + vulnerable +
           "' , '" + pacman_close + "' , '" + action + "' )";

        manager.CommandText = ghost_dbquery;
        reader = manager.ExecuteReader();
        reader.Close();
    }


    public void pinkRecord(GameObject pacman, GameObject red, GameObject pink, IDbCommand manager, IDataReader reader)
    {

        // pink 1
        string vulnerable = pink.GetComponent<GhostController>().vulnerable.ToString();
        string pacman_close = pink.GetComponent<GhostController>().closeToPacman().ToString();
        string pacman_nearest = getState(pacman.GetComponent<PlayerController>().nearestIntersection().name.ToString());
        string pacman_target = getState(pacman.GetComponent<PlayerController>().getTarget().name.ToString());
        string red_nearest = getState(red.GetComponent<GhostController>().nearestIntersection().name.ToString());
        string red_target = getState(red.GetComponent<GhostController>().getTarget().name.ToString());
        string pink_nearest = getState(pink.GetComponent<GhostController>().nearestIntersection().name.ToString());
        string pink_target = getState(pink.GetComponent<GhostController>().getTarget().name.ToString());

        string pink_dbquery = "INSERT INTO pink_1 VALUES( '" + vulnerable + "' , '" + pacman_close +
            "' , '" + pacman_nearest + "' , '" + pacman_target + "' , '" + red_nearest + "' , '" + red_target +
            "' , '" + pink_nearest + "' , '" + pink_target + "' )";

        manager.CommandText = pink_dbquery;
        reader = manager.ExecuteReader();
        reader.Close();

        // pink 2
        string pacman_x = pacman.transform.position.x.ToString();
        string pacman_y = pacman.transform.position.y.ToString();
        string red_x = red.transform.position.x.ToString();
        string red_y = red.transform.position.y.ToString();
        string red_target_x = red.GetComponent<GhostController>().getTarget().transform.position.x.ToString();
        string red_target_y = red.GetComponent<GhostController>().getTarget().transform.position.y.ToString();
        string pink_x = pink.transform.position.x.ToString();
        string pink_y = pink.transform.position.y.ToString();

        pink_dbquery = "INSERT INTO pink_2 VALUES( '" + vulnerable + "' , '" + pacman_x +
            "' , '" + pacman_y + "' , '" + red_x + "' , '" + red_y + "' , '" + red_target_x + "' , '" + red_target_y + "' , '" + pink_x + "' , '" + pink_y + "' , '" + pink_target + "' )";
        manager.CommandText = pink_dbquery;
        reader = manager.ExecuteReader();
        reader.Close();

        //pink 3
        pink_dbquery = "INSERT INTO pink_3 VALUES( '" + vulnerable + "' , '" + pacman_x +
           "' , '" + pacman_y + "' , '" + red_x + "' , '" + red_y + "' , '" + pink_x + "' , '" + pink_y + "' , '" + pink_target + "' )";

        manager.CommandText = pink_dbquery;
        reader = manager.ExecuteReader();
        reader.Close();


        //pink 4
        pink_dbquery = "INSERT INTO pink_4 VALUES( '" + vulnerable + "' , '" + pacman_x +
            "' , '" + pacman_y + "' , '" + pink_x + "' , '" + pink_y + "' , '" + pink_target + "' )";

        manager.CommandText = pink_dbquery;
        reader = manager.ExecuteReader();
        reader.Close();

        //pink 5
        string direction = pink.GetComponent<GhostController>().getDirection().ToString();

        pink_dbquery = "INSERT INTO pink_5 VALUES( '" + vulnerable + "' , '" + pacman_x +
            "' , '" + pacman_y + "' , '" + pink_x + "' , '" + pink_y + "' , '" + direction + "' )";

        manager.CommandText = pink_dbquery;
        reader = manager.ExecuteReader();
        reader.Close();

        //pink 6
        string pink_row = pink.GetComponent<GhostController>().getLine().ToString();
        string pink_column = pink.GetComponent<GhostController>().getColumn().ToString();
        pink_dbquery = "INSERT INTO pink_6 VALUES( '" + vulnerable + "' , '" + pacman_x +
            "' , '" + pacman_y + "' , '" + pink_row + "' , '" + pink_column + "' , '" + direction + "' )";

        manager.CommandText = pink_dbquery;
        reader = manager.ExecuteReader();
        reader.Close();
    }

    public void cyanRecord(GameObject pacman, GameObject red, GameObject pink, GameObject cyan, IDbCommand manager, IDataReader reader)
    {

        // cyan 1
        string vulnerable = cyan.GetComponent<GhostController>().vulnerable.ToString();
        string pacman_close = pink.GetComponent<GhostController>().closeToPacman().ToString();
        string pacman_nearest = getState(pacman.GetComponent<PlayerController>().nearestIntersection().name.ToString());
        string pacman_target = getState(pacman.GetComponent<PlayerController>().getTarget().name.ToString());
        string red_nearest = getState(red.GetComponent<GhostController>().nearestIntersection().name.ToString());
        string red_target = getState(red.GetComponent<GhostController>().getTarget().name.ToString());
        string pink_nearest = getState(pink.GetComponent<GhostController>().nearestIntersection().name.ToString());
        string pink_target = getState(pink.GetComponent<GhostController>().getTarget().name.ToString());
        string cyan_nearest = getState(cyan.GetComponent<GhostController>().nearestIntersection().name.ToString());
        string cyan_target = getState(cyan.GetComponent<GhostController>().getTarget().name.ToString());

        string cyan_dbquery = "INSERT INTO cyan_1 VALUES( '" + vulnerable + "' , '" + pacman_close +
            "' , '" + pacman_nearest + "' , '" + pacman_target + "' , '" + red_nearest + "' , '" + red_target +
            "' , '" + pink_nearest + "' , '" + pink_target + "' , '" + cyan_nearest + "' , '" + cyan_target + "' )";

        manager.CommandText = cyan_dbquery;
        reader = manager.ExecuteReader();
        reader.Close();

        // cyan 2
        string pacman_x = pacman.transform.position.x.ToString();
        string pacman_y = pacman.transform.position.y.ToString();
        string red_x = red.transform.position.x.ToString();
        string red_y = red.transform.position.y.ToString();
        string red_target_x = red.GetComponent<GhostController>().getTarget().transform.position.x.ToString();
        string red_target_y = red.GetComponent<GhostController>().getTarget().transform.position.y.ToString();
        string pink_x = pink.transform.position.x.ToString();
        string pink_y = pink.transform.position.y.ToString();
        string pink_target_x = pink.GetComponent<GhostController>().getTarget().transform.position.x.ToString();
        string pink_target_y = pink.GetComponent<GhostController>().getTarget().transform.position.y.ToString();
        string cyan_x = cyan.transform.position.x.ToString();
        string cyan_y = cyan.transform.position.y.ToString();

        cyan_dbquery = "INSERT INTO cyan_2 VALUES( '" + vulnerable + "' , '" + pacman_x +
            "' , '" + pacman_y + "' , '" + red_x + "' , '" + red_y + "' , '" + red_target_x + "' , '" + red_target_y + "' , '" + pink_x + "' , '" + pink_y + "' , '" + pink_target_x + "' , '" + pink_target_y + "' , '" + cyan_x + "' , '" + cyan_y + "' , '" + cyan_target +"' )";
        manager.CommandText = cyan_dbquery;
        reader = manager.ExecuteReader();
        reader.Close();

        // cyan 3
        cyan_dbquery = "INSERT INTO cyan_3 VALUES( '" + vulnerable + "' , '" + pacman_x +
           "' , '" + pacman_y + "' , '" + red_x + "' , '" + red_y + "' , '" + pink_x + "' , '" + pink_y + "' , '" + cyan_x + "' , '" + cyan_y + "' , '" + cyan_target + "' )";

        manager.CommandText = cyan_dbquery;
        reader = manager.ExecuteReader();
        reader.Close();


        // cyan 4
        cyan_dbquery = "INSERT INTO cyan_4 VALUES( '" + vulnerable + "' , '" + pacman_x +
            "' , '" + pacman_y + "' , '" + cyan_x + "' , '" + cyan_y + "' , '" + cyan_target + "' )";

        manager.CommandText = cyan_dbquery;
        reader = manager.ExecuteReader();
        reader.Close();

        // cyan 5
        string direction = cyan.GetComponent<GhostController>().getDirection().ToString();

        cyan_dbquery = "INSERT INTO cyan_5 VALUES( '" + vulnerable + "' , '" + pacman_x +
            "' , '" + pacman_y + "' , '" + cyan_x + "' , '" + cyan_y + "' , '" + direction + "' )";

        manager.CommandText = cyan_dbquery;
        reader = manager.ExecuteReader();
        reader.Close();

        // cyan 6
        string cyan_row = cyan.GetComponent<GhostController>().getLine().ToString();
        string cyan_column = cyan.GetComponent<GhostController>().getColumn().ToString();
        cyan_dbquery = "INSERT INTO cyan_6 VALUES( '" + vulnerable + "' , '" + pacman_x +
            "' , '" + pacman_y + "' , '" + cyan_row + "' , '" + cyan_column + "' , '" + direction + "' )";

        manager.CommandText = cyan_dbquery;
        reader = manager.ExecuteReader();
        reader.Close();
    }

    public void orangeRecord(GameObject pacman, GameObject red, GameObject pink, GameObject cyan,GameObject orange, IDbCommand manager, IDataReader reader)
    {
        // orange 1
        string vulnerable = orange.GetComponent<GhostController>().vulnerable.ToString();
        string pacman_close = pink.GetComponent<GhostController>().closeToPacman().ToString();
        string pacman_nearest = getState(pacman.GetComponent<PlayerController>().nearestIntersection().name.ToString());
        string pacman_target = getState(pacman.GetComponent<PlayerController>().getTarget().name.ToString());
        string red_nearest = getState(red.GetComponent<GhostController>().nearestIntersection().name.ToString());
        string red_target = getState(red.GetComponent<GhostController>().getTarget().name.ToString());
        string pink_nearest = getState(pink.GetComponent<GhostController>().nearestIntersection().name.ToString());
        string pink_target = getState(pink.GetComponent<GhostController>().getTarget().name.ToString());
        string cyan_nearest = getState(cyan.GetComponent<GhostController>().nearestIntersection().name.ToString());
        string cyan_target = getState(cyan.GetComponent<GhostController>().getTarget().name.ToString());
        string orange_nearest = getState(orange.GetComponent<GhostController>().nearestIntersection().name.ToString());
        string orange_target = getState(orange.GetComponent<GhostController>().getTarget().name.ToString());

        string orange_dbquery = "INSERT INTO orange_1 VALUES( '" + vulnerable + "' , '" + pacman_close +
            "' , '" + pacman_nearest + "' , '" + pacman_target + "' , '" + red_nearest + "' , '" + red_target +
            "' , '" + pink_nearest + "' , '" + pink_target + "' , '" + cyan_nearest + "' , '" + cyan_target + "' , '" + orange_nearest + "' , '" + orange_target + "' )";

        manager.CommandText = orange_dbquery;
        reader = manager.ExecuteReader();
        reader.Close();

        // orange 2
        string pacman_x = pacman.transform.position.x.ToString();
        string pacman_y = pacman.transform.position.y.ToString();
        string red_x = red.transform.position.x.ToString();
        string red_y = red.transform.position.y.ToString();
        string red_target_x = red.GetComponent<GhostController>().getTarget().transform.position.x.ToString();
        string red_target_y = red.GetComponent<GhostController>().getTarget().transform.position.y.ToString();
        string pink_x = pink.transform.position.x.ToString();
        string pink_y = pink.transform.position.y.ToString();
        string pink_target_x = pink.GetComponent<GhostController>().getTarget().transform.position.x.ToString();
        string pink_target_y = pink.GetComponent<GhostController>().getTarget().transform.position.y.ToString();
        string cyan_x = cyan.transform.position.x.ToString();
        string cyan_y = cyan.transform.position.y.ToString();
        string cyan_target_x = cyan.GetComponent<GhostController>().getTarget().transform.position.x.ToString();
        string cyan_target_y = cyan.GetComponent<GhostController>().getTarget().transform.position.y.ToString();
        string orange_x = orange.transform.position.x.ToString();
        string orange_y = orange.transform.position.y.ToString();

        orange_dbquery = "INSERT INTO orange_2 VALUES( '" + vulnerable + "' , '" + pacman_x +
            "' , '" + pacman_y + "' , '" + red_x + "' , '" + red_y + "' , '" + red_target_x + "' , '" + red_target_y + "' , '" + pink_x + "' , '" + pink_y + "' , '" + pink_target_x + "' , '" + pink_target_y + "' , '" + cyan_x + "' , '" + cyan_y +
             "' , '" + cyan_target_x + "' , '" + cyan_target_y + "' , '" + orange_x + "' , '" + orange_y + "' , '" + orange_target + "' )";

        manager.CommandText = orange_dbquery;
        reader = manager.ExecuteReader();
        reader.Close();

        // orange 3
        orange_dbquery = "INSERT INTO orange_3 VALUES( '" + vulnerable + "' , '" + pacman_x +
           "' , '" + pacman_y + "' , '" + red_x + "' , '" + red_y + "' , '" + pink_x + "' , '" + pink_y + "' , '" + cyan_x + "' , '" + cyan_y + "' , '" + orange_x + "' , '" + orange_y + "' , '" + orange_target + "' )";

        manager.CommandText = orange_dbquery;
        reader = manager.ExecuteReader();
        reader.Close();


        // orange 4
        orange_dbquery = "INSERT INTO orange_4 VALUES( '" + vulnerable + "' , '" + pacman_x +
            "' , '" + pacman_y + "' , '" + orange_x + "' , '" + orange_y + "' , '" + orange_target + "' )";

        manager.CommandText = orange_dbquery;
        reader = manager.ExecuteReader();
        reader.Close();

        // orange 5
        string direction = orange.GetComponent<GhostController>().getDirection().ToString();

        orange_dbquery = "INSERT INTO orange_5 VALUES( '" + vulnerable + "' , '" + pacman_x +
            "' , '" + pacman_y + "' , '" + orange_x + "' , '" + orange_y + "' , '" + direction + "' )";

        manager.CommandText = orange_dbquery;
        reader = manager.ExecuteReader();
        reader.Close();

        // orange 6
        string orange_row = orange.GetComponent<GhostController>().getLine().ToString();
        string orange_column = orange.GetComponent<GhostController>().getColumn().ToString();
        orange_dbquery = "INSERT INTO orange_6 VALUES( '" + vulnerable + "' , '" + pacman_x +
            "' , '" + pacman_y + "' , '" + orange_row + "' , '" + orange_column + "' , '" + direction + "' )";

        manager.CommandText = orange_dbquery;
        reader = manager.ExecuteReader();
        reader.Close();
    }
   
    public string getState(string name)
    {
        if (name[0] != 'i')
        {
            return "0";
        }
        else
        {
            string new_name = name.Substring(1);
            return new_name;
        }
            

    }
    public static string GetDataPath(string section)
    {
        string path = System.IO.Path.Combine(Application.persistentDataPath, section);
        if (!System.IO.Directory.Exists(path))
        {
            System.IO.Directory.CreateDirectory(path);
        }
        return path;
    }

    private string GetDbPath()
    {
        string path = GetDataPath(dataFolder);
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        path = Path.Combine(path, sqlLiteDbName);
        return path;
    }

    private string GetConnectionString(string path = null)
    {
        if (path == null)
            path = GetDbPath();
        return string.Format("URI=file:{0}; Version=3", path);
    }
}
