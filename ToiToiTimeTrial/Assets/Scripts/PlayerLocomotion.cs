using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    public PipeSystem PipeSystem;
    public Transform PlayerRootTransform;
    public Transform PlayerControllableTransform;
    public Transform CameraTransform;
    public float ControlRadius = 1.9f;
    public float CameraRadius = 1.6f;

    public float ForwardSpeed;
    public float StrafeSpeed;

    private Pipe _currentPipe;

    private float _radiansTraveled;

    private float _pipeProgress;

    public int AlignmentFrames;

    private float horizontalOffset, verticalOffset;

    // Start is called before the first frame update
    void Start()
    {
        TransferToNextPipe(0);
    }

    private void TransferToNextPipe(float progressFromLastPipe)
    {
        _currentPipe = PipeSystem.GetNextPipe();
        OrientPlayerToPipe();

        _pipeProgress = 0;
        ProgressPlayer(progressFromLastPipe);
    }

    private void OrientPlayerToPipe()
    {
        var originalRotation = PlayerRootTransform.rotation;
        transform.position = _currentPipe.transform.position;
        transform.rotation = _currentPipe.transform.rotation;
        PlayerRootTransform.position = transform.position + transform.up * _currentPipe.CurveRadius;
        PlayerRootTransform.rotation = originalRotation;
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayerForward();
        if (Input.GetAxisRaw("Vertical") != 0f || Input.GetAxisRaw("Horizontal") != 0f)
        {
            ProcessDirectionalInput();
        }
    }

    private void ProcessDirectionalInput()
    {
        var playerDirection = (Vector3.up * Input.GetAxisRaw("Vertical")) +
                              (Vector3.back * Input.GetAxisRaw("Horizontal")).normalized;

        var strafeDistance = playerDirection * StrafeSpeed * Time.smoothDeltaTime;
        if ((PlayerControllableTransform.localPosition + strafeDistance).sqrMagnitude < ControlRadius * ControlRadius)
        {
            PlayerControllableTransform.localPosition += strafeDistance;
        }
        else
        {
            PlayerControllableTransform.localPosition =
                (PlayerControllableTransform.localPosition + strafeDistance).normalized * ControlRadius;
        }

        var cameraOffset = new Vector3(-PlayerControllableTransform.localPosition.z,
            PlayerControllableTransform.localPosition.y, 0f);

        CameraTransform.localPosition = cameraOffset * CameraRadius / ControlRadius;
        CameraTransform.localPosition -= Vector3.forward * 3;
        CameraTransform.LookAt(PlayerRootTransform, PlayerRootTransform.up);
    }

    private void MovePlayerForward()
    {
        var distanceTraveled = ForwardSpeed * Time.smoothDeltaTime  / _currentPipe.CurveRadius;
        _pipeProgress += distanceTraveled;
        if (_pipeProgress + distanceTraveled >= _currentPipe.CurveAngle)
        {
            var nextPipeProgress = _pipeProgress + distanceTraveled - _currentPipe.CurveAngle;
            TransferToNextPipe(nextPipeProgress);
        }
        ProgressPlayer(distanceTraveled);
    }

    private void ProgressPlayer(float distanceTraveled)
    {
        transform.Rotate(new Vector3(0f, 0f, -distanceTraveled));
    }
}
