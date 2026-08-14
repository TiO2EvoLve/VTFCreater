using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using VTFCreater.ViewModels;

namespace VTFCreater.Views;

public partial class MaterialEditorView : UserControl
{
    public MaterialEditorView() => InitializeComponent();

    private void Slot_DragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = e.DataTransfer.TryGetFiles()?.Any() == true ? DragDropEffects.Copy : DragDropEffects.None;
    }

    private void Slot_Drop(object? sender, DragEventArgs e)
    {
        var path = e.DataTransfer.TryGetFiles()?.FirstOrDefault()?.Path.LocalPath;
        if (path is not null && sender is Control { Tag: string key } && DataContext is MaterialEditorViewModel viewModel)
            viewModel.AssignSlotFile(key, path);
    }
}
