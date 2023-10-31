using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldSaveGameManager : MonoBehaviour
{
    public static WorldSaveGameManager instance;

    [SerializeField] int WorldSceneIndex = 2;

    private void Awake()
    {
        //There can only be one!
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator LoadNewGame()
    {
        AsyncOperation LoadOperation = SceneManager.LoadSceneAsync(WorldSceneIndex);

        yield return null;
    }

    public int GetWorldSceneIndex()
    {
        return WorldSceneIndex;
    }
}
