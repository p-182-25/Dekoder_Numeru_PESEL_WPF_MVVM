using System;
using System.Windows.Input;

//http://www.c-sharpcorner.com/UploadFile/20c06b/icommand-and-relaycommand-in-wpf/

namespace DekoderPESEL.ModelWidoku
{
    public class RelayCommand : ICommand
    {
        //Fields
        private Action<object> _execute;
        private Func<object, bool> _canExecute;

        //Class Constructor
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            if (execute == null) throw new ArgumentNullException(nameof(execute));
            _execute = execute;
            _canExecute = canExecute;
        }

        //ICommand Members
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}
