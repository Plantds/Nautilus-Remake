using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

public class CS_Sonar : CS_ElctricalBaseScript
{
    [Header("Refs")]
    [Tooltip("Where the rays will come out of !! NOT THE BEAM, IT'S AN EMPTY!!")]
    [SerializeField] private GameObject _sonarFunc;
    [Tooltip("The object that will spawn where a hit was detected")]
    [SerializeField] private GameObject _dot;
    [Tooltip("The visual beam")]
    [SerializeField] private GameObject _beam;
    [Tooltip("The camera that that renders nothing but the sonar")]
    [SerializeField] private Camera _sonarCamera;
    [Tooltip("where the dots will be childed to")]
    [SerializeField] private GameObject _visual;
    [Tooltip("the sub the sonar is a part of")]
    [SerializeField] private GameObject _subAndPlayerSetUp;
    [Tooltip("bockling plane for turn off")]
    [SerializeField] private GameObject _blockScreen;

    [SerializeField] private Material _SonarBlitMaterial;
    [SerializeField] private RenderTexture _SonarBuffer;
    [SerializeField] private RenderTexture _SonarScan;

    [SerializeField] private SOB_Sonar _settings;

    [Header("Parking Zone")]
    [Tooltip("Empty transform where the parking zone is in the world")]
    [SerializeField] private Transform _parkingZoneTarget;

    [Tooltip("Dot used to show parking zone direction on the sonar")]
    [SerializeField] private GameObject _parkingSpoitPrefab;

    [Tooltip("Scales how far from the sub the parking dot can appear on the sonar (1 = full sonar lengthOfRay)")]
    [Range(0f, 1.5f)]
    [SerializeField] private float _parkingZoneDistanceMultiplier = 0.8f;

    [SerializeField] private Material _m;

    [Min(0.0f)]
    [SerializeField] private float _minFlickerTime = 0.4f;
    [Min(0.0f)]
    [SerializeField] private float _maxFlickerTime = 1.2f;

    private GameObject _parkingSpoitInstance;

    private float[] horizontalDegrees = { };
    private float[] verticalDegrees = { };

    private float timeBetweenRays = 0.0f;

    private int horizontalFullLapCount = 0;

    private float horizontalFireAtDegree = 0.0f;
    private float horizontalStartDegree = 0.0f;

    private float verticalFireAtDegree = 0.0f;
    private float verticalStartDegree = 0.0f;

    private float lastDegree = 0.0f;
    private float targetDegeree = 0.0f;

    private float beamLerpTime = 0.0f;
    private float beamCompanstion = 0.0f;

    [NonSerialized] private StoredSettings storedSettings;
    [NonSerialized] private QueryParameters queryParameters;

    private float _flickerTimer = 0.0f;
    private bool _isFlickering = false;
    private float _timeDelay = 0.0f;
    private bool _active = false;

    private struct StoredSettings
    {
        public int horizontalRayCount;
        public int verticalRayCount;
    }

    [SerializeField] private bool isEnabled = true;

    NativeArray<RaycastCommand> commands;
    NativeArray<RaycastHit> results;

    Vector3[] pointsToBatchAt;

    Dictionary<int, Dictionary<int, Vector3>> lookUpTable;

    private SortedDictionary<float, Vector3> sortedPoints = new SortedDictionary<float, Vector3>();

    private ObjectPool<GameObject> pool;

    bool fireNewRays = false;
    float timer = 0.0f;

    #region Start Functions
    private void Start()
    {
        SetSettings();
        SetQueryParameters();

        CalculateDegrees();
        CallculateTimeBetweenRays();
        getStartDegrees();

        commands = new NativeArray<RaycastCommand>(storedSettings.verticalRayCount, Allocator.Persistent);
        results = new NativeArray<RaycastHit>(storedSettings.verticalRayCount, Allocator.Persistent);

        CreateLookUpTable();

        pointsToBatchAt = new Vector3[20];

        beamCompanstion = 1 / timeBetweenRays;

        pool = new ObjectPool<GameObject>(
            createFunc: CreateItem,
            actionOnGet: OnGet,
            actionOnRelease: OnRelease,
            actionOnDestroy: OnDestroyItem,
            collectionCheck: true,
            defaultCapacity: 50,
            maxSize: 100
        );
    }

    private void SetSettings()
    {
        storedSettings.horizontalRayCount = _settings.amountOfRaysInHorizontal;
        storedSettings.verticalRayCount = _settings.amountOfRaysInVertical;
    }

    private void SetQueryParameters()
    {
        queryParameters.layerMask = _settings.hitLayer;
        queryParameters.hitBackfaces = false;
        queryParameters.hitMultipleFaces = false;
        queryParameters.hitTriggers = QueryTriggerInteraction.UseGlobal;
    }

