using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralClimbing
{
    public class CameraHolder : MonoBehaviour {

        public Transform target;
        public float speed = 9;

        public static CameraHolder singleton;

        private void Awake()
        {
            singleton = this;
        }

        // Update is called once per frame
        void FixedUpdate () {
           if(target == null)
            {
                return;
            }

            Vector3 p = Vector3.Lerp(transform.position, target.position, Time.deltaTime * speed);
            transform.position = p;
        }
    }
}
