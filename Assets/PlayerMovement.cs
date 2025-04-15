using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    // [Movement and Camera]
    public float speed = 5f;

    // [Game Win Management]
    public Transform goalPoint;
    public Text winMessage;
    private bool gameFinished = false;
    public Text timerText;
    public float totalTime = 60f;
    private float remainingTime;

    void Start()
    {
        winMessage.gameObject.SetActive(false);
        remainingTime = totalTime;
    }

    void Update()
    {
        if (gameFinished)
        {
            return;
        }

        // Update timer
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
        }
        else
        {
            remainingTime = 0;
            FinishGame("Time's Up!");
        }

        // Update the timer display in minutes and seconds
        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        // Detect when the player reaches the goal point
        if (Vector3.Distance(transform.position, goalPoint.position) < 5f)
        {
            FinishGame("You Win!");
        }
    }

    void FixedUpdate()
    {
        if (gameFinished) return;
    }

    private void FinishGame(string message)
    {
        // Game End function
        gameFinished = true;
        winMessage.text = message;
        winMessage.gameObject.SetActive(true);
        Debug.Log(message);
    }
}

// References:
// [Camera]
// All Things Game Dev (2022). How To Make An FPS Player In Under A Minute - Unity Tutorial. [online] Available at: https://www.youtube.com/watch?v=qQLvcS9FxnY. [Accessed 3 Jan 2025].

