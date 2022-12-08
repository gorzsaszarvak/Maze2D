using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class PlayerAgent : Agent
{
    private float moveSpeed = 5f;

    [SerializeField] private MazeRenderer mazeRenderer;

    public override void Initialize()
    {
        base.Initialize();
        mazeRenderer = Instantiate(mazeRenderer, transform.parent);
        mazeRenderer.GenerateMaze(transform.parent);
    }

    public override void OnEpisodeBegin()
    {
        mazeRenderer.ReDrawMaze();
        transform.localPosition = new Vector3(0f, 0f, 0f);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];

        transform.localPosition += new Vector3(moveX, moveY, 0f) * Time.deltaTime * moveSpeed;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Goal>(out Goal goal))
        {
            AddReward(1f);
            mazeRenderer.RemoveGoal(goal.gameObject);

        }
        else if (other.TryGetComponent<Wall>(out Wall wall))
        {
            AddReward(-10f);
            EndEpisode();
        }
    }
}
