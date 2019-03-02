using System.Collections.Generic;
using UnityEngine;

namespace Cytus2
{
    public class GridView : MonoBehaviour
    {
        public static GridView instance { get; private set; }
        public static GameObject prefab { get; set; }

        public GridViewAnchor anchor { get; private set; }
        public Transform beatingResultContainer { get; private set; }
        public Vector3 scanLinePosition => _scanLine.transform.position;
        public int scanLineDirection { get; private set; }
        public Grid grid => _grid;

        private GameObject _scanLinePrefab;

        private AudioSource _audioSource;

        private Transform _noteContainer;
        public int cellSize => _cellSize;
        public bool playing => _playing;

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

        public static GridView NewInstance()
        {
            if (instance != null)
            {
                GameObject.Destroy(instance.gameObject);
            }
            var gridView = GameObject.Instantiate(prefab, GameObject.Find("Canvas").transform, false).GetComponent<GridView>();
            instance = gridView;
            return gridView;
        }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();

            //todo: Move ScanLine into ResourceManager
            _scanLinePrefab = Resources.Load<GameObject>("Prefabs/ScanLine");
            _topLine = GameObject.Instantiate(_scanLinePrefab, transform, false);
            _bottomLine = GameObject.Instantiate(_scanLinePrefab, transform, false);
            _scanLine = GameObject.Instantiate(_scanLinePrefab, transform, false);

            _noteContainer = new GameObject("NoteContainer").transform;
            _noteContainer.SetParent(transform, false);
            beatingResultContainer = new GameObject("BeatingResultContainer").transform;
            beatingResultContainer.SetParent(transform, false);
        }

        public void Initialize(SongData songData, int chartIndex)
        {
            ChartData chartData = songData.charts[chartIndex];
            float beatLength = 60f / songData.bpm;
            turnLength = beatLength * songData.beatUnit / chartData.beatUnit;
            _stepLength = turnLength / 16f;
            _turnTimeOffset = turnLength - beatLength * 0.125f;
            _audioSource.clip = songData.audioClip;
            _songData = songData;
            _chartData = chartData;
            ResetState();
        }

        private void ResetState()
        {
            _grid = new Grid(_chartData, 2 / (_songData.beatUnit / _chartData.beatUnit));
            _grid.onAddNote += HandleGridAddNote;
            _grid.onRemoveNote += HandleGridRemoveNote;
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
            NoteView.pool.DespawnAllEntities();
            ShortTapRhythmView.pool.DespawnAllEntities();
            MediumTapRhythmView.pool.DespawnAllEntities();
            LongTapRhythmView.pool.DespawnAllEntities();
            ShakeRhythmView.pool.DespawnAllEntities();
            WaveRhythmView.pool.DespawnAllEntities();
            BeatingResultView.pool.DespawnAllEntities();
        }

        public void StartGame()
        {
            if (_playing == false)
            {
                if (Time.time + _songData.timeOffset >= _audioSource.clip.length)
                {
                    ResetState();
                }
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

        private void UpsideDown()
        {
            var temp = _top;
            _top = _bottom;
            _bottom = temp;
            scanLineDirection *= -1;
        }

        private void HandleGridAddNote(Note note)
        {
            NoteView noteView = NoteView.pool.SpawnEntity(_noteContainer, false);
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