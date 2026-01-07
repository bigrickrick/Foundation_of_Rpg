using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Door : InteractiveTerrain
{
    public bool IsOpen = false;
    [SerializeField]
    private bool IsRotatingDoor = true;
    [SerializeField]
    private float Speed = 1f;
    [Header("Rotation Configs")]
    [SerializeField]
    private float RotationAmount = 90f;
    [SerializeField]
    private float ForwardDirection = 0;
    [Header("Sliding Configs")]
    [SerializeField]
    private Vector3 SlideDirection = Vector3.back;
    [SerializeField]
    private float SlideAmount = 1.9f;

    private Vector3 StartRotation;
    private Vector3 StartPosition;
    private Vector3 Forward;

    private Coroutine AnimationCoroutine;

    private NavMeshObstacle navMeshObstacle;

    private void Awake()
    {
        StartRotation = transform.rotation.eulerAngles;
        Forward = transform.right;
        StartPosition = transform.position;

        navMeshObstacle = GetComponent<NavMeshObstacle>();
        if (navMeshObstacle == null)
        {
            Debug.LogError("NavMeshObstacle component not found on the door.");
        }
        else
        {
            navMeshObstacle.carving = true;
        }
    }

    public override void interact(EntityLegacy entity)
    {
        if (IsOpen)
        {
            Close();
            IsOpen = false;
        }
        else
        {
            Open(entity.transform.position);
            IsOpen = true;
        }
    }

    public void Open(Vector3 UserPosition)
    {
        if (!IsOpen)
        {
            if (AnimationCoroutine != null)
            {
                StopCoroutine(AnimationCoroutine);
            }

            if (IsRotatingDoor)
            {
                float dot = Vector3.Dot(Forward, (UserPosition - transform.position).normalized);
                Debug.Log($"Dot: {dot.ToString("N3")}");
                AnimationCoroutine = StartCoroutine(DoRotationOpen(dot));
            }
            else
            {
                AnimationCoroutine = StartCoroutine(DoSlidingOpen());
            }
        }
    }

    private IEnumerator DoRotationOpen(float ForwardAmount)
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation;

        if (ForwardAmount >= ForwardDirection)
        {
            endRotation = Quaternion.Euler(new Vector3(0, StartRotation.y + RotationAmount, 0));
        }
        else
        {
            endRotation = Quaternion.Euler(new Vector3(0, StartRotation.y - RotationAmount, 0));
        }

        IsOpen = true;
        navMeshObstacle.carving = false; // Disable carving when door is open

        float time = 0;
        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, time);
            yield return null;
            time += Time.deltaTime * Speed;
        }
    }

    private IEnumerator DoSlidingOpen()
    {
        Vector3 endPosition = StartPosition + SlideAmount * SlideDirection;
        Vector3 startPosition = transform.position;

        float time = 0;
        IsOpen = true;
        navMeshObstacle.carving = false; // Disable carving when door is open

        while (time < 1)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, time);
            yield return null;
            time += Time.deltaTime * Speed;
        }
    }

    public void Close()
    {
        if (IsOpen)
        {
            if (AnimationCoroutine != null)
            {
                StopCoroutine(AnimationCoroutine);
            }

            if (IsRotatingDoor)
            {
                AnimationCoroutine = StartCoroutine(DoRotationClose());
            }
            else
            {
                AnimationCoroutine = StartCoroutine(DoSlidingClose());
            }
        }
    }

    private IEnumerator DoRotationClose()
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(StartRotation);

        IsOpen = false;
        navMeshObstacle.carving = true; // Enable carving when door is closed

        float time = 0;
        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, time);
            yield return null;
            time += Time.deltaTime * Speed;
        }
    }

    private IEnumerator DoSlidingClose()
    {
        Vector3 endPosition = StartPosition;
        Vector3 startPosition = transform.position;
        float time = 0;

        IsOpen = false;
        navMeshObstacle.carving = true; // Enable carving when door is closed

        while (time < 1)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, time);
            yield return null;
            time += Time.deltaTime * Speed;
        }
    }
}
