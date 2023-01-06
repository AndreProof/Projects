using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour {

    public float increase = 0.3f;

    public GameObject gm;

    void Start()
    {
        if (PlayerPrefs.GetInt("MaxLevel") < 1)
            PlayerPrefs.SetInt("MaxLevel", 1);
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
            case "playicon":
                SceneManager.LoadScene("menu");
                break;
            case "rateicon":
                Application.OpenURL("https://www.yandex.ru/");
                break;
            case "abouticon":
                if (gm.activeSelf)
                    gm.SetActive(false);
                else
                    gm.SetActive(true);
                break;
        }
    }
}
