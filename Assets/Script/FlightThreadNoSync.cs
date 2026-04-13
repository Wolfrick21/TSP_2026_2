using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;

public class FlightThreadNoSinc : MonoBehaviour
{
    public float speed = 50f;
    public float rotationSpeed = 100f;
    public Transform cameraTransform;
    public Vector2 movementInput;

    //Control de Iteraciones
    public int turbulenceIterations = 1000000;

    //Lista de vectores de pocsicion calculados

    private List<Vector3> turbulenceForces = new List<Vector3>();

    //Variables para manipular el hilo secundario

    private Thread turbulenceThread; //Instancia hilo secundario
    private bool isTurbulenceRunning = false; //Bandera para saber si sigue el calculo
    private bool stopTurbulenceThread = false; //Bandera para saber si el hilo terminó
    private float capturedTime; //Variable para almacenar el tiempo transcurrido

    //Bandera de control sobre lectura
    public bool read = false;
    //Ruta de almacenamiento de archivo
    string filepath;

    //Metodo para mover la nave
    public void OnMovement(InputValue value)
    {
        movementInput = value.Get<Vector2>();
    }

    void Start()
    {
        filepath = Application.dataPath + "/TurbulenceData.txt";
        Debug.Log("Ruta de archivo: "+filepath);
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraTransform == null)
        {
            Debug.LogError("No hay camara asignada");
            return;
        }

        //ACTIVIDAD 1: Proceso en hilo secundario

        //Tiempo transcurrido
        capturedTime = Time.time;

        //Proceso pesado en hilo secundario
        if (!isTurbulenceRunning)
        {
            isTurbulenceRunning = true;
            stopTurbulenceThread = false;

            turbulenceThread = new Thread(() =>
                SimulateTurbulence(capturedTime));
            turbulenceThread.Start();
        }

        //Mover la nave linealmente

        Vector3 moveDirection = cameraTransform.forward * movementInput.y * speed * Time.deltaTime;
        this.transform.position += moveDirection;

        //Mover la nave en rotacion
        float yaw = movementInput.x * rotationSpeed * Time.deltaTime;
        this.transform.Rotate(0, yaw, 0);

        TryReadFile();
    }

    public void SimulateTurbulence(float time)
    {
        turbulenceForces.Clear();

        //Repeticiones

        for (int i = 0; i < turbulenceIterations; i++)
        {
            //Verificar si se debe detener el hilo
            if (stopTurbulenceThread)
            {
                break;
            }
            Vector3 force = new Vector3(
                Mathf.PerlinNoise(i * 0.001f, time) * 2 - 1,
                Mathf.PerlinNoise(i * 0.002f, time) * 2 - 1,
                Mathf.PerlinNoise(i * 0.003f, time) * 2 - 1
                );

            turbulenceForces.Add(force);
        }

        //Señal  en consola de inicio del hilo

        Debug.Log("Iniciando simulacion de turbulencia");


        //Simulcion completa
        isTurbulenceRunning = false;

        //ACTIVIDAD 3. Metodo de escritura de archvivo
        //Escritura del archivo
        using (StreamWriter writer = new StreamWriter(filepath, false))
        {
            foreach (var force in turbulenceForces)
            {
                writer.WriteLine(force.ToString());
            }
            writer.Flush();
        }
    
    }

    void TryReadFile()
    {
        try
        {
            string content = File.ReadAllText(filepath);
            Debug.Log("Archivo leido" + content);
        }
        catch (IOException ex)
        {
            Debug.LogError("Error de acceso al archivo" + ex.Message);
        }
    }

    private void OnDestroy()
    {
        //Indicar el cierre del hilo secundario
        stopTurbulenceThread = true;
        //Verificar si el hilo existe y se esta ejecutando
        if (turbulenceThread != null && turbulenceThread.IsAlive)
        {
            //Lo unimos al hilo principal y cerramos ejecucion
            turbulenceThread.Join();
        }
    }
}
