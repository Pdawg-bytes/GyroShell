using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using System;
using Windows.Storage;

using GyroShell.Library.Models.InternalData;
using GyroShell.Library.Services.Environment;
using GyroShell.Library.Services.Managers;

namespace GyroShell.Library.ViewModels
{
    public sealed class PluginSettingViewModel : ObservableObject
    {
        private ObservableCollection<PluginUIModel> _moduleCollection;
        public ObservableCollection<PluginUIModel> ModuleCollection
        {
            get => _moduleCollection;
            set => SetProperty(ref _moduleCollection, value);
        }

        private readonly ISettingsService m_appSettings;
        private readonly IEnvironmentInfoService m_envService;
        private readonly IPluginManager m_moduleManager;

        public PluginSettingViewModel(ISettingsService appSettings, IEnvironmentInfoService envService, IPluginManager moduleManager)
        {
            m_appSettings = appSettings;
            m_envService = envService;
            m_moduleManager = moduleManager;

            if (m_appSettings.ModulesFolderPath != null || m_appSettings.ModulesFolderPath == string.Empty)
            {
                try
                {
                    m_moduleManager.InitializePluginList(m_appSettings.ModulesFolderPath);
                    ModuleCollection = new ObservableCollection<PluginUIModel>(m_moduleManager.GetPlugins());
                    if(ModuleCollection.Count <= 0)
                    {
                        IsParseFailureInfoOpen = true;
                    }
                }
                catch
                {
                    IsParseFailureInfoOpen = true;
                }
            }
            else
            {
                IsEmptyDirectoryInfoOpen = true;
            }
        }

        private bool _isEmptyDirectoryInfoOpen;
        public bool IsEmptyDirectoryInfoOpen
        {
            get => _isEmptyDirectoryInfoOpen;
            set => SetProperty(ref _isEmptyDirectoryInfoOpen, value);
        }

        private bool _isParseFailureInfoOpen;
        public bool IsParseFailureInfoOpen
        {
            get => _isParseFailureInfoOpen;
            set => SetProperty(ref _isParseFailureInfoOpen, value);
        }

        public FontFamily IconFontFamily => m_appSettings.IconStyle switch
        {
            0 => new FontFamily("Segoe MDL2 Assets"),
            1 => new FontFamily("Segoe Fluent Icons"),
            _ => new FontFamily("Segoe MDL2 Assets")
        };

        public IAsyncRelayCommand OpenFolderCommand => new AsyncRelayCommand(OpenFolderInfoAsync);
        private async Task OpenFolderInfoAsync()
        {
            FolderPicker folderPicker = new FolderPicker();

            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, m_envService.MainWindowHandle);

            folderPicker.SuggestedStartLocation = PickerLocationId.Desktop;
            folderPicker.FileTypeFilter.Add("*");

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();

            if (folder != null)
            {
                m_appSettings.ModulesFolderPath = folder.Path;
                try
                {
                    m_moduleManager.InitializePluginList(m_appSettings.ModulesFolderPath);
                    ModuleCollection = new ObservableCollection<PluginUIModel>(m_moduleManager.GetPlugins());
                    IsParseFailureInfoOpen = false;
                }
                catch
                {
                    IsEmptyDirectoryInfoOpen = true;
                }
                IsEmptyDirectoryInfoOpen = false;
            }
            else
            {
                IsEmptyDirectoryInfoOpen = false;
            }
        }
    }
}
