using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cytus2
{
    public class GridView : MonoBehaviour
    {
        public static GridView instance { get; private set; }
        public GameObjectPool<NoteView> noteViewPool { get; private set; }
        public GameObjectPool<IRhythmView> shortTapRhythmViewPool { get; private set; }
        public GameObjectPool<IRhythmView> mediumTapRhythmViewPool { get; private set; }
        public GameObjectPool<IRhythmView> longTapRhythmViewPool { get; private set; }
        public GameObjectPool<IRhythmView> shakeRhythmViewPool { get; private set; }
        public GameObjectPool<IRhythmView> waveRhythmViewPool { get; private set; }
        public GameObjectPool<BeatingResultView> goodBeatingViewPool { get; private set; }
        public GameObjectPool<BeatingResultView> perfectBeatingViewPool { get; private set; }
        public GameObjectPool<BeatingResultView> missBeatingViewPool { get; private set; }
        public GameObjectPool<BeatingResultView> badBeatingViewPool { get; private set; }
        public GridViewAnchor anchor { get; private set; }
        public Transform beatingResultContainer { get; private set; }
        public Vector3 scanLinePosition => _scanLine.transform.position;
        public int scanLineDirection { get; private set; }

        private GameObject _scanLinePrefab;

        private AudioSource _audioSource;

        [SerializeField]
        private Text _comboTextView;

        [SerializeField]
        private Text _pointTextView;

        [SerializeField]
        private Button _startButton;

        [SerializeField]
        private Button _pauseButton;

        private Transform _noteContainer;
        public int cellSize => _cellSize;

        [SerializeField]
        private int _cellSize;

        private Grid _grid;
        private Dictionary<Note, NoteView> _noteViewMap = new Dictionary<Note, NoteView>();
        private float _stepLength;
        public float turnLength { get; private set; }
        private float _turnTimeOffset;
        private GameObject _scanLine;
        private SongData _songData;
        private ChartData _chartData;

        private bool _playing;
        private int _currentTurn;
        private Vector3 _top;
        private Vector3 _bottom;
        private GameObject _topLine;
        private GameObject _bottomLine;
        private float _startTime;

        private void Awake()
        {
            instance = this;
            _audioSource = GetComponent<AudioSource>();
            Time.fixedDeltaTime = 1f / 60f;
            QualitySettings.vSyncCount = 1;
            Application.targetFrameRate = 60;
            _scanLinePrefab = Resources.Load<GameObject>("Prefabs/ScanLine");
            _topLine = GameObject.Instantiate(_scanLinePrefab, transform, false);
            _bottomLine = GameObject.Instantiate(_scanLinePrefab, transform, false);
            _scanLine = GameObject.Instantiate(_scanLinePrefab, transform, false);
            noteViewPool = new GameObjectPool<NoteView>(Resources.Load<GameObject>("Prefabs/Note"));
            shortTapRhythmViewPool = new GameObjectPool<IRhythmView>(Resources.Load<GameObject>("Prefabs/ShortTapRhythm"));
            mediumTapRhythmViewPool = new GameObjectPool<IRhythmView>(Resources.Load<GameObject>("Prefabs/MediumTapRhythm"));
            longTapRhythmViewPool = new GameObjectPool<IRhythmView>(Resources.Load<GameObject>("Prefabs/LongTapRhythm"));
            shakeRhythmViewPool = new GameObjectPool<IRhythmView>(Resources.Load<GameObject>("Prefabs/ShakeRhythm"));
            waveRhythmViewPool = new GameObjectPool<IRhythmView>(Resources.Load<GameObject>("Prefabs/WaveRhythm"));
            goodBeatingViewPool = new GameObjectPool<BeatingResultView>(Resources.Load<GameObject>("Prefabs/GoodBeating"));
            perfectBeatingViewPool = new GameObjectPool<BeatingResultView>(Resources.Load<GameObject>("Prefabs/PerfectBeating"));
            missBeatingViewPool = new GameObjectPool<BeatingResultView>(Resources.Load<GameObject>("Prefabs/MissBeating"));
            badBeatingViewPool = new GameObjectPool<BeatingResultView>(Resources.Load<GameObject>("Prefabs/BadBeating"));

            _noteContainer = new GameObject("NoteContainer").transform;
            _noteContainer.SetParent(transform, false);
            beatingResultContainer = new GameObject("BeatingResultContainer").transform;
            beatingResultContainer.SetParent(transform, false);

            _startButton.gameObject.SetActive(true);
            _startButton.onClick.AddListener(StartGame);
            _pauseButton.gameObject.SetActive(false);
            _pauseButton.onClick.AddListener(PauseGame);

            ReloadConfig();
        }

        public void ReloadConfig()
        {
            var songData = JsonConvert.DeserializeObject<SongData>(Resources.Load<TextAsset>("Songs/OneWayLove/Config").text);
            Initialize(songData, songData.charts[0], Resources.Load<AudioClip>("Songs/OneWayLove/AudioClip"));
        }

        protected virtual void Initialize(SongData songData, ChartData chartData, AudioClip audioClip)
        {
            float beatLength = 60f / songData.bpm;
            turnLength = beatLength * songData.beatUnit / chartData.beatUnit;
            _stepLength = turnLength / 16f;
            _turnTimeOffset = turnLength - beatLength * 0.125f;
            _audioSource.clip = audioClip;
            _songData = songData;
            _chartData = chartData;
            ResetState();
        }

        private void ResetState()
        {
            _grid = new Grid(_chartData, 2 / (_songData.beatUnit / _chartData.beatUnit));
            _grid.onAddNote += HandleGridAddNote;
            _grid.onRemoveNote += HandleGridRemoveNote;
            _grid.onPointChange += HandleGridPointChange;
            _grid.onComboChange += HandleGridComboChange;
            HandleGridPointChange(0);
            HandleGridComboChange(0);
            scanLineDirection = _songData.upsideDown ? 1 : -1;
            anchor = new GridViewAnchor(16, 8, _cellSize, new Vector2Int(8, 0), _songData.upsideDown);
            _top = anchor.ToWorldPosition(new Vector2Int(0, 8));
            _bottom = anchor.ToWorldPosition(new Vector2Int(0, 0));
            _playing = false;
            _scanLine.SetActive(false);
            _scanLine.transform.position = Vector3.Lerp(_top, _bottom, _turnTimeOffset % turnLength / turnLength);
            _currentTurn = 0;
            _topLine.transform.position = _top;
            _bottomLine.transform.position = _bottom;
            _audioSource.time = _songData.timeOffset;
            _noteViewMap.Clear();
            _scanLine.transform.SetAsLastSibling();
            shortTapRhythmViewPool.DespawnAllEntities();
            mediumTapRhythmViewPool.DespawnAllEntities();
            longTapRhythmViewPool.DespawnAllEntities();
            shakeRhythmViewPool.DespawnAllEntities();
            waveRhythmViewPool.DespawnAllEntities();
            goodBeatingViewPool.DespawnAllEntities();
            perfectBeatingViewPool.DespawnAllEntities();
            missBeatingViewPool.DespawnAllEntities();
            badBeatingViewPool.DespawnAllEntities();
        }

        public void StartGame()
        {
            if (_playing == false)
            {
                if (Time.time + _songData.timeOffset >= _audioSource.clip.length)
                {
                    ResetState();
                }
                _startButton.gameObject.SetActive(false);
                _pauseButton.gameObject.SetActive(true);
                _playing = true;
                _scanLine.SetActive(true);
                _audioSource.Play();
                _startTime = Time.time - _audioSource.time;
                Time.timeScale = 1f;
            }
        }

        public void RestartGame()
        {
            ResetState();
            StartGame();
        }

        public void PauseGame()
        {
            if (_playing)
            {
                _startButton.gameObject.SetActive(true);
                _pauseButton.gameObject.SetActive(false);
                _playing = false;
                _audioSource.Pause();
                Time.timeScale = 0f;
            }
        }

        private void FixedUpdate()
        {
            if (_playing)
            {
                float time = Time.time - _startTime - _songData.timeOffset;
                float turnTime = time + _turnTimeOffset;
                float currentStep = time / _stepLength;
                _grid.Update(currentStep);
                int currentTurn = Mathf.FloorToInt(turnTime / turnLength);
                if (currentTurn > _currentTurn)
                {
                    _currentTurn = currentTurn;
                    UpsideDown();
                }
                _scanLine.transform.position = Vector3.Lerp(_top, _bottom, turnTime % turnLength / turnLength);
                foreach (var noteView in _noteViewMap.Values)
                {
                    noteView.Render(currentStep);
                }
                if (time + _songData.timeOffset >= _audioSource.clip.length)
                {
                    PauseGame();
                }
            }
        }

        private void HandleGridPointChange(int point)
        {
            _pointTextView.text = point.ToString();
        }

        private void HandleGridComboChange(int combo)
        {
            _comboTextView.text = combo.ToString();
        }

        private void UpsideDown()
        {
            var temp = _top;
            _top = _bottom;
            _bottom = temp;
            scanLineDirection *= -1;
        }

        private void HandleGridAddNote(Note note)
        {
            NoteView noteView = noteViewPool.SpawnEntity(_noteContainer, false);
            noteView.Initialize(note);
            noteView.onDestroy += HandleNoteViewDestroy;
            _noteViewMap[note] = noteView;
        }

        private void HandleNoteViewDestroy(NoteView noteView)
        {
            _noteViewMap.Remove(noteView.note);
        }

        private void HandleGridRemoveNote(Note note)
        {
        }
    }
}