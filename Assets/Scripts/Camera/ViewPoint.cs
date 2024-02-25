using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewPoint : MonoBehaviour
{
    CameraSwitcher cameraSwitcher;

    // Start is called before the first frame update
    void Start()
    {
        cameraSwitcher = GetComponent<CameraSwitcher>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!cameraSwitcher.IsFirstPerson) 
        {
            float mousex = Input.GetAxis("Mouse X");
            cameraSwitcher.ThirdPerson.transform.RotateAround(transform.position, transform.up, mousex);
        }
        
    }
}
