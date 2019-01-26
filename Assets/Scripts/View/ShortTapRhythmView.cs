using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cytus2
{
    public class ShortTapRhythmView : MonoBehaviour, IRhythmView, IPointerDownHandler
    {
        public event Action<IRhythmView> onDestroy;

        public Rhythm rhythm { get; private set; }

        public bool testBeat;
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void Initialize(NoteView noteView, Rhythm rhythm)
        {
            this.rhythm = rhythm;
            transform.position = GridView.instance.anchor.ToWorldPosition(rhythm.position);

            _animator.speed = 1f / (GridView.instance.turnLength * 1.25f);
            _animator.Play("ShortTapRhythm", -1, 0);

            onDestroy = delegate { };
        }

        public void ShowBeatingResult()
        {
            BeatingResultView beatingResultView;
            switch (rhythm.beatingResult)
            {
                case BeatingResultType.Good:
                    beatingResultView = GridView.instance.goodBeatingViewPool.SpawnEntity(GridView.instance.beatingResultContainer, false);
                    break;

                case BeatingResultType.Perfect:
                    beatingResultView = GridView.instance.perfectBeatingViewPool.SpawnEntity(GridView.instance.beatingResultContainer, false);
                    break;

                case BeatingResultType.Miss:
                    beatingResultView = GridView.instance.missBeatingViewPool.SpawnEntity(GridView.instance.beatingResultContainer, false);
                    break;

                case BeatingResultType.Bad:
                    beatingResultView = GridView.instance.badBeatingViewPool.SpawnEntity(GridView.instance.beatingResultContainer, false);
                    break;

                default:
                    throw new Exception();
            }
            beatingResultView.Initialize(GridView.instance.anchor.ToWorldPosition(rhythm.position));
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
            GridView.instance.shortTapRhythmViewPool.DespawnEntity(this);
        }
    }
}