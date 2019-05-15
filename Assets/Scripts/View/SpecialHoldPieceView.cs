using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cytus2
{
    public class SpecialHoldPieceView : MonoBehaviour, IPieceView, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
    {
        public static GameObjectPool<SpecialHoldPieceView> pool { get; } = new GameObjectPool<SpecialHoldPieceView>();

        public event Action<IPieceView> onDestroy;

        public Piece piece { get; private set; }

        public bool testBeat;
        private Transform _tempTransform;
        private CanvasGroup _tempCanvas;
        private Transform _tempoFillTransform;
        private Animator _animator;
        private CanvasGroup _icon;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _tempoFillTransform = transform.Find("TempoFill").transform;
            _tempTransform = transform.Find("Tempo").transform;
            _tempCanvas = _tempTransform.GetComponent<CanvasGroup>();
            _icon = transform.Find("Icon").GetComponent<CanvasGroup>();
        }

        public void Initialize(NoteView noteView, Piece piece)
        {
            this.piece = piece;

            transform.position = GridView.instance.anchor.ToWorldPosition(piece.position);

            _animator.speed = 1f / (GridView.instance.turnLength * 1.25f);
            _animator.enabled = true;
            _animator.Play("SpecialHoldPiece", -1, 0);

            _tempTransform.position = GridView.instance.anchor.ToWorldPosition(new Vector2(piece.position.x, 4f));
            onDestroy = delegate { };
            _tempoFillTransform.gameObject.SetActive(false);
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
                    _tempoFillTransform.gameObject.SetActive(true);
                }

                Vector3 pos = _tempoFillTransform.position;
                pos.y = GridView.instance.scanLinePosition.y;
                _tempoFillTransform.position = pos;
                if (GridView.instance.scanLineDirection > 0)
                {
                    _tempoFillTransform.eulerAngles = new Vector3(0f, 0f, 180f);
                }
                else
                {
                    _tempoFillTransform.eulerAngles = new Vector3(0f, 0f, 0f);
                }
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