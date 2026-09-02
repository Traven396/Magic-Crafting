using UnityEngine;
using UnityEngine.Animations;

public class RotateTransformOverTime : MonoBehaviour
{
    [SerializeField] Axis RotationAxis;
    [SerializeField] float Speed;

    public bool PauseRotation = false;
    private void Update()
    {
        if (!PauseRotation)
        {
            switch (RotationAxis)
            {
                case Axis.X:
                    transform.Rotate(Vector3.right * Speed * Time.deltaTime, Space.Self);
                    break;
                case Axis.Y:
                    transform.Rotate(Vector3.up * Speed * Time.deltaTime, Space.Self);
                    break;
                case Axis.Z:
                    transform.Rotate(Vector3.forward * Speed * Time.deltaTime, Space.Self);
                    break;
            } 
        }
    }
}
