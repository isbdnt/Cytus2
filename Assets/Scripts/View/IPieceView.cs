using System;

namespace Cytus2
{
    public interface IPieceView : IGameObjectPoolEntity
    {
        Piece piece { get; }

        void Initialize(NoteView noteView, Piece piece);

        void ShowBeatingResult();

        void Render(float currentStep);

        event Action<IPieceView> onDestroy;
    }
}