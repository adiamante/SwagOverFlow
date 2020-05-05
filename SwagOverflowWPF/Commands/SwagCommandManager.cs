using SwagOverflow.Commands;
using SwagOverflow.ViewModels;
using SwagOverFlow.Utils;
using SwagOverflowWPF.Collections;
using System;
using System.Windows.Input;

namespace SwagOverflowWPF.Commands
{
    public class SwagCommandManager : ViewModelBase
    {
        #region Private Members
        ConcurrentObservableSortedDictionary<Int32, SwagCommand> _commandHistory = new ConcurrentObservableSortedDictionary<Int32, SwagCommand>();
        ConcurrentObservableSortedDictionary<Int32, SwagCommand> _undoHistory = new ConcurrentObservableSortedDictionary<Int32, SwagCommand>();
        ICommand _undoCommand, _redoCommand;
        Boolean _listening = true;
        #endregion Private Members

        #region Properties
        #region CommandHistory
        public ConcurrentObservableSortedDictionary<Int32, SwagCommand> CommandHistory
        {
            get { return _commandHistory; }
            set { SetValue(ref _commandHistory, value); }
        }
        #endregion CommandHistory

        #region UndoHistory
        public ConcurrentObservableSortedDictionary<Int32, SwagCommand> UndoHistory
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
                SwagCommand cmd = _commandHistory[_commandHistory.Count - 1];
                _undoHistory.Add(Int32.MaxValue - _undoHistory.Count, cmd);
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
                SwagCommand cmd = _undoHistory[Int32.MaxValue - _undoHistory.Count + 1];
                _commandHistory.Add(_commandHistory.Count, cmd);
                _undoHistory.Remove(Int32.MaxValue - _undoHistory.Count + 1);
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
            if (_listening && e.PropertyChangedArgs.PropertyName != "CanUndo" && e.SwagItem.CanUndo)
            {
                SwagPropertyChangedCommand cmd = new SwagPropertyChangedCommand(
                    e.PropertyChangedArgs.PropertyName,
                    e.PropertyChangedArgs.Object,
                    e.PropertyChangedArgs.OldValue,
                    e.PropertyChangedArgs.NewValue);
                cmd.Display = e.Message ?? $"{e.PropertyChangedArgs.PropertyName} ({e.PropertyChangedArgs.OldValue}) => {e.PropertyChangedArgs.NewValue}";

                _commandHistory.Add(_commandHistory.Count, cmd);
            }
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