    private void CreateLookUpTable()
    {
        lookUpTable = new Dictionary<int, Dictionary<int, Vector3>>();
        for (int x = 0; x < _settings.amountOfRaysInHorizontal; x++)
        {
            Dictionary<int, Vector3> pairs = new Dictionary<int, Vector3>();
            for (int y = 0; y < _settings.amountOfRaysInVertical; y++)
            {
                pairs.Add(y, Quaternion.Euler(new Vector3(verticalDegrees[y], horizontalDegrees[x], 0.0f)) * Vector3.forward);
            }
            lookUpTable.Add(x, pairs);
        }
    }
    #endregion

    #region Destroy Functions
    void OnDestroy()
    {
        commands.Dispose();
        results.Dispose();
    }
    #endregion

    private void updateOnAndOff()
    {
        _flickerTimer -= Time.deltaTime;
        if (_active)
        {
            if (_isFlickering == false && _flickerTimer > 0.0f)
            {
                StartCoroutine(FlickerOff());
            }
        }
        else
        {
            if (_isFlickering == false && _flickerTimer > 0.0f)
            {
                StartCoroutine(FlickerOn());
            }
        }
    }

    public override void TurnOn(bool flash)
    {
        if (flash)
        {
            _flickerTimer = UnityEngine.Random.Range(_minFlickerTime, _maxFlickerTime);
            _active = false;
        }
        else
        {
            _m.SetFloat("_ON_OFF", 1.0f);
            _blockScreen.SetActive(false);
            isEnabled = true;
        }
    }

    public override void TurnOff(bool flash)
    {
        if (flash)
        {
            _active = true;
            _flickerTimer = UnityEngine.Random.Range(_minFlickerTime, _maxFlickerTime);
        }
        else
        {
            _m.SetFloat("_ON_OFF", 0.0f);
            _blockScreen.SetActive(true);
            isEnabled = false;
        }
    }

    private IEnumerator FlickerOff()
    {
        _isFlickering = true;
        _m.SetFloat("_ON_OFF", 1.0f);
        _blockScreen.SetActive(false);
        isEnabled = true;
        _timeDelay = UnityEngine.Random.Range(0.01f, 0.2f);
        yield return new WaitForSeconds(_timeDelay);
        _m.SetFloat("_ON_OFF", 0.0f);
        _blockScreen.SetActive(true);
        isEnabled = false;
        _timeDelay = UnityEngine.Random.Range(0.01f, 0.2f);
        yield return new WaitForSeconds(_timeDelay);
        _isFlickering = false;
    }
    private IEnumerator FlickerOn()
    {
        _isFlickering = true;
        _m.SetFloat("_ON_OFF", 0.0f);
        _blockScreen.SetActive(true);
        isEnabled = false;
        _timeDelay = UnityEngine.Random.Range(0.01f, 0.2f);
        yield return new WaitForSeconds(_timeDelay);
        _m.SetFloat("_ON_OFF", 1.0f);
        _blockScreen.SetActive(false);
        isEnabled = true;
        _timeDelay = UnityEngine.Random.Range(0.01f, 0.2f);
        yield return new WaitForSeconds(_timeDelay);
        _isFlickering = false;
    }

    // -- Start --
    private void CalculateDegrees()
    {
        horizontalFireAtDegree = 360.0f / storedSettings.horizontalRayCount;
        horizontalStartDegree = _sonarFunc.transform.rotation.eulerAngles.y;
        horizontalDegrees = new float[storedSettings.horizontalRayCount];

        for (int i = 0; i < storedSettings.horizontalRayCount; i++)
        {
            horizontalDegrees[i] = horizontalStartDegree + (horizontalFireAtDegree * i);
        }

        verticalFireAtDegree = _settings.maxDegreeInVertical / _settings.amountOfRaysInVertical;
        verticalStartDegree = _sonarFunc.transform.rotation.eulerAngles.x;
        verticalStartDegree -= _settings.amountOfRaysInVertical / 2;
        verticalDegrees = new float[_settings.amountOfRaysInVertical];

        for (int i = 0; i < _settings.amountOfRaysInVertical; i++)
        {
            verticalDegrees[i] = verticalStartDegree + (verticalFireAtDegree * i);
        }
    }

    private void CallculateTimeBetweenRays()
    {
        timeBetweenRays = _settings.lapTime / _settings.amountOfRaysInHorizontal;
    }

    private void getStartDegrees()
    {
        float a = 360.0f - _subAndPlayerSetUp.transform.rotation.eulerAngles.y;
        lastDegree = horizontalFireAtDegree * _settings.amountOfRaysInHorizontal - a;
        targetDegeree = horizontalFireAtDegree * _settings.amountOfRaysInHorizontal - a - 1;
    }

    quaternion EularToQuternion(float x, float y, float z)
    {
        return UnityEngine.Quaternion.Euler(x, y, z);
    }

    private void ChangeBeamTarget()
    {
        lastDegree = targetDegeree;
        targetDegeree += horizontalFireAtDegree;
        beamLerpTime = 0.0f;
    }

