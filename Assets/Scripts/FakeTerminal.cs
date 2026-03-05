using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FakeTerminal : MonoBehaviour
{
    public TMP_Text terminalText;
    public AudioSource keySound;
    public ZoomAndLookController zoomController;

    private string baseIntroText =
        "PS C:\\Users\\??> cd work\n" +
        "PS C:\\Users\\??\\work> cd today-task\n" +
        "PS C:\\Users\\??\\work\\today-task> git add *\n" +
        "PS C:\\Users\\??\\work\\today-task> git commit -m 'task done'\n";

    private string currentIntroText;
    private string promptText = "PS C:\\Users\\??\\work\\today-task> ";
    
    private string targetCommand = "git push";
    private string typed = "";
    private bool accepted = false;

    void Start()
    {
        currentIntroText = baseIntroText;
        UpdateDisplay();
    }

    void Update()
    {
        if (accepted) return;

        foreach (char c in Input.inputString)
        {
            if (c == '\b' && typed.Length > 0)
            {
                typed = typed.Substring(0, typed.Length - 1);
                PlayTypingSound(); 
            }
            else if (c == '\n' || c == '\r')
            {
                PlayTypingSound(); 
                
                if (typed.Trim().ToLower() == targetCommand)
                {
                    accepted = true;
                    terminalText.text = currentIntroText + promptText + targetCommand;
                    zoomController.StartZoom();
                    return;
                }
                else if (!string.IsNullOrEmpty(typed.Trim()))
                {
                    string errorMessage = $"<color=red>{typed.Trim()} : The term '{typed.Trim()}' is not recognized as the name of a cmdlet, function, script file, or operable program.</color>\n";
                    currentIntroText += promptText + typed + "\n" + errorMessage;
                    typed = "";
                }
                else
                {
                    currentIntroText += promptText + "\n";
                    typed = "";
                }
            }
            else if (c >= ' ' && c <= '~')
            {
                typed += c;
                PlayTypingSound(); 
            }
        }

        UpdateDisplay();
    }

    private void PlayTypingSound()
    {
        if (keySound)
        {
            keySound.Stop();
            
            keySound.pitch = Random.Range(0.95f, 1.05f);
            
            keySound.Play();
        }
    }

    private void UpdateDisplay()
    {
        string display = currentIntroText + promptText + typed;
        if (Time.time % 1f < 0.5f) display += "_";
        terminalText.text = display;
    }
}