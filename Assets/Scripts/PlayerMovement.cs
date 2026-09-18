using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEditor.ShaderGraph.Internal;
using UnityEditor;

public class PlayerMovement : MonoBehaviour
{
    public float zSpeed = 1f;
    public float xySpeed = 10f;
    public float rotationSpeed = 1000f;
    public float tiltLimit = 20f;


    [SerializeField]

    InputActionReference moveAction;
    public GameObject aimObject;
    public Transform model;
    public CinemachineSplineCart splineCart;

    public TrailRenderer trail;

    public GameObject cameraHolder;


    private void Start()
    {
        SetSpeed(zSpeed);
    }
    private void Update()
    {
        Vector2 xyVector = moveAction.action.ReadValue<Vector2>();

        LocalMove(xyVector.x, xyVector.y, xySpeed);
        ClampPosition();
        RotationLook(xyVector.x, xyVector.y, rotationSpeed);
        HorizontalTilt( model, xyVector.x, tiltLimit, 0.1f);
    }

    void LocalMove(float x, float y, float speed)
    {
        transform.localPosition += new Vector3(x, y, 0) * speed * Time.deltaTime;
    }

    void ClampPosition()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        pos.x = Mathf.Clamp01(pos.x);
        pos.y = Mathf.Clamp01(pos.y);

        transform.position = Camera.main.ViewportToWorldPoint(pos);
    }

    void RotationLook(float h, float v, float speed)
    {
        aimObject.transform.parent.position = Vector3.zero;
        aimObject.transform.localPosition = new Vector3(h, v, 1);
        gameObject.transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(aimObject.transform.position), Mathf.Deg2Rad * speed * Time.deltaTime);
    }

    void HorizontalTilt(Transform target, float axis, float tiltLimit, float lerpTime)
    {
        Vector3 targetEulerAngles = target.localEulerAngles;
        target.localEulerAngles = new Vector3(targetEulerAngles.x, targetEulerAngles.y, Mathf.LerpAngle(targetEulerAngles.z, -axis * tiltLimit, lerpTime));
    }

    void SetSpeed(float zSpeed)
    {
        var cartSpeed = splineCart.AutomaticDolly.Method as SplineAutoDolly.FixedSpeed;
        cartSpeed.Speed = zSpeed;
    }

    public void QuickSpin(int direction)
    {
        if (!DOTween.IsTweening(model))
        {
            model.DOLocalRotate(new Vector3(model.localEulerAngles.x, model.localEulerAngles.y, 360 * direction), .5f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear);
        }
        
    }

    public void Boost(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SetSpeed(zSpeed * 2);
            trail.emitting = true;
            FieldOfView(70f);
            SetCameraZoom(-2f, 0f);
            Chromatic(1f);
        }
        else if (context.canceled)
        {
            SetSpeed(zSpeed);
            trail.emitting = false;
            FieldOfView(40f);
            SetCameraZoom(0f, 0f);
            Chromatic(0f);
        }
    }

    void FieldOfView(float fov)
    {
        DOTween.To(
            () => cameraHolder.GetComponentInChildren<CinemachineCamera>().Lens.FieldOfView,
            x => cameraHolder.GetComponentInChildren<CinemachineCamera>().Lens.FieldOfView = x,
            fov,
            0.5f);
    }

    void SetCameraZoom(float zoom, float duration)
    {
        cameraHolder.transform.DOLocalMoveZ(zoom, duration);
    }

    void Chromatic(float chromaticValue)
    {
        Camera.main.GetComponent<Volume>().profile.TryGet(out ChromaticAberration c);

        DOTween.To(
            () => c.intensity.value,
            y => c.intensity.value = y,
            chromaticValue,
            0.5f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(aimObject.transform.position, 0.4f);
        Gizmos.DrawSphere(aimObject.transform.position, 0.1f);
    }
}
