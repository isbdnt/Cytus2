using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cytus2
{
    public class Note
    {
        public event Action<Rhythm> onAddRhythm = delegate { };

        public event Action<Rhythm> onRemoveRhythm = delegate { };

        public Grid grid { get; private set; }
        public Vector2 position { get; private set; }
        public int direction { get; private set; }
        public int stepOffset { get; private set; }
        public BeatingStyleType beatingStyle => _data.beatingStyle;
        public bool ended => _runningRhythms.Count == 0 && _pendingRhythms.Count == 0;

        private Queue<Rhythm> _pendingRhythms = new Queue<Rhythm>();
        private List<Rhythm> _runningRhythms = new List<Rhythm>();
        private NoteData _data;

        public Note(Grid grid, NoteData data, int stepOffset)
        {
            this.grid = grid;
            _data = data;
            this.stepOffset = stepOffset;
            position = new Vector2(_data.rhythms[0].positionX, GridUtility.StepToPositionY(stepOffset + 16 - grid.stepOffset));
            direction = GridUtility.StepToDirection(stepOffset + 16 - grid.stepOffset);
            int rhythmStepOffset = stepOffset;
            Rhythm previousRhythm = null;
            foreach (var rhythmData in _data.rhythms)
            {
                Rhythm rhythm;
                switch (data.beatingStyle)
                {
                    case BeatingStyleType.ShortTap:
                        rhythm = new ShortTapRhythm(this, rhythmData, rhythmStepOffset);
                        break;

                    case BeatingStyleType.MediumTap:
                        rhythm = new MediumTapRhythm(this, rhythmData, rhythmStepOffset);
                        break;

                    case BeatingStyleType.LongTap:
                        rhythm = new LongTapRhythm(this, rhythmData, rhythmStepOffset);
                        break;

                    case BeatingStyleType.Shake:
                        rhythm = new ShakeRhythm(this, rhythmData, rhythmStepOffset);
                        break;

                    case BeatingStyleType.Wave:
                        rhythm = new WaveRhythm(this, rhythmData, rhythmStepOffset);
                        break;

                    default:
                        throw new Exception();
                }
                if (previousRhythm != null)
                {
                    rhythm.previous = previousRhythm;
                    previousRhythm.next = rhythm;
                }
                rhythmStepOffset += rhythm.tempo;
                _pendingRhythms.Enqueue(rhythm);
                previousRhythm = rhythm;
            }
        }

        public void Update(float currentStep, ref int combo, ref int point)
        {
            while (_pendingRhythms.Count > 0 && currentStep >= _pendingRhythms.Peek().stepOffset)
            {
                Rhythm rhythm = _pendingRhythms.Dequeue();
                _runningRhythms.Add(rhythm);
                onAddRhythm(rhythm);
            }

            for (int i = 0; i < _runningRhythms.Count;)
            {
                Rhythm rhythm = _runningRhythms[i];
                rhythm.Update(currentStep);
                if (rhythm.beatingResult != BeatingResultType.Unknown)
                {
                    switch (rhythm.beatingResult)
                    {
                        case BeatingResultType.Good:
                            if (combo >= 0)
                            {
                                combo++;
                            }
                            point += 100;
                            break;

                        case BeatingResultType.Perfect:
                            if (combo >= 0)
                            {
                                combo++;
                            }
                            point += 200;
                            break;

                        case BeatingResultType.Miss:
                        case BeatingResultType.Bad:
                            combo = -1;
                            break;

                        default:
                            break;
                    }
                    _runningRhythms.RemoveAt(i);
                    onRemoveRhythm(rhythm);
                }
                else
                {
                    i++;
                }
            }
        }
    }
}