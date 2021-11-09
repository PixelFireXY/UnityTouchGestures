using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelFingerEdit : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform myCamera = null;

    [Header("Setup")]
    [SerializeField] private Vector3 defaultLocalPos;

    [Header("Pan")]
    [SerializeField] private /*const*/ float PanSpeed = 1f;

    [SerializeField] private /*const*/ float PanDirectionThreshold = 25f;
    [SerializeField] private /*const*/ float PanMagnitudeThreshold = 0.9f;

    [Header("Rotation")]
    [SerializeField] private /*const*/ float RotationSpeed = 30f;

    [SerializeField] private /*const*/ float RotationThreshold = 0.9f;

    [Header("Zoom")]
    private const float ZoomOutMin = 1f;
    private const float ZoomOutMax = 8f;
    [SerializeField] private /*const*/ float ZoomSpeed = 0.01f;

    [SerializeField] private /*const*/ float ZoomThreshold = 0.9f;

    /******************** Mono ********************/

    private void Awake()
    {
        TouchManager.OnOneTouchEnter += OnOneTouchEnter;
        TouchManager.OnOneTouchStay += OnOneTouchStay;
        TouchManager.OnOneTouchExit += OnOneTouchExit;

        TouchManager.OnTwoTouchEnter += OnTwoTouchesEnter;
        TouchManager.OnTwoTouchStay += OnTwoTouchesStay;
        TouchManager.OnTwoTouchExit += OnTwoTouchesExit;
    }

    private void OnDestroy()
    {
        TouchManager.OnOneTouchEnter -= OnOneTouchEnter;
        TouchManager.OnOneTouchStay -= OnOneTouchStay;
        TouchManager.OnOneTouchExit -= OnOneTouchExit;

        TouchManager.OnTwoTouchEnter -= OnTwoTouchesEnter;
        TouchManager.OnTwoTouchStay -= OnTwoTouchesStay;
        TouchManager.OnTwoTouchExit -= OnTwoTouchesExit;
    }

    private void Start()
    {
        Setup();
    }

    /******************** Init ********************/

    public void Setup()
    {
        transform.localPosition = defaultLocalPos;

        // Because when we pan, the model moves always on the oposite we want to go,
        // we must invert the speed to a negative value
        PanSpeed = -PanSpeed;

        if (myCamera == null)
            myCamera = Camera.main.transform;
    }

    /******************** Touch events ********************/

    private void OnOneTouchEnter(Vector2 new0)
    {

    }

    private void OnOneTouchStay(Vector2 old0, Vector2 new0)
    {
        ApplyPan_XZ(old0, new0);
    }

    private void OnOneTouchExit(Vector2 old0)
    {

    }

    private void OnTwoTouchesEnter(Vector2 new0, Vector2 new1)
    {

    }

    private void OnTwoTouchesExit(Vector2 old0, Vector2 old1)
    {

    }

    private void OnTwoTouchesStay(Vector2 old0, Vector2 old1, Vector2 new0, Vector2 new1)
    {
        if (ApplyPan_Y(old0, old1, new0, new1)) { }
        else if (ApplyRotation(old0, old1, new0, new1)) { }
        else if (ApplyZoom(old0, old1, new0, new1)) { }
    }

    /******************** Edit Model ********************/

    private bool ApplyPan_XZ(Vector2 old0, Vector2 new0)
    {
        Vector2 panDirection = old0 - new0;

        // Transform the current direction to camera relative direction
        Vector3 relativeCameraX = myCamera.right * panDirection.x;
        Vector3 relativeCameraZ = myCamera.forward * panDirection.y;

        // Prepare the new direction to move the model
        Vector3 newDirection = relativeCameraX + relativeCameraZ;
        newDirection.y = 0f;

        if (newDirection.magnitude < PanMagnitudeThreshold)
            return false;

        transform.Translate(PanSpeed * Time.deltaTime * -newDirection);

        return true;
    }

    private bool ApplyPan_Y(Vector2 old0, Vector2 old1, Vector2 new0, Vector2 new1)
    {
        Vector2 ZeroDir = old0 - new0;
        Vector2 oneDir = old1 - new1;

        if(Vector2.Angle(oneDir, ZeroDir) > PanDirectionThreshold)
            return false;

        Vector3 oldMiddlePos = (old0 + old1) / 2;
        Vector3 newMiddlePos = (new0 + new1) / 2;

        Vector3 posDirection = oldMiddlePos - newMiddlePos;
        Vector3 newDirection = new Vector3(0f, posDirection.y, 0f);

        if (newDirection.magnitude < PanMagnitudeThreshold)
            return false;

        transform.Translate(PanSpeed * Time.deltaTime * -newDirection);

        return true;
    }

    private bool ApplyRotation(Vector2 old0, Vector2 old1, Vector2 new0, Vector2 new1)
    {
        Vector3 oldDir = (old1 - old0);
        Vector3 currDir = (new1 - new0);

        float rotationAngle = Vector2.Angle(oldDir, currDir);

        if (rotationAngle < RotationThreshold)          // Prevent to confuse the zoom with the rotation
            return false;

        // Set the rotation left or right
        Vector3 cross = Vector3.Cross(oldDir, currDir);
        if (cross.z > 0)
            rotationAngle *= -1;

        transform.Rotate(new Vector3(0, rotationAngle * RotationSpeed * Time.deltaTime, 0), Space.World);

        return true;
    }

    private bool ApplyZoom(Vector2 old0, Vector2 old1, Vector2 new0, Vector2 new1)
    {
        float oldDist = Vector3.Distance(old0, old1);
        float newDist = Vector3.Distance(new0, new1);
        float deltaDist = newDist - oldDist;

        float scaleCalculation = Mathf.Clamp(transform.localScale.x + deltaDist * ZoomSpeed, ZoomOutMin, ZoomOutMax);

        if (scaleCalculation < ZoomThreshold)
            return false;

        transform.localScale = Vector3.one * scaleCalculation;

        return true;
    }
}
