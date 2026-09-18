using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab;

    public Transform bulletOrigin;

    public void Shoot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GameObject newBullet = Instantiate(bulletPrefab, bulletOrigin.position, Quaternion.identity);
            newBullet.transform.forward = bulletOrigin.forward;
        }
    }
}
