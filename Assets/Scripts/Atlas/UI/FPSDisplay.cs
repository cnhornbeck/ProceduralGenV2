using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    int frameCount = 0;
    float deltaTime = 0.0f;
    float fps = 0.0f;
    float updateInterval = 0.2f; // Time between updates
    float nextUpdateTime = 0.0f; // When to update next

    void Update()
    {
        frameCount++;
        deltaTime += Time.unscaledDeltaTime;

        // Update FPS every updateInterval seconds
        if (Time.unscaledTime >= nextUpdateTime)
        {
            // Calculate FPS
            fps = frameCount / deltaTime;

            nextUpdateTime = Time.unscaledTime + updateInterval;

            // Reset for the next interval
            frameCount = 0;
            deltaTime = 0;
        }
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new(0, 0, w, h * 2 / 100);

        style.alignment = TextAnchor.UpperRight;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);

        // Use the calculated FPS from the Update method
        float msec = 1000.0f / fps;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);

        GUI.Label(rect, text, style);
    }
}
