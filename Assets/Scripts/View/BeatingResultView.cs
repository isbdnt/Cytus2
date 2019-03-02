using System;
using System.Collections;
using UnityEngine;

namespace Cytus2
{
    public class BeatingResultView : MonoBehaviour, IGameObjectPoolEntity
    {
        public static GameObjectPool<BeatingResultView> pool { get; private set; } = new GameObjectPool<BeatingResultView>();

        [SerializeField]
        private BeatingResultType beatingResult;

        private IEnumerator animCoroutine;

        public void Initialize(Vector3 position)
        {
            transform.position = position;
            animCoroutine = PlayAnimation();
            StartCoroutine(animCoroutine);
        }

        private IEnumerator PlayAnimation()
        {
            yield return new WaitForSeconds(0.5f);
            Despawn();
        }

        public void Despawn()
        {
            if (animCoroutine != null)
            {
                StopCoroutine(animCoroutine);
            }
            switch (beatingResult)
            {
                case BeatingResultType.Good:
                    pool.DespawnEntity("Good", this);
                    break;

                case BeatingResultType.Perfect:
                    pool.DespawnEntity("Perfect", this);
                    break;

                case BeatingResultType.Miss:
                    pool.DespawnEntity("Miss", this);
                    break;

                case BeatingResultType.Bad:
                    pool.DespawnEntity("Bad", this);
                    break;

                default:
                    throw new Exception();
            }
        }
    }
}