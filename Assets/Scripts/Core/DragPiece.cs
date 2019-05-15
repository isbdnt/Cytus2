using UnityEngine;

namespace Cytus2
{
    public class DragPiece : Piece
    {
        public override float beatingSteps => 0;

        private bool _beating;

        public DragPiece(Note note, RhythmData data, int stepOffset) : base(note, data, stepOffset)
        {
        }

        public override void Update(float currentStep)
        {
            if (_beating)
            {
                if (Mathf.Abs(currentStep - stepOffset - 16f) <= 8f)
                {
                    beatingResult = BeatingResultType.Perfect;
                }
            }
            else if (currentStep >= stepOffset + 20f)
            {
                beatingResult = BeatingResultType.Miss;
            }
        }

        public override void BeatTime()
        {
            _beating = true;
        }

        public override void StopBeating()
        {
            _beating = false;
        }
    }
}