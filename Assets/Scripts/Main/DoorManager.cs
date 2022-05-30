using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{

    [SerializeField]
    GameObject doorTemplate;
    
    [SerializeField]
    GameObject wall;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateDoor(string name = null)
    {
        var door = Instantiate(doorTemplate, wall.transform.position, wall.transform.rotation, transform);
        if (!string.IsNullOrEmpty(name))
        {
            door.name = name;
        }
        Destroy(wall);
    }
}
