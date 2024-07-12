using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GameUIManager : MonoBehaviour
{
    private static GameUIManager instance;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private PlayerCard playerCardPrefab;
    [SerializeField] private Transform playerCardParent;
    [SerializeField] private GameObject scoreboard;
    private Dictionary<int, PlayerCard> _playerCards = new Dictionary<int, PlayerCard>();

    public static bool isPaused = false;
    public GameObject pauseMenu;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
        scoreboard.SetActive(false);
    }

    public static void PlayerJoined(int clientID)
    {
        PlayerCard newCard = Instantiate(instance.playerCardPrefab, instance.playerCardParent);
        instance._playerCards.Add(clientID, newCard);
        newCard.Initialize(clientID.ToString());
    }

    public static void PlayerLeft(int clientID)
    {
        if (instance._playerCards.TryGetValue(clientID, out PlayerCard playerCard))
        {
            Destroy(playerCard.gameObject);
            instance._playerCards.Remove(clientID);
        }
    }

    void PauseGame()
    {
        isPaused = !isPaused;
        if(isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            pauseMenu.SetActive(true);
        }
        else 
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            pauseMenu.SetActive(false);
        }
    }

    public void OnClickResume()
    {
        PauseGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            scoreboard.SetActive(true);

        if (Input.GetKeyUp(KeyCode.Tab))
            scoreboard.SetActive(false);

        if(Input.GetKeyDown(KeyCode.P))
            PauseGame();
    }
}
