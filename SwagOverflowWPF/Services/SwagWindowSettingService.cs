using Newtonsoft.Json;
using SwagOverflowWPF.Controls;
using SwagOverflowWPF.Data;
using SwagOverflowWPF.Iterator;
using SwagOverflowWPF.Repository;
using SwagOverflowWPF.ViewModels;
using System;
using System.Linq;

namespace SwagOverflowWPF.Services
{
    public class SwagWindowSettingService
    {
        private readonly SwagContext _context;

        public SwagWindowSettingService(SwagContext context) => this._context = context;

        public SwagWindowSettingGroup GetWindowSettingGroupByName(String groupName)
        {
            SwagWindowSettingGroup windowSettings = null;
            SwagSettingUnitOfWork work = new SwagSettingUnitOfWork(_context);
            
            windowSettings = work.WindowSettingGroups.Get(sg => sg.Name == groupName, null, "Root").FirstOrDefault();
            if (windowSettings != null)
            {
                #region Load SwagSettingUnitOfWork
                work.Settings.RecursiveLoadChildren(windowSettings.IndexedRootGeneric);
                SwagItemPreOrderIterator<SwagItemViewModel> iterator = windowSettings.CreateIterator();
                for (SwagItemViewModel swagItem = iterator.First(); !iterator.IsDone; swagItem = iterator.Next())
                {
                    SwagSettingViewModel swagItemOriginal = (SwagSettingViewModel)swagItem;
                    if (swagItemOriginal.IconString != null)
                    {
                        Type iconType = JsonConvert.DeserializeObject<Type>(swagItemOriginal.IconTypeString);
                        swagItemOriginal.Icon = (Enum)Enum.Parse(iconType, swagItemOriginal.IconString);
                    }

                    if (!String.IsNullOrEmpty(swagItemOriginal.ValueTypeString))
                    {
                        Type typeGenericTemplate = typeof(SwagSettingViewModel<>);
                        Type valueType = JsonConvert.DeserializeObject<Type>(swagItemOriginal.ValueTypeString);
                        Type[] typeArgs = { valueType };
                        Type typeGeneric = typeGenericTemplate.MakeGenericType(typeArgs);
                        windowSettings.Descendants.Remove(swagItemOriginal);
                        work.Settings.Delete(swagItemOriginal);

                        SwagSettingViewModel newSetting = (SwagSettingViewModel)Activator.CreateInstance(typeGeneric, swagItemOriginal);
                        newSetting.Children = swagItemOriginal.Children;

                        if (newSetting.ItemsSource != null)
                        {
                            Type itemsSourceType = JsonConvert.DeserializeObject<Type>(newSetting.ItemsSourceTypeString);
                            newSetting.ItemsSource = JsonConvert.DeserializeObject(newSetting.ItemsSource.ToString(), itemsSourceType);
                        }

                        swagItemOriginal.Parent.Children.Remove(swagItemOriginal);
                        swagItemOriginal.Parent.Children.Add(newSetting);

                        if (valueType == typeof(Boolean))
                        {
                            newSetting.Value = Boolean.Parse(swagItemOriginal.Value.ToString());
                        }
                        else if (valueType == typeof(String))
                        {
                            newSetting.Value = swagItemOriginal.Value.ToString();
                        }
                        else
                        {
                            newSetting.Value = JsonConvert.DeserializeObject(swagItemOriginal.Value.ToString(), valueType);
                        }

                        work.Settings.Insert(newSetting);
                    }
                }
                #endregion Load SwagSettingUnitOfWork
            }

            if (windowSettings == null)
            {
                #region Create SwagWindowSettingGroup
                windowSettings = new SwagWindowSettingGroup();
                windowSettings.Name = windowSettings.AlternateId = groupName;
                work.SettingGroups.Insert(windowSettings);
                SwagItemPreOrderIterator<SwagItemViewModel> iterator = windowSettings.CreateIterator();
                for (SwagItemViewModel swagItem = iterator.First(); !iterator.IsDone; swagItem = iterator.Next())
                {
                    SwagSettingViewModel setting = (SwagSettingViewModel)swagItem;
                    //Mark these properites as modified to have them save properly
                    setting.Value = setting.Value;
                    setting.ValueTypeString = setting.ValueTypeString;
                    setting.ItemsSource = setting.ItemsSource;
                    setting.ItemsSourceTypeString = setting.ItemsSourceTypeString;
                    setting.IconString = setting.IconString;
                    setting.IconTypeString = setting.IconTypeString;
                }

                work.Complete();
                #endregion Create SwagWindowSettingGroup
            }

            windowSettings.SetContext(_context);
            return windowSettings;
        }
    }
}
