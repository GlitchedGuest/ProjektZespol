using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField] private float fadeDuration;
    private SceneFade sceneFade;

    private void Awake()
    {
        sceneFade = GetComponentInChildren<SceneFade>();
    }

    private IEnumerator Start()
    {
        yield return sceneFade.FadeInCoroutine(fadeDuration);
    }
    public void LoadScene(string name)
    {
        StartCoroutine(LoadSceneCoroutine(name));
    }
    private IEnumerator LoadSceneCoroutine(string name)
    {
        yield return sceneFade.FadeOutCoroutine(fadeDuration);
        yield return SceneManager.LoadSceneAsync(name);
    }
}
