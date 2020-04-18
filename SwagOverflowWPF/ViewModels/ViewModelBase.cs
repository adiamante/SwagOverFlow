using System;
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
            SetValue<T>(ref backingField, value, true, propertyname);
        }

        protected virtual void SetValue<T>(ref T backingField, T value, Boolean doEqualityCheck, [CallerMemberName] string propertyname = null)
        {
            if (doEqualityCheck)
            {
                if (EqualityComparer<T>.Default.Equals(backingField, value))
                    return;
            }
            
            backingField = value;
            OnPropertyChanged(propertyname);
        }
    }

    //https://stackoverflow.com/questions/7677854/notifypropertychanged-event-where-event-args-contain-the-old-value
    public class PropertyChangedExtendedEventArgs : PropertyChangedEventArgs
    {
        public virtual object Object { get; private set; }
        public virtual object OldValue { get; private set; }
        public virtual object NewValue { get; private set; }
        public virtual String Message { get; set; }

        public PropertyChangedExtendedEventArgs(string propertyName, object obj, object oldValue, object newValue)
            : base(propertyName)
        {
            Object = obj;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }

    public abstract class ViewModelBaseExtended : ViewModelBase
    {

        public event EventHandler<PropertyChangedExtendedEventArgs> PropertyChangedExtended;

        protected virtual void OnPropertyChangedExtended(object oldValue, object newValue, [CallerMemberName] string propertyName = null)
        {
            PropertyChangedExtended?.Invoke(this, new PropertyChangedExtendedEventArgs(propertyName, this, oldValue, newValue));
        }

        protected override void SetValue<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null)
        {
            SetValue<T>(ref backingField, value, true, propertyName);
        }

        protected override void SetValue<T>(ref T backingField, T value, Boolean doEqualityCheck, [CallerMemberName] string propertyName = null)
        {
            if (doEqualityCheck)
            {
                if (EqualityComparer<T>.Default.Equals(backingField, value))
                    return;
            }

            T oldValue = backingField;
            backingField = value;
            OnPropertyChanged(propertyName);
            OnPropertyChangedExtended(oldValue, value, propertyName);
        }
    }
}
