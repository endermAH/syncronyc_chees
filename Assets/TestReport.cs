using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevToDev.Analytics;



public class TestReport : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var androidAppID = "54a6a06e-b8f5-02be-961d-9ebfa970dd85";
        var winAppID = "68077484-ddac-02ba-8154-b515ec2e2465";
        var config = new DTDAnalyticsConfiguration
        {
            ApplicationVersion = "0.4.0",
            LogLevel = DTDLogLevel.Debug,
            TrackingAvailability = DTDTrackingStatus.Enable,
            CurrentLevel = 1,
            UserId = "001"
        };
        
#if UNITY_ANDROID
        DTDAnalytics.Initialize(androidAppID);
        UnityEngine.Debug.Log("Initializing Android analytics");
#elif UNITY_STANDALONE_WIN
        DTDAnalytics.Initialize(winAppID);
        UnityEngine.Debug.Log("Initializing WIN analytics");
#endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
