using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour {
    public GameObject camera1;
    public GameObject camera2;
    public bool loadLock = false;
    public void click(int select) {
        if(select == 0) {
            camera1.SetActive(false);
            camera2.SetActive(true);
            StartCoroutine(LoadTutorialScene());
        }
    }

    IEnumerator LoadTutorialScene() {
        if (!loadLock) {
            loadLock = true;
            // This line waits for 3 seconds before executing the next line in the coroutine.
            yield return new WaitForSeconds(3);

            // Start an asynchronous operation to load the scene that was passed to the LoadNewScene coroutine.
            AsyncOperation async = SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);

            // While the asynchronous operation to load the new scene is not yet complete, continue waiting until it's done.
            while (!async.isDone) {
                yield return null;
            }
            if (async.isDone) {
                Debug.Log("Load done");
            }
            loadLock = false;
        }
    }
}
