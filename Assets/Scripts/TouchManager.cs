using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchManager : MonoBehaviour
{
    public static event Action<Vector2> OnOneTouchEnter;
    public static event Action<Vector2, Vector2> OnOneTouchStay;
    public static event Action<Vector2> OnOneTouchExit;

    private Vector2 lastOneTouchStay;

    public static event Action<Vector2, Vector2> OnTwoTouchEnter;
    public static event Action<Vector2, Vector2, Vector2, Vector2> OnTwoTouchStay;
    public static event Action<Vector2, Vector2> OnTwoTouchExit;

    private Vector2 lastTwoTouchStay_0;
    private Vector2 lastTwoTouchStay_1;

    /******************** Mono ********************/

    void Update()
    {
        int touches = Input.touchCount;

        switch (touches)
        {
            case 1:
                {
                    ManageOneTouch();
                    break;
                }
            case 2:
                {
                    ManageTwoTouches();
                    break;
                }
        }
    }

    /******************** Touch managing ********************/

    private void ManageOneTouch()
    {
        Touch firstTouch = Input.GetTouch(0);

        Vector2 touchPosition = firstTouch.position;

        switch (firstTouch.phase)
        {
            case TouchPhase.Began:
                InvokeOneTouchEnter(touchPosition);
                break;
            case TouchPhase.Moved:
                InvokeOneTouchStay(touchPosition);
                break;
            case TouchPhase.Stationary:
                InvokeOneTouchStay(touchPosition);
                break;
            case TouchPhase.Ended:
                InvokeOneTouchExit(touchPosition);
                break;
            case TouchPhase.Canceled:
                InvokeOneTouchExit(touchPosition);
                break;
        }
    }

    private void ManageTwoTouches()
    {
        Touch firstTouch = Input.GetTouch(0);
        Touch secondTouch = Input.GetTouch(1);

        Vector2 firstTouchPosition = firstTouch.position;
        Vector2 secondTouchPosition = secondTouch.position;

        switch (firstTouch.phase)
        {
            case TouchPhase.Began:
                InvokeTwoTouchEnter(firstTouchPosition, secondTouchPosition);
                break;
            case TouchPhase.Moved:
                InvokeTwoTouchStay(firstTouchPosition, secondTouchPosition);
                break;
            case TouchPhase.Stationary:
                InvokeTwoTouchStay(firstTouchPosition, secondTouchPosition);
                break;
            case TouchPhase.Ended:
                InvokeTwoTouchExit(firstTouchPosition, secondTouchPosition);
                break;
            case TouchPhase.Canceled:
                InvokeTwoTouchExit(firstTouchPosition, secondTouchPosition);
                break;
        }
    }

    /******************** One touch invoke methods ********************/

    private void InvokeOneTouchEnter(Vector2 newTouchPos)
    {
        lastOneTouchStay = newTouchPos;

        OnOneTouchEnter?.Invoke(newTouchPos);
    }

    private void InvokeOneTouchStay(Vector2 newTouchPos)
    {
        OnOneTouchStay?.Invoke(lastOneTouchStay, newTouchPos);

        lastOneTouchStay = newTouchPos;
    }

    private void InvokeOneTouchExit(Vector2 newTouchPos)
    {
        OnOneTouchExit?.Invoke(newTouchPos);
    }

    /******************** Two touches invoke methods ********************/

    private void InvokeTwoTouchEnter(Vector2 newFirstTouchPos, Vector2 newSecondTouchPos)
    {
        lastTwoTouchStay_0 = newFirstTouchPos;
        lastTwoTouchStay_1 = newSecondTouchPos;

        OnTwoTouchEnter?.Invoke(newFirstTouchPos, newSecondTouchPos);
    }

    private void InvokeTwoTouchStay(Vector2 newFirstTouchPos, Vector2 newSecondTouchPos)
    {
        OnTwoTouchStay?.Invoke(lastTwoTouchStay_0, lastTwoTouchStay_1, newFirstTouchPos, newSecondTouchPos);

        lastTwoTouchStay_0 = newFirstTouchPos;
        lastTwoTouchStay_1 = newSecondTouchPos;
    }

    private void InvokeTwoTouchExit(Vector2 newFirstTouchPos, Vector2 newSecondTouchPos)
    {
        OnTwoTouchExit?.Invoke(newFirstTouchPos, newSecondTouchPos);
    }
}
