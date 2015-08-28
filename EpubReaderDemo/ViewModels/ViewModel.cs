using System;
using System.ComponentModel;
using System.Linq.Expressions;
using EpubReaderDemo.Utils;
using System.Windows;

namespace EpubReaderDemo.ViewModels
{
    public abstract class ViewModel : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged<T>(Expression<Func<T>> expression)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(ExpressionUtils.GetPropertyName(expression)));
        }
    }
}
