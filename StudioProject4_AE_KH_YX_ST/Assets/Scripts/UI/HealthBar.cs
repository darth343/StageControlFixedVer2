using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Requires component of type health or script won't run i think
[RequireComponent(typeof(Health))]
public class HealthBar : MonoBehaviour {
    private Canvas m_canvas; // The background of the healthbar
    private Image m_hp_bg; // The background of the healthbar
    private Image m_hp_fg; // The foreground of the healthbar
    private Health m_playerHealth; // Player current health object
    private float fg_width;
    //private float bg_width;
    public static int m_childIndex = 0; // Index starting from 0 of which UI component to access from parent
    // Use this for initialization
    void Start()
    {
        m_playerHealth = GetComponent<Health>();
        m_canvas = transform.parent.GetChild(0).gameObject.GetComponent<Canvas>(); // Get object controller's canvas. For example Building Controller Game Object's canvas;
        m_hp_bg = m_canvas.transform.GetChild(m_childIndex).GetComponent<Image>(); // Get healthbar background from canvas
        m_hp_fg = m_hp_bg.transform.GetChild(0).GetComponent<Image>(); // Get child healthbar foreground from parent healthbar background 
        //bg_width = m_hp_bg.rectTransform.rect.width;
        fg_width = m_hp_fg.rectTransform.rect.width;
        m_childIndex++;        
    }

    // Update is called once per frame
    void Update()
    {
        // m_hp_bg.rectTransform.rect.Set(m_hp_bg.rectTransform.rect.x, m_hp_bg.rectTransform.rect.y, m_hp_bg.rectTransform.rect.width * (player_health.MAX_HEALTH * 0.01f), m_hp_bg.rectTransform.rect.height);
        // m_hp_fg.rectTransform.rect.Set(m_hp_fg.rectTransform.rect.x, m_hp_fg.rectTransform.rect.y, m_hp_fg.rectTransform.rect.width * (player_health.GetHealth() * 0.01f), m_hp_fg.rectTransform.rect.height);
        //m_hp_bg.rectTransform.sizeDelta = new Vector2(bg_width * (m_playerHealth.MAX_HEALTH * 0.01f), m_hp_bg.rectTransform.rect.height);
        m_hp_fg.rectTransform.sizeDelta = new Vector2(fg_width * (m_playerHealth.GetHealth() * 0.01f), m_hp_fg.rectTransform.rect.height);
        m_hp_bg.rectTransform.position = new Vector3(this.transform.position.x - 20, this.transform.position.y * 2 + 20f, this.transform.position.z);
        //m_hp_bg.rectTransform.anchoredPosition = new Vector3(this.transform.position.x, this.transform.position.y + this.transform.localScale.y * 0.5f, this.transform.position.z);
    }

    void OnGUI()
    {
        //m_hpfgRect.position.Set(Camera.main.ScreenToWorldPoint(m_hp_bg.rectTransform.rect.position).x + m_offset.x, Screen.height - Camera.main.ScreenToWorldPoint(m_hp_bg.rectTransform.rect.position).y + m_offset.y);
        //m_hpbgRect.position.Set(Camera.main.WorldToScreenPoint(transform.position).x + m_offset.x, Screen.height - Camera.main.WorldToScreenPoint(transform.position).y + m_offset.y);
        //GUI.DrawTexture(m_hpfgRect, m_Background, ScaleMode.ScaleToFit);

        //partialRect.x = m_Rectangle.x;

        //GUI.DrawTexture(partialRect, m_Foreground);
    }

    void OnDestroy()
    {
        --m_childIndex;
        UnityEngine.Object.Destroy(m_hp_bg.gameObject);
        UnityEngine.Object.Destroy(m_hp_fg.gameObject);
    }
}
