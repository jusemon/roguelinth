using UnityEngine;
using System.Collections.Generic;
public static class Direction
{

    public static readonly IEnumerable<Vector3> All = new Vector3[] { Vector3.left, Vector3.forward, Vector3.right, Vector3.back };

    public static Vector3 Reverse(Vector3 direction)
    {
        return direction * -1;
    }

    public static Vector3 GetRandom()
    {
        var directions = new List<Vector3>();
        directions.AddRange(new Vector3[] { Vector3.left, Vector3.forward, Vector3.right, Vector3.left, Vector3.forward, Vector3.right, Vector3.back });
        var index = Random.Range(1, directions.Count + 1) - 1;
        return directions[index];
    }
}