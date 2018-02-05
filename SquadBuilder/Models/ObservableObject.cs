using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;

namespace SquadBuilder
{
	public class ObservableObject : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void NotifyPropertyChanged ([CallerMemberName]string propertyName = null)
		{
			OnPropertyChanged (new PropertyChangedEventArgs (propertyName));
		}

		protected virtual void NotifyPropertyChanged<T> (Expression<Func<T>> propertyExpression)
		{
			string propertyName = GetPropertyName (propertyExpression);
			OnPropertyChanged (new PropertyChangedEventArgs (propertyName));
		}

		protected virtual void OnPropertyChanged (PropertyChangedEventArgs e)
		{
			var eventHandler = PropertyChanged;
			if (eventHandler != null) {
				eventHandler (this, e);
			}
		}

		protected bool SetProperty<T> (ref T storage, T value, Expression<Func<T>> propertyExpression)
		{
			var propertyName = GetPropertyName (propertyExpression);
			return SetProperty<T> (ref storage, value, propertyName);
		}

		protected bool SetProperty<T> (ref T storage, T value, [CallerMemberName] string propertyName = null)
		{
			if (EqualityComparer<T>.Default.Equals (storage, value)) {
				return false;
			}

			storage = value;
			this.NotifyPropertyChanged (propertyName);
			return true;
		}

		private string GetPropertyName<T> (Expression<Func<T>> propertyExpression)
		{
			if (propertyExpression == null) {
				throw new ArgumentNullException ("propertyExpression");
			}

			if (propertyExpression.Body.NodeType != ExpressionType.MemberAccess) {
				throw new ArgumentException ("Should be a member access lambda expression", "propertyExpression");
			}

			var memberExpression = (MemberExpression) propertyExpression.Body;
			return memberExpression.Member.Name;
		}
	}
}
