using EAUnity.Core;

public class AppManager: SingletonBehaviour<AppManager> {
    public override void Awake() {
        base.Awake();
        if (Instance == null || Instance == this) {
            DontDestroyOnLoad(gameObject);	
        }
    }

    public void OnStart() {
        CustomSceneLoader.LoadScene("GameScene");
    }

    public void OnMainMenuClick() {
        CustomSceneLoader.LoadScene("MainMenu");
    }
}