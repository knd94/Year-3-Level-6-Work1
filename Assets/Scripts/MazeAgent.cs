using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class MazeAgent : Agent
{
    [Header("Scene Setup")]
    public Transform goalTransform;
    public float moveSpeed = 2f;
    public float turnSpeed = 180f;

    private Rigidbody rBody;
    private float startTime;

    public override void Initialize()
    {
        // Cache Rigidbody
        rBody = GetComponent<Rigidbody>();

        // Disable vertical physics
        rBody.useGravity = false;
        rBody.constraints = RigidbodyConstraints.FreezePositionY
                          | RigidbodyConstraints.FreezeRotationX
                          | RigidbodyConstraints.FreezeRotationZ;

        // Auto‑toggle behavior type:
        var bp = GetComponent<BehaviorParameters>();
        if (Academy.Instance.IsCommunicatorOn)
        {
            // Python trainer is connected → RL training
            bp.BehaviorType = BehaviorType.Default;
        }
        else
        {
            // No trainer → manual (heuristic) testing
            bp.BehaviorType = BehaviorType.HeuristicOnly;
            // Ensure periodic decisions for Heuristic
            var dr = gameObject.GetComponent<DecisionRequester>();
            if (dr == null) dr = gameObject.AddComponent<DecisionRequester>();
            dr.DecisionPeriod = 1;
        }
    }

    public override void OnEpisodeBegin()
    {
        // Reset velocities
        rBody.linearVelocity = Vector3.zero;
        rBody.angularVelocity = Vector3.zero;

        // Randomize agent start
        Vector3 randomPos = MazeGenerator.Instance.GetRandomEmptyCellWorldPos();
        transform.position = randomPos + Vector3.up * 0.5f;
        transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

        // Randomize goal position
        Vector3 goalPos = MazeGenerator.Instance.GetRandomGoalCellWorldPos();
        goalTransform.position = goalPos + Vector3.up * 0.5f;

        // Track start time
        startTime = Time.time;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // 1) Vector to goal in local space
        Vector3 toGoalWorld = goalTransform.position - transform.position;
        Vector3 toGoalLocal = transform.InverseTransformVector(toGoalWorld);

        // 2) Normalize by maze dimensions
        Vector2Int size = MazeGenerator.Instance.MazeSize;
        Vector3 norm = new Vector3(
            toGoalLocal.x / size.x,
            toGoalLocal.y,              // optional: keep raw or divide by 1
            toGoalLocal.z / size.y
        );
        sensor.AddObservation(norm);

        // 3) Agent’s forward direction
        sensor.AddObservation(transform.forward);

        // 4) RayPerceptionSensor3D component (if added) handles walls
    }

    public override void OnActionReceived(ActionBuffers aBuffers)
    {
        float move = Mathf.Clamp(aBuffers.ContinuousActions[0], -1f, 1f);
        float turn = Mathf.Clamp(aBuffers.ContinuousActions[1], -1f, 1f);

        // Apply rotation & translation
        transform.Rotate(Vector3.up, turn * turnSpeed * Time.deltaTime);
        Vector3 newPos = transform.position + transform.forward * move * moveSpeed * Time.deltaTime;
        rBody.MovePosition(newPos);

        // Small time penalty
        AddReward(-0.001f);

        // Penalty if stuck while trying to move
        if (Mathf.Abs(move) > 0.1f && rBody.linearVelocity.magnitude < 0.01f)
        {
            AddReward(-0.05f);
        }
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.collider.CompareTag("Wall"))
        {
            AddReward(-0.1f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            float timeTaken = Time.time - startTime;
            AddReward(+10f);
            Debug.Log($"Episode completed in {timeTaken:F2} seconds.");
            Invoke(nameof(EndEpisode), 1f);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var cont = actionsOut.ContinuousActions;
        cont[0] = Input.GetKey(KeyCode.W) ? 1f
                  : Input.GetKey(KeyCode.S) ? -1f : 0f;
        cont[1] = Input.GetKey(KeyCode.D) ? 1f
                  : Input.GetKey(KeyCode.A) ? -1f : 0f;
    }
}
