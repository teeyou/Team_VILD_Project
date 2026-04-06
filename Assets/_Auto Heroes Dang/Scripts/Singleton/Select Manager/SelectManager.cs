using UnityEngine;
using UnityEngine.UI;

public class SelectManager : MonoBehaviour
{
    [SerializeField] private GameObject _shield01;
    [SerializeField] private GameObject _shield02;

    [SerializeField] int _playAnimType = 0;

    [SerializeField] private GameObject _back01;
    [SerializeField] private GameObject _back02;

    [SerializeField] private Button _startButton;

    private Camera _cam;
    private SelectScenePlayer unit, unit2;

    private int _selectedCharacterNumber = -1;

    private bool _isPressedStartButton = false;

    void Start()
    {
        _cam = Camera.main;

        unit = _shield01.GetComponent<SelectScenePlayer>();
        unit2 = _shield02.GetComponent<SelectScenePlayer>();

        _startButton.onClick.AddListener(StartGame);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject == _shield01)
                {
                    SelectCharacter((int)ECharacterNumber.Shield_01, true, unit);
                }
                else if (hit.collider.gameObject == _shield02)
                {
                    SelectCharacter((int)ECharacterNumber.Shield_02, false, unit2);
                }
            }
        }
    }

    private void SelectCharacter(int characterNumber, bool isShield01, SelectScenePlayer selectedUnit)
    {
        if (_selectedCharacterNumber == characterNumber)
            return;

        _selectedCharacterNumber = characterNumber;

        ToggleBG(isShield01);
        PlaySelectAnimation(selectedUnit);
    }

    private void PlaySelectAnimation(SelectScenePlayer selectedUnit)
    {
        if (selectedUnit == null)
            return;

        if (_playAnimType == 0)
        {
            selectedUnit.PlayVictoryAnimation();
        }
        else if (_playAnimType == 1)
        {
            selectedUnit.PlayAttackAnimation();
        }
        else if (_playAnimType == 2)
        {
            selectedUnit.PlaySkillAnimation();
        }
    }

    private void ToggleBG(bool flag)
    {
        _back01.SetActive(flag);
        _back02.SetActive(!flag);

        AudioManager.Instance.PlaySFX("SelectChar");
    }

    private void StartGame()
    {
        if (_selectedCharacterNumber == -1)
        {
            Debug.Log("캐릭터 선택 필요");
            return;
        }

        if (_isPressedStartButton)
        {
            Debug.Log("선택 완료 처리중...");
            return;
        }

        _isPressedStartButton = true;

        DataSource.Instance.MainCharacterIdx = _selectedCharacterNumber;
        SceneLoader.Instance.LoadScene(ESceneId.FieldScene);
    }
}