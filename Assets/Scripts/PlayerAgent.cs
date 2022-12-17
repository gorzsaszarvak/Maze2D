using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;

public class PlayerAgent : Agent
{
    private float moveSpeed = 3f;

    [SerializeField] private MazeRenderer mazeRenderer;
    private int episodes = 0;
    //BehaviorParameters behaviorParameters;

    public override void Initialize()
    {
        base.Initialize();
        //behaviorParameters.BrainParameters.VectorObservationSize = 3 + mazeRenderer.goals.Count * 3;
        mazeRenderer = Instantiate(mazeRenderer, transform.parent);
        mazeRenderer.GenerateMaze(transform.parent);
        mazeRenderer.ReDrawMaze();
    }

    public override void OnEpisodeBegin()
    {
        mazeRenderer.ResetGoals();
        episodes++;

        


        if (episodes == 100) {
            mazeRenderer.ReDrawMaze();
            episodes = 0;
            Debug.Log("Maze redrawn");
        }

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
        //AddReward(-0.01f);
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
            AddReward(2f);
            mazeRenderer.RemoveGoal(goal.gameObject);
        }
        else if (other.TryGetComponent<Wall>(out Wall wall))
        {
            AddReward(-1f);
            EndEpisode();
        }
    }
}
