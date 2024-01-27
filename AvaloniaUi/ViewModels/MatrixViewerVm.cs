using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using AvaloniaUi.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaUi.ViewModels;
public partial class MatrixViewerVm : VmBase, IDisposable
{
    public ObservableCollection<ItemVm> Items {get; set;} = new();
    public ObservableCollection<int> data = new ObservableCollection<int>();

    private IMatrixSquareMovingWindow window = new NullMatrixMovingWindow(10);

    [ObservableProperty]
    public int topRow = 1;

    [ObservableProperty]
    public int leftColumn = 1;

    [RelayCommand]
    private void MoveLeft()
    {
        Move(dx: -1);
    }

    [RelayCommand]
    private void MoveRight()
    {
        Move(dx: 1);
    }

    [RelayCommand]
    private void MoveUp()
    {
        Move(dy: -1);
    }

    [RelayCommand]
    private void MoveDown()
    {
        Move(dy: 1);
    }

    private void Move(int dx = 0, int dy = 0) {
        window.TryMoveWindow(dx, dy);
        LeftColumn = window.Location.X + 1;
        TopRow = window.Location.Y + 1;
        UpdateDataFrom(window.GetWindowContent());
    }

    private void UpdateDataFrom(IList<int> newData)
    {
        for(int i = 0; i < newData.Count; ++i)
        {
            if(i < data.Count)
            {
                data[i] = newData[i];
            }
            else
            {
                data.Add(newData[i]);
            }
        }
    }

    public MatrixViewerVm(IMatrixSquareMovingWindow src)
    {
        //If it's working then do not touch!
        window = src;
        UpdateDataFrom(window.GetWindowContent());
        data.CollectionChanged += OnDataChanged;
        Items = new ObservableCollection<ItemVm>() {};
        OnDataChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    private int GetItemsIndex(int dataIndex)
    {
        return window.SideLength + ((dataIndex + window.SideLength) / window.SideLength) + dataIndex + 1;
    }
    private void OnDataChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null && e.NewItems != null && e.OldItems!.Count == e.NewItems?.Count)
        {
            for (int i = e.OldStartingIndex; i < e.OldStartingIndex + e.OldItems!.Count; ++i)
            {
                Items[GetItemsIndex(i)].Text = data[i].ToString();
            }
            return;
        }
        
        for (int i = Items.Count; i < window.SideLength+1; ++i)
        {
            Items.Add(new ItemVm() {
                Text = (LeftColumn + i - 1).ToString(),
                Meaningfull = false,
            });
        }
        Items[0].Text = "";

        var oldCount = int.Max(0, Items.Count - window.SideLength * 2 - 1);
        var minCount = int.Min(data.Count, oldCount);
        for (int i = 0; i < minCount; ++i)
        {
            Items[GetItemsIndex(i)] = new ItemVm() {
                Text = data[i].ToString(),
                Meaningfull = true,
            };
        }
        if(oldCount > data.Count) {
            for (int i = GetItemsIndex(data.Count); i < Items.Count; ++i)
            {
                Items.RemoveAt(i);
            }
            return;
        }
        for (int i = minCount; i < data.Count; ++i)
        {
            if(i % window.SideLength == 0)
            {
                Items.Add(new ItemVm() {
                    Text = (TopRow + i / window.SideLength).ToString(),
                    Meaningfull = false,
                });
            }
            Items.Add(new ItemVm() {
                Text = data[i].ToString(),
                Meaningfull = true,
            });
        }
        updateIndexes();
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if(e.PropertyName == nameof(LeftColumn) || e.PropertyName == nameof(TopRow))
        {
            updateIndexes();
        }
        base.OnPropertyChanged(e);
    }

    private void updateIndexes()
    {
        for (int i = 0; i < window.SideLength; ++i)
        {
            Items[i+1].Text = (LeftColumn + i).ToString();
            Items[i+1].Meaningfull = false;
            Items[(i+1)*(window.SideLength+1)].Text = (TopRow + i).ToString();
            Items[(i+1)*(window.SideLength+1)].Meaningfull = false;
        }
    }

    public void Dispose()
    {
        window.Dispose();
    }

}