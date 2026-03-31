using System.Collections.Generic;
using UnityEngine;

public static class CombatSlotManager
{
    private class SlotGroup
    {
        public Dictionary<Transform, int> requesterToSlotIndex = new Dictionary<Transform, int>();
        public Dictionary<int, Transform> slotIndexToRequester = new Dictionary<int, Transform>();
    }

    private static readonly Dictionary<int, SlotGroup> _groups = new Dictionary<int, SlotGroup>();

    /// <summary>
    /// 타겟 주변 슬롯 위치를 반환한다.
    /// - 이미 슬롯을 가지고 있으면 그 슬롯 유지
    /// - 빈 슬롯이 있으면 가장 가까운 빈 슬롯 배정
    /// - 빈 슬롯이 없으면 Vector3.zero 반환
    /// </summary>
    public static Vector3 GetSlotPosition(Transform target, Transform requester, int slotCount, float radius)
    {
        if (target == null || requester == null)
            return Vector3.zero;

        int targetId = target.GetInstanceID();

        if (!_groups.TryGetValue(targetId, out SlotGroup group))
        {
            group = new SlotGroup();
            _groups.Add(targetId, group);
        }

        CleanupInvalidEntries(group);

        // 이미 점유한 슬롯이 있으면 그대로 유지
        if (group.requesterToSlotIndex.TryGetValue(requester, out int ownedSlotIndex))
        {
            return CalculateSlotPosition(target, ownedSlotIndex, slotCount, radius);
        }

        int bestSlotIndex = -1;
        float bestDistanceSqr = float.MaxValue;

        // 빈 슬롯 중에서 가장 가까운 슬롯 찾기
        for (int i = 0; i < slotCount; i++)
        {
            if (group.slotIndexToRequester.ContainsKey(i))
                continue;

            Vector3 slotPos = CalculateSlotPosition(target, i, slotCount, radius);
            float distSqr = GetFlatDistanceSqr(requester.position, slotPos);

            if (distSqr < bestDistanceSqr)
            {
                bestDistanceSqr = distSqr;
                bestSlotIndex = i;
            }
        }

        // 빈 슬롯이 없으면 배정하지 않음
        if (bestSlotIndex == -1)
        {
            return Vector3.zero;
        }

        group.requesterToSlotIndex[requester] = bestSlotIndex;
        group.slotIndexToRequester[bestSlotIndex] = requester;

        return CalculateSlotPosition(target, bestSlotIndex, slotCount, radius);
    }

    /// <summary>
    /// 특정 타겟에 대해 내가 점유한 슬롯을 해제한다.
    /// </summary>
    public static void ReleaseSlot(Transform target, Transform requester)
    {
        if (target == null || requester == null)
            return;

        int targetId = target.GetInstanceID();

        if (!_groups.TryGetValue(targetId, out SlotGroup group))
            return;

        if (group.requesterToSlotIndex.TryGetValue(requester, out int slotIndex))
        {
            group.requesterToSlotIndex.Remove(requester);

            if (group.slotIndexToRequester.TryGetValue(slotIndex, out Transform owner) && owner == requester)
            {
                group.slotIndexToRequester.Remove(slotIndex);
            }
        }

        if (group.requesterToSlotIndex.Count == 0)
        {
            _groups.Remove(targetId);
        }
    }

    /// <summary>
    /// requester가 점유 중인 모든 슬롯을 해제한다.
    /// </summary>
    public static void ReleaseAllSlotsForRequester(Transform requester)
    {
        if (requester == null)
            return;

        List<int> emptyGroupIds = new List<int>();

        foreach (var pair in _groups)
        {
            SlotGroup group = pair.Value;

            if (group.requesterToSlotIndex.TryGetValue(requester, out int slotIndex))
            {
                group.requesterToSlotIndex.Remove(requester);

                if (group.slotIndexToRequester.TryGetValue(slotIndex, out Transform owner) && owner == requester)
                {
                    group.slotIndexToRequester.Remove(slotIndex);
                }
            }

            if (group.requesterToSlotIndex.Count == 0)
            {
                emptyGroupIds.Add(pair.Key);
            }
        }

        for (int i = 0; i < emptyGroupIds.Count; i++)
        {
            _groups.Remove(emptyGroupIds[i]);
        }
    }

    /// <summary>
    /// 특정 타겟의 슬롯 그룹 전체를 비운다.
    /// </summary>
    public static void ClearTargetSlots(Transform target)
    {
        if (target == null)
            return;

        int targetId = target.GetInstanceID();
        _groups.Remove(targetId);
    }

    /// <summary>
    /// 모든 슬롯 정보를 초기화한다.
    /// </summary>
    public static void ClearAll()
    {
        _groups.Clear();
    }

    private static void CleanupInvalidEntries(SlotGroup group)
    {
        List<Transform> invalidRequesters = new List<Transform>();
        List<int> invalidSlotIndices = new List<int>();

        foreach (var pair in group.requesterToSlotIndex)
        {
            if (pair.Key == null)
            {
                invalidRequesters.Add(pair.Key);
            }
        }

        foreach (var pair in group.slotIndexToRequester)
        {
            if (pair.Value == null)
            {
                invalidSlotIndices.Add(pair.Key);
            }
        }

        for (int i = 0; i < invalidRequesters.Count; i++)
        {
            group.requesterToSlotIndex.Remove(invalidRequesters[i]);
        }

        for (int i = 0; i < invalidSlotIndices.Count; i++)
        {
            group.slotIndexToRequester.Remove(invalidSlotIndices[i]);
        }
    }

    private static Vector3 CalculateSlotPosition(Transform target, int slotIndex, int slotCount, float radius)
    {
        float angleStep = 360f / slotCount;
        float angle = angleStep * slotIndex;

        Vector3 dir = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
        Vector3 slotPos = target.position + dir * radius;
        slotPos.y = target.position.y;

        return slotPos;
    }


    private static float GetFlatDistanceSqr(Vector3 a, Vector3 b)
    {
        a.y = 0f;
        b.y = 0f;
        return (a - b).sqrMagnitude;
    }
}