﻿using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cytus2
{
    public class WaveRhythmView : MonoBehaviour, IRhythmView, IDragHandler
    {
        public static GameObjectPool<WaveRhythmView> pool { get; } = new GameObjectPool<WaveRhythmView>();

        public event Action<IRhythmView> onDestroy;

        public Rhythm rhythm { get; private set; }

        public bool testBeat;
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void Initialize(NoteView noteView, Rhythm rhythm)
        {
            this.rhythm = rhythm;
            transform.position = GridView.instance.anchor.ToWorldPosition(rhythm.position);

            _animator.speed = 1f / (GridView.instance.turnLength * 1.25f);
            _animator.Play("WaveRhythm", -1, 0);

            onDestroy = delegate { };
        }

        public void ShowBeatingResult()
        {
            BeatingResultView beatingResultView = GridUtility.SpawnBeatingResultView(rhythm.beatingResult);
            beatingResultView.Initialize(GridView.instance.anchor.ToWorldPosition(rhythm.position));
            Destroy();
        }

        private void FixedUpdate()
        {
            if (testBeat)
            {
                testBeat = false;
                rhythm.BeatTime();
            }
        }

        public void Render(float currentStep)
        {
        }

        public void OnDrag(PointerEventData eventData)
        {
            rhythm.BeatTime();
        }

        private void Destroy()
        {
            onDestroy(this);
            Despawn();
        }

        public void Despawn()
        {
            pool.DespawnEntity(this);
        }
    }
}