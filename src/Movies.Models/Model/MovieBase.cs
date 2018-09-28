using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Windows.Controls;

namespace Movies.Models.Model
{
    public class MovieBase : UserControl,INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged<T>(Expression<Func<T>> expr)
        {
            var bodyExpr = expr.Body as MemberExpression;
            var propInfo = bodyExpr.Member as PropertyInfo;
            var propName = propInfo.Name;
            RaisePropertyChanged(propName);
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
