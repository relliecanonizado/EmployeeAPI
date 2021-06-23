using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using System.Windows.Threading;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace EmployeeApiCore.Core.Class
{
	[JsonArray()]
	public class ThreadSafeObservableCollection<T> : IList<T>, INotifyCollectionChanged
	{
		#region Variables

		[XmlIgnore]
		[JsonIgnore]
		private IList<T> _items;

		[XmlIgnore]
		[JsonIgnore]
		private ReaderWriterLock _sync;

		[XmlIgnore]
		[JsonIgnore]
		private Dispatcher _dispathcher;

		#endregion

		#region Constructor

		public ThreadSafeObservableCollection()
		{

			_sync = new ReaderWriterLock();
			_dispathcher = Dispatcher.CurrentDispatcher;
			_items = new List<T>();

		}

		#endregion

		#region Properties

		[XmlIgnore]
		[JsonIgnore]
		public IList<T> Items
		{
			get { return _items; }
		}

		#endregion

		#region Function

		private bool DoRemove(T item)
		{

			int index;
			bool result;

			_sync.AcquireWriterLock(Timeout.Infinite);

			index = _items.IndexOf(item);

			if(index == -1)
			{
				_sync.ReleaseWriterLock();
				return false;
			}

			result = _items.Remove(item);

			if(result)
			{
				if(this.CollectionChanged != null)
				{
					this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
				}
			}

			_sync.ReleaseWriterLock();

			return result;

		}

		#endregion

		#region Procedure 

		private void DoAdd(T item)
		{

			_sync.AcquireWriterLock(Timeout.Infinite);
			_items.Add(item);

			if(this.CollectionChanged != null)
			{
				this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
			}

			_sync.ReleaseWriterLock();

		}

		private void DoClear()
		{

			_sync.AcquireWriterLock(Timeout.Infinite);
			_items.Clear();

			if(this.CollectionChanged != null)
			{
				this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}
			_sync.ReleaseWriterLock();

		}

		private void DoInsert(int index, T item)
		{

			_sync.AcquireWriterLock(Timeout.Infinite);

			_items.Insert(index, item);

			if(this.CollectionChanged != null)
			{
				this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
			}
			_sync.ReleaseWriterLock();

		}

		private void DoRemoveAt(int index)
		{

			_sync.AcquireWriterLock(Timeout.Infinite);
			{
				if(_items.Count == 0 || _items.Count <= index)
				{
					_sync.ReleaseWriterLock();
					return;
				}

				_items.RemoveAt(index);

				if(this.CollectionChanged != null)
				{
					this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
				}

				_sync.ReleaseWriterLock();
			}

		}

		public void Refresh()
		{

			if(this.CollectionChanged != null)
				this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

		}

		#endregion

		#region IList<T> Members

		public int IndexOf(T item)
		{

			int index;
			_sync.AcquireReaderLock(Timeout.Infinite);
			index = _items.IndexOf(item);
			_sync.ReleaseReaderLock();

			return index;

		}

		public void Insert(int index, T item)
		{

			if(object.ReferenceEquals(Thread.CurrentThread, _dispathcher.Thread))
			{
				this.DoInsert(index, item);
			}
			else
			{
				_dispathcher.BeginInvoke((Action)(() =>
				{
					this.DoInsert(index, item);
				}));
			}

		}

		public void RemoveAt(int index)
		{

			if(object.ReferenceEquals(Thread.CurrentThread, _dispathcher.Thread))
			{
				this.DoRemoveAt(index);
			}
			else
			{
				_dispathcher.BeginInvoke((Action)(() =>
				{
					this.DoRemoveAt(index);
				}));
			}

		}

		[XmlIgnore]
		[JsonIgnore]
		public T this[int index]
		{

			get
			{
				T result;

				_sync.AcquireReaderLock(Timeout.Infinite);
				result = _items[index];
				_sync.ReleaseReaderLock();

				return result;
			}
			set
			{
				_sync.AcquireWriterLock(Timeout.Infinite);

				if(_items.Count == 0 || _items.Count <= index)
				{
					_sync.ReleaseWriterLock();
					return;
				}

				_items[index] = value;
				_sync.ReleaseWriterLock();
			}

		}

		#endregion

		#region ICollection<T> Members

		public void Add(T item)
		{

			if(object.ReferenceEquals(Thread.CurrentThread, _dispathcher.Thread))
			{
				this.DoAdd(item);
			}
			else
			{
				_dispathcher.BeginInvoke((Action)(() =>
				{
					this.DoAdd(item);
				}));
			}

		}

		public void Clear()
		{

			if(object.ReferenceEquals(Thread.CurrentThread, _dispathcher.Thread))
			{
				this.DoClear();
			}
			else
			{
				_dispathcher.BeginInvoke((Action)(() =>
				{
					this.DoClear();
				}));
			}

		}

		public bool Contains(T item)
		{

			bool result;

			_sync.AcquireReaderLock(Timeout.Infinite);
			result = _items.Contains(item);
			_sync.ReleaseReaderLock();

			return result;

		}

		public void CopyTo(T[] array, int arrayIndex)
		{

			_sync.AcquireWriterLock(Timeout.Infinite);

			_items.CopyTo(array, arrayIndex);

			_sync.ReleaseWriterLock();

		}

		[XmlIgnore]
		[JsonIgnore]
		public int Count
		{
			get
			{
				int result;

				_sync.AcquireReaderLock(Timeout.Infinite);
				result = _items.Count;
				_sync.ReleaseReaderLock();

				return result;
			}
		}

		[XmlIgnore]
		[JsonIgnore]
		public bool IsReadOnly
		{
			get { return _items.IsReadOnly; }
		}

		public bool Remove(T item)
		{
			DispatcherOperation dispatch;

			if(object.ReferenceEquals(Thread.CurrentThread, _dispathcher.Thread))
			{
				return this.DoRemove(item);
			}
			else
			{
				dispatch = _dispathcher.BeginInvoke(new Func<T, bool>(this.DoRemove), item);

				if(dispatch == null || dispatch.Result == null)
				{
					return false;
				}

				return (bool)dispatch.Result;

			}
		}

		#endregion

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		#endregion

		#region INotifyCollectionChanged Members

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		#endregion

	}
}
