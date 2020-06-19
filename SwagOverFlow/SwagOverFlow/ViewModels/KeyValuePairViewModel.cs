using SwagOverFlow.ViewModels;
using System;

namespace SwagOverFlow.ViewModels
{
    public class KeyValuePairViewModel<TKey, TValue> : ViewModelBaseExtended
    {
        #region Private Properties
        Boolean _enabled;
        TKey _key;
        TValue _value;
        #endregion Private Properties

        #region Properties
        #region Enabled
        public Boolean Enabled
        {
            get { return _enabled; }
            set { SetValue(ref _enabled, value); }
        }
        #endregion Enabled
        #region Key
        public TKey Key
        {
            get { return _key; }
            set { SetValue(ref _key, value); }
        }
        #endregion Key
        #region Value
        public TValue Value
        {
            get { return _value; }
            set 
            { 
                SetValue(ref _value, value);
                Init();
            }
        }
        #endregion Value
        #endregion Properties

        #region Initialization
        public KeyValuePairViewModel()
        {
            Init();
        }

        public KeyValuePairViewModel(TKey key, TValue value) : this()
        {
            _key = key;
            _value = value;
            Init();
        }
        #endregion Initialization

        public void Init()
        {
            if (Value == null && typeof(TValue).GetConstructor(Type.EmptyTypes) != null)
            {
                Value = (TValue)Activator.CreateInstance(typeof(TValue));
            }

            if (typeof(TValue).IsSubclassOf(typeof(ViewModelBaseExtended)))
            {
                ViewModelBaseExtended vm = Value as ViewModelBaseExtended;
                //If property of Value changes, let the owning ViewModel know
                vm.PropertyChangedExtended += (s, e) =>
                {
                    OnPropertyChangedExtended(vm, vm, "Value");
                };
            }
        }
    }
}
