using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class InteractionsManager : MonoBehaviour {

    public static InteractionsManager Instance;
    public List<int> TimesPromptedList = new List<int>();
    public List<int> TimesInteractedWithUIList = new List<int>();
    public int TimesPrompted;
    public int TimesInteractedWithUI;
    private bool first = true;

    private void Awake() {
        Instance = this;
        TimesPrompted = 0;
        TimesInteractedWithUI = 0;
    }

    public void SaveAndResetInteractions() {
        // reset when starting the first question
        if (first) {
            TimesPrompted = 0;
            TimesInteractedWithUI = 0;
            first = false;
            return;
        }

        TimesPromptedList.Add(TimesPrompted);
        TimesPrompted = 0;

        TimesInteractedWithUIList.Add(TimesInteractedWithUI);
        TimesInteractedWithUI = 0;
    }

    public void IncrementTimesInteractedWithUI() {
        TimesInteractedWithUI++;
    }
}
