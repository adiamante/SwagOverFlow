using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SwagOverflowWPF.Interface
{
    public interface iSwagItem<THeirarchy>
    {
        String Group { get; set; }
        String Key { get; set; }
        Int32 Id { get; set; }
        Int32? ParentId { get; set; }
        String Display { get; set; }
        Byte [] Data { get; set; }
        THeirarchy Parent { get; set; }
        ObservableCollection<THeirarchy> Children { get; set; }

    }
}
