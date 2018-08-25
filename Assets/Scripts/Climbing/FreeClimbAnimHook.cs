using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralClimbing
{
    public class FreeClimbAnimHook : MonoBehaviour {
        Animator anim;

        IKSnapshot ikBase;
        IKSnapshot current = new IKSnapshot();
        IKSnapshot next = new IKSnapshot();

        public float weight_rh;
        public float weight_lh;
        public float weight_rf;
        public float weight_lf;

        Vector3 rh, lh, rf, lf;
        Transform helper;
        bool isMirror;

        public float wallOffset = 0f;

        public void Init(FreeClimb c, Transform helper)
        {
            anim = c.anim;
            ikBase = c.baseIKsnapshot;
            this.helper = helper; 
        }

        public void CreatePositions(Vector3 origin, Vector3 moveDir, bool isMid)
        {
            HandleAnim(moveDir, isMid);

            IKSnapshot ik = CreateSnapShot(origin);
            CopySnapshot(ref current, ik);

            UpdateIKPosition(AvatarIKGoal.LeftHand, current.lh);
            UpdateIKPosition(AvatarIKGoal.RightHand, current.rh);
            UpdateIKPosition(AvatarIKGoal.LeftFoot, current.lf);
            UpdateIKPosition(AvatarIKGoal.RightFoot, current.rf);

            UpdateIKWeight(AvatarIKGoal.LeftHand, 1);
            UpdateIKWeight(AvatarIKGoal.RightHand, 1);
            UpdateIKWeight(AvatarIKGoal.LeftFoot, 1);
            UpdateIKWeight(AvatarIKGoal.RightFoot, 1);
        }

        void HandleAnim(Vector3 moveDir, bool isMid)
        {
            if(isMid)
            {
                if(moveDir.y != 0)
                {
                    if(moveDir.y < 0)
                    {

                    }
                    else
                    {

                    }

                    isMirror = !isMirror;
                    anim.SetBool("mirror", isMirror);

                    anim.CrossFade("Climb Up", 0.2f);
                }
            }
            else
            {
                anim.CrossFade("Hanging Idle", 0.2f);
            }
        }

        public IKSnapshot CreateSnapShot(Vector3 origin)
        {
            IKSnapshot r = new IKSnapshot();
            Vector3 _lh = LocalToWorld(ikBase.lh);
            r.lh = GetPosActual(_lh);
            Vector3 _rh = LocalToWorld(ikBase.rh);
            r.rh = GetPosActual(_rh);
            Vector3 _lf = LocalToWorld(ikBase.lf);
            r.lf = GetPosActual(_lf);
            Vector3 _rf = LocalToWorld(ikBase.rf);
            r.rf = GetPosActual(_rf);
            return r;
        }

        Vector3 LocalToWorld(Vector3 position)
        {
            Vector3 r = helper.position;
            r += helper.right * position.x;
            r += helper.forward * position.z;
            r += helper.up * position.y;
            return r;
        }

        Vector3 GetPosActual(Vector3 o)
        {
            Vector3 returnPos = o;
            Vector3 origin = o;
            Vector3 dir = helper.forward;

            origin += -(dir * 0.2f);
            RaycastHit hit;

            if(Physics.Raycast(origin, dir, out hit, 1.5f))
            {
                Vector3 _r = hit.point + (hit.normal * wallOffset);
                returnPos = _r;


            }

            return returnPos;
        }
        
        public void CopySnapshot(ref IKSnapshot to, IKSnapshot from)
        {
            to.lh = from.lh;
            to.rh = from.rh;
            to.lf = from.lf;
            to.rf = from.rf;
        }

        public void UpdateIKPosition(AvatarIKGoal goal, Vector3 pos)
        {
            switch(goal)
            {
                case AvatarIKGoal.LeftHand:
                    lh = pos;
                    break;
                case AvatarIKGoal.RightHand:
                    rh = pos;
                    break;
                case AvatarIKGoal.LeftFoot:
                    lf = pos;
                    break;
                case AvatarIKGoal.RightFoot:
                    rf = pos;
                    break;
            }
        }
        public void UpdateIKWeight(AvatarIKGoal goal, float weight)
        {
            switch(goal)
            {
                case AvatarIKGoal.LeftHand:
                    weight_lh = weight;
                    break;
                case AvatarIKGoal.RightHand:
                    weight_rh = weight;
                    break;
                case AvatarIKGoal.LeftFoot:
                    weight_lf = weight;
                    break;
                case AvatarIKGoal.RightFoot:
                    weight_rf = weight;
                    break;
            }
        }


        private void OnAnimatorIK(int layerIndex)
        {
            SetIKPos(AvatarIKGoal.LeftHand, lh, weight_lh);
            SetIKPos(AvatarIKGoal.RightHand, rh, weight_rh);
            SetIKPos(AvatarIKGoal.LeftFoot, lf, weight_lf);
            SetIKPos(AvatarIKGoal.RightFoot, rf, weight_rf);
        }

        void SetIKPos(AvatarIKGoal goal, Vector3 targetPos, float weight)
        {
            anim.SetIKPositionWeight(goal, weight);
            anim.SetIKPosition(goal, targetPos);
        }

        // Use this for initialization
        void Start () {
            
        }
        
        // Update is called once per frame
        void Update () {
            
        }
    }
}
