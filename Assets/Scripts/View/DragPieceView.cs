using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Cytus2
{
    public class DragPieceView : MonoBehaviour, IPieceView, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
    {
        public static GameObjectPool<DragPieceView> pool { get; } = new GameObjectPool<DragPieceView>();

        public event Action<IPieceView> onDestroy;

        public Piece piece { get; private set; }

        public bool testBeat;
        private Animator _animator;
        private Image _tempo;
        private GameObject _icon;
        private Piece _currentPiece;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _icon = transform.Find("Icon").gameObject;
            _tempo = transform.Find("Tempo").GetComponent<Image>();
        }

        public void Initialize(NoteView noteView, Piece piece)
        {
            this.piece = piece;
            transform.SetAsFirstSibling();
            transform.position = GridView.instance.anchor.ToWorldPosition(piece.position);

            if (this.piece.previous == null)
            {
                _icon.transform.localScale = new Vector3(2f, 2f, 1f);
            }
            else
            {
                _icon.transform.localScale = Vector3.one;
            }
            _icon.transform.localPosition = Vector3.zero;
            _animator.speed = 1f / (GridView.instance.turnLength * 1.25f);
            _animator.enabled = true;
            _animator.Play("DragPiece", -1, 0);

            if (this.piece.next == null)
            {
                _tempo.gameObject.SetActive(false);
            }
            else
            {
                _tempo.gameObject.SetActive(true);
                _tempo.transform.position = GridView.instance.anchor.ToWorldPosition(this.piece.position);
                Vector3 dir = this.piece.next.position - this.piece.position;
                if (GridView.instance.anchor.direction < 0)
                {
                    dir.y = -dir.y;
                }
                _tempo.transform.rotation = Quaternion.LookRotation(Vector3.forward, dir);
                _tempo.rectTransform.sizeDelta = new Vector2(25f, GridView.instance.cellSize * dir.magnitude);
                _tempo.fillAmount = 0f;
                _tempo.fillOrigin = 0;
            }
            onDestroy = delegate { };
            _currentPiece = null;
            _icon.SetActive(true);
        }

        public void ShowBeatingResult()
        {
            BeatingResultView beatingResultView = GridUtility.SpawnBeatingResultView(piece.beatingResult);
            beatingResultView.Initialize(GridView.instance.anchor.ToWorldPosition(piece.position));
            if (piece.previous != null)
            {
                if (_tempo.gameObject.activeSelf)
                {
                    _icon.SetActive(false);
                }
                else
                {
                    Destroy();
                }
            }
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
            if (_tempo.gameObject.activeSelf)
            {
                if (currentStep < piece.stepOffset + 16)
                {
                    _tempo.fillAmount = (currentStep - piece.stepOffset) / piece.tempo;
                }
                else
                {
                    if (_tempo.fillOrigin == 0)
                    {
                        _tempo.fillOrigin = 1;
                    }
                    _tempo.fillAmount = 1f - (currentStep - piece.stepOffset - 16f) / piece.tempo;
                }
            }

            if (piece.previous == null)
            {
                if (_currentPiece == null)
                {
                    if (currentStep >= piece.stepOffset + 16f)
                    {
                        _currentPiece = piece;
                        _animator.enabled = false;
                    }
                }

                if (_currentPiece != null && _currentPiece.next != null)
                {
                    if (currentStep >= _currentPiece.stepOffset + 16f + _currentPiece.tempo)
                    {
                        _currentPiece = _currentPiece.next;
                        transform.SetSiblingIndex(transform.GetSiblingIndex() - 1);
                    }
                    if (_currentPiece.next != null)
                    {
                        _icon.transform.position = GridView.instance.anchor.ToWorldPosition(Vector2.Lerp(_currentPiece.position, _currentPiece.next.position, (currentStep - _currentPiece.stepOffset - 16f) / _currentPiece.tempo));
                    }
                    else
                    {
                        _icon.SetActive(false);
                    }
                }
            }
        }

        private void LateUpdate()
        {
            if (piece.previous == null)
            {
                if (_currentPiece != null && _currentPiece.next == null && _currentPiece.note.grid.currentStep >= _currentPiece.stepOffset + 16f + _currentPiece.tempo && piece.beatingResult != BeatingResultType.Unknown)
                {
                    Destroy();
                }
            }
            else
            {
                if (piece.note.grid.currentStep >= piece.stepOffset + 16 + piece.tempo && piece.beatingResult != BeatingResultType.Unknown)
                {
                    Destroy();
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            piece.BeatTime();
        }

        public void OnPointerUp(PointerEventData eventData)
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

        public void OnPointerExit(PointerEventData eventData)
        {
            piece.StopBeating();
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