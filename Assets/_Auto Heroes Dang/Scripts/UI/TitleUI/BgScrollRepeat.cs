using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgScrollRepeat : MonoBehaviour
{
    [System.Serializable]
    public class LayerData
    {
        public List<Transform> segments = new List<Transform>();
        public float speedRatio = 1f;

        [HideInInspector] public float segmentWidth;
        [HideInInspector] public float halfWidth;
    }

    [SerializeField] private float _speed = 2.5f;
    [SerializeField] private Transform _cameraTr;

    [SerializeField] private float _fallbackSpeed = 2.0f;

    [SerializeField] private List<LayerData> _layers = new List<LayerData>();

    [SerializeField] private float _extraBuffer = 1.0f;
    [SerializeField] private float _overlap = 0.1f;

    private Camera _cam;
    
    private void Awake()
    {
        if (_cameraTr == null && Camera.main != null)  _cameraTr = Camera.main.transform;
        if (_cameraTr != null) _cam = _cameraTr.GetComponent<Camera>();
        if (_cam == null && Camera.main != null)  _cam = Camera.main;

        if (_layers == null || _layers.Count == 0)
        {
            enabled = false;
            return;
        }

        for (int i = 0; i < _layers.Count; i++)
        {
            LayerData layer = _layers[i];

            if (layer.segments == null || layer.segments.Count == 0)
                continue;

            layer.segmentWidth = MeasureSegmentWidth(layer.segments[0]);
            layer.halfWidth = layer.segmentWidth * 0.5f;

            if (layer.segmentWidth <= 0.0001f)
            {
                enabled = false;
                return;
            }
        }

        if (_overlap < 0f) _overlap = 0f;

        for (int i = 0; i < _layers.Count; i++)
        {
            LayerData layer = _layers[i];
            float startX = layer.segments[0].position.x;

            for (int j = 0; j < layer.segments.Count; j++)
            {
                Transform seg = layer.segments[j];

                Vector3 p = seg.position;
                p.x = startX + ((layer.segmentWidth - _overlap) * j);
                seg.position = p;
            }
        }


    }

    void Update()
    {
        for (int i = 0; i < _layers.Count; i++)
        {
            LayerData layer = _layers[i];

            float speed = _speed * layer.speedRatio;
            float dx = speed * Time.deltaTime;

            for (int j = 0; j < layer.segments.Count; j++)
            {
                Transform seg = layer.segments[j];
                if (seg == null) continue;

                Vector3 p = seg.position;
                p.x -= dx;
                seg.position = p;
            }

            TickLoop(layer);
        }
    }

    private float MeasureSegmentWidth(Transform seg)
    {
        SpriteRenderer sr = seg.GetComponent<SpriteRenderer>();

        if (sr != null) return sr.bounds.size.x;

        return 0f;
    }

    private float GetCameraLeftEdgeX()
    {
        if (_cam == null)
        {
            float camX = (_cameraTr != null) ? _cameraTr.position.x : 0f;
            return camX - 10f;
        }

        float z = Mathf.Abs(_cam.transform.position.z - transform.position.z);
        Vector3 w = _cam.ViewportToWorldPoint(new Vector3(0f, 0.5f, z));

        return w.x;
    }

    private void TickLoop(LayerData layer)
    {
        float camLeftX = GetCameraLeftEdgeX();
        float leftLimit = camLeftX - _extraBuffer;

        float rightMost = float.NegativeInfinity;

        for (int i = 0; i < layer.segments.Count; i++)
        {
            Transform seg = layer.segments[i];

            if (seg == null) continue;

            float rightEdge = seg.position.x + layer.halfWidth;
            if (rightEdge > rightMost) rightMost = rightEdge;
        }

        for (int i = 0; i < layer.segments.Count; i++)
        {
            Transform seg = layer.segments[i];
            if (seg == null) continue;

            float rightEdge = seg.position.x + layer.halfWidth;

            if (rightEdge < leftLimit)
            {
                Vector3 p = seg.position;
                p.x = rightMost + layer.halfWidth - _overlap;
                seg.position = p;

                rightMost = p.x + layer.halfWidth;
            }
        }
    }

}