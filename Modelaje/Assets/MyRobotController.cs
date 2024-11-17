using UnityEngine;

public class MyRobotController : MonoBehaviour
{

    [Header("Joints")]
    public Transform joint0;
    public Transform joint1;
    public Transform joint2;
    public Transform endJoint;

    [Header("Target")]
    public Transform Stud_target;
    public Transform Workbench_destination;
    public Transform wall;
    public Transform firstLerpPosition;
    public Transform secondLerpPosition;
    public Transform thirdLerpPosition;

    [Header("Line Renderer")]
    public LineRenderer lineRenderer1;
    public LineRenderer lineRenderer2;
    public LineRenderer lineRenderer3;

    public enum RobotState { WALKINGTARGET, GRABBINGTARGET, RECOMPOSITEARMTARGET, WALKINGTABLE, DROPPINGTARGET, RECOMPOSITEARMDROPPING, END }
    public RobotState currentState;

    public enum WalkingState { WALKINGFORWARDBEFOREWALL, FIRSTLERP, SECONDLERP, THIRDLERP, FOURTHLERP, WALKINGFORWARDAFTERWALL }
    public WalkingState currentWalkState;

    [Header("Time/Speed")]
    public float grabTime;
    public float robotGrabSpeed;
    public float robotWalkSpeed;

    [Header("Distance")]
    public float targetDistance;
    public float dropDistance;
    public float walkingTargetDistance;
    public float walkingTableDistance;
    public float startWallRotation;

    [Header("Amplitudes")]
    public float joint1Amplitude;
    public float joint2Amplitude;
    public float endJointAmplitudeGrab;
    public float endJointAmplitudeDrop;

    private Quaternion initJoint1Rotation;
    private Quaternion initJoint2Rotation;

    float joint1Rotation;
    float joint2Rotation;
    float endJointRotation;

    [Header("Rotation")]
    public float firstLerpDistance;
    public float secondLerpDistance;
    public float thirdLerpDistance;
    public float rotationDegree;

    private float wallInitialDistance;
    private Quaternion initialRotation;


    private void Start()
    {
        InitializeLineRenderer(lineRenderer1);
        InitializeLineRenderer(lineRenderer2);
        InitializeLineRenderer(lineRenderer3);
        InitValues();

        initialRotation = transform.rotation;
    }

    private void InitValues()
    {
        joint1Rotation = Mathf.Sin(grabTime) * joint1Amplitude;
        joint2Rotation = Mathf.Sin(-(grabTime + Mathf.PI / 4)) * joint2Amplitude;
        endJointRotation = Mathf.Sin(-(grabTime + Mathf.PI / 2)) * endJointAmplitudeGrab;

        joint1.localRotation = Quaternion.Euler(joint1Rotation, 0, 0);
        joint2.localRotation = Quaternion.Euler(joint2Rotation, 0, 0);
        endJoint.localRotation = Quaternion.Euler(endJointRotation, 0, 0);

        initJoint1Rotation = joint1.rotation;
        initJoint2Rotation = joint2.rotation;

        UpdateVisualLinks();
    }

    private void Update()
    {
        PickStudAnim();
        UpdateVisualLinks();
    }

    private void PickStudAnim()
    {
        switch(currentState) 
        { 
            case RobotState.WALKINGTARGET:
                WalkingToTarget();
                break;
            case RobotState.GRABBINGTARGET:
                Grab();
                break;
            case RobotState.RECOMPOSITEARMTARGET:
                RecompositeArmTarget();
                break;
            case RobotState.WALKINGTABLE:
                WalkingToTable();
                break;
            case RobotState.DROPPINGTARGET:
                Drop();
                break;
            case RobotState.RECOMPOSITEARMDROPPING:
                RecompositeArmDrop();
                break;
            case RobotState.END:
                break;
        }
    }

