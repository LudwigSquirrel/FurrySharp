using System.IO;
using ImGuiNET;

namespace MonoGame.ImGuiNet;

public class DeerFileDialog(string defaultPath, string defaultName)
{
    private bool dirty = true;
    private string folderPath = defaultPath;
    private string fileName = defaultName;
    public string ActionLabel { get; set; } = "Save";

    private DirectoryInfo directoryInfo;

    public bool DoDialog(out string destination)
    {
        if (dirty && Directory.Exists(folderPath))
        {
            directoryInfo = new DirectoryInfo(folderPath);
            dirty = false;
        }
        ImGui.Begin($"{ActionLabel} File");
        dirty = ImGui.InputText("Folder", ref folderPath, 100);
        if (directoryInfo.Parent != null && ImGui.Button("../"))
        {
            folderPath = directoryInfo.Parent.FullName;
            dirty = true;
        }
        foreach (FileSystemInfo info in directoryInfo.EnumerateFileSystemInfos())
        {
            if (ImGui.Button(info.Name))
            {
                if ((info.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    folderPath = info.FullName;
                    dirty = true;
                }
                else
                {
                    fileName = info.Name;
                }
            }
        }

        ImGui.InputText("File Name", ref fileName, 100);
        ImGui.SameLine();
        bool done = ImGui.Button(ActionLabel);
        ImGui.End();
        destination = Path.Combine(folderPath, fileName);
        return done;
    }
}