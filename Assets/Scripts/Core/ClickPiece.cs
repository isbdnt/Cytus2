using UnityEngine;

namespace Cytus2
{
    public class ClickPiece : Piece
    {
        public override float beatingSteps => 0;

        public ClickPiece(Note note, RhythmData data, int stepOffset) : base(note, data, stepOffset)
        {
        }

        public override void Update(float currentStep)
        {
            if (currentStep >= stepOffset + 20f && beatingResult == BeatingResultType.Unknown)
            {
                beatingResult = BeatingResultType.Miss;
            }
        }

        public override void BeatTime()
        {
            float error = Mathf.Abs(note.grid.currentStep - stepOffset - 16f);
            if (error <= 2f)
            {
                beatingResult = BeatingResultType.Perfect;
            }
            else if (error <= 3.25f)
            {
                beatingResult = BeatingResultType.Good;
            }
            else if (error <= 10f)
            {
                beatingResult = BeatingResultType.Bad;
            }
        }
    }
}