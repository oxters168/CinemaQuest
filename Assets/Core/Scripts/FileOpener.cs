using UnityEngine;
using SimpleFileBrowser;

[System.Serializable]
public class StringEvent : UnityEngine.Events.UnityEvent<string> {}

public class FileOpener : MonoBehaviour
{
    public string title = "Load";
    public string buttonText = "Select";
    public StringEvent onFileSelected;

    public void ShowOpenPrompt()
    {
        FileBrowser.SetFilters(false, ".mp4");
        if (!FileBrowser.IsOpen)
            FileBrowser.ShowLoadDialog(OnSuccess, OnCancel, false, null, title, buttonText);
    }

    private void OnSuccess(string path)
    {
        onFileSelected?.Invoke(path);
    }
    private void OnCancel()
    {
        Debug.LogWarning("User cancelled open file dialog");
    }
}
