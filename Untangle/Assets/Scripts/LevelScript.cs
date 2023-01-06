using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Line
{
    int fst;
    int snd;

    public Line(int vec1, int vec2)
    {
        fst = vec1;
        snd = vec2;
    }

    public Line(string vec1, string vec2)
    {
        fst = Convert.ToInt16(vec1);
        snd = Convert.ToInt16(vec2);
    }

    public int X
    {
        get { return fst; }
        set { X = value; }
    }
    public int Y
    {
        get { return snd; }
        set { Y = value; }
    }
}

public class LevelScript : MonoBehaviour {

    static List<Line> lines;



    public float increase = 0.3f;

    public Material mat;

    public int level;



    private GameObject lineObject;

    private GameObject[] levels;

    private LineRenderer line;



    void Start()
    {
        lines = new List<Line>();
        levels = GameObject.FindGameObjectsWithTag("gamebuttons");
        Debug.Log("+");
        Load();
        bool check = true;
        while (check)
        {
            SoRandom();
            for (int i = 0; i< lines.Count; i++)
            {
                for(int j = 0; j< lines.Count; j++)
                {
                    if (Cross(lines[i], lines[j]))
                    {
                        check = false;
                        break;
                    }

                }
            }
        }
        
        Paint();
    }

    void OnMouseDown()
    {
        transform.localScale = new Vector2(transform.localScale.x + increase, transform.localScale.y + increase);
    }

    void OnMouseUp()
    {
        transform.localScale = new Vector2(transform.localScale.x - increase, transform.localScale.y - increase);
    }

    void OnMouseUpAsButton()
    {
        switch (gameObject.name)
        {
            case "gameicon": Paint(); break;

            case "saveicon": Save(); break;

            case "loadicon": Load(); Paint(); break;

            case "reloadicon": SoRandom(); Paint(); break;
        }
    }

    void Paint()
    {
        for (int i = 0; i < levels.Length - 1; i++)
        {
            if (levels[i].GetComponent<Active>().active)
            {
                for (int j = i + 1; j < levels.Length; j++)
                {
                    if (levels[j].GetComponent<Active>().active)
                    {
                        if (lines.Count != 0)
                        {
                            if (lines.Exists(x => x.X == i && x.Y == j))
                                lines.RemoveAll(x => x.X == i && x.Y == j);
                            else
                                lines.Add(new Line(i, j));
                        }
                        else
                            lines.Add(new Line(i, j));
                    }
                }
            }
        }
        foreach (GameObject gm in GameObject.FindGameObjectsWithTag("linerenderer"))
        {
            Destroy(gm);
        }

        for (int i = 0; i < lines.Count; i++)
        {
            lineObject = new GameObject("Line");
            lineObject.tag = "linerenderer";
            line = lineObject.AddComponent<LineRenderer>();
            line.tag = "linerenderer";
            line.SetVertexCount(2);
            line.SetWidth(0.1f, 0.1f);
            line.material = mat;
            for (int j = 0; j < lines.Count; j++)
            {
                if (i != j)
                    if (Cross(lines[i], lines[j]))
                    {
                        line.material = null;
                    }
            }
            line.SetPosition(0, levels[lines[i].X].transform.position);
            line.SetPosition(1, levels[lines[i].Y].transform.position);
        }
    }

    void Save()
    {
        using (StreamWriter str = new StreamWriter(Application.dataPath + "/savedGames.txt"))
        {
            foreach (Line ln in lines)
            {
                str.Write(ln.X + " " + ln.Y + " ");
            }
            str.Close();
        }
    }

    void Load()
    {
        string text = GameObject.FindGameObjectWithTag("linestext").GetComponent<Text>().text;
        string[] xy;
        xy = text.Split(' ');
        for (int i = 0; i < xy.Length; i += 2)
        {
            lines.Add(new Line(xy[i], xy[i + 1]));
        }
    }

    void SoRandom()
    {
        int counter = 0, i, j;
        Vector3 tmp = new Vector3();
        System.Random rand = new System.Random();
        while (counter < 100)
        {
            i = rand.Next(levels.Length);
            j = rand.Next(levels.Length);

            if (i != j)
            {
                tmp = Vector3.MoveTowards(tmp, levels[i].transform.position, 500 * Time.deltaTime);
                levels[i].transform.position = Vector3.MoveTowards(levels[i].transform.position, levels[j].transform.position, 500 * Time.deltaTime);
                levels[j].transform.position = Vector3.MoveTowards(levels[j].transform.position, tmp, 500 * Time.deltaTime);
            }
            counter++;
        }
    }

    void Update()
    {
        bool check = false; ;
        for (int i = 0; i < lines.Count; i++)
        {
            for (int j = 0; j < lines.Count; j++)
            {
                if (Cross(lines[i], lines[j]))
                {
                    check = true;
                }
            }
        }
        if (!check)
        {
            if (level != 20)
            {
                if (PlayerPrefs.GetInt("MaxLevel") < level + 1)
                    PlayerPrefs.SetInt("MaxLevel", level + 1);
                SceneManager.LoadScene("level " + (level + 1));
            }
            else
                SceneManager.LoadScene("main");
        }

        if (CountActive() == 2)
        {
            Vector3 tmp = new Vector3();
            for (int i = 0; i < levels.Length - 1; i++)
            {
                if (levels[i].GetComponent<Active>().active)
                {
                    for (int j = i + 1; j < levels.Length; j++)
                    {
                        if (levels[j].GetComponent<Active>().active)
                        {
                            tmp = Vector3.MoveTowards(tmp, levels[i].transform.position, 500 * Time.deltaTime);
                            levels[i].transform.position = Vector3.MoveTowards(levels[i].transform.position, levels[j].transform.position, 500 * Time.deltaTime);
                            levels[j].transform.position = Vector3.MoveTowards(levels[j].transform.position, tmp, 500 * Time.deltaTime);
                            levels[i].GetComponent<Active>().active = false;
                            levels[j].GetComponent<Active>().active = false;
                            Paint();
                            levels[j].GetComponent<Active>().CheckActiv();
                            levels[i].GetComponent<Active>().CheckActiv();
                            return;
                        }
                    }
                }
            }
        }
    }

    int CountActive()
    {
        int count = 0;
        for (int i = 0; i < levels.Length; i++)
        {
            if (levels[i].GetComponent<Active>().active)
                count++;
        }
        return count;
    }

    bool Cross(Line fst, Line snd)
    {
        if (fst.X != snd.X && fst.X != snd.Y && fst.Y != snd.X && fst.Y != snd.Y)
        {
            Vector2 p1 = levels[fst.X].transform.position, p2 = levels[fst.Y].transform.position;
            Vector2 p3 = levels[snd.X].transform.position, p4 = levels[snd.Y].transform.position;
            float A1 = p2.y - p1.y;
            float B1 = p1.x - p2.x;
            float C1 = -A1 * p1.x - B1 * p1.y;

            float A2 = p4.y - p3.y;
            float B2 = p3.x - p4.x;
            float C2 = -A2 * p3.x - B2 * p3.y;

            float f1 = A1 * p3.x + B1 * p3.y + C1;
            float f2 = A1 * p4.x + B1 * p4.y + C1;
            float f3 = A2 * p1.x + B2 * p1.y + C2;
            float f4 = A2 * p2.x + B2 * p2.y + C2;

            if (Math.Abs((p2.x - p1.x) / (p4.x - p3.x) - (p2.y - p1.y) / (p4.y - p3.y)) < Math.Pow(10, -3))
                return true;

            return (f1 * f2 < 0 && f3 * f4 < 0);
        }
        else
            return false;
    }
}
