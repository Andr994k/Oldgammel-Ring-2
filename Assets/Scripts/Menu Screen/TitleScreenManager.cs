using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class TitleScreenManager : MonoBehaviour
{

    public void StartNetworkAsHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StartNewGame()
    {
        StartCoroutine(WorldSaveGameManager.instance.LoadNewGame());
    }
}
