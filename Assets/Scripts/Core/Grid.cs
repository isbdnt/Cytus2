using System;
using System.Collections.Generic;

namespace Cytus2
{
    public class Grid
    {
        public event Action<Note> onAddNote = delegate { };

        public event Action<Note> onRemoveNote = delegate { };

        public event Action<int> onPointChange = delegate { };

        public event Action<int> onComboChange = delegate { };

        public event Action onSongdEnd = delegate { };

        public float currentStep { get; private set; }
        public int stepOffset { get; private set; }
        public int combo { get; private set; }
        public int point { get; private set; }

        private Queue<Note> _pendingNotes = new Queue<Note>();
        private List<Note> _runningNotes = new List<Note>();

        public Grid(ChartData chart, int stepOffset)
        {
            this.stepOffset = stepOffset;
            int noteStepOffset = stepOffset - 16;
            foreach (var noteData in chart.notes)
            {
                noteStepOffset += noteData.stepOffset;
                _pendingNotes.Enqueue(new Note(this, noteData, noteStepOffset));
            }
        }

        public void Update(float currentStep)
        {
            this.currentStep = currentStep;
            while (_pendingNotes.Count > 0 && currentStep >= _pendingNotes.Peek().stepOffset)
            {
                Note note = _pendingNotes.Dequeue();
                _runningNotes.Add(note);
                onAddNote(note);
            }

            int combo = this.combo, point = this.point;
            for (int i = 0; i < _runningNotes.Count;)
            {
                Note note = _runningNotes[i];
                note.Update(currentStep, ref combo, ref point);
                if (note.ended)
                {
                    _runningNotes.RemoveAtSwapBack(i);
                    onRemoveNote(note);
                }
                else
                {
                    i++;
                }
            }

            if (combo < 0)
            {
                combo = 0;
            }
            if (combo != this.combo)
            {
                this.combo = combo;
                if (combo != 1)
                {
                    onComboChange(combo);
                }
            }
            if (point != this.point)
            {
                this.point = point;
                onPointChange(point);
            }
        }
    }
}