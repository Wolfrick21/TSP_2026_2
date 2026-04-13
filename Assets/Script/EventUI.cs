using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EventUI : MonoBehaviour
{
    public List<GameObject> listaInstrucciones;
    public List<string> mensajesInstrucciones;
    public int currentIndex = 0;
    public TextMeshProUGUI textMeshProUGUI;
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Acutualizar visibilidad paneles
        UpdateVisibility();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Metodo para actualizar visiilidad de paneles
    private void UpdateVisibility() 
    {
        for (int i = 0; i < listaInstrucciones.Count; i++)
        {
            //Solo el panel en el indice acual esta activo
            listaInstrucciones[i].SetActive(i == currentIndex);
        }
    }

    //Metodo para cambiar de escena
    public void ChangeSceneByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void ChangeSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    //Recargar escena actual
    public void ReloadCurrentScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
    //Metdo para cambiar entre paneles
    public void CycleObject(int direccion)
    {
        //Incrmenta el indice y vuelve al principio
        currentIndex = (currentIndex + direccion + listaInstrucciones.Count) % listaInstrucciones.Count;

        UpdateVisibility();
    }

    //Metodo para actualizar el texto mostrado
    private void UpdateText()
    {
        if (mensajesInstrucciones.Count > 0 && textMeshProUGUI != null)
        {
            textMeshProUGUI.text = mensajesInstrucciones[currentIndex];
        }
    }

    public void CycleText(int direccion)
    {
        //Incrmenta el indice y vuelve al principio
        currentIndex = (currentIndex + direccion + mensajesInstrucciones.Count) % mensajesInstrucciones.Count;

        UpdateText();
    }

    //Metodo para salir de la aplicación
    public void ExitGame()
    {
        Debug.Log("Va a salir");
        Application.Quit();
        Debug.Log("Ya salió");
    }
}

/*
Concurrente: "Al mismo tiempo"
-> Instancias de código que pueden ser inciadas, pausadas, o reaunadas que parecen 
ejecutarse al mismo tiempo pero son alternadas rapidamente.

- Paralelo: Donde varias tareas se ejecutan al mismo tiempo en diferentes nucleos.

- Hilos: Unidad de ejecución mas pequeña que puede realizar tareas de manera 
independiente. 

- Task: Abstraccion de hilos C#

-Corrutinas: Tipo especial de concurrencia en Unity, para ejecutar eventos de forma
asincrona.
*/
