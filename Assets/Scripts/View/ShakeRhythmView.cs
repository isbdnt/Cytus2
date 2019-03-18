using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Cytus2
{
    public class ShakeRhythmView : MonoBehaviour, IRhythmView, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
    {
        public static GameObjectPool<ShakeRhythmView> pool { get; } = new GameObjectPool<ShakeRhythmView>();

        public event Action<IRhythmView> onDestroy;

        public Rhythm rhythm { get; private set; }

        public bool testBeat;
        private Animator _animator;
        private Image _tempo;
        private GameObject _icon;
        private Rhythm _currentRhythm;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _icon = transform.Find("Icon").gameObject;
            _tempo = transform.Find("Tempo").GetComponent<Image>();
        }

        public void Initialize(NoteView noteView, Rhythm rhythm)
        {
            this.rhythm = rhythm;
            transform.SetAsFirstSibling();
            transform.position = GridView.instance.anchor.ToWorldPosition(rhythm.position);

            if (this.rhythm.previous == null)
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
            _animator.Play("ShakeRhythm", -1, 0);

            if (this.rhythm.next == null)
            {
                _tempo.gameObject.SetActive(false);
            }
            else
            {
                _tempo.gameObject.SetActive(true);
                _tempo.transform.position = GridView.instance.anchor.ToWorldPosition(this.rhythm.position);
                Vector3 dir = this.rhythm.next.position - this.rhythm.position;
                _tempo.transform.rotation = Quaternion.LookRotation(Vector3.forward, dir);
                _tempo.rectTransform.sizeDelta = new Vector2(25f, GridView.instance.cellSize * dir.magnitude);
                _tempo.fillAmount = 0f;
                _tempo.fillOrigin = 0;
            }
            onDestroy = delegate { };
            _currentRhythm = null;
            _icon.SetActive(true);
        }

        public void ShowBeatingResult()
        {
            BeatingResultView beatingResultView = GridUtility.SpawnBeatingResultView(rhythm.beatingResult);
            beatingResultView.Initialize(GridView.instance.anchor.ToWorldPosition(rhythm.position));
            if (rhythm.previous != null)
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
                rhythm.BeatTime();
            }
        }

        public void Render(float currentStep)
        {
            if (_tempo.gameObject.activeSelf)
            {
                if (currentStep < rhythm.stepOffset + 16)
                {
                    _tempo.fillAmount = (currentStep - rhythm.stepOffset) / rhythm.tempo;
                }
                else
                {
                    if (_tempo.fillOrigin == 0)
                    {
                        _tempo.fillOrigin = 1;
                    }
                    _tempo.fillAmount = 1f - (currentStep - rhythm.stepOffset - 16f) / rhythm.tempo;
                }
            }

            if (rhythm.previous == null)
            {
                if (_currentRhythm == null)
                {
                    if (currentStep >= rhythm.stepOffset + 16f)
                    {
                        _currentRhythm = rhythm;
                        _animator.enabled = false;
                    }
                }

                if (_currentRhythm != null && _currentRhythm.next != null)
                {
                    if (currentStep >= _currentRhythm.stepOffset + 16f + _currentRhythm.tempo)
                    {
                        _currentRhythm = _currentRhythm.next;
                        transform.SetSiblingIndex(transform.GetSiblingIndex() - 1);
                    }
                    if (_currentRhythm.next != null)
                    {
                        _icon.transform.position = GridView.instance.anchor.ToWorldPosition(Vector2.Lerp(_currentRhythm.position, _currentRhythm.next.position, (currentStep - _currentRhythm.stepOffset - 16f) / _currentRhythm.tempo));
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
            if (rhythm.previous == null)
            {
                if (_currentRhythm != null && _currentRhythm.next == null && _currentRhythm.note.grid.currentStep >= _currentRhythm.stepOffset + 16f + _currentRhythm.tempo && rhythm.beatingResult != BeatingResultType.Unknown)
                {
                    Destroy();
                }
            }
            else
            {
                if (rhythm.note.grid.currentStep >= rhythm.stepOffset + 16 + rhythm.tempo && rhythm.beatingResult != BeatingResultType.Unknown)
                {
                    Destroy();
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            rhythm.BeatTime();
        }

        public void OnPointerUp(PointerEventData eventData)
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

        public void OnPointerExit(PointerEventData eventData)
        {
            rhythm.StopBeating();
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