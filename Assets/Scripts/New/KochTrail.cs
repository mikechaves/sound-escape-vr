using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KochTrail : KochGenerator
{
    public class TrailObject
    {
        public GameObject GO { get; set; }
        public TrailRenderer Trail { get; set; }
        public int CurrentTargetNum { get; set; }
        public Vector3 TargetPosition { get; set; }
        public Color EmmisionColor { get; set; }
    }

    [HideInInspector]
    public List<TrailObject> _trail;

    [Header("Trail Properties")]
    public GameObject _trailPrefab;
    public AnimationCurve _trailWidthCurve;
    [Range(0,8)]
    public int _trailEndCapVertices;
    public Material _trailMaterial;
    public Gradient _trailColor;

    [Header("Audio")]
    public SampleManager _audioPeer;
    public int[] _audioBand;
    public Vector2 _speedMinMax, _widthMinMax, _trailTimeMinMax;
    public float _colorMultiplier;

    //Private Variables
    private float _lerpPosSpeed;
    private float _distanceSnap;
    private Color _startColor, _endColor;

    // Start is called before the first frame update
    void Start()
    {
        _startColor = new Color(0, 0, 0, 0);
        _endColor = new Color(0, 0, 0, 1);

        _trail = new List<TrailObject>();

        for (int i = 0; i < _initiatorPointAmount; i++)
        {
            GameObject trailInstance = (GameObject)Instantiate(_trailPrefab, transform.position, Quaternion.identity, this.transform);
            TrailObject trailObjectInstance = new TrailObject();
            trailObjectInstance.GO = trailInstance;
            trailObjectInstance.Trail = trailInstance.GetComponent<TrailRenderer>();
            trailObjectInstance.Trail.material = new Material(_trailMaterial);
            trailObjectInstance.EmmisionColor = _trailColor.Evaluate(i * (1.0f / _initiatorPointAmount));
            trailObjectInstance.Trail.numCapVertices = _trailEndCapVertices;
            trailObjectInstance.Trail.widthCurve = _trailWidthCurve;

            Vector3 instantiatePosition;

            if (_generationCount > 0)
            {
                int step;
                if (_useBezierCurves)
                {
                    step = _bezierPosition.Length / _initiatorPointAmount;
                    instantiatePosition = _bezierPosition[i * step];
                    trailObjectInstance.CurrentTargetNum = (i * step) + 1;
                    trailObjectInstance.TargetPosition = _bezierPosition[trailObjectInstance.CurrentTargetNum];
                }
                else
                {
                    step = _position.Length / _initiatorPointAmount;
                    instantiatePosition = _position[i * step];
                    trailObjectInstance.CurrentTargetNum = (i * step) + 1;
                    trailObjectInstance.TargetPosition = _position[trailObjectInstance.CurrentTargetNum];
                }
            }
            else
            {
                instantiatePosition = _position[i];
                trailObjectInstance.CurrentTargetNum = i + 1;
                trailObjectInstance.TargetPosition = _position[trailObjectInstance.CurrentTargetNum];
            }

            trailObjectInstance.GO.transform.localPosition = instantiatePosition;
            _trail.Add(trailObjectInstance);
        }
    }

    void Movement()
    {
        _lerpPosSpeed = Mathf.Lerp(_speedMinMax.x, _speedMinMax.y, SampleManager._Amplitude);
        for (int i = 0; i < _trail.Count; i++)
        {
        
            _distanceSnap = Vector3.Distance(_trail[i].GO.transform.localPosition, _trail[i].TargetPosition);

            if (_distanceSnap < 0.05f)
            {
                _trail[i].GO.transform.localPosition = _trail[i].TargetPosition;
                if (_useBezierCurves && _generationCount > 0)
                {
                    if (_trail[i].CurrentTargetNum < _bezierPosition.Length - 1)
                    {
                        _trail[i].CurrentTargetNum += 1;
                    }
                    else
                    {
                        _trail[i].CurrentTargetNum = 1;
                    }
                    _trail[i].TargetPosition = _bezierPosition[_trail[i].CurrentTargetNum];
                }
                else
                {
                    if (_trail[i].CurrentTargetNum < _position.Length - 1)
                    {
                        _trail[i].CurrentTargetNum += 1;
                    }
                    else
                    {
                        _trail[i].CurrentTargetNum = 1;
                    }
                    _trail[i].TargetPosition = _targetPosition[_trail[i].CurrentTargetNum];
                }
            }
            _trail[i].GO.transform.localPosition = Vector3.MoveTowards(_trail[i].GO.transform.localPosition, _trail[i].TargetPosition, Time.deltaTime * _lerpPosSpeed);
        }
    }

    void AudioBehavior()
    {
        for (int i = 0; i < _initiatorPointAmount; i++)
        {
            Color colorLerp = Color.Lerp(_startColor, _trail[i].EmmisionColor * _colorMultiplier, SampleManager._audioBand[_audioBand[i]]);
            _trail[i].Trail.material.SetColor("_EmissionColor", colorLerp);
            colorLerp = Color.Lerp(_startColor, _endColor, SampleManager._audioBand[_audioBand[i]]);
            _trail[i].Trail.material.SetColor("_Color", colorLerp);
            _trail[i].Trail.material.EnableKeyword("_EMISSION");

            float widthLerp = Mathf.Lerp(_widthMinMax.x, _widthMinMax.y, SampleManager._audioBandBuffer[_audioBand[i]]);
            _trail[i].Trail.widthMultiplier = widthLerp;

            float timeLerp = Mathf.Lerp(_trailTimeMinMax.x, _trailTimeMinMax.y, SampleManager._audioBandBuffer[_audioBand[i]]);
            _trail[i].Trail.time = timeLerp;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        AudioBehavior();
    }
}
