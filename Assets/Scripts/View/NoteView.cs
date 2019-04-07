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

        protected Dictionary<Rhythm, IRhythmView> _rhythmViewMap;

        public void Initialize(Note note)
        {
            this.note = note;
            note.onAddRhythm += HandleNoteAddRhythm;
            note.onRemoveRhythm += HandleNoteRemoveRhythm;
            transform.SetAsFirstSibling();
            transform.position = GridView.instance.anchor.ToWorldPosition(note.position);
            _rhythmViewMap = new Dictionary<Rhythm, IRhythmView>();
            onDestroy = delegate { };
        }

        private void HandleNoteAddRhythm(Rhythm rhythm)
        {
            IRhythmView rhythmView;
            switch (note.beatingStyle)
            {
                case BeatingStyleType.ShortTap:
                    rhythmView = ShortTapRhythmView.pool.SpawnEntity(transform, false);
                    break;

                case BeatingStyleType.MediumTap:
                    rhythmView = MediumTapRhythmView.pool.SpawnEntity(transform, false);
                    break;

                case BeatingStyleType.LongTap:
                    rhythmView = LongTapRhythmView.pool.SpawnEntity(transform, false);
                    break;

                case BeatingStyleType.Shake:
                    rhythmView = ShakeRhythmView.pool.SpawnEntity(transform, false);
                    break;

                case BeatingStyleType.Wave:
                    rhythmView = WaveRhythmView.pool.SpawnEntity(transform, false);
                    break;

                default:
                    throw new Exception();
            }
            rhythmView.Initialize(this, rhythm);
            rhythmView.onDestroy += HandleRhythmViewDestroy;
            _rhythmViewMap[rhythm] = rhythmView;
        }

        private void HandleRhythmViewDestroy(IRhythmView rhythmView)
        {
            _rhythmViewMap.Remove(rhythmView.rhythm);
            if (_rhythmViewMap.Count == 0)
            {
                onDestroy(this);
                Despawn();
            }
        }

        private void HandleNoteRemoveRhythm(Rhythm rhythm)
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