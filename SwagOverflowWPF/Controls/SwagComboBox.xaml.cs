using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Xml;

namespace SwagOverflowWPF.Controls
{
    /// <summary>
    /// Interaction logic for SwagComboBox.xaml
    /// </summary>
    public partial class SwagComboBox : UserControl, INotifyPropertyChanged
    {
        
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void SetValue<T>(ref T backingField, T value, [CallerMemberName] string propertyname = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
                return;

            backingField = value;
            OnPropertyChanged(propertyname);
        }
        #endregion INotifyPropertyChanged

        #region ValueChanged
        public static readonly RoutedEvent ValueChangedEvent =
                    EventManager.RegisterRoutedEvent(
                    "ValueChanged",
                    RoutingStrategy.Bubble,
                    typeof(RoutedEventHandler),
                    typeof(SwagComboBox));

        public event RoutedEventHandler ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }
        #endregion ValueChanged

        #region FilterEventTimeDelay
        private DispatcherTimer filterEventDelayTimer;

        public static DependencyProperty FilterEventTimeDelayProperty =
                    DependencyProperty.Register(
                    "FilterEventTimeDelay",
                    typeof(Duration),
                    typeof(SwagComboBox),
                    new FrameworkPropertyMetadata(
                        new Duration(new TimeSpan(0, 0, 0, 0, 300))));

        public Duration FilterEventTimeDelay
        {
            get { return (Duration)GetValue(FilterEventTimeDelayProperty); }
            set { SetValue(FilterEventTimeDelayProperty, value); }
        }
        #endregion FilterEventTimeDelay

        #region Text
        public static DependencyProperty TextProperty =
                DependencyProperty.Register(
                    "Text",
                    typeof(String),
                    typeof(SwagComboBox),
                    new UIPropertyMetadata("", TextPropertyChanged));

