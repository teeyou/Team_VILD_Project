using UnityEngine;

public class HpBarSpawner : MonoBehaviour
{
    public static HpBarSpawner Instance { get; private set; }

    [SerializeField] private Canvas _targetCanvas;
    [SerializeField] private HpBar _hpBarPrefab;
    [SerializeField] private Vector3 _defaultOffset = new Vector3(0f, 2f, 0f);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public HpBar SpawnHpBar(Unit owner, Transform target = null, Vector3? customOffset = null)
    {
        if (_targetCanvas == null)
        {
            Debug.LogWarning("Spawner : Canvas가 연결되지 않았습니다.");
            return null;
        }

        if (_hpBarPrefab == null)
        {
            Debug.LogWarning("Spawner : HPBar 프리팹이 연결되지 않았습니다.");
            return null;
        }

        if (owner == null)
            return null;

        Transform followTarget = target != null ? target : owner.transform;
        Vector3 offset = customOffset ?? _defaultOffset;

        HpBar hpBar = Instantiate(_hpBarPrefab, _targetCanvas.transform);
        hpBar.Initialize(followTarget, offset);
        hpBar.SetHp(owner.CurHp, owner.MaxHp);

        owner.SetHpBar(hpBar);

        return hpBar;
    }
}
