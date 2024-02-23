using UnityEngine;
using UnityEngine.UI;

public class TopBar : MonoBehaviour
{
    public Timer timer;
    [SerializeField] private Text p1DeadCountTxT;
    [SerializeField] private Text p2DeadCountTxT;
    [SerializeField] private Image p1HelthImg;
    [SerializeField] private Image p2HelthImg;
    [SerializeField] private Sprite Dead;
    [SerializeField] private Sprite Alive;

    private int p1DC = 0;
    private int p2DC = 0;
    public int p1DeadCount { get => p1DC; }
    public int p2DeadCount { get => p2DC; }

    /// <summary>
    /// Add or Minus player Dead Count
    /// </summary>
    /// <param name="player">p1 for player1, p2 for player2</param>
    /// <param name="count"></param>
    public void AddDead(string player, int count)
    {
        switch (player)
        {
            case "p1":
                p1DC += count;
                p1HelthImg.sprite = (p1DC > 0) ? Dead : Alive;
                p1DeadCountTxT.text = (p1DC > 0) ? p1DC.ToString() : (p1DC * -1).ToString();
                break;
            case "p2":
                p2DC += count;
                p2HelthImg.sprite = (p2DC > 0) ? Dead : Alive;
                p2DeadCountTxT.text = (p2DC > 0) ? p2DC.ToString() : (p2DC * -1).ToString();
                break;
        }
    }
    public void ResetDead()
    {
        p1DC = 0;
        p1HelthImg.sprite = Alive;
        p1DeadCountTxT.text = p1DC.ToString();
        p2DC = 0;
        p2HelthImg.sprite = Alive;
        p2DeadCountTxT.text = p2DC.ToString();
    }
    public void Reset()
    {
        ResetDead();
        timer.Reset();
    }
}
