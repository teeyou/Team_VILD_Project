using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectManager : MonoBehaviour
{
    [SerializeField] private GameObject _shield01;
    [SerializeField] private GameObject _shield02;

    private Camera _cam;
    void Start()
    {
        _cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    Unit unit = hit.collider.gameObject.GetComponent<Unit>();
                }
            }
        }

    }
}
