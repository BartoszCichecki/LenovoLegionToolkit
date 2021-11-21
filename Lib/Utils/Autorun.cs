using Microsoft.Win32.TaskScheduler;
using System.Diagnostics;

namespace LenovoLegionToolkit.Lib.Utils
{
    internal class Autorun
    {
        private const string TaskName = "LenovoLegionToolkit_Autorun_6efcc882-924c-4cbc-8fec-f45c25696f98";

        public bool IsEnabled => TaskService.Instance.GetTask(TaskName) != null;

        public void Enable()
        {
            Disable();

            var ts = TaskService.Instance;
            var td = ts.NewTask();
            td.Principal.RunLevel = TaskRunLevel.Highest;
            td.Triggers.Add(new LogonTrigger());
            td.Actions.Add($"\"{Process.GetCurrentProcess().MainModule.FileName}\"", "--minimized");
            ts.RootFolder.RegisterTaskDefinition(TaskName, td);
        }

        public void Disable() => TaskService.Instance.RootFolder.DeleteTask(TaskName, false);
    }
}
