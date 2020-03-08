using System;
using System.Collections.ObjectModel;

namespace SwagOverflowWPF.Interface
{
    public interface ISwagItem
    {
        //Int32 GroupId { get; set; }
        Int32 ItemId { get; set; }
        String AlternateId { get; set; }
        Int32 Sequence { get; set; }
        Int32? ParentId { get; set; }
        String Display { get; set; }

    }

    public interface ISwagHeirarchy<THeirarchy> : ISwagItem
    {
        THeirarchy Parent { get; set; }
        ObservableCollection<THeirarchy> Children { get; set; }
    }

    public interface ISwagItemIterator<THeirarchy> where THeirarchy : ISwagHeirarchy<THeirarchy>
    {
        THeirarchy First();
        THeirarchy Next();
        bool IsDone { get; }
        THeirarchy CurrentItem { get; }
    }
}
