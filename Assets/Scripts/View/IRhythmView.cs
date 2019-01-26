using System;

namespace Cytus2
{
    public interface IRhythmView : IGameObjectPoolEntity
    {
        Rhythm rhythm { get; }

        void Initialize(NoteView noteView, Rhythm rhythm);

        void ShowBeatingResult();

        void Render(float currentStep);

        event Action<IRhythmView> onDestroy;
    }
}