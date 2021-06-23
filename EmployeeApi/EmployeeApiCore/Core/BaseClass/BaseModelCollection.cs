using System;
using System.ComponentModel;
using System.Reflection;
using EmployeeApiCore.Core.Class;
using Newtonsoft.Json;

namespace EmployeeApiCore.Core.BaseClass
{	
	public class BaseModelCollection<T> : ThreadSafeObservableCollection<T>, Interface.IModelCollection<T> where T : BaseClass.BaseModel
	{
		#region Delegates

		public delegate void ItemPropertyChangedEventHandler(dynamic sender, PropertyChangedEventArgs e);

		#endregion

		#region Events

		public event ItemPropertyChangedEventHandler ItemPropertyChanged;

		#endregion

		#region Constructor

		
		public BaseModelCollection()
		{
			this.Guid = System.Guid.NewGuid();
		}

		#endregion

		#region Properties

		public T this[string primaryKeyValue]
		{

			get
			{
				if(!string.IsNullOrEmpty(primaryKeyValue))
				{
					foreach(var item in this.Items)
					{
						if(string.Equals(item.PrimaryKey.Value, primaryKeyValue, StringComparison.OrdinalIgnoreCase))
						{
							return item;
						}
					}
				}

				return null;
			}

		}

		#endregion

		#region Function

		public ThreadSafeObservableCollection<T> GetItems(int startIndex, int limit)
		{

			var result = new ThreadSafeObservableCollection<T>();

			for(int i = startIndex ; i <= (startIndex + limit) - 1 ; i++)
			{
				if(this.Items.Count > i)
				{
					result.Add(this[i]);					
				}
			}

			return result;

		}

		public bool Contains(string primaryKeyValue)
		{
			return (this[primaryKeyValue] != null);
		}
		
		public bool Add(T item, bool checkExists)
		{

			if(checkExists)
			{
				if(!base.Contains(item))
				{
					this.Add(item);
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				this.Add(item);
				return true;
			}

		}

		#endregion

		#region Procedure

		private void OnPropertyChanged(dynamic sender, PropertyChangedEventArgs e)
		{

			if(this.ItemPropertyChanged != null)
			{
				this.ItemPropertyChanged(sender, e);
			}

		}

		public new void Add(T item)
		{
			base.Add(item);
		}

		public void Remove(string primaryKeyValue)
		{

			foreach(var item in this.Items)
			{
				if(item.PrimaryKey.Value == primaryKeyValue)
				{
					base.Remove(item);
				}
			}

		}

		#endregion

		#region ICollectionModel<T> Members

		public System.Guid Guid { get; set; }

		public dynamic Clone()
		{
			return (object)this.DeepCopy();
		}

		public bool Exists(T value, string propertyNames, bool skipSelf)
		{

			PropertyInfo propertyInfo;
			Type type = typeof(T);
			dynamic valueA;
			dynamic valueB;
			string[] propertyName = propertyNames.Split(',');
			bool result = true;
			bool currentResult = true;

			if(this.Count < 1)
			{
				return false;
			}

			for(int i = 0 ; i <= (propertyName.Length - 1) ; i++)
			{
				propertyInfo = type.GetProperty(propertyName[i].Trim());

				if(propertyInfo != null)
				{
					valueA = propertyInfo.GetValue(value, null);

					foreach(T item in this)
					{
						valueB = propertyInfo.GetValue(item, null);

						if(!skipSelf)
						{
							if(valueB is string)
							{
								if(valueB.ToString().Equals(valueA.ToString(), System.StringComparison.OrdinalIgnoreCase))
								{
									currentResult = true;
									break;
								}
							}
							else
							{
								if(valueB.Equals(valueA))
								{
									currentResult = true;
									break;
								}
							}
						}
						else
						{
							if(item.PrimaryKey.Value != value.PrimaryKey.Value)
							{
								if(valueB is string)
								{
									if(valueB.ToString() == valueA.ToString())
									{
										currentResult = true;
										break;
									}
								}
								else
								{
									if(valueB.Equals(valueA))
									{
										currentResult = true;
										break;
									}
								}
							}
						}

						currentResult = false;

					}
					result = result && currentResult;

				}
				else
				{
					result = false;
				}
			}

			return result;

		}

		#endregion

	}
}
