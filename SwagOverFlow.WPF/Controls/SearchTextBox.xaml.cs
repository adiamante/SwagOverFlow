using MahApps.Metro;
using SwagOverFlow.Utils;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace SwagOverFlow.WPF.Controls
{
    /// <summary>
    /// Interaction logic for SearchTextBox.xaml
    /// </summary>
    public partial class SearchTextBox : SwagControlBase
    {
        #region Members
        DispatcherTimer _searchEventDelayTimer;
        #endregion Members

        #region RoutedEvents
        public static readonly RoutedEvent SearchEvent =
            EventManager.RegisterRoutedEvent(
            "Search",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(SearchTextBox));

        public event RoutedEventHandler Search
        {
            add { AddHandler(SearchEvent, value); }
            remove { RemoveHandler(SearchEvent, value); }
        }
        #endregion RoutedEvents

        #region Properties
        #region Text
        public static DependencyProperty TextProperty =
                    DependencyProperty.Register(
                        "Text",
                        typeof(string),
                        typeof(SearchTextBox),
                        new UIPropertyMetadata("", TextPropertyChanged));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private static void TextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SearchTextBox stb = d as SearchTextBox;
            stb.OnPropertyChanged("Text");
        }
        #endregion Text
        #region LabelText
        public static DependencyProperty LabelTextProperty =
                    DependencyProperty.Register(
                        "LabelText",
                        typeof(string),
                        typeof(SearchTextBox));

        public string LabelText
        {
            get { return (string)GetValue(LabelTextProperty); }
            set { SetValue(LabelTextProperty, value); }
        }
        #endregion LabelText
        #region LabelTextColor
        public static DependencyProperty LabelTextColorProperty =
                    DependencyProperty.Register(
                        "LabelTextColor",
                        typeof(Brush),
                        typeof(SearchTextBox),
                        new PropertyMetadata(Brushes.Gray));

        public Brush LabelTextColor
        {
            get { return (Brush)GetValue(LabelTextColorProperty); }
            set { SetValue(LabelTextColorProperty, value); }
        }
        #endregion LabelTextColor
        #region SearchMode
        public static DependencyProperty SearchModeProperty =
                    DependencyProperty.Register(
                        "SearchMode",
                        typeof(SearchMode),
                        typeof(SearchTextBox),
                        new PropertyMetadata(SearchMode.Instant));

        public SearchMode SearchMode
        {
            get { return (SearchMode)GetValue(SearchModeProperty); }
            set { SetValue(SearchModeProperty, value); }
        }
        #endregion SearchMode
        #region FilterMode
        public static DependencyProperty FilterModeProperty =
                    DependencyProperty.Register(
                        "FilterMode",
                        typeof(FilterMode),
                        typeof(SearchTextBox),
                        new PropertyMetadata(FilterMode.CONTAINS));

        public FilterMode FilterMode
        {
            get { return (FilterMode)GetValue(FilterModeProperty); }
            set { SetValue(FilterModeProperty, value); }
        }
        #endregion FilterMode
        #region HasText
        private static DependencyPropertyKey HasTextPropertyKey =
                    DependencyProperty.RegisterReadOnly(
                        "HasText",
                        typeof(bool),
                        typeof(SearchTextBox),
                        new PropertyMetadata());
        public static DependencyProperty HasTextProperty = HasTextPropertyKey.DependencyProperty;

        public bool HasText
        {
            get { return (bool)GetValue(HasTextProperty); }
            private set { SetValue(HasTextPropertyKey, value); }
        }
        #endregion HasText
        #region IsNonNumericOnly
        public static DependencyProperty IsNonNumericOnlyProperty =
                DependencyProperty.Register(
                    "IsNonNumericOnly",
                    typeof(bool),
                    typeof(SearchTextBox),
                    new PropertyMetadata(false));

        public bool IsNonNumericOnly
        {
            get { return (bool)GetValue(IsNonNumericOnlyProperty); }
            set { SetValue(IsNonNumericOnlyProperty, value); }
        }
        #endregion IsNonNumericOnly
        #region IncludeRegex
        public static DependencyProperty IncludeRegexPropery =
                DependencyProperty.Register(
                    "IncludeRegex",
                    typeof(bool),
                    typeof(SearchTextBox),
                    new PropertyMetadata(true));

        public bool IncludeRegex
        {
            get { return (bool)GetValue(IncludeRegexPropery); }
            set { SetValue(IncludeRegexPropery, value); }
        }
        #endregion IncludeRegex
        #region SearchEventTimeDelay
        public static DependencyProperty SearchEventTimeDelayProperty =
                    DependencyProperty.Register(
                    "SearchEventTimeDelay",
                    typeof(Duration),
                    typeof(SearchTextBox),
                    new FrameworkPropertyMetadata(
                        new Duration(new TimeSpan(0, 0, 0, 0, 500)),
                        new PropertyChangedCallback(OnSearchEventTimeDelayChanged)));

        public Duration SearchEventTimeDelay
        {
            get { return (Duration)GetValue(SearchEventTimeDelayProperty); }
            set { SetValue(SearchEventTimeDelayProperty, value); }
        }

        static void OnSearchEventTimeDelayChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SearchTextBox stb = o as SearchTextBox;
            if (stb != null)
            {
                stb._searchEventDelayTimer.Interval = ((Duration)e.NewValue).TimeSpan;
                stb._searchEventDelayTimer.Stop();
            }
        }
        #endregion SearchEventTimeDelay
        #endregion Properties

        #region Initialization
        public SearchTextBox()
        {
            InitializeComponent();
            _searchEventDelayTimer = new DispatcherTimer();
            _searchEventDelayTimer.Interval = SearchEventTimeDelay.TimeSpan;
            _searchEventDelayTimer.Tick += new EventHandler(OnSeachEventDelayTimerTick);
        }
        #endregion Initialization

        #region Events
        private void FilterModeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem filterMenuItem = e.OriginalSource as MenuItem;
            FilterMode = (FilterMode)filterMenuItem.Tag;
        }

        private void RaiseSearchEvent()
        {
            RoutedEventArgs args = new RoutedEventArgs(SearchEvent);
            RaiseEvent(args);
        }

        void OnSeachEventDelayTimerTick(object o, EventArgs e)
        {
            _searchEventDelayTimer.Stop();
            RaiseSearchEvent();
        }

        private void SearchTextBoxControl_TextChanged(object sender, TextChangedEventArgs e)
        {
            HasText = Text != null && Text.Length != 0;

            if (_searchEventDelayTimer != null && SearchMode == SearchMode.Instant)
            {
                _searchEventDelayTimer.Stop();
                _searchEventDelayTimer.Start();
            }
        }

        private void SearchTextBoxControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && SearchMode == SearchMode.Instant)
            {
                this.Text = "";
            }
            else if ((e.Key == Key.Return || e.Key == Key.Enter) && SearchMode == SearchMode.Delayed)
            {
                RaiseSearchEvent();
            }
            else
            {
                base.OnKeyDown(e);
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            RaiseSearchEvent();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            Text = "";
        }

        #endregion Events
    }

    
}
