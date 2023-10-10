using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Earthquake : MonoBehaviour
{
public int power;
public int powerSide;
    private void Start() {
        
    }
    public void Quake(){
        GetComponent<Rigidbody>().AddForce(Vector3.back*power
         + new Vector3(Random.Range(-1.0f,1.0f),Random.Range(-1.0f,1.0f),0)*powerSide);
    }


      void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("space key was pressed");
            Quake();
        }
    }
}
