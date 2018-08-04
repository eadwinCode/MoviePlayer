using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Common.Interfaces;

namespace SearchComponent
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:SearchComponent"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:SearchComponent;assembly=SearchComponent"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:CustomControl1/>
    ///
    /// </summary
    /// 

    public partial class SearchControl<T> : ISearchControl<T>
    {
        private IDataSource<T> datasource;
        public IDataSource<T> DataSource { get { return datasource; }
            set
            {
                datasource = value;
            }
        }

        private void SearchTextBlock_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.TextChangeEvent != null)
                this.TextChangeEvent(sender, e);

            if (CancelBtnVisible == Visibility.Collapsed)
                CancelBtnVisible = Visibility.Visible;

            if (IsSearchButtonEnabled)
                return;

            StartSearch(Text, CustomPropertyName);
        }

        private void StartSearch(string text, string customPropertyName)
        {
            

            if (ArrayToSort == null || SortedArray == null)
                return;

            if (ArrayToSort.Count <= SortedArray.Count)
                ArrayToSort = LastArray;

            switch (SearchPattern)
            {
                case SearchMode.ByProperty:
                    if (customPropertyName == null || customPropertyName == "")
                        throw new ArgumentNullException("CustomPropertyName");

                    SortedArray = SortByProperty(ArrayToSort, text, customPropertyName);
                    break;
                case SearchMode.DeepSearch:
                    SortedArray = SortByProperties(ArrayToSort, text);
                    break;
                case SearchMode.Object:
                    SortedArray = Sort(ArrayToSort, text);
                    break;
                default:
                    SortedArray = Sort(ArrayToSort, text);
                    break;
            }

            LastArray = ArrayToSort;
        }

        private void UpdateAction(string customPropertyName)
        {
            UpdateEvent(SortedArray, customPropertyName);
            Clear();
        }

        public void Clear()
        {
            this.Text = "";
            //if(issearchbuttonenabled && SortedArray.Count > 0)
            //{
            //    SearchAction();
            //}
            this.Focus();
            CancelBtnVisible = Visibility.Collapsed;
        }

        private IList<T> Sort(IList<T> arrayToSort, string text)
        {
            IList<T> result = new List<T>();
            if (arrayToSort is ObservableCollection<T>)
                result = new ObservableCollection<T>();

            if (text == "")
                return arrayToSort;

            if (arrayToSort == null || text == null)
                return result;
            string inputText = text.ToLower();
            string[] inputstring = inputText.Split(delimeter, StringSplitOptions.RemoveEmptyEntries);

            foreach (var item in arrayToSort)
            {
                string indexedString = item.ToString().ToLower();
                if (indexedString.Contains(inputText))
                    result.Add(item);
                else if(DeepMatch(inputstring,indexedString))
                    result.Add(item);
            }

            return result;
        }

        private bool DeepMatch(string[] inputstring, string indexedString)
        {
            int i = 0;
            foreach (var item in inputstring)
            {
                if (indexedString.Contains(item))
                    i++;
            }
            var result = (i * 1.0 / inputstring.Length) * 100;
            if (result >= 50)
                return true;
            return false;
        }

        private IList<T> SortByProperties(IList<T> arrayToSort, string text)
        {
            IList<T> result = new List<T>();
            if (arrayToSort is ObservableCollection<T>)
                result = new ObservableCollection<T>();

            if (text == "")
                return arrayToSort;

            if (arrayToSort == null || text == null)
                return result;

            string inputstring = text.ToLower();
           
            foreach (var item in arrayToSort)
            {
                foreach (var property in TProperties)
                {
                    var propertyval = property.GetValue(item, null);
                    if (propertyval == null) continue;

                    string indexedString = propertyval.ToString().ToLower();
                    if (indexedString.Contains(inputstring))
                        result.Add(item);
                }
            }

            return result;
        }

        private IList<T> SortByProperty(IList<T> arrayToSort, string text, string customPropertyName)
        {
            IList<T> result = new List<T>();
            if (arrayToSort is ObservableCollection<T>)
                result = new ObservableCollection<T>();

            if (text == "")
                return arrayToSort;

            if (arrayToSort == null || text == null)
                return result;

            string inputstring = text.ToLower();

            foreach (var item in arrayToSort)
            {
                PropertyInfo itemprop = item.GetType().GetProperty(customPropertyName);
                var propertyval = itemprop.GetValue(item, null);
                if (propertyval == null) continue;

                string indexedString = propertyval.ToString().ToLower();
                if (indexedString.Contains(inputstring))
                    result.Add(item);
            }

            return result;
        }

        public void UseExtendedSearchTextbox(object searchTextBox)
        {
            this.searchTextBlock = (SearchTextBox)searchTextBox;
            searchTextBlock.TextChanged += SearchTextBlock_TextChanged;
            searchTextBlock.GotKeyboardFocus += SearchTextBlock_GotKeyboardFocus;
        }
    }


    public partial class SearchControl<T> : ContentControl,INotifyPropertyChanged
    {
        string[] delimeter = { " " };
        public event PropertyChangedEventHandler PropertyChanged;
        private SearchTextBox searchTextBlock;
        public delegate void UpdateHandler(ICollection<T> arrayArgs,string searchquery);
        public delegate void OnSearchAsynStart(object sender);
        private Task Task;
        private PropertyInfo[] TProperties;
        private bool issearchbuttonenabled;

        protected IList<T> SortedArray { get; set; }
        protected IList<T> LastArray { get; set; }

        public IList FlexibleATS { get; set; }
        public SearchMode SearchPattern { get; set; }
        public CustomCommand SearchCommand { get; private set; }
        public CustomCommand CancelCommand { get; private set; }

        public bool HasText
        {
            get { return searchTextBlock.HasText; }
        }
        /// <summary>
        /// 
        /// </summary>
        private IList<T> ArrayToSort { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public event TextChangedEventHandler TextChangeEvent;

        public bool IsSearchButtonEnabled
        {
            get { return issearchbuttonenabled; }
            set
            {
                issearchbuttonenabled = value;
                SearchBtnVisible = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public string Text
        {
            get { return searchTextBlock == null ? null : this.searchTextBlock.Text; }
            set
            {
                if (searchTextBlock == null) return;
                this.searchTextBlock.Text = value;
            }
        }

        private Visibility searchbtnVisible = Visibility.Collapsed;

        public Visibility SearchBtnVisible
        {
            get { return searchbtnVisible; }
            set { searchbtnVisible = value; RaisePropertyChanged(() => this.SearchBtnVisible);  }
        }

        private Visibility cancelbtnVisible = Visibility.Collapsed;

        public Visibility CancelBtnVisible
        {
            get { return cancelbtnVisible; }
            private set { cancelbtnVisible = value; RaisePropertyChanged(() => this.CancelBtnVisible); }
        }

        public string CustomPropertyName { get; set; }

        public event UpdateHandler UpdateEvent;
        public event OnSearchAsynStart OnSearchStarted;

        public SearchControl()
        {
            this.Template = (ControlTemplate)Application.Current.FindResource("SearchContentControl");
            InitializeComponent();
            this.DataContext = this;
        }

        private void InitializeComponent()
        {
            SortedArray = new List<T>();
            LastArray = new List<T>();

            CancelCommand = new CustomCommand(CancelAction);
            SearchCommand = new CustomCommand(SearchAction, CanSearch);
            TProperties = typeof(T).GetProperties();
            SearchPattern = SearchMode.Object;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            searchTextBlock = (SearchTextBox)this.Template.FindName("searchtbx", this);
            searchTextBlock.TextChanged += SearchTextBlock_TextChanged;
            searchTextBlock.GotKeyboardFocus += SearchTextBlock_GotKeyboardFocus;
        }

        private void SearchTextBlock_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (CancelBtnVisible == Visibility.Collapsed)
                CancelBtnVisible = Visibility.Visible;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Enter && IsSearchButtonEnabled)
            {
                SearchAction();
            }
        }

       
        private bool CanSearch()
        {
            return this.searchTextBlock == null? false : HasText;
        }

        private void SearchAction()
        {
            Search(Text, CustomPropertyName);
        }

        private void Search(string text, string customPropertyName)
        {
            this.IsEnabled = false;
            if (OnSearchStarted != null)
                OnSearchStarted(this);

            Task = Task.Factory.StartNew(() => BeginSearch(text, customPropertyName)).ContinueWith(t => SearchComplete(), TaskScheduler.FromCurrentSynchronizationContext());
            //Task.Wait();
        }

        private void BeginSearch(string text, string customPropertyName)
        {
            if (DataSource.Data.Count == 0) return;
            ArrayToSort = DataSource.Data;
            StartSearch(text, customPropertyName);
        }

        private void SearchComplete()
        {
            this.IsEnabled = true;

            UpdateAction(Text);
        }
        
        public void QuickSearch(string query)
        {
            this.Text = query;
            if(IsSearchButtonEnabled)
            {
                SearchAction();
                return;
            }
            StartSearch(Text, CustomPropertyName);
        }

        private void CancelAction()
        {
            this.Clear();
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        protected void RaisePropertyChanged<T>(Expression<Func<T>> property)
        {
            this.RaisePropertyChanged((property.Body as MemberExpression).Member.Name);
        }
    }

    public enum SearchMode
    {
        ByProperty,
        DeepSearch,
        Object
    }

    public sealed class CustomCommand : ICommand
    {
        readonly Action _execute;
        readonly Func<bool> _canExecute;

        public CustomCommand(Action execute)
            : this(execute, null)
        {

        }

        public CustomCommand(Action execute,Func<bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute();
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecute != null)
                    CommandManager.RequerySuggested += value;
            }

            remove
            {
                if (_canExecute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }

        public void Execute(object parameter)
        {
            _execute();
        }
    }

    public class SearchTextBox : TextBox
    {
        public bool HasText
        {
            get { return (bool)GetValue(HasTextProperty); }
            set { SetValue(HasTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HasText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HasTextProperty =
            DependencyProperty.Register("HasText", typeof(bool), typeof(SearchTextBox), new PropertyMetadata(false));

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            HasText = this.Text != "";
        }
        static SearchTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SearchTextBox), new FrameworkPropertyMetadata(typeof(SearchTextBox)));
        }

    }
}
