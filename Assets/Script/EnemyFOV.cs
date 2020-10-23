using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class EnemyFOV : MonoBehaviour
{

    public bool playerIsSpotted;

    [Range(0, 360)]
    public float Angle;
    public float Radius;

    public LayerMask playerMask;
    public LayerMask obstacle;
    Transform player;

    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();

    public float meshResolution;

    public int edgeResolveIterations;
    public float edgeDistanceThreshold;

    public MeshFilter viewMeshFilter;
    public MeshRenderer colorSet;

    public Material spotTex;
    public Material idleTex;
    Color originalColor;
    Color spottedColor;
    
    Mesh viewMesh;

    public float spotTimer = .5f;
    float playerSpottedTimer = 0;

    private void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;

        StartCoroutine("FindTargetsWithDelay", .1f);

        player = GameObject.FindGameObjectWithTag("Player").transform;
        originalColor = idleTex.color;
        spottedColor = spotTex.color;
    }


    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            InViewTargets();
        }
    }

    private void LateUpdate()
    {
        DrawFieldOfView();
    }

    private void Update()
    {
        if(visibleTargets.Count > 0)
        {
            playerSpottedTimer += Time.deltaTime;
        }
        else
        {
            playerSpottedTimer -= Time.deltaTime;
        }
        playerSpottedTimer = Mathf.Clamp(playerSpottedTimer, 0, spotTimer);
        colorSet.material.color = Color.Lerp(originalColor, spottedColor, playerSpottedTimer / spotTimer);

        if(playerSpottedTimer >= spotTimer)
        {
            playerIsSpotted = true;
        }
    } 

    void InViewTargets()
    {
        
        visibleTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, Radius, playerMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform targetSighted = targetsInViewRadius[i].transform;
            Vector3 DirectionToTarget = (targetSighted.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, DirectionToTarget) < Angle / 2)
            {
                float DistanceToTarget = Vector3.Distance(transform.position, targetSighted.position);

                if (!Physics.Raycast(transform.position, DirectionToTarget, DistanceToTarget, obstacle) )
                {
                    visibleTargets.Add(targetSighted);
                }
                
            }
        }
    }

    /*bool CanSeePlayer()
    {
        if(Vector3.Distance(transform.position,player.position) < Radius)
        {
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            float angleBetweenGuardAndplayer = Vector3.Angle(transform.forward, dirToPlayer);
            if(angleBetweenGuardAndplayer < Angle / 2f)
            {
                if(!Physics.Linecast(transform.position, player.position, obstacle))
                {
                    return true;
                }
            }
        }
        return false;
    }*/

    void DrawFieldOfView()
    {
        int rayCount = Mathf.RoundToInt(Angle * meshResolution);
        float rayAngleSize = Angle / rayCount;
        

        List<Vector3> viewPoints = new List<Vector3>();

        ViewCastInfo oldViewCast = new ViewCastInfo();
        for (int i = 0; i <= rayCount; i++)
        {
            float rayAngle = transform.eulerAngles.y - Angle / 2 + rayAngleSize * i;
            //Debug.DrawLine(transform.position, transform.position + DirectionFromAngle(rayAngle, true) * Radius, Color.red);
            ViewCastInfo newViewCast = ViewCast(rayAngle);

            if (i > 0)
            {
                bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.v_dst - newViewCast.v_dst) > edgeDistanceThreshold;
                if(oldViewCast.v_hit != newViewCast.v_hit || (oldViewCast.v_hit && newViewCast.v_hit && edgeDstThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if(edge.pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }
            }

            viewPoints.Add(newViewCast.v_point);
            oldViewCast = newViewCast;
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if(i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }
        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.v_angle;
        float maxAngle = maxViewCast.v_angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float midAngle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(midAngle);

            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.v_dst - newViewCast.v_dst) > edgeDistanceThreshold;
            if (newViewCast.v_hit == minViewCast.v_hit && !edgeDstThresholdExceeded)
            {
                minAngle = midAngle;
                minPoint = newViewCast.v_point;
            }
            else
            {
                maxAngle = midAngle;
                maxPoint = newViewCast.v_point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    ViewCastInfo ViewCast(float gloabalAngle)
    {
        Vector3 dir = DirectionFromAngle(gloabalAngle, true);
        RaycastHit hit;

        if(Physics.Raycast(transform.position, dir, out hit, Radius, obstacle))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, gloabalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * Radius, Radius, gloabalAngle);
        }
    }

    public Vector3 DirectionFromAngle(float angle, bool GlobalAngle)
    {
        if (!GlobalAngle)
        {
            angle += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    public struct ViewCastInfo
    {
        public bool v_hit;
        public Vector3 v_point;
        public float v_dst;
        public float v_angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            v_hit = _hit;
            v_point = _point;
            v_dst = _dst;
            v_angle = _angle;
        }
    }

    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }
}
