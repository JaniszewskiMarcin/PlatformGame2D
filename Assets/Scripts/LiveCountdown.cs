using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(Text))]
public class LiveCountdown : MonoBehaviour
{
    private Text livesText;

    // Start is called before the first frame update
    void Awake()
    {
        livesText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        livesText.text = "LIVES : " + GameMaster.RemainingLives.ToString();
    }
}
