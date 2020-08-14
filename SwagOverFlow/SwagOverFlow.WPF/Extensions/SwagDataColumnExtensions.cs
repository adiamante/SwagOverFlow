using SwagOverFlow.Utils;
using SwagOverFlow.ViewModels;
using SwagOverFlow.WPF.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xml;

namespace SwagOverFlow.WPF.Extensions
{
    public static class SwagDataColumnExtensions
    {
        public static void Init(this SwagDataColumn sdc)
        {
            if (!sdc.HasApplyDistinctValuesFilterHandler)
            {
                sdc.ApplyDistinctValuesFilter += (s, e) =>
                {
                    if (sdc.DistinctValues != null)
                    {
                        UIHelper.GetCollectionView(sdc.DistinctValues).Filter = item =>
                        {
                            KeyValuePair<Object, SwagDataCell> kvp = (KeyValuePair<Object, SwagDataCell>)item;

                            if (String.IsNullOrEmpty(sdc.ListValuesFilter))
                            {
                                return kvp.Value.Count > 0 || sdc.ShowAllDistinctValues;
                            }
                            else
                            {
                                return SearchHelper.Evaluate(kvp.Value.Value.ToString(), sdc.ListValuesFilter, false, sdc.ListValuesFilterMode, true)
                                    && (kvp.Value.Count > 0 || sdc.ShowAllDistinctValues);
                            }
                        };
                    }
                };
            }
        }

        public static DataGridColumn DataGridColumn(this SwagDataColumn sdc)
        {
            if (!String.IsNullOrEmpty(sdc.DataTemplate))
            {
                StringReader stringReader = new StringReader(sdc.DataTemplate);
                XmlReader xmlReader = XmlReader.Create(stringReader);
                DataTemplate template = XamlReader.Load(xmlReader) as DataTemplate;
                DataGridTemplateColumn dgtc = new DataGridTemplateColumn();
                PropertyCopy.Copy(sdc, dgtc);
                dgtc.CellTemplate = template;
                return dgtc;
            }

            DataGridTextColumn dgc = new DataGridTextColumn();
            dgc.Binding = new Binding(sdc.Display);
            PropertyCopy.Copy(sdc, dgc);

            return dgc;
        }
    }
}
