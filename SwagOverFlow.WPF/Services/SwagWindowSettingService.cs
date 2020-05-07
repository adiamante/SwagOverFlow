using SwagOverflow.Iterator;
using SwagOverFlow.Logger;
using SwagOverflow.WPF.Data;
using SwagOverflow.WPF.Repository;
using SwagOverflow.WPF.ViewModels;
using System;
using System.Linq;
using SwagOverflow.ViewModels;

namespace SwagOverflow.WPF.Services
{
    public class SwagWindowSettingService
    {
        private readonly SwagContext _context;

        public SwagWindowSettingService(SwagContext context) => this._context = context;

        public SwagWindowSettingGroup GetWindowSettingGroupByName(String groupName)
        {
            SwagLogger.LogStart(this, "{Service} {Action}", "WindowSettingService", "GetWindowSettingGroupByName");
            SwagWindowSettingGroup windowSettings = null;
            SwagSettingUnitOfWork work = new SwagSettingUnitOfWork(_context);

            windowSettings = work.WindowSettingGroups.Get(sg => sg.Name == groupName, null).FirstOrDefault();
            if (windowSettings != null)
            {
                #region Load SwagSettingUnitOfWork
                work.SettingGroups.RecursiveLoadChildren(windowSettings);

                #region OLD - Dynamically creates generic type (handled in Properties of SwagItemViewModel/SwagSettingView instead) 
                //SwagItemPreOrderIterator<SwagItemViewModel> iterator = windowSettings.CreateIterator();
                //for (SwagItemViewModel swagItem = iterator.First(); !iterator.IsDone; swagItem = iterator.Next())
                //{
                //    SwagSettingViewModel swagSetting = (SwagSettingViewModel)swagItem;
                //    if (swagSetting.IconString != null)
                //    {
                //        Type iconType = JsonConvert.DeserializeObject<Type>(swagSetting.IconTypeString);
                //        swagSetting.Icon = (Enum)Enum.Parse(iconType, swagSetting.IconString);
                //    }

                //    if (swagSetting.ItemsSource != null)
                //    {
                //        Type itemsSourceType = JsonConvert.DeserializeObject<Type>(swagSetting.ItemsSourceTypeString);
                //        swagSetting.ItemsSource = JsonConvert.DeserializeObject(swagSetting.ItemsSource.ToString(), itemsSourceType);
                //    }

                //    if (!String.IsNullOrEmpty(swagSetting.ValueTypeString))
                //    {
                //        Type typeGenericTemplate = typeof(SwagSettingViewModel<>);
                //        Type valueType = JsonConvert.DeserializeObject<Type>(swagSetting.ValueTypeString);
                //        Type[] typeArgs = { valueType };
                //        Type typeGeneric = typeGenericTemplate.MakeGenericType(typeArgs);
                //        windowSettings.Descendants.Remove(swagSetting);
                //        work.Settings.Delete(swagSetting);

                //        SwagSettingViewModel newSetting = (SwagSettingViewModel)Activator.CreateInstance(typeGeneric, swagSetting);
                //        newSetting.Children = swagSetting.Children;

                //        if (newSetting.ItemsSource != null)
                //        {
                //            Type itemsSourceType = JsonConvert.DeserializeObject<Type>(newSetting.ItemsSourceTypeString);
                //            newSetting.ItemsSource = JsonConvert.DeserializeObject(newSetting.ItemsSource.ToString(), itemsSourceType);
                //        }

                //        swagSetting.Parent.Children.Remove(swagSetting);
                //        swagSetting.Parent.Children.Add(newSetting);

                //        if (valueType == typeof(Boolean))
                //        {
                //            newSetting.Value = Boolean.Parse(swagSetting.Value.ToString());
                //        }
                //        else if (valueType == typeof(String) && swagSetting.Value != null)
                //        {
                //            newSetting.Value = swagSetting.Value.ToString();
                //        }
                //        else if (swagSetting.Value != null)
                //        {
                //            newSetting.Value = JsonConvert.DeserializeObject(swagSetting.Value.ToString(), valueType);
                //        }

                //        work.Settings.Insert(newSetting);
                //    }
                //}
                #endregion OLD
                #endregion Load SwagSettingUnitOfWork
                SwagLogger.Log("{Service} {Action}", "WindowSettingService", "Loaded from database");
            }

            if (windowSettings == null)
            {
                #region Create SwagWindowSettingGroup
                windowSettings = new SwagWindowSettingGroup(true);
                windowSettings.Name = windowSettings.AlternateId = groupName;
                work.SettingGroups.Insert(windowSettings);
                SwagItemPreOrderIterator<SwagSetting> iterator = windowSettings.CreateIterator();
                for (SwagSetting setting = iterator.First(); !iterator.IsDone; setting = iterator.Next())
                {
                    //Mark these properites as modified to have them save properly
                    setting.ObjValue = setting.ObjValue;
                    setting.ValueTypeString = setting.ValueTypeString;
                    setting.ObjItemsSource = setting.ObjItemsSource;
                    setting.ItemsSourceTypeString = setting.ItemsSourceTypeString;
                    setting.IconString = setting.IconString;
                    setting.IconTypeString = setting.IconTypeString;
                }

                work.Complete();
                SwagLogger.Log("{Service} {Action}", "WindowSettingService", "Created then saved to database");
                #endregion Create SwagWindowSettingGroup
            }

            windowSettings.SetContext(_context);
            SwagLogger.LogEnd(this, "{Service} {Action}", "WindowSettingService", "GetWindowSettingGroupByName");
            return windowSettings;
        }
    }
}
