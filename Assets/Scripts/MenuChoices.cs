using UnityEngine;
using System;
using FishNet.Transporting.Tugboat;
using System.Collections.Generic;
using Random = System.Random;

public class MenuChoices : MonoBehaviour
{
    public static MenuChoices Instance;
    public static string serverIP;
    public static float firstPhaseLen = 10;
    public static float secondPhaseLen = 5;
    public static bool amIServer = false;
    public static string playerName = "";
    public static int playerSkin = 0;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static void SetServerIP (string serverIP_selected) 
    {
        Debug.Log("SetupMenu script received " + serverIP_selected + " as serverIp");
        serverIP = serverIP_selected;
    }

    public void SetFirstPhaseLen(float len)
    {
        Debug.Log(len);
        firstPhaseLen = len;
    }

    public static void SetSecondPhaseLen(float len)
    {
        Debug.Log(len);
        secondPhaseLen = len;
    }

    public static void SetMeAsServer() 
    {
        Debug.Log("You have been setted as server in MainManager");
        amIServer = true;
    }

    public static void SetPlayer()
    {
        playerName = PlayerNameConfigurator.playerName;
        if (string.IsNullOrWhiteSpace(playerName))
            playerName = RandomPlayerName();
        playerSkin = PlayerSelection.currentPG;
    }

    public static string RandomPlayerName()
    {
        string[] names = new [] {
            "CrunchyPotato", "SpicyTaco", "ChocoMuffin", "SavorySteak", "JuicyBurger", "FrostyPopsicle", "SweetBerryPie", "CheesyNachos", "TangyLemon", "CrispyBacon", "ButteryPopcorn", "BerrySmoothie", "HotPepperoni", "GarlicBreadKing", "NuttyBrownie", "HoneyGlazedHam", "MeltyCheese", "FierySalsa", "GummyBear", "SushiSamurai"
        };

        Random random = new Random();
        int index = random.Next(names.Length);
        return names[index];
    }
    
}
