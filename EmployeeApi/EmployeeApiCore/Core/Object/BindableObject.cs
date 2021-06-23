using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace EmployeeApiCore.Core.Object
{	
	public class BindableObject : INotifyPropertyChanged, IDisposable
	{
		#region Variables

		private Dictionary<string, dynamic> _propertyBag = new Dictionary<string, dynamic>(StringComparer.InvariantCultureIgnoreCase);

		#endregion

		#region Constructor

		public BindableObject()
		{

			this.Guid = Guid.NewGuid();
			this.Clear();
			this.Initialize();

		}

		#endregion

		#region Properties

		[XmlIgnore]
		[JsonIgnore]
		public System.Guid Guid { get; set; }

		#endregion

		#region Functions

		public dynamic GetPropertyValue(string propertyName)
		{

			if(_propertyBag == null)
				_propertyBag = new Dictionary<string, dynamic>();

			if(_propertyBag.ContainsKey(propertyName.Replace("get_", string.Empty)))
				return _propertyBag[propertyName.Replace("get_", string.Empty)];

			return null;

		}

		public bool SetPropertyValue(string propertyName, dynamic value)
		{

			if(_propertyBag == null)
				_propertyBag = new Dictionary<string, dynamic>();

			if(_propertyBag.ContainsKey(propertyName.Replace("set_", string.Empty)))
			{
				_propertyBag[propertyName.Replace("set_", string.Empty)] = value;
			}
			else
			{
				_propertyBag.Add(propertyName.Replace("set_", string.Empty), value);
			}

			this.InvokeChange(propertyName.Replace("set_", string.Empty));

			return true;

		}

		public override bool Equals(object obj)
		{

			if(obj is BindableObject)
			{
				if(((BindableObject)obj).Guid == this.Guid)
				{
					return true;
				}
			}

			return false;

		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		#endregion

		#region Procedures

		private void Initialize()
		{

			Type objectType = this.GetType();
			PropertyInfo[] properties = objectType.GetProperties();
			dynamic value = null;

			foreach(var propertyInfo in properties)
			{
				if(propertyInfo.CanWrite)
				{
					if(propertyInfo.Name == "Guid" && this.GetPropertyValue(propertyInfo.Name) == string.Empty)
					{
						value = System.Guid.NewGuid();
					}
					else
					{
						switch(Type.GetTypeCode(propertyInfo.PropertyType))
						{
							case TypeCode.String:
								value = string.Empty;
								break;
							case TypeCode.Int32:
								value = 0;
								break;
							case TypeCode.Int16:
								value = 0;
								break;
							case TypeCode.Int64:
								value = 0;
								break;
							case TypeCode.Single:
								value = 0;
								break;
							case TypeCode.Double:
								value = 0;
								break;
							case TypeCode.Boolean:
								value = false;
								break;
							case TypeCode.DateTime:
								value = System.DateTime.Now;
								break;
							default:
								value = null;
								break;
						}
					}

					this.SetPropertyValue(propertyInfo.Name, value);
				}
			}

		}

		public void InvokeChange(string propertyName = null)
		{
			if(this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public virtual void Clear()
		{
			_propertyBag.Clear();
		}

		#endregion

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region IDisposable Members

		private bool _disposedValue;

		protected virtual void Dispose(bool isDisposing)
		{

			if(!_disposedValue)
			{
				if(isDisposing)
				{
					//TODO: dispose managed state (managed objects).
				}
			}

			// TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
			// TODO: set large fields to null.

			_disposedValue = true;

		}

		public void Dispose()
		{

			this.Dispose(true);
			GC.SuppressFinalize(this);

		}

		#endregion
	}
}
