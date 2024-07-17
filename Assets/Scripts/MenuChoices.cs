using UnityEngine;
using System;
using FishNet.Transporting.Tugboat;
using System.Collections.Generic;
using Random = System.Random;
using Unity.VisualScripting;

public class MenuChoices : MonoBehaviour
{
    public static MenuChoices Instance;
    public static string serverIP;
    public static float firstPhaseLen = 60;
    public static float secondPhaseLen = 3;
    public static int playersNumber = 3;
    public static bool amIServer = false;
    public static string playerName = "";
    public static int playerSkin = 0;
    public int playerSkinNonStatic = playerSkin;
    public static float musicVolume = 100;
    public static float sfxVolume = 100;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        //
        //comment

        serverIP = "";
        firstPhaseLen = 60;
        secondPhaseLen = 3;
        playersNumber = 3;
        amIServer = false;
        playerName = "";
        playerSkin = 0;
        musicVolume = 100;
        sfxVolume = 100;
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

    public static void SetPlayersNumber(float num)
    {
        Debug.Log(num);
        playersNumber = (int)num;
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

    public void Prova()
    {

    }
    public void SetMusicVolume(float vol)
    {
        Debug.Log(vol);
        musicVolume = vol;
    }

    public static void SetSFXVolume(float vol)
    {
        Debug.Log(vol);
        sfxVolume = vol;
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
