using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour {
    private float timer;
    private float delay; // in seconds
    private float period; // before it executes again
    private float original_time;
    public bool can_run;
    private float new_delay;
    // Use this for initialization
    public void Start()
    {
        new_delay = -1;
    }

    // Update is called once per frame
    public void Update()
    {
        if (new_delay != -1)
            delay = new_delay;
        if (timer < 0)
            return;
        if (timer >= period)
        {
            if (timer < delay)
            {
                timer += Time.deltaTime;
            }
            else
            {
                can_run = true;
                timer = original_time;
            }
        }
    }

    public void Init(float starttime_, float delay_, float period_)
    {
        original_time = starttime_;
        timer = starttime_;
        delay = delay_;
        period = period_;
        can_run = false;
    }

    public void Reset()
    {
        timer = original_time;
        can_run = false;
    }
}
