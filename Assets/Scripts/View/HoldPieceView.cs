using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cytus2
{
    public class HoldPieceView : MonoBehaviour, IPieceView, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
    {
        public static GameObjectPool<HoldPieceView> pool { get; } = new GameObjectPool<HoldPieceView>();

        public event Action<IPieceView> onDestroy;

        public Piece piece { get; private set; }

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

        public void Initialize(NoteView noteView, Piece piece)
        {
            this.piece = piece;

            transform.position = GridView.instance.anchor.ToWorldPosition(piece.position);

            _animator.speed = 1f / (GridView.instance.turnLength * 1.25f);
            _animator.enabled = true;
            _animator.Play("HoldPiece", -1, 0);

            _tempoFillTransform.rotation = Quaternion.identity;
            float angleZ = piece.note.direction * GridView.instance.anchor.direction > 0 ? 180f : 0f;
            _tempoFillTransform.Rotate(new Vector3(0f, 0f, angleZ));
            _tempoFillTransform.sizeDelta = Vector2.zero;

            _tempTransform.rotation = Quaternion.identity;
            _tempTransform.sizeDelta = new Vector2(GridView.instance.cellSize * 1.7f, piece.tempo / 2 * GridView.instance.cellSize);
            _tempTransform.Rotate(new Vector3(0f, 0f, angleZ));
            onDestroy = delegate { };
        }

        public void ShowBeatingResult()
        {
            BeatingResultView beatingResultView = GridUtility.SpawnBeatingResultView(piece.beatingResult);
            switch (piece.beatingResult)
            {
                case BeatingResultType.Good:
                case BeatingResultType.Perfect:
                case BeatingResultType.Bad:
                    beatingResultView.Initialize(new Vector3(GridView.instance.anchor.ToWorldPosition(piece.position).x, GridView.instance.scanLinePosition.y));
                    break;

                case BeatingResultType.Miss:
                    beatingResultView.Initialize(GridView.instance.anchor.ToWorldPosition(piece.position));
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
                piece.BeatTime();
            }
        }

        public void Render(float currentStep)
        {
            if (piece.beatingSteps > 0f)
            {
                if (_animator.enabled)
                {
                    _animator.enabled = false;
                    _icon.alpha = 1f;
                    _tempCanvas.alpha = 1f;
                }
                _tempoFillTransform.sizeDelta = new Vector2(GridView.instance.cellSize * 1.7f, GridView.instance.cellSize * (currentStep - piece.stepOffset - 16f) / 2f);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            piece.StopBeating();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
#if UNITY_EDITOR
            if (Input.GetMouseButton(0))
            {
                piece.BeatTime();
            }
#else
            piece.BeatTime();
#endif
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            piece.StopBeating();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            piece.BeatTime();
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