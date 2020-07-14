using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Comdata.AppSupport.PPOLTestFileGenerator.MvvmHelper
{
    public class ActionCommand : ICommand
    {
        private readonly Action<string> _codeToExecute;
        private readonly Action<string> _canExecteCodeToExecute;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _codeToExecute(null);
        }

        public ActionCommand(Action<string> codeToExecute)
        {
            _codeToExecute = codeToExecute;
        }

        public ActionCommand(Action<string> codeToExecute, Action<string> canExecteCodeToExecute)
        {
            _codeToExecute = codeToExecute;
            _canExecteCodeToExecute = canExecteCodeToExecute;
        }
    }
}
