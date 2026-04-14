using UnityEngine;

public static class ItemIdGenerator
{
    private static int _nextId = 1;

    public static int GetNextId()
    {
        return _nextId++;
    }
}