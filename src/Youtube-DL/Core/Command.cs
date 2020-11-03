using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Youtube_DL.Core
{
    public sealed class Command : ICommand
    {
        readonly Action<object> _execute;

        public Command(Action<object> execute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        }

        public Command(Action execute) : this(o => execute())
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public event EventHandler CanExecuteChanged;
    }
}
