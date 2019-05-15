using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cytus2
{
    public class NoteView : MonoBehaviour, IGameObjectPoolEntity
    {
        public static GameObjectPool<NoteView> pool { get; } = new GameObjectPool<NoteView>();

        public event Action<NoteView> onDestroy;

        public Note note { get; private set; }

        protected Dictionary<Piece, IPieceView> _pieceViewMap;

        public void Initialize(Note note)
        {
            this.note = note;
            note.onAddPiece += HandleNoteAddPiece;
            note.onRemovePiece += HandleNoteRemovePiece;
            transform.SetAsFirstSibling();
            transform.position = GridView.instance.anchor.ToWorldPosition(note.position);
            _pieceViewMap = new Dictionary<Piece, IPieceView>();
            onDestroy = delegate { };
        }

        private void HandleNoteAddPiece(Piece piece)
        {
            IPieceView pieceView;
            switch (note.beatingStyle)
            {
                case BeatingStyleType.Click:
                    pieceView = ClickPieceView.pool.SpawnEntity(transform, false);
                    break;

                case BeatingStyleType.Hold:
                    pieceView = HoldPieceView.pool.SpawnEntity(transform, false);
                    break;

                case BeatingStyleType.SpecialHold:
                    pieceView = SpecialHoldPieceView.pool.SpawnEntity(transform, false);
                    break;

                case BeatingStyleType.Drag:
                    pieceView = DragPieceView.pool.SpawnEntity(transform, false);
                    break;

                case BeatingStyleType.Flick:
                    pieceView = FlickPieceView.pool.SpawnEntity(transform, false);
                    break;

                default:
                    throw new Exception();
            }
            pieceView.Initialize(this, piece);
            pieceView.onDestroy += HandlePieceViewDestroy;
            _pieceViewMap[piece] = pieceView;
        }

        private void HandlePieceViewDestroy(IPieceView pieceView)
        {
            _pieceViewMap.Remove(pieceView.piece);
            if (_pieceViewMap.Count == 0)
            {
                onDestroy(this);
                Despawn();
            }
        }

        private void HandleNoteRemovePiece(Piece piece)
        {
            _pieceViewMap[piece].ShowBeatingResult();
        }

        public void Render(float currentStep)
        {
            foreach (var pieceView in _pieceViewMap.Values)
            {
                pieceView.Render(currentStep);
            }
        }

        public void Despawn()
        {
            note.onAddPiece -= HandleNoteAddPiece;
            note.onRemovePiece -= HandleNoteRemovePiece;
            pool.DespawnEntity(this);
        }
    }
}