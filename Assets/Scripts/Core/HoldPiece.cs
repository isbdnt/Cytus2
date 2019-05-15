namespace Cytus2
{
    public class HoldPiece : Piece
    {
        public override float beatingSteps => _beatingSteps;

        private float _beatingStep = -1;
        private float _beatingSteps;
        private bool _beating;

        public HoldPiece(Note note, PieceData data, int stepOffset) : base(note, data, stepOffset)
        {
        }

        public override void Update(float currentStep)
        {
            if (currentStep >= stepOffset + 16f + tempo)
            {
                SetBeatingResult();
            }
            else
            {
                if (_beating)
                {
                    if (currentStep >= stepOffset + 16f && _beatingStep < 0f)
                    {
                        _beatingStep = currentStep;
                    }
                }
                else if (_beatingStep >= 0f)
                {
                    SetBeatingResult();
                }
                else if (currentStep >= stepOffset + 20f)
                {
                    beatingResult = BeatingResultType.Miss;
                }
            }
            _beatingSteps = _beatingStep < 0 ? 0 : (currentStep - _beatingStep);
        }

        private void SetBeatingResult()
        {
            float error = tempo - beatingSteps;
            if (error <= 1f)
            {
                beatingResult = BeatingResultType.Perfect;
            }
            else if (error <= 2f)
            {
                beatingResult = BeatingResultType.Good;
            }
            else
            {
                beatingResult = BeatingResultType.Bad;
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