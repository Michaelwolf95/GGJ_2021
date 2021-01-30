using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicShineEffect : MonoBehaviour
{

    public Material shineMat;
    public Material plainMat;

    private void Awake()
    {
        StartCoroutine("Shine");
    }

    // Start is called before the first frame update
    void Start()
    {
        shineMat.SetFloat("_reflectSpeed", 0);
        GetComponent<Renderer>().material = plainMat;
    }

    IEnumerator Shine()
    {
        while (true)
        {
            yield return new WaitForSeconds(3.0f);
            GetComponent<Renderer>().material = shineMat;
            shineMat.SetFloat("_reflectSpeed", 1.5f);
            yield return new WaitForSeconds(2.5f);
            shineMat.SetFloat("_reflectSpeed", 0);
            GetComponent<Renderer>().material = plainMat;
        }
        
    }

}
