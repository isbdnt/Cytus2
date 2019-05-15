using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cytus2
{
    public class Note
    {
        public event Action<Piece> onAddPiece = delegate { };

        public event Action<Piece> onRemovePiece = delegate { };

        public Grid grid { get; private set; }
        public Vector2 position { get; private set; }
        public int direction { get; private set; }
        public int stepOffset { get; private set; }
        public BeatingStyleType beatingStyle => _data.beatingStyle;
        public bool ended => _runningPieces.Count == 0 && _pendingPieces.Count == 0;

        private Queue<Piece> _pendingPieces = new Queue<Piece>();
        private List<Piece> _runningPieces = new List<Piece>();
        private NoteData _data;

        public Note(Grid grid, NoteData data, int stepOffset)
        {
            this.grid = grid;
            _data = data;
            this.stepOffset = stepOffset;
            position = new Vector2(_data.pieces[0].positionX, GridUtility.StepToPositionY(stepOffset + 16 - grid.stepOffset));
            direction = GridUtility.StepToDirection(stepOffset + 16 - grid.stepOffset);
            int pieceStepOffset = stepOffset;
            Piece previousPiece = null;
            foreach (var pieceData in _data.pieces)
            {
                Piece piece;
                switch (data.beatingStyle)
                {
                    case BeatingStyleType.Click:
                        piece = new ClickPiece(this, pieceData, pieceStepOffset);
                        break;

                    case BeatingStyleType.Hold:
                        piece = new HoldPiece(this, pieceData, pieceStepOffset);
                        break;

                    case BeatingStyleType.SpecialHold:
                        piece = new SpecialHoldPiece(this, pieceData, pieceStepOffset);
                        break;

                    case BeatingStyleType.Drag:
                        piece = new DragPiece(this, pieceData, pieceStepOffset);
                        break;

                    case BeatingStyleType.Flick:
                        piece = new FlickPiece(this, pieceData, pieceStepOffset);
                        break;

                    default:
                        throw new Exception();
                }
                if (previousPiece != null)
                {
                    piece.previous = previousPiece;
                    previousPiece.next = piece;
                }
                pieceStepOffset += piece.tempo;
                _pendingPieces.Enqueue(piece);
                previousPiece = piece;
            }
        }

        public void Update(float currentStep, ref int combo, ref int point)
        {
            while (_pendingPieces.Count > 0 && currentStep >= _pendingPieces.Peek().stepOffset)
            {
                Piece piece = _pendingPieces.Dequeue();
                _runningPieces.Add(piece);
                onAddPiece(piece);
            }

            for (int i = 0; i < _runningPieces.Count;)
            {
                Piece piece = _runningPieces[i];
                piece.Update(currentStep);
                if (piece.beatingResult != BeatingResultType.Unknown)
                {
                    switch (piece.beatingResult)
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
                    _runningPieces.RemoveAt(i);
                    onRemovePiece(piece);
                }
                else
                {
                    i++;
                }
            }
        }
    }
}