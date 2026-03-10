using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReloadProgress : MonoBehaviour
{
    [field: SerializeField] public List<TurretHandler> TurretObjectsToTrack { get; set; }
    private Image progressImage; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        progressImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        float lowestProgress = 1;

        //Could be optimized by listening for an OnShoot event and calculating which turret would take the longest to reload on invoke.
        foreach (TurretHandler turret in TurretObjectsToTrack)
        {
            (float reloadTime, float reloadTimer) = turret.GetReloadTimeAndTimer();
            float progress = (reloadTime - reloadTimer) / reloadTime;
            if (progress < lowestProgress)
            {
                lowestProgress = progress;
            }
        }

        progressImage.fillAmount = lowestProgress;

        if (progressImage.fillAmount == 1)
        {
            progressImage.color = Color.green;
        }
        else
        {
            progressImage.color = Color.red;
        }
    }
}
