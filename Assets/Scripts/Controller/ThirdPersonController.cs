using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralClimbing
{
    public class ThirdPersonController : MonoBehaviour {
        float horizontal;
        float vertical;

        Vector3 moveDirection;
        float moveAmount;

        Vector3 camYForward;
        Transform camHolder;

        Rigidbody rigid;
        Collider col;

        Animator anim;

        public float moveSpeed = 4;
        public float rotateSpeed = 9;

        // Use this for initialization
        void Start () {
            rigid = GetComponent<Rigidbody>();
            rigid.angularDrag = 999;
            rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

            col = GetComponent<Collider>();

            camHolder = CameraHolder.singleton.transform;
            anim = GetComponentInChildren<Animator>();
        }
        
        // Update is called once per frame
        void Update () {
            anim.SetFloat("move", moveAmount);
        }

        void FixedUpdate () {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");

            camYForward = camHolder.forward;

            Vector3 v = vertical * camHolder.forward;
            Vector3 h = horizontal * camHolder.right;

            moveDirection = (v + h).normalized;
            moveAmount = Mathf.Clamp01((Mathf.Abs(horizontal) + Mathf.Abs(vertical)));


            Vector3 targetDir = moveDirection;
            targetDir.y = 0;
            if(targetDir == Vector3.zero)
            {
                targetDir = transform.forward;
            }

            Quaternion lookDir = Quaternion.LookRotation(targetDir);
            Quaternion targetRot = Quaternion.Slerp(transform.rotation, lookDir, Time.deltaTime * rotateSpeed);
            transform.rotation = targetRot;

            Vector3 dir = transform.forward * (moveSpeed * moveAmount);
            rigid.velocity = dir;
        }
    }
}
