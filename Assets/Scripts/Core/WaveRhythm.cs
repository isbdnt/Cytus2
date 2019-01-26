using UnityEngine;

namespace Cytus2
{
    public class WaveRhythm : Rhythm
    {
        public override float beatingSteps => 0;

        public WaveRhythm(Note note, RhythmData data, int stepOffset) : base(note, data, stepOffset)
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
            if (error <= 10f)
            {
                beatingResult = BeatingResultType.Perfect;
            }
        }
    }
}