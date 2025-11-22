using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float lifeTime = 5f;

        private void Start()
        {
            Destroy(gameObject, lifeTime);
        }
    }
}
