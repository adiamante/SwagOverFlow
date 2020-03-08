using Newtonsoft.Json;
using SwagOverflowWPF.Controls;
using SwagOverflowWPF.Data;
using SwagOverflowWPF.Iterator;
using SwagOverflowWPF.Repository;
using SwagOverflowWPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SwagOverflowWPF.Controllers
{
    public class SwagWindowSettingController
    {
        private readonly SwagContext context;

        public SwagWindowSettingController(SwagContext context) => this.context = context;

        public SwagWindowSettingGroup GetWindowSettingGroupByName(String groupName)
        {
            SwagWindowSettingGroup windowSettings = null;
            using (SwagSettingUnitOfWork work = new SwagSettingUnitOfWork(new SwagContext()))
            {
                windowSettings = work.WindowSettingGroups.Get(sg => sg.Name == groupName, null, "Root").FirstOrDefault();
                if (windowSettings != null)
                {
                    #region Load SwagSettingUnitOfWork
                    work.Settings.RecursiveLoadChildren(windowSettings.IndexedRootGeneric);
                    SwagItemPreOrderIterator<SwagItemViewModel> iterator = windowSettings.CreateIterator();
                    for (SwagItemViewModel swagItem = iterator.First(); !iterator.IsDone; swagItem = iterator.Next())
                    {
                        if (!String.IsNullOrEmpty(swagItem.ValueTypeString))
                        {
                            Type typeGenericTemplate = typeof(SwagSettingViewModel<>);
                            Type valueType = JsonConvert.DeserializeObject<Type>(swagItem.ValueTypeString);
                            Type[] typeArgs = { valueType };
                            Type typeGeneric = typeGenericTemplate.MakeGenericType(typeArgs);
                            windowSettings.Descendants.Remove(swagItem);
                            work.Settings.Delete((SwagSettingViewModel)swagItem);

                            SwagSettingViewModel newSetting = (SwagSettingViewModel)Activator.CreateInstance(typeGeneric, (SwagSettingViewModel)swagItem);
                            newSetting.Children = swagItem.Children;

                            if (newSetting.ItemsSource != null)
                            {
                                Type itemsSourceType = JsonConvert.DeserializeObject<Type>(newSetting.ItemsSourceTypeString);
                                newSetting.ItemsSource = JsonConvert.DeserializeObject(newSetting.ItemsSource.ToString(), itemsSourceType);
                            }

                            swagItem.Parent.Children.Remove(swagItem);
                            swagItem.Parent.Children.Add(newSetting);

                            if (valueType == typeof(Boolean))
                            {
                                newSetting.Value = Boolean.Parse(swagItem.Value.ToString());
                            }
                            else if (valueType == typeof(String))
                            {
                                newSetting.Value = swagItem.Value.ToString();
                            }
                            else
                            {
                                newSetting.Value = JsonConvert.DeserializeObject(swagItem.Value.ToString(), valueType);
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
                    }

                    work.Complete();
                    #endregion Create SwagWindowSettingGroup
                }
            }

            return windowSettings;
        }
    }
}
