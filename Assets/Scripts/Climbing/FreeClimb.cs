using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralClimbing
{
    public class FreeClimb : MonoBehaviour {
        public Animator anim;
        public bool isClimbing;
        public bool isMid;

        bool inPosition;
        bool isLerping;
        float t;
        float delta;
        Vector3 startPos;
        Vector3 targetPos;
        Quaternion startRot;
        Quaternion targetRot;
        public float positionOffset;
        public float offsetFromWall = 0.3f;
        public float speedMultiplier = 0.2f;
        public float climbSpeed = 3;
        public float rotateSpeed = 5;
        public float inAngleDistance = 1;

        public IKSnapshot baseIKsnapshot;

        public FreeClimbAnimHook aHook;

        Transform helper;

        // Use this for initialization
        void Start () {
            Init();
        }
        
        // Update is called once per frame
        void Update () {
            delta = Time.deltaTime;
            Tick(delta);
        }

        public void Init()
        {
            helper = new GameObject().transform;
            helper.name = "Climb Helper";
            aHook.Init(this, helper);
            CheckForClimb();
        }

        public void Tick(float delta)
        {
            if(!inPosition)
            {
                GetInPosition();
                return;
            }

            if(!isLerping)
            {
                float hor = Input.GetAxis("Horizontal");
                float vert = Input.GetAxis("Vertical");
                float m = Mathf.Abs(hor) + Mathf.Abs(vert);

                Vector3 h = helper.right * hor;
                Vector3 v = helper.up * vert;
                Vector3 moveDir = (h + v).normalized;

                if(isMid)
                {
                    if(moveDir == Vector3.zero)
                    {
                        return;
                    }
                }
                else
                {
                    bool canMove = CanMove(moveDir);
                    if (!canMove || moveDir == Vector3.zero)
                    {
                        return;
                    }
                }

                isMid = !isMid;


                t = 0;
                isLerping = true;
                startPos = transform.position;
                Vector3 tp = helper.position - transform.position;
                float distance = Vector3.Distance(helper.position, startPos) / 2;
                tp *= positionOffset;
                tp += transform.position;
                targetPos = isMid ? tp : helper.position;

                aHook.CreatePositions(targetPos, moveDir, isMid);
            }
            else
            {
                t += delta * climbSpeed;
                if(t > 1)
                {
                    t = 1;
                    isLerping = false;
                }

                Vector3 cp = Vector3.Lerp(startPos, targetPos, t);
                transform.position = cp;
                transform.rotation = Quaternion.Slerp(transform.rotation, helper.rotation, delta * rotateSpeed);
            }
        }

        public void CheckForClimb()
        {
            Vector3 origin = transform.position;
            origin.y += 1.4f;
            Vector3 dir = transform.forward;
            RaycastHit hit;
            if(Physics.Raycast(origin, dir, out hit, 5))
            {
                helper.position = PosWithOffset(origin, hit.point);
                InitForClimb(hit);
            }
        }

        void InitForClimb(RaycastHit hit)
        {
            isClimbing = true;
            helper.transform.rotation = Quaternion.LookRotation(-hit.normal);
            startPos = transform.position;
            targetPos = hit.point + (hit.normal * offsetFromWall);
            t = 0;
            inPosition = false;
            anim.CrossFade("Hanging Idle", 2);
        }

        bool CanMove(Vector3 moveDir)
        {
            Vector3 origin = transform.position;
            float dis = positionOffset;
            Vector3 dir = moveDir;
            Debug.DrawRay(origin, dir * dis);
            RaycastHit hit;

            if(Physics.Raycast(origin, dir, out hit, dis))
            {
                return false;
            }

            origin += moveDir * dis;
            dir = helper.forward;

            float dis2 = inAngleDistance;

            Debug.DrawRay(origin, dir * dis2);
            if(Physics.Raycast(origin, dir, out hit, dis))
            {
                helper.position = PosWithOffset(origin, hit.point);
                helper.rotation = Quaternion.LookRotation(-hit.normal);
                return true;
            }

            origin += dir * dis2;
            dir = -Vector3.up;

            Debug.DrawRay(origin, dir);
            if(Physics.Raycast(origin, dir, out hit, dis2))
            {
                float angle = Vector3.Angle(helper.up, hit.normal);
                if(angle < 40)
                {
                    helper.position = PosWithOffset(origin, hit.point);
                    helper.rotation = Quaternion.LookRotation(-hit.normal);
                    return true;
                }
            }

            return false;
        }
        void GetInPosition()
        {
            t += delta;

            if(t > 1)
            {
                t = 1;
                inPosition = true;

                aHook.CreatePositions(targetPos, Vector3.zero, false);
            }

            Vector3 tp = Vector3.Lerp(startPos, targetPos, t);
            transform.position = tp;
            transform.rotation = Quaternion.Slerp(transform.rotation, helper.rotation, delta * rotateSpeed);
        }

        Vector3 PosWithOffset(Vector3 origin, Vector3 target)
        {
            Vector3 direction = origin - target;
            direction.Normalize();
            Vector3 offset = direction * offsetFromWall;
            return target + offset;
        }

    }

    [System.Serializable]
    public class IKSnapshot
    {
        public Vector3 rh, lh, rf, lf;
    }
}
