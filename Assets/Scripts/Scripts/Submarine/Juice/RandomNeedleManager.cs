using UnityEngine;
using System.Collections.Generic;
public class RandomNeedleManager : MonoBehaviour
{
    [System.Serializable]
        public enum Axis
        {
            X,
            Y,
            Z
        }
    
        [System.Serializable]
        public class RandomNeedle
        {
            public Transform needle;
            public Axis axis = Axis.Z;
    
            [Header("Angle Range")]
            public float minAngle = -90f;
            public float maxAngle = 90f;
    
            [Header("Movement")]
            public float moveSpeed = 50f;      // degrees per second
            public float newTargetThreshold = 1f; // how close before picking new target
    
            [HideInInspector] public float currentTargetAngle;
        }
    
        [SerializeField] private List<RandomNeedle> needles = new List<RandomNeedle>();
    
        private void Start()
        {
            // Set first random target for each needle
            foreach (var n in needles)
            {
                if (n.needle == null) continue;
                n.currentTargetAngle = Random.Range(n.minAngle, n.maxAngle);
            }
        }
    
        private void Update()
        {
            float dt = Time.deltaTime;
    
            foreach (var n in needles)
            {
                if (n.needle == null) continue;
    
                // Get current angle on chosen axis
                Vector3 euler = n.needle.localEulerAngles;
                float currentAngle = GetAxisAngle(euler, n.axis);
    
                // Move towards target
                float newAngle = Mathf.MoveTowardsAngle(
                    currentAngle,
                    n.currentTargetAngle,
                    n.moveSpeed * dt
                );
    
                // Write back to the chosen axis
                euler = SetAxisAngle(euler, n.axis, newAngle);
                n.needle.localEulerAngles = euler;
    
                // If close to target, pick a new random target
                if (Mathf.Abs(Mathf.DeltaAngle(newAngle, n.currentTargetAngle)) < n.newTargetThreshold)
                {
                    n.currentTargetAngle = Random.Range(n.minAngle, n.maxAngle);
                }
            }
        }
    
        private float GetAxisAngle(Vector3 euler, Axis axis)
        {
            switch (axis)
            {
                case Axis.X: return euler.x;
                case Axis.Y: return euler.y;
                case Axis.Z: return euler.z;
            }
            return 0f;
        }
    
        private Vector3 SetAxisAngle(Vector3 euler, Axis axis, float angle)
        {
            switch (axis)
            {
                case Axis.X:
                    euler.x = angle;
                    break;
                case Axis.Y:
                    euler.y = angle;
                    break;
                case Axis.Z:
                    euler.z = angle;
                    break;
            }
            return euler;
        }
}
