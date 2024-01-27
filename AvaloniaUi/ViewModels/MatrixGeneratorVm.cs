using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data;
using Avalonia.Platform.Storage;
using AvaloniaUi.Models;
using CommunityToolkit.Mvvm.Input;
using org.matheval;

namespace AvaloniaUi.ViewModels;

public partial class MatrixGeneratorVm : VmBase
{
    private string itemExpression = "kr";
    private bool isItemExpressionValid = true;
    public string ItemExpression
    {
        get => itemExpression;
        set
        {
            isItemExpressionValid = false;
            if (string.IsNullOrEmpty(value))
            {
                throw new DataValidationException("This field is required");
            }
            var expr = new Expression(value);
            var errors = expr.GetError();
            if(errors.Count != 0) {
                throw new DataValidationException(errors.Aggregate(
                    (accumulator, i) => accumulator.Replace("EOF", "end") + i + '\n')
                );
            }
            isItemExpressionValid = true;
            RaiseAndSetIfChanged(ref itemExpression, value);
        }
    }
    
    private bool isMatrixSideSizeValid = true;
    private int matrixSideSize = 1;

    public string MatrixSideSize
    {
        get => matrixSideSize.ToString();
        set
        {
            isMatrixSideSizeValid = false;
            if (string.IsNullOrEmpty(value))
            {
                throw new DataValidationException("This field is required");
            }
            int number;
            if (!int.TryParse(value, out number))
            {
                throw new DataValidationException("Must be int32");
            }
            if (number <= 0)
            {
                throw new DataValidationException("Must be positive");
            }
            isMatrixSideSizeValid = true;
            RaiseAndSetIfChanged(ref matrixSideSize, number);
        }
    }

    private async Task<IStorageFile?> DoSaveFilePickerAsync()
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
            desktop.MainWindow?.StorageProvider is not { } provider)
        {
            throw new NullReferenceException("Missing StorageProvider instance.");
        }
        
        return await provider.SaveFilePickerAsync(new FilePickerSaveOptions()
        {
            Title = "Save Generated Matrix File"
        });
    }


    [RelayCommand]
    private async Task Generate()
    {
        if(!isItemExpressionValid || !isMatrixSideSizeValid)
        {
            throw new UnreachableException();
        }
        var generator = new MatrixGenerator(itemExpression, matrixSideSize);

        var file = await DoSaveFilePickerAsync();
        if(file == null)
        {
            return;
        }
        generator.Generate(file.Path.AbsolutePath);
    }
}