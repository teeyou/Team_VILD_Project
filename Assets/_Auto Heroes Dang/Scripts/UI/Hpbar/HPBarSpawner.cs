using UnityEngine;

public class HpBarSpawner : MonoBehaviour
{
    public static HpBarSpawner Instance { get; private set; }

    [Header("Canvas")]
    [SerializeField] private Canvas _targetCanvas;

    [Header("HP바 프리팹")]
    [SerializeField] private HpBar _hpBarPrefab;

    [Header("기본 오프셋")]
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
            return;
        }
    }

    public HpBar SpawnHpBar(Unit owner)
    {
        if (_targetCanvas == null)
        {
            Debug.LogWarning("WorldHpBarSpawner : Canvas가 연결되지 않았습니다.");
            return null;
        }

        if (_hpBarPrefab == null)
        {
            Debug.LogWarning("WorldHpBarSpawner : HP바 프리팹이 연결되지 않았습니다.");
            return null;
        }

        if (owner == null)
            return null;

        Transform followTarget = owner.transform;
        Vector3 offset = _defaultOffset;

        NormalEnemyBattle monster = owner as NormalEnemyBattle;
        if (monster != null)
        {
            if (monster.HpBarPoint != null)
            {
                followTarget = monster.HpBarPoint;
                offset = Vector3.zero;
            }
            else
            {
                offset = monster.HpBarOffset;
            }
        }

        HpBar hpBar = Instantiate(_hpBarPrefab, _targetCanvas.transform);
        hpBar.Initialize(followTarget, offset);
        hpBar.SetHp(owner.CurHp, owner.MaxHp);

        owner.SetHpBar(hpBar);

        return hpBar;
    }
}