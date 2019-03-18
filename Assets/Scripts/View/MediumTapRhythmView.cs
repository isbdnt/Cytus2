using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cytus2
{
    public class MediumTapRhythmView : MonoBehaviour, IRhythmView, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
    {
        public static GameObjectPool<MediumTapRhythmView> pool { get; } = new GameObjectPool<MediumTapRhythmView>();

        public event Action<IRhythmView> onDestroy;

        public Rhythm rhythm { get; private set; }

        public bool testBeat;
        private RectTransform _tempTransform;
        private CanvasGroup _tempCanvas;
        private RectTransform _tempoFillTransform;
        private Animator _animator;
        private CanvasGroup _icon;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _tempoFillTransform = transform.Find("TempoFill").GetComponent<RectTransform>();
            _tempTransform = transform.Find("Tempo").GetComponent<RectTransform>();
            _tempCanvas = _tempTransform.GetComponent<CanvasGroup>();
            _icon = transform.Find("Icon").GetComponent<CanvasGroup>();
        }

        public void Initialize(NoteView noteView, Rhythm rhythm)
        {
            this.rhythm = rhythm;

            transform.position = GridView.instance.anchor.ToWorldPosition(rhythm.position);

            _animator.speed = 1f / (GridView.instance.turnLength * 1.25f);
            _animator.enabled = true;
            _animator.Play("MediumTapRhythm", -1, 0);

            _tempoFillTransform.rotation = Quaternion.identity;
            float angleZ = rhythm.note.direction * GridView.instance.anchor.direction > 0 ? 180f : 0f;
            _tempoFillTransform.Rotate(new Vector3(0f, 0f, angleZ));
            _tempoFillTransform.sizeDelta = Vector2.zero;

            _tempTransform.rotation = Quaternion.identity;
            _tempTransform.sizeDelta = new Vector2(GridView.instance.cellSize * 1.7f, rhythm.tempo / 2 * GridView.instance.cellSize);
            _tempTransform.Rotate(new Vector3(0f, 0f, angleZ));
            onDestroy = delegate { };
        }

        public void ShowBeatingResult()
        {
            BeatingResultView beatingResultView = GridUtility.SpawnBeatingResultView(rhythm.beatingResult);
            switch (rhythm.beatingResult)
            {
                case BeatingResultType.Good:
                case BeatingResultType.Perfect:
                case BeatingResultType.Bad:
                    beatingResultView.Initialize(new Vector3(GridView.instance.anchor.ToWorldPosition(rhythm.position).x, GridView.instance.scanLinePosition.y));
                    break;

                case BeatingResultType.Miss:
                    beatingResultView.Initialize(GridView.instance.anchor.ToWorldPosition(rhythm.position));
                    break;

                default:
                    throw new Exception();
            }
            Destroy();
        }

        private void FixedUpdate()
        {
            if (testBeat)
            {
                testBeat = false;
                rhythm.BeatTime();
            }
        }

        public void Render(float currentStep)
        {
            if (rhythm.beatingSteps > 0f)
            {
                if (_animator.enabled)
                {
                    _animator.enabled = false;
                    _icon.alpha = 1f;
                    _tempCanvas.alpha = 1f;
                }
                _tempoFillTransform.sizeDelta = new Vector2(GridView.instance.cellSize * 1.7f, GridView.instance.cellSize * (currentStep - rhythm.stepOffset - 16f) / 2f);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            rhythm.StopBeating();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
#if UNITY_EDITOR
            if (Input.GetMouseButton(0))
            {
                rhythm.BeatTime();
            }
#else
            rhythm.BeatTime();
#endif
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            rhythm.StopBeating();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            rhythm.BeatTime();
        }

        private void Destroy()
        {
            onDestroy(this);
            Despawn();
        }

        public void Despawn()
        {
            pool.DespawnEntity(this);
        }
    }
}