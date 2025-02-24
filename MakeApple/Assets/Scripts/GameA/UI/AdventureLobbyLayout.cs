using GameA;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AdventureLobbyLayout : UILayout
{
    public KButton startButton;

    private void Start()
    {
        startButton.onClick.AddListener(()=>Managers.Event.MovePage(EventController.Page.AdventureGame));
    }
}
