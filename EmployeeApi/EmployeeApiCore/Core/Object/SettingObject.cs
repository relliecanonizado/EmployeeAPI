using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using EmployeeApiCore.Core.BaseClass;
using EmployeeApiCore.Core.Object.DataObject;

namespace EmployeeApiCore.Core.Object
{
    namespace DataObject
    {
        public class SettingObject : BaseModel
        {

            #region Fields

            [XmlElement(ElementName = "SettingName")]
            public string SettingName
            {
                get { return this.GetPropertyValue(MethodBase.GetCurrentMethod().Name); }
                set { this.SetPropertyValue(MethodBase.GetCurrentMethod().Name, value); }
            }

            [XmlElement(ElementName = "SettingValue")]
            public string SettingValue
            {
                get { return this.GetPropertyValue(MethodBase.GetCurrentMethod().Name); }
                set { this.SetPropertyValue(MethodBase.GetCurrentMethod().Name, value); }
            }

            [XmlElement(ElementName = "DefaultValue")]
            public string DefaultValue
            {
                get { return this.GetPropertyValue(MethodBase.GetCurrentMethod().Name); }
                set { this.SetPropertyValue(MethodBase.GetCurrentMethod().Name, value); }
            }

            [XmlElement(ElementName = "Description")]
            public string Description
            {
                get { return this.GetPropertyValue(MethodBase.GetCurrentMethod().Name); }
                set { this.SetPropertyValue(MethodBase.GetCurrentMethod().Name, value); }
            }

            #endregion

            #region PropertySpecified

            [XmlIgnore]
            public bool SettingNameSpecified
            {
                get { return this.SettingName != null && this.SettingName != string.Empty; }
            }

            #endregion

            public override KeyValuePair<string, string> PrimaryKey
            {
                get { return new KeyValuePair<string, string>("Id", this.Id); }
            }
        }
    }

    namespace Collection
    {
        public class SettingObjectCollection : BaseModelCollection<DataObject.SettingObject>
        {
            public SettingObject GetSettingByName(string settingName)
            {
                SettingObject setting = null;

                foreach (var item in this)
                {
                    if (item.SettingName.Trim().Equals(settingName))
                    {
                        return item;
                    }
                }

                return setting;
            }

            public string GetSettingValue(string settingName)
            {
                string value = null;

                foreach (var item in this)
                {
                    if (item.SettingName.Trim().Equals(settingName))
                    {
                        return item.SettingValue;
                    }
                }

                return value;
            }

            public string GetSettingValue(string settingName, string defaultValue)
            {
                foreach (var item in this)
                {
                    if (item.SettingName.Trim().Equals(settingName))
                    {
                        return item.SettingValue;
                    }
                }

                return defaultValue;
            }

            public int GetSettingValueInt(string settingName)
            {
                int value = 0;

                foreach (var item in this)
                {
                    if (item.SettingName.Trim().Equals(settingName))
                    {
                        try
                        {
                            value = Int32.Parse(item.SettingValue);
                        }
                        catch (Exception e)
                        {
                            value = 0;
                        }
                        return value;
                    }
                }

                return value;
            }

            public int GetSettingValueInt(string settingName, int defaultValue)
            {
                int value = 0;

                foreach (var item in this)
                {
                    if (item.SettingName.Trim().Equals(settingName))
                    {
                        try
                        {
                            value = Int32.Parse(item.SettingValue);
                        }
                        catch (Exception e)
                        {
                            value = 0;
                        }
                        return value;
                    }
                }

                return defaultValue;
            }
        }
    }
}

