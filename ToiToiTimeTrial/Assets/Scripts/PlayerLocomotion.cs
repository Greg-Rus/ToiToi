using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    public PipeSystem PipeSystem;
    public Transform PlayerTransform;

    public float ForwardSpeed;
    public float StrafeSpeed;

    private Pipe _currentPipe;

    private float _radiansTraveled;

    private float _pipeProgress;

    public int AlignmentFrames;

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
        var originalRotation = PlayerTransform.rotation;
        transform.position = _currentPipe.transform.position;
        transform.rotation = _currentPipe.transform.rotation;
        PlayerTransform.position = transform.position + transform.up * _currentPipe.CurveRadius;
        PlayerTransform.rotation = originalRotation;
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
