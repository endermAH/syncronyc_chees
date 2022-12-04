using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BugsnagUnity;

public class TestCrash : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Bugsnag.Notify(new System.Exception("This is test Bugsnag notification!"));
        throw new System.Exception("This is test exception for Bugsnag test!");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
