using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SwagOverflowWPF.Interface
{
    public interface ISwagItem
    {
        //Int32 GroupId { get; set; }
        Int32 ItemId { get; set; }
        String AlternateId { get; set; }
        Int32? ParentId { get; set; }
        String Display { get; set; }
        Byte[] Data { get; set; }

    }

    public interface ISwagItem<THeirarchy> : ISwagItem
    {
        THeirarchy Parent { get; set; }
        ObservableCollection<THeirarchy> Children { get; set; }
    }
}
