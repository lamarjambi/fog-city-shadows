using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FakeTerminal : MonoBehaviour
{
    public TMP_Text terminalText;
    public AudioSource keySound;
    public ZoomAndLookController zoomController;

    private string introText =
        "PS C:\\Users\\player> cd work\n" +
        "PS C:\\Users\\player\\work> today-task\n" +
        "PS C:\\Users\\player\\work> ";

    private string targetCommand = "git push";
    private string typed = "";
    private bool accepted = false;

    void Start()
    {
        terminalText.text = introText + "_";
    }

    void Update()
    {
        if (accepted) return;

        foreach (char c in Input.inputString)
        {
            if (c == '\b' && typed.Length > 0)
                typed = typed.Substring(0, typed.Length - 1);
            else if (c == '\n' || c == '\r')
            {
                if (typed.Trim().ToLower() == targetCommand)
                {
                    accepted = true;
                    terminalText.text = introText + targetCommand;
                    zoomController.StartZoom();
                }
                else typed = "";
            }
            else if (c >= ' ' && c <= '~')
            {
                typed += c;
                if (keySound) keySound.Play();
            }
        }

        string display = introText + typed;
        if (Time.time % 1f < 0.5f) display += "_";
        terminalText.text = display;
    }
}
