using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelGenerator : MonoBehaviour
{

    [SerializeField]
    GameObject sectionBase;

    [SerializeField]
    float distance = 10f;

    [SerializeField]
    public bool randomRooms = true;

    [HideInInspector]
    [Range(6, 9)]
    public int numberOfRooms = 9;

    private List<GameObject> rooms;

    // Start is called before the first frame update
    void Start()
    {
        CreateLevel();
    }

    private void CreateLevel()
    {
        var roomsCount = randomRooms ? Random.Range(6, 10) : numberOfRooms;
        var tries = 0;
        rooms = new List<GameObject>(roomsCount);
        for (int index = 0; index < roomsCount; index++)
        {
            Debug.Log($"> index {index}");
            bool isInitial = false, isFinal = false;
            Vector3? currentDirection;
            Vector3? previousDirection;
            var position = CalculatePosition(rooms, index, out currentDirection);
            previousDirection = Direction.Reverse(currentDirection.GetValueOrDefault());
            // We are stuck, probably an espiral
            if (!currentDirection.HasValue)
            {
                // Delete at least 5 rooms and try again
                var roomsToRemove = 5 + tries / 2;
                roomsToRemove = index > roomsToRemove ? roomsToRemove : index;
                var limit = index - roomsToRemove - 1;
                for (int backwardIndex = index - 1; backwardIndex > limit; backwardIndex--)
                {
                    Debug.Log($"> backwardindex {backwardIndex}");
                    DeleteRoom(backwardIndex);
                }
                index = limit;
                tries++;
                Debug.Log($"> tries {tries}");
                continue;
            }
            else
            {
                // Initial level
                if (index == 0)
                {
                    previousDirection = null;
                    isInitial = true;
                }
                else
                {
                    UpdateRoom(index - 1, currentDirection.Value);

                    if (index == (roomsCount - 1))
                    {
                        isFinal = true;
                    }
                }
            }

            var room = CreateRoom(index, position, previousDirection, isInitial, isFinal);
            previousDirection = currentDirection;
            rooms.Add(room);
            if (isFinal)
            {
                UpdateRoom(index, true);
            }
        }
    }

    private Vector3 CalculatePosition(List<GameObject> map, int index, out Vector3? currentDirection)
    {
        currentDirection = CalculateDirection(map, index);
        var position = transform.position;
        if (index > 0 && currentDirection.HasValue)
        {
            position = map[index - 1].transform.position + currentDirection.Value * distance;
        }
        return position;
    }

    private Vector3? CalculateDirection(List<GameObject> map, int index, int tries = 5)
    {
        var currentDirection = Direction.GetRandom();
        if (tries == 0)
        {
            var positions = Direction.All.ToDictionary(key => key, value => map[index - 1].transform.position + value * distance);
            var validPos = positions.Where(pos => !map.Select(room => room.transform.position).Contains(pos.Value)).ToArray();
            if (validPos.Length > 0)
            {
                currentDirection = validPos[0].Key;
            }
            else
            {
                return null;
            }
        }

        if (index > 0)
        {
            var position = map[index - 1].transform.position + currentDirection * distance;
            if (map.Select(room => room.transform.position).Any(pos => pos == position))
            {
                return CalculateDirection(map, index, --tries);
            }
        }
        return currentDirection;
    }

    private GameObject CreateRoom(int index, Vector3 position, Vector3? prevDirection, bool isInitial, bool isFinal)
    {
        Debug.Log($"Create Room {index + 1}");
        var room = Instantiate(sectionBase, position, Quaternion.identity, transform);
        room.name = $"Room {index}";
        var roomManager = room.GetComponent<RoomManager>();
        roomManager.isInitial = isInitial;
        roomManager.isFinal = isFinal;
        roomManager.prevDoorDirection = prevDirection;
        return room;
    }

    private void UpdateRoom(int index, Vector3 nextDirection)
    {
        Debug.Log($"Update Room {index + 1}");
        var room = rooms[index];
        var roomManager = room.GetComponent<RoomManager>();
        roomManager.nextDoorDirection = nextDirection;
        roomManager.ProcessRoom();
    }

    private void UpdateRoom(int index, bool isFinal)
    {
        Debug.Log($"Update Room {index + 1}");
        var room = rooms[index];
        var roomManager = room.GetComponent<RoomManager>();
        roomManager.isFinal = isFinal;
        roomManager.ProcessRoom();
    }

    private void DeleteRoom(int index)
    {
        Debug.Log($"Delete Room {index + 1}");
        var room = rooms[index];
        rooms.Remove(room);
        Destroy(room);
    }

    // Update is called once per frame
    void Update()
    {

    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(LevelGenerator))]
public class LevelGenerator_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // for other non-HideInInspector fields
        LevelGenerator script = (LevelGenerator)target;
        if (!script.randomRooms) // if bool is true, show other fields
        {
            Debug.Log("Toggled");
            script.numberOfRooms = EditorGUILayout.IntField("Number of Rooms", script.numberOfRooms);
        }
    }
}
#endif