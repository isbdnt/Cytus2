using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cytus2
{
    public class ClickPieceView : MonoBehaviour, IPieceView, IPointerDownHandler
    {
        public static GameObjectPool<ClickPieceView> pool { get; } = new GameObjectPool<ClickPieceView>();

        public event Action<IPieceView> onDestroy;

        public Piece piece { get; private set; }

        public bool testBeat;
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void Initialize(NoteView noteView, Piece piece)
        {
            this.piece = piece;
            transform.position = GridView.instance.anchor.ToWorldPosition(piece.position);

            _animator.speed = 1f / (GridView.instance.turnLength * 1.25f);
            _animator.Play("ClickPiece", -1, 0);

            onDestroy = delegate { };
        }

        public void ShowBeatingResult()
        {
            BeatingResultView beatingResultView = GridUtility.SpawnBeatingResultView(piece.beatingResult);
            beatingResultView.Initialize(GridView.instance.anchor.ToWorldPosition(piece.position));
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