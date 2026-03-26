using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{

    public enum Scene
    {
        MainMenuScene,
        LoadingScene,
        GameScene,
    }
    public static Scene targetScene = Scene.MainMenuScene;

    public static void Load(Scene targetScene)
    {
        Loader.targetScene = targetScene;

        SceneManager.LoadScene((int)Scene.LoadingScene);
    }

    public static void LoaderCallback()
    {
		SceneManager.LoadScene((int)targetScene);
	}
}
