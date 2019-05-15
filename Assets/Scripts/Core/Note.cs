using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cytus2
{
    public class Note
    {
        public event Action<Piece> onAddRhythm = delegate { };

        public event Action<Piece> onRemoveRhythm = delegate { };

        public Grid grid { get; private set; }
        public Vector2 position { get; private set; }
        public int direction { get; private set; }
        public int stepOffset { get; private set; }
        public BeatingStyleType beatingStyle => _data.beatingStyle;
        public bool ended => _runningRhythms.Count == 0 && _pendingRhythms.Count == 0;

        private Queue<Piece> _pendingRhythms = new Queue<Piece>();
        private List<Piece> _runningRhythms = new List<Piece>();
        private NoteData _data;

        public Note(Grid grid, NoteData data, int stepOffset)
        {
            this.grid = grid;
            _data = data;
            this.stepOffset = stepOffset;
            position = new Vector2(_data.rhythms[0].positionX, GridUtility.StepToPositionY(stepOffset + 16 - grid.stepOffset));
            direction = GridUtility.StepToDirection(stepOffset + 16 - grid.stepOffset);
            int rhythmStepOffset = stepOffset;
            Piece previousRhythm = null;
            foreach (var rhythmData in _data.rhythms)
            {
                Piece rhythm;
                switch (data.beatingStyle)
                {
                    case BeatingStyleType.Click:
                        rhythm = new ClickPiece(this, rhythmData, rhythmStepOffset);
                        break;

                    case BeatingStyleType.Hold:
                        rhythm = new HoldPiece(this, rhythmData, rhythmStepOffset);
                        break;

                    case BeatingStyleType.SpecialHold:
                        rhythm = new SpecialHoldPiece(this, rhythmData, rhythmStepOffset);
                        break;

                    case BeatingStyleType.Drag:
                        rhythm = new DragPiece(this, rhythmData, rhythmStepOffset);
                        break;

                    case BeatingStyleType.Flick:
                        rhythm = new FlickPiece(this, rhythmData, rhythmStepOffset);
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
                Piece rhythm = _pendingRhythms.Dequeue();
                _runningRhythms.Add(rhythm);
                onAddRhythm(rhythm);
            }

            for (int i = 0; i < _runningRhythms.Count;)
            {
                Piece rhythm = _runningRhythms[i];
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