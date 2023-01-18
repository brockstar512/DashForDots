using UnityEngine;
using TMPro;
using System.Threading.Tasks;


public class TimerManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timeTitle;
    [SerializeField] TextMeshProUGUI timeText;
    public float timeRemaining = 10;
    public bool timerIsRunning = false;

    public async Task StartTimer()
    {
        await Task.Delay(1000);
        timeRemaining = 20;

        // Starts the timer automatically
        timerIsRunning = true;
    }
    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);

            }
            else
            {
                Debug.Log("Time has run out!");
                timeRemaining = 0;
                timerIsRunning = false;
            }
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        if ( minutes <=0 && seconds <= 5)
        {
            timeTitle.text = "Hurry up!";
            timeTitle.color = Color.red;
            timeText.color = Color.red;

        }
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
