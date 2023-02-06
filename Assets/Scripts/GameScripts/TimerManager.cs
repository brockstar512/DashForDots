using UnityEngine;
using TMPro;
using System.Threading.Tasks;


public class TimerManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timeTitle;
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] GameObject screenBlocker;
    public float timeRemaining = 20;
    public bool timerIsRunning = false;
    Color32 normalColor = new Color32(101, 138, 167, 255);


    public async Task GameStartDelay()
    {
        timeTitle.color = Color.red;
        //await Task.Delay(2000);
        timeTitle.text = "Get Ready";
        await Task.Delay(3000);
        timeTitle.text = "Go!";
        await Task.Delay(1000);
        timeTitle.color = normalColor;
        PlayerHandler.Instance.UpdateScore(0,false);
        await Task.Yield();
        Destroy(screenBlocker);
    }

    public async Task StartTimer()
    {
        //timerIsRunning = false;

        timeText.color = normalColor;
        timeTitle.color = normalColor;
        timeTitle.text = "Time";
        timeText.text = "00:20";


        timeRemaining = 20;
        await Task.Delay(1000);

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

        if (minutes <= 0 && seconds <= 5)
        {
            timeTitle.text = "Hurry up!";
            timeTitle.color = Color.red;
            timeText.color = Color.red;
           
        }
        if (timeRemaining<=0) 
        {
            PlayerHandler.Instance.NextPlayer();

        }
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
