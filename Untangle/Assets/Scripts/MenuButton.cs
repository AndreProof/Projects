using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour {

    public float increase = 0.3f;
    public Color col;

    void Start()
    {
        foreach(GameObject gm in GameObject.FindGameObjectsWithTag("levelbuttons"))
        {
            if (Convert.ToInt32(gm.name.Substring(0, gm.name.Length - 4)) > PlayerPrefs.GetInt("MaxLevel"))
            {
                gm.GetComponent<SpriteRenderer>().color = col;
                gm.GetComponent<CircleCollider2D>().enabled = false;
            }
        }
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
        if (gameObject.name != "backicon")
        {
            SceneManager.LoadScene("level " + gameObject.name.Substring(0, gameObject.name.Length - 4));
        }
        else
            SceneManager.LoadScene("main");
    }
}
