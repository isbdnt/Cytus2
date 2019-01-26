using System;
using System.Collections;
using UnityEngine;

namespace Cytus2
{
    public class BeatingResultView : MonoBehaviour, IGameObjectPoolEntity
    {
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
                    GridView.instance.goodBeatingViewPool.DespawnEntity(this);
                    break;

                case BeatingResultType.Perfect:
                    GridView.instance.perfectBeatingViewPool.DespawnEntity(this);
                    break;

                case BeatingResultType.Miss:
                    GridView.instance.missBeatingViewPool.DespawnEntity(this);
                    break;

                case BeatingResultType.Bad:
                    GridView.instance.badBeatingViewPool.DespawnEntity(this);
                    break;

                default:
                    throw new Exception();
            }
        }
    }
}