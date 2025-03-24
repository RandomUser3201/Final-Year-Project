using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HeartRateData : MonoBehaviour
{
    public static HeartRateData Instance;
    public int playerAge = 20;  
    public int minHR, maxHR;

    public Dropdown ageSelector;
    private Dictionary<int, (int min, int max, int maxHR)> heartRateTable = new Dictionary<int, (int, int, int)>
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
        if (ageSelector == null)
        {
            Debug.LogError("Age Selector Dropdown is not assigned in the Inspector");
            return;
        }

        ageSelector.onValueChanged.AddListener(delegate { SetPlayerAge(); });

        SetPlayerAge();
    }

    public void SetPlayerAge()
    {
        if (int.TryParse(ageSelector.options[ageSelector.value].text, out int selectedAge))
        {
            playerAge = selectedAge;

            if (heartRateTable.ContainsKey(selectedAge))
            {
                (minHR, maxHR, _) = heartRateTable[selectedAge];
            }
            else
            {
                minHR = 90;
                maxHR = 170;
            }
            Debug.Log("Updated Player Age: " + playerAge + ", MinHR: " + minHR + ", MaxHR: " + maxHR);
        }
        else
        {
            Debug.LogError("Invalid age selected: " + ageSelector.options[ageSelector.value].text);
        }
    }

    public void StartGame()
    {
        // string selectedAgeText = ageSelector.options[ageSelector.value].text;
        // Debug.Log("Selected age text: " + selectedAgeText);
        
        // int selectedAge = int.Parse(selectedAgeText);   
        // Debug.Log("Selected age conv to int): " + selectedAge);        
        // HeartRateData.Instance.SetPlayerAge(selectedAge);
        
        // SceneManager.LoadScene("SampleScene");

        Debug.Log("Starting game with Age: " + playerAge);
        SceneManager.LoadScene("SampleScene");
    }
}
