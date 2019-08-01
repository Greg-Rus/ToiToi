using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeSystem : MonoBehaviour
{
    // Start is called before the first frame update
    public Pipe pipePrefab;

    public int pipeCount;

    private Pipe[] pipes;

    private int _currentPipeIndex = 0;

    private void Awake()
    {
        pipes = new Pipe[pipeCount];
        for (int i = 0; i < pipes.Length; i++)
        {
            Pipe pipe = pipes[i] = Instantiate<Pipe>(pipePrefab);

            pipe.transform.SetParent(transform, false);
            if (i > 0)
            {
                pipe.AlignWith(pipes[i - 1]);
            }
        }
    }

    public Pipe GetNextPipe()
    {
        var pipe = pipes[_currentPipeIndex];
        _currentPipeIndex++;
        return pipe;
    }
}
