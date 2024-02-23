using UnityEngine;

public class StageEditor : MonoBehaviour
{
    [SerializeField] private GameObject m_grid;

    public void OpenEditor()
    {
        if (m_grid.activeSelf == false)
            m_grid.SetActive(true);
        else
            m_grid.SetActive(false);
    }
}
