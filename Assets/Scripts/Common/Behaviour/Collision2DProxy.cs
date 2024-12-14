using System;
using UnityEngine;
namespace Minefactory.Common.Behaviour
{
    // Motivation: https://discussions.unity.com/t/having-more-than-one-collider-in-a-gameobject/32067/9
    public class Collision2DProxy : MonoBehaviour
    {

        public Action<Collider2D> OnTriggerEnter2D_Action;
        public Action<Collider2D> OnTriggerStay2D_Action;
        public Action<Collider2D> OnTriggerExit2D_Action;

        private void OnTriggerEnter2D(Collider2D collider)
        {
            OnTriggerEnter2D_Action?.Invoke(collider);
        }

        private void OnTriggerStay2D(Collider2D collider)
        {
            OnTriggerStay2D_Action?.Invoke(collider);
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            OnTriggerExit2D_Action?.Invoke(collider);
        }
    }
}