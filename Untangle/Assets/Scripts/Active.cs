using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Active : MonoBehaviour {

    public bool active;
    public Sprite actsprite;
    public Sprite nonactsprite;

    void Start () {
        CheckActiv();
    }

	void OnMouseUpAsButton()
    {
        GetComponent<Active>().active = !GetComponent<Active>().active;
        CheckActiv();
    }

    public void CheckActiv()
    {
        if (active)
        {
            GetComponent<SpriteRenderer>().sprite = actsprite;
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = nonactsprite;
        }
    }
}
