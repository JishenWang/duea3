using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NoControl : MonoBehaviour
{
    // Start is called before the first frame update
    public void LoadMyScene(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber);
        StartCoroutine("PlayMySound", sceneNumber);
    }

    IEnumerator PlayMySound(int SceneNumber)
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(SceneNumber);
    }


    public void LoadMyScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        StartCoroutine("PlayMySound2", sceneName);
    }
    IEnumerator PlayMySound2(string SceneName)
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(SceneName);
    }
}