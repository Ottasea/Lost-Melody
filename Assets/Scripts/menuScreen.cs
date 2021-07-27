using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menuScreen : MonoBehaviour
{
  
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Application.LoadLevel("mainGame");
        }
    }
}
