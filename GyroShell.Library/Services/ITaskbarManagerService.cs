namespace GyroShell.Library.Services
{
    public interface ITaskbarManagerService
    {
        public void Initialize();

        public void ShowTaskbar();
        public void HideTaskbar();

        public void ToggleStartMenu();
        public void ToggleControlCenter();
        public void ToggleActionCenter();
        public void ToggleAutoHideExplorer(bool doHide);

        public void NotifyWinlogonShowShell();
    }
}
