using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Walls
{
    public GameObject left;
    public GameObject up;
    public GameObject right;
    public GameObject down;
}

public class RoomManager : MonoBehaviour
{

    public bool isInitial;

    public bool isFinal;

    public Vector3 nextDoorDirection;

    public Vector3? prevDoorDirection;

    [SerializeField]
    public Walls walls;

    // Start is called before the first frame update
    public void ProcessRoom()
    {
        

        if (isInitial)
        {
            // Setup store & weapon chest
        }

        if (isFinal)
        {
            // Setup boss and mark final door as win
        }

        CreateDoorManager(prevDoorDirection, "Previous Door");
        CreateDoorManager(nextDoorDirection, "Next Door");

    }

    private void CreateDoorManager(Vector3? doorDirection, string name = null)
    {
        if (doorDirection.HasValue)
        {
            CreateDoorManager(doorDirection.Value, name);
        }
    }

    private void CreateDoorManager(Vector3 doorDirection, string name = null)
    {

        if (doorDirection == Vector3.left)
        {
            CreateDoor(walls.left, name);
        }
        if (doorDirection == Vector3.forward)
        {
            CreateDoor(walls.up, name);
        }
        if (doorDirection == Vector3.right)
        {
            CreateDoor(walls.right, name);
        }
        if (doorDirection == Vector3.back)
        {
            CreateDoor(walls.down, name);
        }

    }

    private void CreateDoor(GameObject wall, string name = null)
    {
        var doorManager = wall.GetComponentInChildren<DoorManager>();
        doorManager.CreateDoor(name);
    }

    // Update is called once per frame
    void Update()
    {

    }
}