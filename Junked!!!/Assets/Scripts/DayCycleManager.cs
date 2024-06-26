using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DayCycleManager : MonoBehaviour
{
    //Scene References
    [SerializeField] private Light DirectionalLight;
    [SerializeField] private LightingPreset Preset;
    [SerializeField] private Animator clockAnimator;
    public QuotaSystem quotaSystem;
    public ScrapSpawner scrapManager;
    public Signs signs;
    public GameObject gameOverSign;
    public TrashBag trashbag;

    //Variables
    [SerializeField, Range(0, 24)] private float TimeOfDay;
    [SerializeField] private float timeElapsed = 0;
    public float startTime = 9f;
    public int day;

    [SerializeField] bool bypassReset = false;

    private void Awake()
    {
        // Just intializes values
        TimeOfDay = startTime;
        day = 1;
    }

    private void Update()
    {
        // Checks if preset is available to avoid script goofin out
        if (Preset == null)
            return;

        TimeOfDay += Time.deltaTime / 60; // Divides by 60 so 1 sec irl = 1 min in game
        TimeOfDay %= 24; // Modulus to ensure time always stays between 0-24. Makes it like the clock

        timeElapsed += Time.deltaTime; // Is needed for minutehand
        if (clockAnimator != null)
        {
            clockAnimator.SetFloat("minuteHand", ((timeElapsed % 60) / 60));
            clockAnimator.SetFloat("hourHand", ((TimeOfDay - startTime) / 8f)); // Game runs over 8 hours from 9 to 17
        }

        if ((!bypassReset && TimeOfDay >= 17)) // Restarts the day if is has passed more than "17 o'clock"
        {
            if (quotaSystem.CheckQuota()) // Checks if quota is met
            {
                // Restarts day and updates quota
                print("Day Resetting");
                TimeOfDay = startTime;
                timeElapsed = 0;
                day += 1;
                quotaSystem.currentCO2 -= quotaSystem.co2Quota;
                quotaSystem.currentCO2 -= quotaSystem.currentCO2/2;
                quotaSystem.currentMoney -= quotaSystem.moneyQuota;
                quotaSystem.UpdateQuota(day);
                scrapManager.CleanScrap();
                scrapManager.RandomizeHeatmap();
                scrapManager.GenerateScrapSpawns();
                scrapManager.PopulateSpawns();
                trashbag.IncreaseTrashBagSize(5);
                if(day >= 1)
                {
                    signs.RemoveSigns();
                }
            }
            else
            {
                TimeOfDay = startTime;
                signs.RemoveSigns();
                scrapManager.CleanScrap();
                Instantiate(gameOverSign);
            }
        }
       
        // Adds money for debugging
        if (Input.GetKeyDown("8"))
        {
            quotaSystem.currentCO2 += 500;
            quotaSystem.currentMoney += 500;
            quotaSystem.UpdateQuotaUI();

        }
        // Ups time/day for debugging
        if (Input.GetKeyDown("9"))
        {
            TimeOfDay += 7;
            
        }
        UpdateLighting(TimeOfDay / 24f);
    }


    private void UpdateLighting(float timePercent) // Just updates the sun as the directional light
    {
        // Set ambient and fog based on time ingame. This is a percentage in decimals from 0-1
        RenderSettings.ambientLight = Preset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = Preset.FogColor.Evaluate(timePercent);

        // If the directional light is set then rotate and set it's color
        if (DirectionalLight != null)
        {
            DirectionalLight.color = Preset.DirectionalColor.Evaluate(timePercent);

            DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));
        }

    }
}