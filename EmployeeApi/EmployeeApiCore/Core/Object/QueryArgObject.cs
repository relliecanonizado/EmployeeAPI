using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using EmployeeApiCore.Core.Class;

namespace EmployeeApiCore.Core.Object
{
	public enum SearchOperator
	{
		AND,
		OR
	}

	public enum SearchCondition
	{
		Equals,
		StartsWith,
		EndsWith,
		Contains,
		NotEqual,
		GreaterThan,
		LessThan,
		GreaterThanOrEqual,
		LesserThanOrEqual
	}

	public class QueryArg
	{

		#region Constructor

		public QueryArg()
		{
			this.Clear();
		}

		#endregion

		#region Properties

		public FilterCollection Filter { get; set; }

		public List<string> Fields { get; set; }
		public List<string> Sort { get; set; }

		public int Offset { get; set; }

		public int Limit { get; set; }
	
		#endregion

		#region Procedures

		public void Clear()
		{
			this.Filter = new FilterCollection();
			this.Fields = new List<string>();
			this.Sort = new List<string>();
			this.Offset = 0;
			this.Limit = 0;
		}

		#endregion

		#region Function

		private static List<string> GetQueryArgsSort(IEnumerable<KeyValuePair<string, string>> keyValues)
		{
			List<string> results = new List<string>();

			foreach(var item in keyValues)
			{
				if(item.Key.ToLower() == "sort")
				{
					results.AddRange(item.Value.ToLower().Split(','));
					continue;
				}
			}

			return results;

		}

		private static List<string> GetQueryArgsFields(IEnumerable<KeyValuePair<string, string>> keyValues)
		{
			List<string> results = new List<string>();

			foreach(var item in keyValues)
			{
				if(item.Key.ToLower() == "fields")
				{
					results.AddRange(item.Value.ToLower().Split(','));
					continue;
				}
			}

			return results;

		}

		private static int GetQueryArgsOffset(IEnumerable<KeyValuePair<string, string>> keyValues)
		{

			foreach(var item in keyValues)
			{
				if(item.Key.ToLower() == "offset")
				{
					if(item.Value.IsNumeric())
					{
						return int.Parse(item.Value);
					}
				}
			}

			return 0;

		}

		private static int GetQueryArgsLimit(IEnumerable<KeyValuePair<string, string>> keyValues)
		{

			foreach(var item in keyValues)
			{
				if(item.Key.ToLower() == "limit")
				{
					if(item.Value.IsNumeric())
					{
						return int.Parse(item.Value);
					}
				}
			}

			return 0;
		}

		private static FilterCollection GetQueryArgsFilter(IEnumerable<KeyValuePair<string, string>> keyValues, Type type)
		{
			FilterCollection filterCollection = new FilterCollection();

			filterCollection.SearchOperator = SearchOperator.AND;

			foreach(var item in keyValues)			
			{
				if(item.Key.ToLower() == "filter")
				{
					var filters = item.Value.ToLower().Split(',');

					foreach(var itemFilter in filters)
					{
						var propertyValues = itemFilter.ToLower().Split('=');

						if(propertyValues.Length == 2)
						{
							string condition = string.Empty;
							var value = propertyValues[1] ?? string.Empty;
							var propertyOperator = propertyValues[0].ToLower().Split(new string[] { "__" }, StringSplitOptions.None);

							var property = propertyOperator[0] ?? string.Empty;

							if(property.ToLower() != "searchoperator")
							{
								if(propertyOperator.Length == 2)
								{
									condition = propertyOperator[1] ?? string.Empty;
									filterCollection.Add(new FilterModel(property, condition.ToSQLFilterFormat(), value));
								}
							}
							else
							{
								if(value.ToLower() == "or")
									filterCollection.SearchOperator = SearchOperator.OR;

							}

						}
					}
				}				
			}

			return filterCollection;

		}

		public static QueryArg SetQueryArgs(IEnumerable<KeyValuePair<string, string>> keyvalues, Type type)
		{
			QueryArg queryArgs = new QueryArg();

			queryArgs.Fields = QueryArg.GetQueryArgsFields(keyvalues);
			queryArgs.Sort = QueryArg.GetQueryArgsSort(keyvalues);
			queryArgs.Offset = QueryArg.GetQueryArgsOffset(keyvalues);
			queryArgs.Limit = QueryArg.GetQueryArgsLimit(keyvalues);
			queryArgs.Filter = QueryArg.GetQueryArgsFilter(keyvalues, type);

			return queryArgs;
		}

		#endregion

	}

	public class FilterCollection : ObservableCollection<FilterModel>
	{
		public SearchOperator SearchOperator { get; set; }

		public void Add(string field, SearchCondition searchCondition, string value)
		{
			this.Add(new FilterModel(field, searchCondition, value));		
		}
	}

	public class FilterModel
	{
		public FilterModel(string field, SearchCondition condition, string value)
		{
			this.Field = field;
			this.Condition = condition;
			this.Value = value;
		}

		public string Field { get; set; }
		public SearchCondition Condition { get; set; }
		public string Value { get; set; }

	}
}
