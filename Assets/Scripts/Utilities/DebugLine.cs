using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralClimbing
{
    public class DebugLine : MonoBehaviour {

        public int maxRenderers;

        List<LineRenderer> lines = new List<LineRenderer>();

        public static DebugLine singleton;
        private void Awake()
        {
            singleton = this;
        }
        
        // Use this for initialization
        void Start () {

        }

        void CreateLine(int i)
        {
            GameObject go = new GameObject();
            lines.Add(go.AddComponent<LineRenderer>());
            lines[i].widthMultiplier = 0.05f;         
        }

        public void SetLine(Vector3 startPos, Vector3 endPos, int index)
        {
            if(index > lines.Count - 1)
            {
                CreateLine(index);
            }

            lines[index].SetPosition(0, startPos);
            lines[index].SetPosition(1, endPos);
        }

        // Update is called once per frame
        void Update () {
            
        }
    }
}
