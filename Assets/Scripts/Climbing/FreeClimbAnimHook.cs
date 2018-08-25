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
        IKGoals goals = new IKGoals();

        public float weight_rh;
        public float weight_lh;
        public float weight_rf;
        public float weight_lf;

        Vector3 rh, lh, rf, lf;
        Transform helper;
        bool isMirror;
        bool isLeft;
        Vector3 prevMovDir;
        List<IKStates> ikStates = new List<IKStates>();

        float delta;
        public float lerpSpeed = 1.0f;
        public float wallOffset = 0f;

        public void Init(FreeClimb c, Transform helper)
        {
            anim = c.anim;
            ikBase = c.baseIKsnapshot;
            this.helper = helper; 
        }

        public void CreatePositions(Vector3 origin, Vector3 moveDir, bool isMid)
        {
            delta = Time.deltaTime;
            HandleAnim(moveDir, isMid);

            IKSnapshot ik = CreateSnapShot(origin);
            CopySnapshot(ref current, ik);

            if(!isMid)
            {
                UpdateGoals(moveDir);
                prevMovDir = moveDir;
            }
            else
            {
                UpdateGoals(prevMovDir);
            }

            SetIKPosition(isMid, goals.lh, current.lh, AvatarIKGoal.LeftHand);
            SetIKPosition(isMid, goals.rh, current.rh, AvatarIKGoal.RightHand);
            SetIKPosition(isMid, goals.lf, current.lf, AvatarIKGoal.LeftFoot);
            SetIKPosition(isMid, goals.rf, current.rf, AvatarIKGoal.RightFoot);

            UpdateIKWeight(AvatarIKGoal.LeftHand, 1);
            UpdateIKWeight(AvatarIKGoal.RightHand, 1);
            UpdateIKWeight(AvatarIKGoal.LeftFoot, 1);
            UpdateIKWeight(AvatarIKGoal.RightFoot, 1);
        }

        void UpdateGoals(Vector3 moveDir)
        {
            isLeft = moveDir.x <= 0;

            goals.lh = isLeft;
            goals.rh = !isLeft;
            goals.lf = isLeft;
            goals.rf = !isLeft;
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

        void SetIKPosition(bool isMid, bool isTrue, Vector3 pos, AvatarIKGoal goal)
        {
            if(isMid)
            {
                if(isTrue)
                {
                    Vector3 p = GetPosActual(pos);
                    UpdateIKPosition(goal, p);
                }
            }
            else
            {
                if(!isTrue)
                {
                    Vector3 p = GetPosActual(pos);
                    UpdateIKPosition(goal, p);
                }
            }


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
            delta = Time.deltaTime;

            SetIKPos(AvatarIKGoal.LeftHand, lh, weight_lh);
            SetIKPos(AvatarIKGoal.RightHand, rh, weight_rh);
            SetIKPos(AvatarIKGoal.LeftFoot, lf, weight_lf);
            SetIKPos(AvatarIKGoal.RightFoot, rf, weight_rf);
        }

        void SetIKPos(AvatarIKGoal goal, Vector3 targetPos, float weight)
        {
            IKStates ikState = GetIKStates(goal);
            if(ikState == null)
            {
                ikState = new IKStates();
                ikState.goal = goal;
                ikStates.Add(ikState);
            }

            if(weight == 0)
            {
                ikState.isSet = false;
            }

            if(!ikState.isSet)
            {
                ikState.position = GoalToBodyBones(goal).position;
                ikState.isSet = true;
            }

            ikState.positionWeight = weight;
            ikState.position = Vector3.Lerp(ikState.position, targetPos, delta * lerpSpeed);

            anim.SetIKPositionWeight(goal, ikState.positionWeight);
            anim.SetIKPosition(goal, ikState.position);
        }

        Transform GoalToBodyBones(AvatarIKGoal goal)
        {
            switch (goal)
            {
                case AvatarIKGoal.LeftFoot:
                    return anim.GetBoneTransform(HumanBodyBones.LeftFoot);
                case AvatarIKGoal.RightFoot:
                    return anim.GetBoneTransform(HumanBodyBones.RightFoot);
                case AvatarIKGoal.LeftHand:
                    return anim.GetBoneTransform(HumanBodyBones.LeftHand);
                default:
                case AvatarIKGoal.RightHand:
                    return anim.GetBoneTransform(HumanBodyBones.RightHand);
            }
        }

        IKStates GetIKStates(AvatarIKGoal goal)
        {
            IKStates r = null;
            foreach(IKStates i in ikStates)
            {
                if(i.goal == goal)
                {
                    r = i;
                    break;
                }

            }

            return r;
        }


        class IKStates
        {
            public AvatarIKGoal goal;
            public Vector3 position;
            public float positionWeight;
            public bool isSet = false;
        }
    }

    public class IKGoals
    {
        public bool rh, lh, rf, lf;
    }
}
