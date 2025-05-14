using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// In this minigame, theres a bunch of stats the player can modify to get different endings.
/// Similar to Tokemeki Memori where you can only call on certain days 
/// Random encounters while walking home from school. Sometimes they can literally just be RPG enemies that give you a stat bonus when you win 
/// All of the villains are also in the same highschool as Hank, for some reason
/// 
/// No matter what, you will get with Hank by the end, but your choices make the ending differ slightly
/// Hank is dating future Hank.
/// </summary>

// Stats
// - Health
// - Charm
// - Intellect
// - Swag
// - Pheremones
// - Stress
// - Strength

// REJECTED:
// - Lunch Moneys
// - Metal Mario
// - Macklemore Morning

public class DatingSimulatorMinigame : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private DatingStat _healthStat;
    [SerializeField] private DatingStat _charmStat;
    [SerializeField] private DatingStat _intellectStat;
    [SerializeField] private DatingStat _pheremonesStat;
    [SerializeField] private DatingStat _stressStat;
    [SerializeField] private DatingStat _strengthStat;
    [SerializeField] private DatingStat _looksStat;

    public enum EDay { Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday };
    public enum EMonth { September, October, November, December, January, Feburary, March, April, May, June, July, August };

    private Dictionary<EMonth, int> monthDayPairs = new();

    private int currentDay = 1;
    private EMonth currentMonth = EMonth.September;

    private void Start()
    {
        monthDayPairs.Add(EMonth.September, 30);
        monthDayPairs.Add(EMonth.October, 31);
        monthDayPairs.Add(EMonth.November, 30);
        monthDayPairs.Add(EMonth.December, 31);
        monthDayPairs.Add(EMonth.January, 31);
        monthDayPairs.Add(EMonth.Feburary, 28);
        monthDayPairs.Add(EMonth.March, 31);
        monthDayPairs.Add(EMonth.April, 30);
        monthDayPairs.Add(EMonth.May, 31);
        monthDayPairs.Add(EMonth.June, 30);
        monthDayPairs.Add(EMonth.July, 31);
        monthDayPairs.Add(EMonth.August, 31);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            ProgressDay();
        }
    }

    public void ProgressDay()
    {
        NextDay();

        Debug.Log("Day: " + currentDay + ", Month: " + currentMonth);
    }

    public void CompleteDay()
    {
        // different events happen based on your stats
        if (IsWeekday())
        {

        }
        else if(IsWeekend())
        {

        }

        // if weekday, check if a scripted event is supposed to happen, OR chance of random encounter

        // if weekend, check if scripted event is supposed to happen
    }

    private void NextDay()
    {
        currentDay++;

        // If the current days
        if(currentDay > monthDayPairs[currentMonth])
        {
            currentMonth++;
            currentMonth = (EMonth)((int)currentMonth % Enum.GetNames(typeof(EMonth)).Length);
            currentDay = 1;
        }
    }

    private bool IsWeekend() => (EDay)(currentDay % 7) == EDay.Saturday || (EDay)(currentDay % 7) == EDay.Sunday;
    private bool IsWeekday() => (EDay)(currentDay % 7) != EDay.Saturday && (EDay)(currentDay % 7) != EDay.Sunday;
}

[System.Serializable]
public struct DatingStat
{
    public int MaxStat;
    public int InitialStat;
}