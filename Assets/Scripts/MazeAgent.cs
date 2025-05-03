using UnityEngine;
using Unity.MLAgents;
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
        rBody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        // Reset agent velocity & position
        rBody.linearVelocity = Vector3.zero;
        rBody.angularVelocity = Vector3.zero;

        // Randomize start (you’ll need to implement your own sampling)
        Vector3 randomPos = MazeGenerator.Instance.GetRandomEmptyCellWorldPos();
        transform.position = randomPos + Vector3.up * 0.5f;
        transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

        // Randomize or re‑place goal
        Vector3 goalPos = MazeGenerator.Instance.GetRandomGoalCellWorldPos();
        goalTransform.position = goalPos + Vector3.up * 0.5f;

        // Track start time
        startTime = Time.time;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // 1. Agent‑to‑goal vector in local space
        Vector3 toGoalWorld = goalTransform.position - transform.position;
        Vector3 toGoalLocal = transform.InverseTransformVector(toGoalWorld);

        // 2. Normalize x and z by maze dimensions (leave y untouched or normalized by 1)
        Vector2Int size = MazeGenerator.Instance.MazeSize;
        Vector3 normalizedObs = new Vector3(
            toGoalLocal.x / size.x,
            toGoalLocal.y,                // or divide by some constant if you want to include height
            toGoalLocal.z / size.y
        );

        sensor.AddObservation(normalizedObs);

        // 3. Agent’s forward vector
        sensor.AddObservation(transform.forward);

        // 4. RayPerceptionSensor3D will handle wall sensing automatically
    }

    public override void OnActionReceived(ActionBuffers aBuffers)
    {
        // Continuous actions: [0] = move, [1] = turn
        float move = Mathf.Clamp(aBuffers.ContinuousActions[0], -1f, 1f);
        float turn = Mathf.Clamp(aBuffers.ContinuousActions[1], -1f, 1f);

        // Apply motion
        transform.Rotate(Vector3.up, turn * turnSpeed * Time.deltaTime);
        rBody.MovePosition(transform.position + transform.forward * move * moveSpeed * Time.deltaTime);

        // Small time penalty to encourage shorter paths
        AddReward(-0.001f);

        // (Optional) Detect dead‑end: if speed < ε after move → penalty
        if (Mathf.Abs(move) > 0.1f && rBody.linearVelocity.magnitude < 0.01f)
        {
            AddReward(-0.05f);
        }
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.collider.CompareTag("Wall"))
        {
            // Hitting a wall is bad
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
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Optional: manual testing with keyboard
        var cont = actionsOut.ContinuousActions;
        cont[0] = Input.GetKey(KeyCode.W) ? 1f : (Input.GetKey(KeyCode.S) ? -1f : 0f);
        cont[1] = Input.GetKey(KeyCode.D) ? 1f : (Input.GetKey(KeyCode.A) ? -1f : 0f);
    }
}
