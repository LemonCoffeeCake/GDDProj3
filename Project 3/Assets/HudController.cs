using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudController : MonoBehaviour
{
    #region Editor Variables
    [SerializeField]
    private RectTransform m_StaminaBar;
    [SerializeField]
    private Image[] m_Hearts;
    #endregion

    #region Private Variables
    private float p_StaminaBarOrigWidth;
    #endregion

    #region Initialization
    private void Awake()
    {
        p_StaminaBarOrigWidth = m_StaminaBar.sizeDelta.x;
    }
    #endregion

    #region Update Stamina Bar
    public void UpdateStamina(float percent)
    {
        m_StaminaBar.sizeDelta = new Vector2(p_StaminaBarOrigWidth * percent, m_StaminaBar.sizeDelta.y);
    }

    public void UpdateHealth(int health)
    {
        print(health);
        for (int i = 0; i < health; i++)
        {
            m_Hearts[i].color = Color.red;
        }
        for (int i = health; i < m_Hearts.Length; i++)
        {
            m_Hearts[i].color = Color.black;
        }
    }

    #endregion
}
