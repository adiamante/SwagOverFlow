using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using SwagOverflowWPF.Utilities;

namespace SwagOverflowWPF.UI
{
    /// <summary>
    /// Provides a link between a value and a <see cref="DataStyle"/>
    /// for the <see cref="DynamicStyleSelector"/>
    /// </summary>
    /// <remarks>
    /// In this case, our value is a <see cref="System.Type"/> which we are attempting to match
    /// to a <see cref="DataStyle"/>
    /// </remarks>
    public class SwagStyle : DependencyObject
    {
        /// <summary>
        /// Provides the value used to match this <see cref="DataStyle"/> to an item
        /// </summary>
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(Type), typeof(SwagStyle));

        /// <summary>
        /// Provides the value used to match this <see cref="DataStyle"/> to an item
        /// </summary>
        public static readonly DependencyProperty TypePathProperty = DependencyProperty.Register("TypePath", typeof(String), typeof(SwagStyle));

        /// <summary>
        /// Provides the Option used to match this <see cref="DataStyle"/> to an item
        /// </summary>
        public static readonly DependencyProperty CompareValueProperty = DependencyProperty.Register("CompareValue", typeof(object), typeof(SwagStyle));

        /// <summary>
        /// Provides the <see cref="DataStyle"/> used to render items matching the <see cref="Value"/>
        /// </summary>
        public static readonly DependencyProperty DataStyleProperty =
           DependencyProperty.Register("DataStyle", typeof(Style), typeof(SwagStyle));


        /// <summary>
        /// Gets or Sets the value used to match this <see cref="DataStyle"/> to an item
        /// </summary>
        public Type Type
        { get { return (Type)GetValue(TypeProperty); } set { SetValue(TypeProperty, value); } }

        /// <summary>
        /// Gets or Sets the value used to match this <see cref="DataStyle"/> to an item
        /// </summary>
        public String TypePath
        { get { return (String)GetValue(TypePathProperty); } set { SetValue(TypePathProperty, value); } }

        /// <summary>
        /// Gets or Sets the Option used to match this <see cref="DataStyle"/> to an item
        /// </summary>
        public object CompareValue
        { get { return (object)GetValue(CompareValueProperty); } set { SetValue(CompareValueProperty, value); } }

        /// <summary>
        /// Gets or Sets the <see cref="DataStyle"/> used to render items matching the <see cref="Value"/>
        /// </summary>
        public Style Style
        { get { return (Style)GetValue(DataStyleProperty); } set { SetValue(DataStyleProperty, value); } }
    }

    /// <summary>
    /// Holds a collection of <see cref="SwagStyle"/> items
    /// for application as a control's DataStyle.
    /// https://stackoverflow.com/questions/28247883/simple-existing-implementation-of-icollectiont
    /// </summary>
    public class SwagStyleCollection : List<SwagStyle>
    {
    }

    public class SwagStyleSelector : StyleSelector
    {
        public SwagStyle DefaultStyle { get; set; }
        public SwagStyleCollection StaticStyles { get; set; }
        public String ComparePath { get; set; }

        #region CustomStyles
        public static readonly DependencyProperty CustomStylesProperty =
                    DependencyProperty.RegisterAttached("CustomStyles", typeof(IEnumerable), typeof(SwagStyleSelector),
                    new FrameworkPropertyMetadata(new SwagStyleCollection(), FrameworkPropertyMetadataOptions.Inherits));

        public static IEnumerable GetCustomStyles(UIElement element)
        {
            return (IEnumerable)element.GetValue(CustomStylesProperty);
        }

        public static void SetCustomStyles(UIElement element, SwagStyleCollection collection)
        {
            element.SetValue(CustomStylesProperty, collection);
        }
        #endregion CustomStyles

        #region CustomDefaultStyle
        public static readonly DependencyProperty CustomDefaultStyleProperty =
                    DependencyProperty.RegisterAttached("CustomDefaultStyle", typeof(SwagStyle), typeof(SwagStyleSelector));

        public static SwagStyle GetCustomDefaultStyle(UIElement element)
        {
            return (SwagStyle)element.GetValue(CustomDefaultStyleProperty);
        }

        public static void SetCustomDefaultStyle(UIElement element, SwagStyle style)
        {
            element.SetValue(CustomDefaultStyleProperty, style);
        }
        #endregion CustomDefaultStyle

        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (!(container is UIElement))
                return base.SelectStyle(item, container);


            SwagStyleCollection styles = new SwagStyleCollection();
            IEnumerable customSource = GetCustomStyles(container as UIElement);
            ICollectionView customViewSource = CollectionViewSource.GetDefaultView(customSource);

            if (customViewSource != null)
            {
                foreach (SwagStyle customStyle in customViewSource)
                {
                    styles.Add(customStyle);
                }
            }

            if (StaticStyles != null)
            {
                foreach (SwagStyle staticStyle in StaticStyles)
                {
                    styles.Add(staticStyle);
                }
            }

            foreach (SwagStyle style in styles)
            {
                if (!String.IsNullOrEmpty(ComparePath) && style.CompareValue != null)
                {
                    PropertyInfo propInfo = ReflectionHelper.PropertyInfoCollection[item.GetType()][ComparePath];
                    if (propInfo.CanRead)
                    {
                        Object targetValue = propInfo.GetValue(item);
                        if (targetValue.Equals(style.CompareValue))
                        {
                            return style.Style;
                        }
                    }
                }

                if (style.Type != null && !String.IsNullOrEmpty(style.TypePath))
                {
                    PropertyInfo propInfo = ReflectionHelper.PropertyInfoCollection[item.GetType()][style.TypePath];
                    if (propInfo.CanRead)
                    {
                        Object targetType = propInfo.GetValue(item);
                        if (targetType.Equals(style.Type))
                        {
                            return style.Style;
                        }
                    }
                }

                if (style.Type != null && style.Type.IsInstanceOfType(item))
                {
                    return style.Style;
                }
            }

            SwagStyle customDefaultStyle = GetCustomDefaultStyle(container as UIElement);

            if (customDefaultStyle == null &&
                ((container is TreeViewItem) ||
                (container is FrameworkElement && ((FrameworkElement)container).TemplatedParent is TreeViewItem)))
            {
                TreeView treeParent = container.TryFindParent<TreeView>();
                customDefaultStyle = GetCustomDefaultStyle(treeParent);
            }

            if (customDefaultStyle != null)
            {
                return customDefaultStyle.Style;
            }

            if (DefaultStyle != null)
            {
                return DefaultStyle.Style;
            }

            return base.SelectStyle(item, container);
        }
    }
}
