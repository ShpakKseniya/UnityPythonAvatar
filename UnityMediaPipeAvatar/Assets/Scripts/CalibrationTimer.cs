using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CalibrationTimer : MonoBehaviour
{
    public PipeServer server;
    public int timer = 5;
    public KeyCode calibrationKey = KeyCode.C;
    public TextMeshProUGUI text;
    public bool isCalibration = false;
    private void Start()
    {
        bool shouldEnable = false;
        Avatar[] a = FindObjectsByType<Avatar>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (Avatar aa in a)
        {
            if (!aa.isActiveAndEnabled) continue;
            if (!aa.calibrationData)
            {
                shouldEnable = true;
                break;
            }
        }
        text.text = shouldEnable ? "Press " + calibrationKey + "or button to start calibration" : "";

        gameObject.SetActive(shouldEnable);
        if (!shouldEnable)
        {
           // server.SetVisible(false);
        }
    }

    public void StartCalibration()
    {
    if(!isCalibration)
        StartCoroutine(Timer());
    }

    private void Update()
    {
        if (Input.GetKeyDown(calibrationKey))
        {
            StartCalibration();
        }
    }
    public IEnumerator Timer()
    {
    	isCalibration=true;
        int t = timer;
        while (t > 0)
        {
            text.text = "Copy the avatars starting pose: "+t.ToString();
            yield return new WaitForSeconds(1f);
            --t;
        }
        Avatar[] a = FindObjectsByType<Avatar>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach(Avatar aa in a)
        {
            if (!aa.isActiveAndEnabled) continue;
            aa.Calibrate();
        }
        if (a.Length>0)
        {
            text.text = "Calibration Completed";
         //   server.SetVisible(false);
        }
        else
        {
            text.text = "Avatar in scene not found...";
        }
        yield return new WaitForSeconds(1.5f);
        isCalibration=false;
        text.text = "";
    }
}