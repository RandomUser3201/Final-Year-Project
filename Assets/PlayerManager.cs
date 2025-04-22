using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public Transform GoalPoint;
    public Text winMessage;
    private bool _gameFinished = false;
    public Text TimerText;
    public float TotalTime = 60f;
    private float _remainingTime;

    void Start()
    {
        winMessage.gameObject.SetActive(false);
        _remainingTime = TotalTime;
    }

    void Update()
    {
        if (_gameFinished)
        {
            return;
        }

        // Update timer
        if (_remainingTime > 0)
        {
            _remainingTime -= Time.deltaTime;
        }
        else
        {
            _remainingTime = 0;
            FinishGame("Time's Up!");
        }

        // Update the timer display in minutes and seconds
        int minutes = Mathf.FloorToInt(_remainingTime / 60f);
        int seconds = Mathf.FloorToInt(_remainingTime % 60f);
        TimerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        // Detect when the player reaches the goal point
        if (Vector3.Distance(transform.position, GoalPoint.position) < 5f)
        {
            FinishGame("You Win!");
        }
    }

    void FixedUpdate()
    {
        if (_gameFinished) return;
    }

    private void FinishGame(string message)
    {
        // Game End function
        _gameFinished = true;
        winMessage.text = message;
        winMessage.gameObject.SetActive(true);
        Debug.Log(message);
    }
}

// References:
// [Camera]
// All Things Game Dev (2022). How To Make An FPS Player In Under A Minute - Unity Tutorial. [online] Available at: https://www.youtube.com/watch?v=qQLvcS9FxnY. [Accessed 3 Jan 2025].
