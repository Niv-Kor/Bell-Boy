using Constants;
using System.Collections.Generic;
using UnityEngine;

public class WindowJumpJourney : Journey
{
    private static readonly float DISTANCE_TO_JUMP = 1;
    private static readonly float DISTANCE_TO_FALL = .7f;
    private static readonly float MAX_JUMP_DISTANCE = .3f;
    private static readonly float FALL_SPEED = 9;

    private GameObject window;
    private Vector3 throwVector, fallPoint;
    private bool thrown, fall;

    public WindowJumpJourney(Passenger passenger, Floor floor, float walkSpeed) :
    base(passenger, floor, walkSpeed) {
        this.thrown = false;
        this.fall = false;
    }

    protected override void Travel(bool finishMovement, bool finishRotation) {
        if (!thrown) {
            //start running
            if (!animationControl.IsAnimating(StateManchine.RUN)) {
                animationControl.Animate(StateManchine.RUN);
                Speed *= passenger.RunSpeedMultiplier;
            }

            //jump at window
            if (finishMovement) {
                animationControl.Animate(StateManchine.JUMP, true);
                animationControl.Animate(StateManchine.RUN, false);
                ContinuePath();
                thrown = true;

                //break glass (once per floor)
                ShatterableGlass glass = window.GetComponentInChildren<ShatterableGlass>();
                if (glass != null) glass.Shatter(new Vector2(0, 0), throwVector);
            }
        }
        else if (finishMovement) {
            //fall
            if (!fall) {
                animationControl.Animate(StateManchine.FALL, true);
                animationControl.Animate(StateManchine.JUMP, false);
                Speed = FALL_SPEED;
                ContinuePath();
                fall = true;
            }
            //crash
            else {
                animationControl.Animate(StateManchine.CRASH, true);
                animationControl.Animate(StateManchine.FALL, false);
                ContinuePath();
            }
        }
    }

    protected override bool Move() {
        transform.position = Vector3.MoveTowards(transform.position, nextPoint, Speed * Time.deltaTime);
        bool reached = VectorSensitivity.EffectivelyReached(transform.position, nextPoint, LERP_TOLERANCE);
        return reached;
    }

    protected override Queue<Vector3> GeneratePath() {
        Queue<Vector3> path = new Queue<Vector3>();
        Vector3 passengerPos = passenger.transform.position;

        //select side
        float leftDist = Vector3.Distance(passengerPos, floor.LeftWindow.transform.position);
        float rightDist = Vector3.Distance(passengerPos, floor.RightWindow.transform.position);
        bool leftSide = leftDist > rightDist;
        throwVector = leftSide ? Vector3.left : Vector3.right;
        window = leftSide ? floor.LeftWindow : floor.RightWindow;

        //select z axis value to jump from
        Vector3 hallCenter = floor.transform.position + floor.WaitingHallContainer.center;
        float z = hallCenter.z + Random.Range(-1f, 1f) * floor.WaitingHallContainer.bounds.size.z / 2;

        //point of jump
        Vector3 jumpPoint = window.transform.position;
        jumpPoint.x -= DISTANCE_TO_JUMP * throwVector.x;
        jumpPoint.y = passengerPos.y;
        jumpPoint.z = z;
        path.Enqueue(jumpPoint);

        //point of fall
        fallPoint = window.transform.position;
        fallPoint.x += DISTANCE_TO_FALL * throwVector.x;
        fallPoint.y = passengerPos.y;
        fallPoint.z = z;
        path.Enqueue(fallPoint);

        //crash point
        bool crashToLeft = fallPoint.x < floor.transform.position.x;
        CrashPoints crashGenerator = Object.FindObjectOfType<CrashPoints>();
        BoxCollider crashArea = crashToLeft ? crashGenerator.LeftArea : crashGenerator.RightArea;
        Vector3 crashPoint = crashToLeft ? crashGenerator.GenerateLeft() : crashGenerator.GenerateRight();

        //modify the crash position so the passenger won't hit another building
        float halfDimension = dimension.y * throwVector.x / 2;
        float areaXCenter = crashArea.bounds.center.x;
        float headX = crashPoint.x + halfDimension;
        float legsX = crashPoint.x - halfDimension;
        float headToCenterDist = Mathf.Abs(headX - areaXCenter);
        float legsToCenterDist = Mathf.Abs(legsX - areaXCenter);
        float xExtent = crashArea.bounds.extents.x;

        if (headToCenterDist > xExtent) crashPoint.x -= (headToCenterDist - xExtent) * throwVector.x;
        else if (legsToCenterDist > xExtent) crashPoint.x += (legsToCenterDist - xExtent) * throwVector.x;

        crashPoint.y += dimension.z * .3f;
        path.Enqueue(crashPoint);

        return path;
    }

    public override void ContinuePath() {
        specialRotationTarget = throwVector * 10_000;
        usingSpecialRotation = true;

        base.ContinuePath();
    }

    protected override void OnStart() {
        passenger.WaitingForElevator = false;
        passenger.TargetMark.Activate(false);
        floorplan.Free(passenger.LeasedPath);
    }

    protected override bool LookAtElevatorOnFinish() { return false; }
}