        public String Text
        {
            get { return (String)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private static void TextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SwagComboBox scbx = d as SwagComboBox;

            if (e.OldValue.ToString() != "" && e.NewValue.ToString() == "")
            {
                scbx.Value = null;
                RoutedEventArgs args = new RoutedEventArgs(ValueChangedEvent);
                scbx.RaiseEvent(args);
                scbx.IsOpen = false;
            }

            scbx.InvokeFilter();
        }
        #endregion Text

        #region Value
        public static DependencyProperty ValueProperty =
                    DependencyProperty.Register(
                       "Value",
                       typeof(Object),
                       typeof(SwagComboBox));

        public Object Value
        {
            get { return (Object)GetValue(ValueProperty); }
            set 
            { 
                SetValue(ValueProperty, value);
                OnPropertyChanged();
            }
        }
        #endregion Value

        #region IsOpen
        public static DependencyProperty IsOpenProperty =
                    DependencyProperty.Register(
                       "IsOpen",
                       typeof(Boolean),
                       typeof(SwagComboBox));

        public Boolean IsOpen
        {
            get { return (Boolean)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }
        #endregion IsOpen

        #region DisplayMemberProperty
        public static DependencyProperty DisplayMemberPropertyProperty =
                DependencyProperty.Register(
                    "DisplayMemberProperty",
                    typeof(String),
                    typeof(SwagComboBox),
                    new UIPropertyMetadata("", DisplayMemberPropertyChanged));

        public String DisplayMemberProperty
        {
            get { return (String)GetValue(DisplayMemberPropertyProperty); }
            set { SetValue(DisplayMemberPropertyProperty, value); }
        }

        private static void DisplayMemberPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //https://stackoverflow.com/questions/5471405/create-datatemplate-in-code-behind
            SwagComboBox scbx = d as SwagComboBox;

            StringReader stringReader = new StringReader(
                @"<DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
                    <TextBlock Text=""{ Binding FullDescription }"" />
                  </DataTemplate>");
            XmlReader xmlReader = XmlReader.Create(stringReader);
            DataTemplate myTemplate = XamlReader.Load(xmlReader) as DataTemplate;

            scbx.ItemTemplate = myTemplate;
        }
        #endregion DisplayMemberProperty

        #region ValueMemberProperty
        public static DependencyProperty ValueMemberPropertyProperty =
                DependencyProperty.Register(
                    "ValueMemberProperty",
                    typeof(String),
                    typeof(SwagComboBox));

        public String ValueMemberProperty
        {
            get { return (String)GetValue(ValueMemberPropertyProperty); }
            set { SetValue(ValueMemberPropertyProperty, value); }
        }
        #endregion ValueMemberProperty

        #region ItemsSource
        public static DependencyProperty ItemsSourceProperty =
                DependencyProperty.Register(
                    "ItemsSource",
                    typeof(IEnumerable),
                    typeof(SwagComboBox));

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set
            {
                SetValue(ItemsSourceProperty, value);
                OnPropertyChanged();
            }
        }
        #endregion ItemsSource

        #region SelectionMode
        public static DependencyProperty SelectionModeProperty =
               DependencyProperty.Register(
                   "SelectionMode",
                   typeof(SelectionMode),
                   typeof(SwagComboBox),
                   new PropertyMetadata(SelectionMode.Single));

        public SelectionMode SelectionMode
        {
            get { return (SelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }
        #endregion SelectionMode

        #region ItemTemplate
        public static DependencyProperty ItemTemplateProperty =
                DependencyProperty.Register(
                    "ItemTemplate",
                    typeof(DataTemplate),
                    typeof(SwagComboBox));

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }
        #endregion ItemTemplate

        #region ItemContainerStyle
        public static DependencyProperty ItemContainerStyleProperty =
                DependencyProperty.Register(
                    "ItemContainerStyle",
                    typeof(Style),
                    typeof(SwagComboBox));

        public Style ItemContainerStyle
        {
            get { return (Style)GetValue(ItemContainerStyleProperty); }
            set { SetValue(ItemContainerStyleProperty, value); }
        }
        #endregion ItemContainerStyle

        public SwagComboBox()
        {
            InitializeComponent();

            filterEventDelayTimer = new DispatcherTimer();
            filterEventDelayTimer.Interval = FilterEventTimeDelay.TimeSpan;
            filterEventDelayTimer.Tick += new EventHandler(OnFilterEventDelayTimerTick);
        }

        void OnFilterEventDelayTimerTick(object o, EventArgs e)
        {
            filterEventDelayTimer.Stop();
            FilterList();
        }

        public void InvokeFilter()
        {
            filterEventDelayTimer.Stop();
            filterEventDelayTimer.Start();
        }

        private void FilterList()
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(ItemsSource);
            if (view is BindingListCollectionView)      //Assuming you are DataView for now
            {
                BindingListCollectionView bindingView = (BindingListCollectionView)view;
                //https://stackoverflow.com/questions/9385489/why-errors-when-filters-datatable-with-collectionview
                bindingView.CustomFilter = $"[{DisplayMemberProperty}] LIKE '%{Text}%'";
            }
        }

        private void ControlInstance_Loaded(object sender, RoutedEventArgs e)
        {
            //https://stackoverflow.com/questions/1600218/how-can-i-move-a-wpf-popup-when-its-anchor-element-moves
            Window w = Window.GetWindow(txtInput);
            // w should not be Null now!
            if (null != w)
            {
                w.LocationChanged += delegate (object sender2, EventArgs args)
                {
                    var offset = popList.HorizontalOffset;
                    // "bump" the offset to cause the popup to reposition itself
                    //   on its own
                    popList.HorizontalOffset = offset + 1;
                    popList.HorizontalOffset = offset;
                };

                // Also handle the window being resized (so the popup's position stays
                //  relative to its target element if the target element moves upon 
                //  window resize)
                w.SizeChanged += delegate (object sender3, SizeChangedEventArgs e2)
                {
                    var offset = popList.HorizontalOffset;
                    popList.HorizontalOffset = offset + 1;
                    popList.HorizontalOffset = offset;
                };
            }
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Select((ListViewItem)sender);
        }

        private void ControlInstance_KeyUp(object sender, KeyEventArgs e)
        {
            if (Keyboard.FocusedElement is TextBox)
            {
                if (!IsOpen)
                {
                    IsOpen = true;
                }
            }

            if (e.Key == Key.Down && Keyboard.FocusedElement is TextBox)
            {
                ListViewItem item = lvItems.ItemContainerGenerator.ContainerFromIndex(0) as ListViewItem;
                item.IsSelected = true;
                lvItems.SelectedItem = item;
                Keyboard.Focus(item);
                lvItems.ScrollIntoView(item);
            }

            if (e.Key == Key.Enter && Keyboard.FocusedElement is ListViewItem)
            {
                Select((ListViewItem)Keyboard.FocusedElement);
            }

            if (e.Key == Key.Escape && Keyboard.FocusedElement is ListViewItem)
            {
                Keyboard.Focus(txtInput);
            }
        }

        private void Select(ListViewItem lvi)
        {
            var context = lvi.DataContext;
            if (DisplayMemberProperty != null && DisplayMemberProperty != "")
            {
                switch (context)
                {
                    case DataRowView drv:
                        this.Text = drv[DisplayMemberProperty].ToString();
                        break;
                }
            }
            else
            {
                this.Text = context.ToString();
            }

            if (ValueMemberProperty != null && ValueMemberProperty != "")
            {
                switch (context)
                {
                    case DataRowView drv:
                        this.Value = drv[ValueMemberProperty].ToString();
                        break;
                }
            }
            else
            {
                this.Value = context;
            }

            RoutedEventArgs args = new RoutedEventArgs(ValueChangedEvent);
            RaiseEvent(args);
            this.IsOpen = false;
        }
    }
}
