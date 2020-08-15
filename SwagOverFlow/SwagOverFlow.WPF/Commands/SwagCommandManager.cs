using SwagOverFlow.ViewModels;
using SwagOverFlow.Utils;
using System;
using System.Windows.Input;
using SwagOverFlow.Commands;
using SwagOverFlow.Collections;
using SwagOverFlow.WPF.UI;
using System.ComponentModel;

namespace SwagOverFlow.WPF.Commands
{
    public class SwagCommandManager : ViewModelBase
    {
        #region Private Members
        SwagObservableOrderedConcurrentDictionary<Int32, SwagCommand> _commandHistory = new SwagObservableOrderedConcurrentDictionary<Int32, SwagCommand>();
        SwagObservableOrderedConcurrentDictionary<Int32, SwagCommand> _undoHistory = new SwagObservableOrderedConcurrentDictionary<Int32, SwagCommand>();
        ICommand _undoCommand, _redoCommand;
        Boolean _listening = true;
        #endregion Private Members

        #region Initialization
        public SwagCommandManager()
        {
            ICollectionView col = UIHelper.GetCollectionView(_undoHistory);
            col.SortDescriptions.Add(new SortDescription("Key", ListSortDirection.Descending));
        }
        #endregion Initialization

        #region Properties
        #region CommandHistory
        public SwagObservableOrderedConcurrentDictionary<Int32, SwagCommand> CommandHistory
        {
            get { return _commandHistory; }
            set { SetValue(ref _commandHistory, value); }
        }
        #endregion CommandHistory

        #region UndoHistory
        public SwagObservableOrderedConcurrentDictionary<Int32, SwagCommand> UndoHistory
        {
            get { return _undoHistory; }
            set { SetValue(ref _undoHistory, value); }
        }
        #endregion UndoHistory

        #region UndoCommand
        public ICommand UndoCommand
        {
            get
            {
                return _undoCommand ?? (_undoCommand =
                    new RelayCommand(() => Undo()
                ));
            }
        }
        #endregion UndoCommand

        #region RedoCommand
        public ICommand RedoCommand
        {
            get
            {
                return _redoCommand ?? (_redoCommand =
                    new RelayCommand(() => Redo()
                ));
            }
        }
        #endregion RedoCommand
        #endregion Properties

        #region Methods
        public void Undo()
        {
            if (_commandHistory.Count > 0)
            {
                _listening = false;
                SwagCommand cmd = _commandHistory.Get(_commandHistory.Count - 1);
                _undoHistory.Add(_undoHistory.Count, cmd);
                _commandHistory.Remove(_commandHistory.Count - 1);
                cmd.Undo();
                _listening = true;
            }
        }

        public void Redo()
        {
            if (_undoHistory.Count > 0)
            {
                _listening = false;
                SwagCommand cmd = _undoHistory.Get(_undoHistory.Count - 1);
                _commandHistory.Add(_commandHistory.Count, cmd);
                _undoHistory.Remove(_undoHistory.Count - 1);
                cmd.Execute();
                _listening = true;
            }
        }

        public void Attach(ISwagItemChanged subject)
        {
            subject.SwagItemChanged += Subject_SwagItemChanged;
        }

        public void Detatch(ISwagItemChanged subject)
        {
            subject.SwagItemChanged -= Subject_SwagItemChanged;
        }
        #endregion Methods

        #region Events
        private void Subject_SwagItemChanged(object sender, SwagItemChangedEventArgs e)
        {
            //FIX_THIS Going to redesign the Command Manager, The View will be responsible for calling
            //if (_listening && e.PropertyChangedArgs.PropertyName != "CanUndo" && e.SwagItem.CanUndo)
            //{
            //    SwagPropertyChangedCommand cmd = new SwagPropertyChangedCommand(
            //        e.PropertyChangedArgs.PropertyName,
            //        e.PropertyChangedArgs.Object,
            //        e.PropertyChangedArgs.OldValue,
            //        e.PropertyChangedArgs.NewValue);
            //    cmd.Display = e.Message ?? $"{e.PropertyChangedArgs.PropertyName} ({e.PropertyChangedArgs.OldValue}) => {e.PropertyChangedArgs.NewValue}";

            //    _commandHistory.Add(_commandHistory.Count, cmd);
            //}
        }
        #endregion Events
    }

    #region SwagCommand
    public abstract class SwagCommand
    {
        public String Display { get; set; }

        public abstract void Execute();
        public abstract void Undo();
    }
    #endregion SwagCommand

    #region SwagPropertyChangedCommand
    public class SwagPropertyChangedCommand : SwagCommand
    {
        public String Property { get; set; }
        public object Object { get; set; }
        public object OldValue { get; set; }
        public object NewValue { get; set; }

        public SwagPropertyChangedCommand(String property, object obj, object oldValue, object newValue)
        {
            Property = property;
            Object = obj;
            OldValue = oldValue;
            NewValue = newValue;
        }

        public override void Execute()
        {
            ReflectionHelper.PropertyInfoCollection[Object.GetType()][Property].SetValue(Object, NewValue);
        }

        public override void Undo()
        {
            ReflectionHelper.PropertyInfoCollection[Object.GetType()][Property].SetValue(Object, OldValue);
        }
    }
    #endregion SwagPropertyChangedCommand
}
