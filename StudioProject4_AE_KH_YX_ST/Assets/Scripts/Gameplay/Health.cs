using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {
    protected float m_health;
    public float MAX_HEALTH;
    private float m_limiter;
    private bool m_decrease;
    private bool m_increase;
    private float m_multiplier;
    // Use this for initialization
    void Start()
    {
        m_health = MAX_HEALTH;
        m_increase = false;
        m_decrease = false;
        m_limiter = 0;
        m_multiplier = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_increase && m_health < m_limiter)
            m_health += (m_multiplier * Time.deltaTime);
        else if (m_increase)
        {
            m_increase = false;
            m_limiter = 0;
        }
        if (m_decrease && m_health > m_limiter)
            m_health -= (m_multiplier * Time.deltaTime);
        else if (m_decrease)
        {
            m_decrease = false;
            m_limiter = 0;
        }
    }

    public void IncreaseHealthGradually(float dt, int limit)
    {
        if (limit < 0)
            limit = 0;
        if (limit > MAX_HEALTH)
            limit = (int)MAX_HEALTH;
        m_limiter = m_health + limit;
        m_multiplier = limit;
        m_increase = true;
    }

    public void DecreaseHealthGradually(float dt, int limit)
    {
        if (limit < 0)
            limit = 0;
        if (limit > MAX_HEALTH)
            limit = (int)MAX_HEALTH;
        m_limiter = m_health - limit;
        m_multiplier = limit;
        m_decrease = true;
    }

    public void SetHealth(float value)
    {
        m_health = value;
    }

    public float GetHealth()
    {
        return m_health;
    }
}
