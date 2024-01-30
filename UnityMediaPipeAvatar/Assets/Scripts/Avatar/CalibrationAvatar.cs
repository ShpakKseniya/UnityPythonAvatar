using RootMotion.FinalIK;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calibration
{
    public class CalibrationAvatar : MonoBehaviour
    {
        private Vector3 scale = Vector3.zero;
        public VRIK ik;
        public VRIKCalibrator.Settings settings;
        public Transform headTracker;
        public Transform bodyTracker;
        public Transform leftHandTracker;
        public Transform rightHandTracker;
        public Transform leftFootTracker;
        public Transform rightFootTracker;
        private PipeServer server;

        void Start() => Initialize();

        private void OnDestroy() => DeInitialize();

        void LateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                Calibrate();
            }
        }

        private void Initialize()
        {
            SetLocalTransform();
            ik.solver.OnPostInitiate += Calibrate;

            server = FindObjectOfType<PipeServer>();
            if (server == null)
            {
                Debug.LogError("You must have a PipeServer in the scene!");
            }
            else
            {
                SetHeadTracker();
                //SetBodyTracker();
                SetHandTracker();
                SetFootTracker();
            }
        }

		private void DeInitialize()
        {
            ik.solver.OnPostInitiate -= Calibrate;
        }

        [ContextMenu("Calibrate")]
        void Calibrate()
        {
            VRIKCalibrator.Calibrate(ik, settings, headTracker, null, leftHandTracker, rightHandTracker, leftFootTracker, rightFootTracker);
            GetLocalTransform();
        }
        private void SetFootTracker()
        {
            leftFootTracker = SetTarget(server.GetLandmark(Landmark.LEFT_HEEL), server.GetLandmark(Landmark.LEFT_FOOT_INDEX));
            rightFootTracker = SetTarget(server.GetLandmark(Landmark.RIGHT_HEEL), server.GetLandmark(Landmark.RIGHT_FOOT_INDEX));
        }

        private void SetHandTracker()
        {
            leftHandTracker = SetTarget(server.GetLandmark(Landmark.LEFT_WRIST), server.GetLandmark(Landmark.LEFT_INDEX));
            rightHandTracker = SetTarget(server.GetLandmark(Landmark.RIGHT_WRIST), server.GetLandmark(Landmark.RIGHT_INDEX));
        }

        private void SetBodyTracker()
        {
            bodyTracker = SetTarget(server.GetVirtualHip());
            var heading = server.GetLandmark(Landmark.NOSE).position - server.GetVirtualHip().position;
            var direction = heading / heading.magnitude;
            bodyTracker.transform.position = new Vector3(
              0,
              -direction.y * 3,
              0);
            bodyTracker.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        }

        private void SetHeadTracker()
        {
            headTracker = SetTarget(server.GetVirtualNeck(), server.GetLandmark(Landmark.NOSE));
            var heading = server.GetLandmark(Landmark.NOSE).position - server.GetVirtualNeck().position;
            var direction = heading / heading.magnitude;
            headTracker.transform.position = new Vector3(
              0,
              -direction.y * 3,
              -direction.z / 2);
            headTracker.transform.localEulerAngles = new Vector3(180f, 0f, 0f);
        }

        private Transform SetTarget(Transform pt, Transform lookTarget = null)
        {
            GameObject t = Instantiate(new GameObject(), pt);
            t.name = "TargetNode";
            pt.localEulerAngles = new Vector3(0, 0f, 180f);
            pt.localPosition = Vector3.zero;
            if (lookTarget != null)
            {
                LookRotationTarget lrt = t.gameObject.AddComponent<LookRotationTarget>();
                lrt.target = lookTarget;
            }
            return t.transform;
        }

        private void SetLocalTransform()
        {
            transform.position = Vector3.zero;
            scale = ik.transform.localScale;
        }

        private void GetLocalTransform()
        {
            ik.transform.localScale = new Vector3(scale.x, scale.y, scale.z);
        }
    }
}