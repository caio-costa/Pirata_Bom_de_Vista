using UnityEditor;
using System.Collections;
using UnityEditor.SceneManagement;

public class OpenSceneEditor : Editor {

    [MenuItem("Abrir Level/Main")]
    static void AbrirMain()
    {
        if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        { 
            EditorSceneManager.OpenScene("Assets/Cenas/Main.unity");
        }
        
    }

    [MenuItem("Abrir Level/Navegacao")]
    static void AbrirNavegacao()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene("Assets/Cenas/Navegacao.unity");
        }

    }

    [MenuItem("Abrir Level/Menu Principal")]
    static void AbrirMenu()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene("Assets/Cenas/MenuPrincipal.unity");
        }
    }

    [MenuItem("Abrir Level/Splash Screen")]
    static void AbrirSplash()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene("Assets/Cenas/SplashScreen.unity");
        }
    }

    [MenuItem("Abrir Level/Loading")]
    static void AbrirLoading()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene("Assets/Cenas/LoadingScreen.unity");
        }
    }

    



}
