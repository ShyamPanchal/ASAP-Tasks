using AsapTasks.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace AsapTasks.ViewModel
{
    public class ComboBoxViewModel : BaseViewModel
    {
        //private static readonly string[] _emails =
        //        {
        //    "@gmail.com",
        //    "@hotmail.com",
        //    "@me.com",
        //    "@outlook.com",
        //    "@live.com", // does anyone care about this one? haha
        //    "@yahoo.com" // seriously, does anyone use this anymore?
        //};

        public static List<string> _emails = new List<string>();

        #region Bindable Properties

        public static readonly BindableProperty EmailAddressProperty = BindableProperty.Create(nameof(EmailAddress),
            typeof(string),
            typeof(Xfx.XfxComboBox),
            default(string),
            propertyChanged: EmailAddressPropertyChanged);

        public static readonly BindableProperty EmailSuggestionsProperty =
            BindableProperty.Create(nameof(EmailSuggestions),
                typeof(ObservableCollection<string>),
                typeof(Xfx.XfxComboBox),
                new ObservableCollection<string>());

        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem),
            typeof(object),
            typeof(ComboBoxViewModel));

        #endregion

        public ComboBoxViewModel()
        {
        }

        #region Object Properties

        /// <summary>
        ///     Text . This is a bindable property.
        /// </summary>
        public string EmailAddress
        {
            get => (string)GetValue(EmailAddressProperty);
            set => SetValue(EmailAddressProperty, value);
        }

        /// <summary>
        ///     Email Suggestions collection . This is a bindable property.
        /// </summary>
        public ObservableCollection<string> EmailSuggestions
        {
            get => (ObservableCollection<string>)GetValue(EmailSuggestionsProperty);
            set => SetValue(EmailSuggestionsProperty, value);
        }

        ///// <summary>
        /////     Foo summary. This is a bindable property.
        ///// </summary>
        //public string Foo
        //{
        //    get => (string)GetValue(FooProperty);
        //    set
        //    {
        //        SetValue(FooProperty, value);
        //        ValidateProperty();
        //    }
        //}

        /// <summary>
        ///     SelectedItem summary. This is a bindable property.
        /// </summary>
        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set
            {
                SetValue(SelectedItemProperty, value);
                Debug.WriteLine($"Selected Item from ViewModel {value}");
            }
        }


        #endregion

        #region Functions

        // you can customize your sorting algorithim to however you want it to work.
        public Func<string, ICollection<string>, ICollection<string>> SortingAlgorithm { get; } =
            (text, values) => values
                .Where(x => x.ToLower().StartsWith(text.ToLower()))
                .OrderBy(x => x)
                .ToList();

        private static void EmailAddressPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var model = (ComboBoxViewModel)bindable;
            // make sure we have the latest string.
            var text = newvalue.ToString();

            // if the text is empty or already contains an @ symbol, don't update anything.
            //if (string.IsNullOrEmpty(text) || text.Contains("@")) return;
            if (string.IsNullOrEmpty(text)) return;

            // clear the old suggestions, you're starting over. This also can be more efficient, 
            // I'll leave that for you to figure out.
            model.EmailSuggestions.Clear();

            // side note: for loops will add a tiny performance boost over foreach
            for (var i = 0; i < _emails.Count; i++)
                model.EmailSuggestions.Add($"{_emails[i]}");
        }

        public override void ValidateProperty([CallerMemberName]string propertyName = null)
        {
            IsValid = Errors.Any();
            RaiseErrorsChanged(propertyName);
        }


        #endregion
    }
}