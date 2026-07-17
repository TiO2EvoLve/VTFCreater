using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VTFCreater.Enum;
using VTFCreater.Models;
using VTFCreater.Services;

namespace VTFCreater.ViewModels;

public partial class HomeViewModel : ViewModelBase
{
    private readonly ConfigService _configService;
    private readonly ProcessingService _processingService;
    private readonly LogService _logService;

    [ObservableProperty] private bool _isProcessing;

    public ObservableCollection<LogEntry> LogEntries => _logService.Entries;

    public string SourceDirectory => _configService.Config.SourceDirectory;

    public string OutputDirectory => _configService.Config.OutputDirectory;

    public Formats Format => _configService.Config.Format;

    public HomeViewModel(ConfigService configService, ProcessingService processingService, LogService logService)
    {
        _configService = configService;
        _processingService = processingService;
        _logService = logService;
    }

    public void RefreshSummary()
    {
        OnPropertyChanged(nameof(SourceDirectory));
        OnPropertyChanged(nameof(OutputDirectory));
        OnPropertyChanged(nameof(Format));
    }

    [RelayCommand(CanExecute = nameof(CanProcess))]
    private async Task StartProcessingAsync()
    {
        IsProcessing = true;
        StartProcessingCommand.NotifyCanExecuteChanged();

        try
        {
            await _processingService.ProcessAsync(_configService.Config, _logService);
        }
        catch (Exception ex)
        {
            _logService.Error(ex.Message);
        }
        finally
        {
            IsProcessing = false;
            StartProcessingCommand.NotifyCanExecuteChanged();
        }
    }

    [RelayCommand]
    private void ClearLogs()
    {
        _logService.Clear();
    }

    private bool CanProcess() => !IsProcessing;
}
