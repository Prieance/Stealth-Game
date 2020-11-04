using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    public Transform path;
    public GameObject player;

    public EnemyFOV FOVScript;
    public float moveSpeed = 5;
    public float turnSpeed = 90;
    public float pause = .5f;
    public bool playerSpotted;
    public bool stop;
    public Vector3 movePosition;

    void Start()
    {
        Vector3[] waypoints = new Vector3[path.childCount];
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = path.GetChild(i).position;
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
        }

        StartCoroutine(FollowPath(waypoints));
    }

    private void Update()
    {
        if (FOVScript.playerIsSpotted)
        {
            playerSpotted = true;
        }
    }

    IEnumerator FollowPath(Vector3[] waypoints)
    {
        transform.position = waypoints[0];

        int targetWaypointIndex = 1;
        Vector3 targetWaypoints = waypoints[targetWaypointIndex];
        transform.LookAt(targetWaypoints);

        while (!playerSpotted)
        {
            movePosition = Vector3.MoveTowards(transform.position, targetWaypoints, moveSpeed * Time.deltaTime);
            transform.position = movePosition;
            if (transform.position == targetWaypoints)
            {
                targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                targetWaypoints = waypoints[targetWaypointIndex];
                stop = true;
                yield return new WaitForSeconds(pause);

                yield return StartCoroutine(TurnToFace(targetWaypoints));
            }
            else
            {
                stop = false;
            }
            yield return null;
        }

        while (playerSpotted)
        {
            yield return null;
        }
    }

    IEnumerator TurnToFace(Vector3 lookTarget)
    {
        Vector3 dirToLookTarget = (lookTarget - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2(dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;

        if (!playerSpotted)
        {
            while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f)
            {
                float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
                transform.eulerAngles = Vector3.up * angle;
                yield return null;
            }
        }
    }

    void OnDrawGizmos()
    {
        Vector3 startPath = path.GetChild(0).position;
        Vector3 previousPath = startPath;
        foreach (Transform waypoint in path)
        {
            Gizmos.DrawSphere(waypoint.position, .2f);
            Gizmos.DrawLine(previousPath, waypoint.position);
            previousPath = waypoint.position;
        }
        Gizmos.DrawLine(previousPath, startPath);
    }
}
