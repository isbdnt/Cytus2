using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cytus2
{
    public class NoteView : MonoBehaviour, IGameObjectPoolEntity
    {
        public event Action<NoteView> onDestroy;

        public Note note { get; private set; }

        protected Dictionary<Rhythm, IRhythmView> _rhythmViewMap;

        public virtual void Initialize(Note note)
        {
            this.note = note;
            note.onAddRhythm += HandleNoteAddRhythm;
            note.onRemoveRhythm += HandleNoteRemoveRhythm;
            transform.SetAsFirstSibling();
            transform.position = GridView.instance.anchor.ToWorldPosition(note.position);
            _rhythmViewMap = new Dictionary<Rhythm, IRhythmView>();
            onDestroy = delegate { };
        }

        protected virtual void HandleNoteAddRhythm(Rhythm rhythm)
        {
            IRhythmView rhythmView;
            switch (note.beatingStyle)
            {
                case BeatingStyleType.ShortTap:
                    rhythmView = GridView.instance.shortTapRhythmViewPool.SpawnEntity(transform, false);
                    break;

                case BeatingStyleType.MediumTap:
                    rhythmView = GridView.instance.mediumTapRhythmViewPool.SpawnEntity(transform, false);
                    break;

                case BeatingStyleType.LongTap:
                    rhythmView = GridView.instance.longTapRhythmViewPool.SpawnEntity(transform, false);
                    break;

                case BeatingStyleType.Shake:
                    rhythmView = GridView.instance.shakeRhythmViewPool.SpawnEntity(transform, false);
                    break;

                case BeatingStyleType.Wave:
                    rhythmView = GridView.instance.waveRhythmViewPool.SpawnEntity(transform, false);
                    break;

                default:
                    throw new Exception();
            }
            rhythmView.Initialize(this, rhythm);
            rhythmView.onDestroy += HandleRhythmViewDestroy;
            _rhythmViewMap[rhythm] = rhythmView;
        }

        protected virtual void HandleRhythmViewDestroy(IRhythmView rhythmView)
        {
            _rhythmViewMap.Remove(rhythmView.rhythm);
            if (_rhythmViewMap.Count == 0)
            {
                onDestroy(this);
                Despawn();
            }
        }

        protected virtual void HandleNoteRemoveRhythm(Rhythm rhythm)
        {
            _rhythmViewMap[rhythm].ShowBeatingResult();
        }

        public virtual void Render(float currentStep)
        {
            foreach (var rhythmView in _rhythmViewMap.Values)
            {
                rhythmView.Render(currentStep);
            }
        }

        public void Despawn()
        {
            GridView.instance.noteViewPool.DespawnEntity(this);
        }
    }
}