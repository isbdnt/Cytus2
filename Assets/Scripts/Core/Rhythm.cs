using UnityEngine;

namespace Cytus2
{
    public abstract class Rhythm
    {
        public Note note { get; private set; }
        public Rhythm previous { get; set; }
        public Rhythm next { get; set; }
        public Vector2 position { get; private set; }
        public BeatingResultType beatingResult { get; protected set; }
        public abstract float beatingSteps { get; }
        public int tempo => _data.tempo;
        public int stepOffset { get; private set; }

        protected RhythmData _data;

        public Rhythm(Note note, RhythmData data, int stepOffset)
        {
            this.note = note;
            _data = data;
            this.stepOffset = stepOffset;
            position = new Vector2(data.positionX, GridUtility.StepToPositionY(stepOffset + 16 - note.grid.stepOffset));
        }

        public abstract void Update(float currentStep);

        public abstract void BeatTime();

        public virtual void StopBeating()
        {
        }
    }
}