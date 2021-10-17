using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskTile : MonoBehaviour
{
    SpriteRenderer sr;
    SpriteMask sm;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sm = GetComponent<SpriteMask>();

        sm.sprite = sr.sprite;
    }
}