    //PARKING ZONE MARKERR
    /*
    private void UpdateParkingZoneMarker()
    {
        // No parking zone or sonar disabled check
        if (_parkingZoneTarget == null || !_settings._enabled)
            return;

        //Create the parking 
        if (_parkingSpoitInstance == null && _parkingSpoitPrefab != null)
        {
            _parkingSpoitInstance = Instantiate(
                _parkingSpoitPrefab,
                _sonarFunc.transform.position,
                Quaternion.identity,
                _visual.transform   //sonar visual
            );
        }

        if (_parkingSpoitInstance == null)
            return;

        _parkingSpoitInstance.SetActive(true);

        // Direction from sonar origin to parking zone target horizontal only
        Vector3 dir = _parkingZoneTarget.position - _sonarFunc.transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.001f)
            return;

        dir.Normalize();

        //dynamic distance moves towards you when close 
        float worldDistance = Vector3.Distance(_parkingZoneTarget.position, _sonarFunc.transform.position);

        // Max distance show on the sonar (edge)
        float maxOnSonar = _settings.lengthOfRay * _parkingZoneDistanceMultiplier;

        // Use actual distance while is <= maxOnSonar, and clamp to edge when further
        float usedDistance = Mathf.Min(worldDistance, maxOnSonar);

        Vector3 markerPos = _sonarFunc.transform.position + dir * usedDistance;

        _parkingSpoitInstance.transform.position = markerPos;
    }
    */

    private void Update()
    {
        updateOnAndOff();

        if (!isEnabled)
            return;

        UpdateBeam();

        if (fireNewRays)
        {
            fireNewRays = false;
            CastBatchedRays();
            BatchInstantiate();
        }

        //UpdateParkingZoneMarker();

        _sonarCamera.Render();
        _SonarBlitMaterial.SetTexture("_SonarTex", _sonarCamera.activeTexture);
    }

    #region The Kjell Region

    #region Beam
    private void UpdateBeam()
    {
        timer += Time.deltaTime;
        if (timer >= timeBetweenRays)
        {
            fireNewRays = true;
            timer = 0.0f;
        }

        _beam.transform.localRotation = EularToQuternion(
            _beam.transform.localEulerAngles.x,
            Mathf.Lerp(
              a: lastDegree,
              b: targetDegeree,
              t: timer),
            _beam.transform.localEulerAngles.z);
    }
    #endregion

    #region Hyper-Optimized Functions
    private void CastBatchedRays()
    {
        Vector3 origin = _sonarFunc.transform.position;
        float parentYOffset = transform.rotation.eulerAngles.y;

        sortedPoints.Clear();
        for (int o = 0; o < storedSettings.verticalRayCount; o++)
        {
            commands[o] = new RaycastCommand(origin,
                Quaternion.Euler(0.0f, parentYOffset, 0.0f) *
                lookUpTable[(horizontalFullLapCount) % _settings.amountOfRaysInHorizontal][o],
                queryParameters,
                _settings.lengthOfRay);
        }

        JobHandle handle = RaycastCommand.ScheduleBatch(commands, results, 1, default);
        handle.Complete();

        for (int o = 0; o < storedSettings.verticalRayCount; o++)
        {
            if (results[o].collider)
            {
                sortedPoints.TryAdd(results[o].distance, results[o].point);
            }
        }

        ChangeBeamTarget();

        horizontalFullLapCount++;

        if (sortedPoints.Count > 0)
        {
            pointsToBatchAt[0] = sortedPoints.First().Value;
        }
    }

    //Use Object pooling instead
    void BatchInstantiate()
    {
        GameObject instance = pool.Get();
        instance.transform.position = pointsToBatchAt[0];
        instance.transform.parent = _visual.transform;

        StartCoroutine(ReturnAfter(instance, 0.8f));
    }
    #endregion

    #region Public Functions
    public void SetEnabled(bool _enabled)
    {
        isEnabled = _enabled;

        /*
        if (_parkingSpoitInstance != null)
            _parkingSpoitInstance.SetActive(_enabled);
        */
    }
    #endregion

    #region Object Pool Functions
    private GameObject CreateItem()
    {
        GameObject instance = Instantiate(_dot, Vector3.zero, EularToQuternion(0, 0, 0));
        instance.SetActive(false);
        return instance;
    }

    private void OnGet(GameObject gameObject)
    {
        gameObject.SetActive(true);
    }

    private void OnRelease(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }

    private void OnDestroyItem(GameObject gameObject)
    {
        Destroy(gameObject);
    }

    private IEnumerator ReturnAfter(GameObject gameObject, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        pool.Release(gameObject);
    }
    #endregion
}

public struct BatchedRaysJob : IJobParallelFor
{
    public void Execute(int index)
    {
        throw new NotImplementedException();
    }
}
#endregion
