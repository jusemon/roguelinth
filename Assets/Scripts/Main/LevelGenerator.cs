using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{

    [SerializeField]
    GameObject sectionBase;

    [SerializeField]
    [Min(8)]
    int numberOfRooms = 8;

    [SerializeField]
    float distance = 10f;

    private List<GameObject> rooms;


    // Start is called before the first frame update
    void Start()
    {
        CreateLevel();
    }

    private void CreateLevel()
    {
        rooms = new List<GameObject>(numberOfRooms);
        for (int index = 0; index < numberOfRooms; index++)
        {
            bool isInitial = false, isFinal = false;
            Vector3 currentDirection;
            Vector3? previousDirection;
            var position = CalculatePosition(rooms, index, out currentDirection);
            previousDirection = Direction.Reverse(currentDirection);
            // Initial level
            if (index == 0)
            {
                previousDirection = null;
                isInitial = true;
            }
            else
            {
                UpdateRoom(index - 1, currentDirection);
                if (index == (numberOfRooms - 1))
                {
                    // UpdateRoom(index, currentDirection);
                    isFinal = true;
                }
            }

            var room = CreateRoom(position, previousDirection, isInitial, isFinal, index);
            previousDirection = currentDirection;
            rooms.Add(room);
        }
    }

    private Vector3 CalculatePosition(List<GameObject> map, int index, out Vector3 currentDirection)
    {
        currentDirection = CalculateDirection(map, index);
        var position = transform.position;
        if (index > 0)
        {
            position = map[index - 1].transform.position + currentDirection * distance;
        }
        return position;
    }

    private Vector3 CalculateDirection(List<GameObject> map, int index)
    {
        var currentDirection = Direction.GetRandom();
        if (index > 0)
        {
            var position = map[index - 1].transform.position + currentDirection * distance;
            if (map.Select(room => room.transform.position).Any(pos => pos == position))
            {
                return CalculateDirection(map, index);
            }
        }
        return currentDirection;
    }


    private GameObject CreateRoom(Vector3 position, Vector3? prevDirection, bool isInitial, bool isFinal, int index)
    {
        Debug.Log($"Creating Room {index + 1}");
        var room = Instantiate(sectionBase, position, Quaternion.identity, transform);
        room.name = $"Room {index + 1}";
        var roomManager = room.GetComponent<RoomManager>();

        Debug.Log($"  With: ");
        roomManager.isInitial = isInitial;
        roomManager.isFinal = isFinal;
        Debug.Log($"    position: {position}");
        Debug.Log($"    prevDirection: {prevDirection}");
        roomManager.prevDoorDirection = prevDirection;
        return room;
    }

    private void UpdateRoom(int index, Vector3 nextDirection)
    {
        Debug.Log($"Updating Room {index + 1}");
        var room = rooms[index];
        var roomManager = room.GetComponent<RoomManager>();
        Debug.Log($"  With: ");
        Debug.Log($"    nextDirection: {nextDirection}");
        roomManager.nextDoorDirection = nextDirection;
        Debug.Log($"Proccesing Room {index + 1}");
        roomManager.ProcessRoom();
    }



    // Update is called once per frame
    void Update()
    {

    }
}
