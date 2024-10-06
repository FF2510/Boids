using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI : MonoBehaviour
{
    public TextMeshProUGUI fpsText;
    public TextMeshProUGUI boidsText;
    public Camera camera;

    private float scale = 0;
    private float deltaTime = 0.0f;


    void Start()
    {
        Cursor.visible = true;
        scale = camera.orthographicSize;
    }


    // Update is called once per frame
    void Update()
    {
        // Calc. and show fps.
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText.text = Mathf.Ceil(fps).ToString() + " FPS";

        // Get and show boids.
        boidsText.text = BoidSpawner.Boids.Count.ToString();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }

    public void SliderValue(float value)
    {
        float size = Mathf.Clamp(scale * value, 5, 25);


        camera.orthographicSize = size;
    }

    void QuitGame()
    {
        Application.Quit();
    }
}