    private void WalkingToTarget()
    {
        transform.position = new Vector3(transform.position.x + (robotWalkSpeed * Mathf.Sin(transform.eulerAngles.y * Mathf.Deg2Rad) * Time.deltaTime),
            transform.position.y,
            transform.position.z + (robotWalkSpeed * Mathf.Cos(transform.eulerAngles.y * Mathf.Deg2Rad) * Time.deltaTime));

       switch(currentWalkState)
       {
            case WalkingState.WALKINGFORWARDBEFOREWALL:
                if(Vector3.Distance(transform.position, firstLerpPosition.position) < startWallRotation)
                {
                    firstLerpDistance = firstLerpPosition.position.z - transform.position.z;
                    currentWalkState = WalkingState.FIRSTLERP;
                }
                break;
            case WalkingState.FIRSTLERP:
                transform.rotation = Quaternion.Lerp(initialRotation, Quaternion.Euler(0, rotationDegree, 0), 1 - ((firstLerpPosition.position.z - transform.position.z) / firstLerpDistance));
                if (transform.position.z > firstLerpPosition.position.z)
                {
                    wallInitialDistance = wall.position.z - transform.position.z;
                    currentWalkState = WalkingState.SECONDLERP;
                }
                break;
            case WalkingState.SECONDLERP:
                transform.rotation = Quaternion.Lerp(Quaternion.Euler(0, rotationDegree, 0), Quaternion.Euler(0, 0, 0), 1 - ((wall.position.z - transform.position.z) / wallInitialDistance));
                if (transform.position.z > wall.position.z)
                {
                    secondLerpDistance = secondLerpPosition.position.z - transform.position.z;
                    currentWalkState = WalkingState.THIRDLERP;
                }
                break;
            case WalkingState.THIRDLERP:
                transform.rotation = Quaternion.Lerp(Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, -rotationDegree, 0), 1 - ((secondLerpPosition.position.z - transform.position.z) / secondLerpDistance));
                if (transform.position.z > secondLerpPosition.position.z)
                {
                    thirdLerpDistance = thirdLerpPosition.position.z - transform.position.z;
                    currentWalkState = WalkingState.FOURTHLERP;
                }
                break;
            case WalkingState.FOURTHLERP:
                transform.rotation = Quaternion.Lerp(Quaternion.Euler(0, -rotationDegree, 0), Quaternion.Euler(0, 0, 0), 1 - ((thirdLerpPosition.position.z - transform.position.z) / thirdLerpDistance));
                if (transform.position.z > thirdLerpPosition.position.z)
                {
                    currentWalkState = WalkingState.WALKINGFORWARDAFTERWALL;
                }
                break;
            case WalkingState.WALKINGFORWARDAFTERWALL:
                break;
        }
        if (Vector3.Distance(transform.position, Stud_target.position) < walkingTargetDistance)
            currentState = RobotState.GRABBINGTARGET;
    }

    private void WalkingToTable()
    {
        transform.position = new Vector3(transform.position.x + (robotWalkSpeed * Mathf.Sin(transform.eulerAngles.y * Mathf.Deg2Rad) * Time.deltaTime),
            transform.position.y,
            transform.position.z + (robotWalkSpeed * Mathf.Cos(transform.eulerAngles.y * Mathf.Deg2Rad) * Time.deltaTime));
        if (Vector3.Distance(transform.position, Workbench_destination.position) < walkingTableDistance)
            currentState = RobotState.DROPPINGTARGET;
    }

    private void Grab()
    {
        grabTime += Time.deltaTime * robotGrabSpeed;

        CalculateRotation(endJointAmplitudeGrab);
        CalculateJointRotation();

        if (Vector3.Distance(endJoint.position, Stud_target.position) < targetDistance)
        {
            currentState = RobotState.RECOMPOSITEARMTARGET;
            Stud_target.SetParent(endJoint, true);
        }
    }

    private void Drop()
    {
        grabTime += Time.deltaTime * robotGrabSpeed;

        CalculateRotation(endJointAmplitudeDrop);
        CalculateJointRotation();


        if (Vector3.Distance(Stud_target.position, Workbench_destination.position) < dropDistance)
        {
            currentState = RobotState.RECOMPOSITEARMDROPPING;
            Stud_target.SetParent(null);
        }
    }

    private void RecompositeArmTarget()
    {
        grabTime -= Time.deltaTime * robotGrabSpeed;

        CalculateRotation(endJointAmplitudeGrab);
        CalculateJointRotationWithConditions(RobotState.WALKINGTABLE);
    }

    private void RecompositeArmDrop()
    {
        grabTime -= Time.deltaTime * robotGrabSpeed;

        CalculateRotation(endJointAmplitudeDrop);
        CalculateJointRotationWithConditions(RobotState.END);   
    }

    private void CalculateRotation(float _amplitude)
    {
        joint1Rotation = Mathf.Sin(grabTime) * joint1Amplitude;
        joint2Rotation = Mathf.Sin(-(grabTime + Mathf.PI / 4)) * joint2Amplitude;
        endJointRotation = Mathf.Sin(-(grabTime + Mathf.PI / 2)) * _amplitude;
    }

    private void CalculateJointRotation()
    {
        joint1.localRotation = Quaternion.Euler(joint1Rotation, 0, 0);
        joint2.localRotation = Quaternion.Euler(joint2Rotation, 0, 0);
        endJoint.localRotation = Quaternion.Euler(endJointRotation, 0, 0);
    }

    private void CalculateJointRotationWithConditions(RobotState state)
    {
        if (joint1.localRotation.x > initJoint1Rotation.x)
            joint1.localRotation = Quaternion.Euler(joint1Rotation, 0, 0);
        if (joint2.localRotation.x > initJoint2Rotation.x)
            joint2.localRotation = Quaternion.Euler(joint2Rotation, 0, 0);
        else
        {
            grabTime = 0;
            currentState = state;
        }
        endJoint.localRotation = Quaternion.Euler(endJointRotation, 0, 0);
    }

    void InitializeLineRenderer(LineRenderer lineRenderer)
    {
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
    }


    void UpdateVisualLinks()
    {
        lineRenderer1.SetPosition(0, joint0.position);
        lineRenderer1.SetPosition(1, joint1.position);

        lineRenderer2.SetPosition(0, joint1.position);
        lineRenderer2.SetPosition(1, joint2.position);

        lineRenderer3.SetPosition(0, joint2.position);
        lineRenderer3.SetPosition(1, endJoint.position);
    }
}
