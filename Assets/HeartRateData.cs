using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HeartRateData : MonoBehaviour
{
    public static HeartRateData Instance;
    public int PlayerAge = 20;  
    public int MinHR; 
    public int MaxHR;

    public Dropdown AgeSelector;
    private Dictionary<int, (int min, int max, int MaxHR)> heartRateTable = new Dictionary<int, (int, int, int)>
    {

    // Age - Target HR - Zone - Predicted MaxHR
        { 20, (120, 170, 200) },
        { 25, (117, 166, 195) },
        { 30, (114, 162, 190) },
        { 35, (111, 157, 185) },
        { 40, (108, 153, 180) },
        { 45, (105, 149, 175) },
        { 50, (102, 145, 170) },
        { 55, (99, 140, 165) },
        { 60, (96, 136, 160) },
        { 65, (93, 132, 155) },
        { 70, (90, 128, 150) }
    };

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (AgeSelector == null)
        {
            Debug.LogError("Age Selector Dropdown is not assigned in the Inspector");
            return;
        }

        AgeSelector.onValueChanged.AddListener(delegate { SetPlayerAge(); });

        SetPlayerAge();
    }

    public void SetPlayerAge()
    {
        if (int.TryParse(AgeSelector.options[AgeSelector.value].text, out int _selectedAge))
        {
            PlayerAge = _selectedAge;

            if (heartRateTable.ContainsKey(_selectedAge))
            {
                (MinHR, MaxHR, _) = heartRateTable[_selectedAge];
            }
            else
            {
                MinHR = 90;
                MaxHR = 170;
            }
            Debug.Log($"Updated Player Age: {PlayerAge} MinHR: {MinHR} MaxHR: {MaxHR}");
        }
        else
        {
            Debug.LogError($"Invalid age selected: {AgeSelector.options[AgeSelector.value].text}");
        }
    }

    public void StartGame()
    {
        Debug.Log($"Starting game with Age: {PlayerAge}");
        SceneManager.LoadScene("SampleScene");
    }
}
