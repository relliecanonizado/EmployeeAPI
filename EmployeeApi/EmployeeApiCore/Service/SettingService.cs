using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeApiCore.Core.Class;
using EmployeeApiCore.Core.Object.Collection;
using EmployeeApiCore.Core.Object.DataObject;

namespace EmployeeApiCore.Service
{
    public class SettingService
    {
        #region Variables

        private LoggerService _logger = new LoggerService();

        #endregion

        #region Constructor

        public SettingService()
        {

        }

        #endregion

        #region Methods

        public SettingObjectCollection LoadSettings()
        {
            SettingObjectCollection settings = new SettingObjectCollection();
            string settingPath = Path.Combine(Global.EnvironmentPath, "settings.xml");

            try
            {
                if (File.Exists(settingPath))
                {
                    settings = Serializer.DeserializeFromFile(settingPath, typeof(SettingObjectCollection));
                }
                else
                {
                    // Settings file does not exist. Load defaults
                    settings = LoadDefaultSettings();
                }
                
            }
            catch(Exception e)
            {
                // A problem with deserialization
                settings = LoadDefaultSettings();
            }
            
            return settings;
        }

        public SettingObjectCollection LoadDefaultSettings()
        {
            SettingObjectCollection defaultSettings = new SettingObjectCollection();

            try
            {
                SettingObject EscrowedExpiryMinutes = new SettingObject();
                EscrowedExpiryMinutes.SettingName = "EscrowedExpiryMinutes";
                EscrowedExpiryMinutes.SettingValue = "10";
                EscrowedExpiryMinutes.DefaultValue = "10";
                defaultSettings.Add(EscrowedExpiryMinutes);

                SettingObject WaitingPrintExpiryMinutes = new SettingObject();
                WaitingPrintExpiryMinutes.SettingName = "WaitingPrintExpiryMinutes";
                WaitingPrintExpiryMinutes.SettingValue = "10";
                WaitingPrintExpiryMinutes.DefaultValue = "10";
                defaultSettings.Add(WaitingPrintExpiryMinutes);

                SettingObject QuartzIntervalSeconds = new SettingObject();
                QuartzIntervalSeconds.SettingName = "QuartzIntervalSeconds";
                QuartzIntervalSeconds.SettingValue = "300";
                QuartzIntervalSeconds.DefaultValue = "300";
                defaultSettings.Add(QuartzIntervalSeconds);

                SettingObject TicketExpiryDays = new SettingObject();
                TicketExpiryDays.SettingName = "TicketExpiryDays";
                TicketExpiryDays.SettingValue = "0";
                TicketExpiryDays.DefaultValue = "0";
                defaultSettings.Add(TicketExpiryDays);

                SettingObject ServerUrl = new SettingObject();
                ServerUrl.SettingName = "ServerUrl";
                ServerUrl.SettingValue = "";
                ServerUrl.DefaultValue = "";
                defaultSettings.Add(ServerUrl);
            }
            catch(Exception e)
            {
                _logger.LogApplicationError(e.ToString());
            }

            return defaultSettings;
        }

        public string GetDefaultValue(string settingName)
        {
            string value = string.Empty;

            SettingObjectCollection defaultSettings = this.LoadDefaultSettings();

            foreach (var item in defaultSettings)
            {
                if (item.SettingName.Trim().Equals(settingName))
                {
                    value = item.SettingValue;
                    return value;
                }
            }

            return value;
        }

        public int GetDefaultValueInt(string settingName)
        {
            int value = 0;

            SettingObjectCollection defaultSettings = this.LoadDefaultSettings();

            foreach (var item in defaultSettings)
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

        public SettingObject GetSettingByName(string settingName)
        {
            SettingObject setting = null;
            SettingObjectCollection settings = this.LoadSettings();

            foreach (var item in settings)
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
            SettingObjectCollection settings = this.LoadSettings();

            foreach (var item in settings)
            {
                if (item.SettingName.Trim().Equals(settingName))
                {
                    if (item.SettingValue.Trim().Equals(string.Empty))
                    {
                        value = this.GetDefaultValue(settingName);
                        return value;
                    }
                    else
                    {
                        return item.SettingValue;
                    }
                    
                }
            }

            return value;
        }

        public int GetSettingValueInt(string settingName)
        {
            int value = 0;
            SettingObjectCollection settings = this.LoadSettings();

            foreach (var item in settings)
            {
                if (item.SettingName.Trim().Equals(settingName))
                {
                    if (item.SettingValue.Trim().Equals(string.Empty))
                    {
                        value = this.GetDefaultValueInt(settingName);
                        return value;
                    }
                    else
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
            }

            return value;
        }
        
        #endregion
    }
}
