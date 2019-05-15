using System;

namespace Cytus2
{
    public interface IPieceView : IGameObjectPoolEntity
    {
        Piece rhythm { get; }

        void Initialize(NoteView noteView, Piece rhythm);

        void ShowBeatingResult();

        void Render(float currentStep);

        event Action<IPieceView> onDestroy;
    }
}