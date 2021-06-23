using System;
using System.Diagnostics;
using System.Windows.Input;

namespace EmployeeApiCore.Core.Object
{
	public class CommandObject : ICommand
	{
		#region Variables

		private readonly System.Action<dynamic> _execute;
		private readonly System.Func<bool> _canExecute;
		private readonly System.Func<dynamic, bool> _canExecute1;

		#endregion

		#region Constructor

		public CommandObject(System.Action<dynamic> execute = null) : this(execute, null)
		{
		}

		
		public CommandObject(System.Action<dynamic> execute, System.Func<bool> canExecute)
		{
			if(execute == null)
			{
				throw new System.ArgumentNullException("execute");
			}

			_execute = execute;
			_canExecute = canExecute;

		}

		public CommandObject(System.Action<dynamic> execute, System.Func<dynamic, bool> canExecute, bool useCanExecuteParam)
		{
			if(execute == null)
			{
				throw new System.ArgumentNullException("execute");
			}

			_execute = execute;
			_canExecute1 = canExecute;
		}

		#endregion

		#region ICommand Members

		[DebuggerStepThrough]
		public bool CanExecute(object parameter)
		{
			if(_canExecute != null)
			{
				return _canExecute();
			}
			else if(_canExecute1 != null)
			{
				return _canExecute1(parameter);
			}
			else
			{
				return true;
			}
		}

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		public void Execute(object parameter)
		{
			_execute(parameter);
		}

		#endregion

	}
}
