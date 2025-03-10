using TMPro;
using UnityEngine;

public class HistoryBall : MonoBehaviour
{
    [SerializeField] private TextMeshPro numberText;
    public void Init(int number)
    {
        var ballColor = GameConstants.GetColorFromNumber(number);
        numberText.text = number.ToString();
        GetComponent<MeshRenderer>().material.color = ballColor;
    }
}
