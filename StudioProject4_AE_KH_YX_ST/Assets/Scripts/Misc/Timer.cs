using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour
{
    private float timer;
    private float in_timer;
    private float delay; // in seconds
    private float period; // before it executes again
    private float original_time;
    public bool can_run;
    private float new_delay;
    private bool start_period;
    // Use this for initialization
    public void Start()
    {
        new_delay = -1;
        start_period = false;
    }

    // Update is called once per frame
    public void Update()
    {
        if (new_delay != -1)
            delay = new_delay;
        if (timer < 0)
            return;

        //if (start_period)
        //{
        //    can_run = false;
        //    if (timer < period)
        //    {
        //        timer += Time.deltaTime;
        //    }
        //    else
        //    {
        //        start_period = false;
        //        timer = original_time;
        //    }
        //}
        if (period > 0)
        {
            if (in_timer < period && start_period)
            {
                can_run = false;
                in_timer += Time.deltaTime;
            }
            else if (start_period)
            {
                start_period = false;
                in_timer = 0;
            }
        }
        if (timer < delay && !start_period)
        {
            timer += Time.deltaTime;
        }
        else if (!start_period)
        {
            can_run = true;
            timer = original_time;
            if (period > 0)
                start_period = true;
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
