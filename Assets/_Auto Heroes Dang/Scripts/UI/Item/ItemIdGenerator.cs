using UnityEngine;

public static class ItemIdGenerator
{
    private static int _nextId = 1;

    public static int GetNextId()
    {
        return _nextId++;
    }

    public static void SetNextId(int nextId)
    {
        _nextId = Mathf.Max(1, nextId);
    }
}