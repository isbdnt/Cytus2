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

        protected Dictionary<Piece, IPieceView> _rhythmViewMap;

        public void Initialize(Note note)
        {
            this.note = note;
            note.onAddRhythm += HandleNoteAddRhythm;
            note.onRemoveRhythm += HandleNoteRemoveRhythm;
            transform.SetAsFirstSibling();
            transform.position = GridView.instance.anchor.ToWorldPosition(note.position);
            _rhythmViewMap = new Dictionary<Piece, IPieceView>();
            onDestroy = delegate { };
        }

        private void HandleNoteAddRhythm(Piece rhythm)
        {
            IPieceView rhythmView;
            switch (note.beatingStyle)
            {
                case BeatingStyleType.Click:
                    rhythmView = ClickPieceView.pool.SpawnEntity(transform, false);
                    break;

                case BeatingStyleType.Hold:
                    rhythmView = HoldPieceView.pool.SpawnEntity(transform, false);
                    break;

                case BeatingStyleType.SpecialHold:
                    rhythmView = SpecialHoldPieceView.pool.SpawnEntity(transform, false);
                    break;

                case BeatingStyleType.Drag:
                    rhythmView = DragPieceView.pool.SpawnEntity(transform, false);
                    break;

                case BeatingStyleType.Flick:
                    rhythmView = FlickPieceView.pool.SpawnEntity(transform, false);
                    break;

                default:
                    throw new Exception();
            }
            rhythmView.Initialize(this, rhythm);
            rhythmView.onDestroy += HandleRhythmViewDestroy;
            _rhythmViewMap[rhythm] = rhythmView;
        }

        private void HandleRhythmViewDestroy(IPieceView rhythmView)
        {
            _rhythmViewMap.Remove(rhythmView.rhythm);
            if (_rhythmViewMap.Count == 0)
            {
                onDestroy(this);
                Despawn();
            }
        }

        private void HandleNoteRemoveRhythm(Piece rhythm)
        {
            _rhythmViewMap[rhythm].ShowBeatingResult();
        }

        public void Render(float currentStep)
        {
            foreach (var rhythmView in _rhythmViewMap.Values)
            {
                rhythmView.Render(currentStep);
            }
        }

        public void Despawn()
        {
            note.onAddRhythm -= HandleNoteAddRhythm;
            note.onRemoveRhythm -= HandleNoteRemoveRhythm;
            pool.DespawnEntity(this);
        }
    }
}