//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class DragAndDropMove : MonoBehaviour
//{
//    private Camera _cam;
//    private bool _isDragging = false;
//    void Start()
//    {
//        _cam = Camera.main;

//    }
    
//    void Update()
//    {
//        if (_isDragging)
//        {
//            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
//            Plane plane = new Plane(Vector3.up, Vector3.zero);

//            if (plane.Raycast(ray, out float enter))
//            {
//                Vector3 hitPoint = ray.GetPoint(enter);
//                transform.position = hitPoint;
//            }

//        }
//    }
//}
