using RootMotion.FinalIK;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            headTracker = server.GetLandmark(Landmark.NOSE);
            //headTracker = server.GetVirtualNeck();
           // bodyTracker = server.GetVirtualHip();
            leftHandTracker = server.GetLandmark(Landmark.LEFT_WRIST);
            rightHandTracker = server.GetLandmark(Landmark.RIGHT_WRIST);
            leftFootTracker = server.GetLandmark(Landmark.LEFT_ANKLE);
            rightFootTracker = server.GetLandmark(Landmark.RIGHT_ANKLE);
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
