using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Data;
using Avalonia.Platform.Storage;
using AvaloniaUi.Models;
using AvaloniaUi.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using org.matheval;

namespace AvaloniaUi.ViewModels;

public partial class MatrixGeneratorVm : TabInnerVmBase
{

    public MatrixGeneratorVm()
    {
        Header = "Generator";
    }
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
    
    [ObservableProperty]
    private int progress = 0;

    [ObservableProperty]
    private bool isGenerating = false;


    private async Task<IStorageFile?> DoSaveFilePickerAsync()
    {
        var filesService = App.Current?.Services?.GetService<IFilesService>();
        if (filesService == null)
        {
            throw new NullReferenceException("Missing File Service instance.");
        }
        
        return await filesService.SaveFileAsync("Save Generated Matrix File");
    }

    private CancellationTokenSource generateCancellationSource = new CancellationTokenSource();

    [RelayCommand]
    private async Task Generate()
    {
        if(!isItemExpressionValid || !isMatrixSideSizeValid)
        {
            throw new UnreachableException();
        }
        
        var generator = new MatrixGenerator(itemExpression, matrixSideSize);
        generator.ProgressEvent += (sender, e) => {
            Progress = e.ProgressPercentage;
        };
        
        var file = await DoSaveFilePickerAsync();
        if(file == null)
        {
            return;
        }

        Header = $"Generating {file.Name}";

        try
        {
            IsGenerating = true;
            await generator.Generate(file.Path.AbsolutePath, generateCancellationSource.Token);
        }
        catch(TaskCanceledException e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            IsGenerating = false;
        }

        Header = $"Generated {file.Name}";
    }

    public void StopGeneration()
    {
        generateCancellationSource.Cancel();
    }

    public override void Dispose()
    {
        StopGeneration();
    }
}