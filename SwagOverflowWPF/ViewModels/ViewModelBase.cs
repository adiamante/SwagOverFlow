using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SwagOverflowWPF.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void SetValue<T>(ref T backingField, T value, [CallerMemberName] string propertyname = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
                return;

            backingField = value;
            OnPropertyChanged(propertyname);
        }
    }

    //https://stackoverflow.com/questions/7677854/notifypropertychanged-event-where-event-args-contain-the-old-value
    public class PropertyChangedExtendedEventArgs<T> : PropertyChangedEventArgs
    {
        public virtual T OldValue { get; private set; }
        public virtual T NewValue { get; private set; }

        public PropertyChangedExtendedEventArgs(string propertyName, T oldValue, T newValue)
            : base(propertyName)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }

    public abstract class ViewModelBaseExtended : ViewModelBase
    {
        public event PropertyChangedEventHandler PropertyChangedExtended;

        protected virtual void OnPropertyChangedExtended<T>(T oldValue, T newValue, [CallerMemberName] string propertyName = null)
        {
            PropertyChangedExtended?.Invoke(this, new PropertyChangedExtendedEventArgs<T>(propertyName, oldValue, newValue));
        }

        protected override void SetValue<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
                return;

            T oldValue = backingField;
            backingField = value;
            OnPropertyChanged(propertyName);
            OnPropertyChangedExtended(oldValue, value, propertyName);
        }
    }
}
