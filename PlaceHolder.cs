﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ACST
{
	class PlaceHolderBehavior
	{
		public static readonly DependencyProperty PlaceHolderTextProperty = DependencyProperty.RegisterAttached(
			"PlaceHolderText",
			typeof(string),
			typeof(PlaceHolderBehavior),
			new PropertyMetadata(null, OnPlaceHolderChanged));

		private static void OnPlaceHolderChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var textBox = sender as TextBox;
			if (textBox == null)
			{
				return;
			}

			var placeHolder = e.NewValue as string;
			var handler = CreateEventHandler(placeHolder);
			if (string.IsNullOrEmpty(placeHolder))
			{
				textBox.TextChanged -= handler;
			}
			else
			{
				textBox.TextChanged += handler;
				if (string.IsNullOrEmpty(textBox.Text))
				{
					textBox.Background = CreateVisualBrush(placeHolder);
				}
			}
		}

		private static TextChangedEventHandler CreateEventHandler(string placeHolder)
		{
			return (sender, e) =>
			{
				var textBox = (TextBox)sender;
				if (string.IsNullOrEmpty(textBox.Text))
				{
					textBox.Background = CreateVisualBrush(placeHolder);
				}
				else
				{
					textBox.Background = new SolidColorBrush(Colors.White);
				}
			};
		}

		private static VisualBrush CreateVisualBrush(string placeHolder)
		{
			var visual = new Label()
			{
				Content = placeHolder,
				Padding = new Thickness(5, 1, 1, 1),
				Foreground = new SolidColorBrush(Colors.LightGray),
				HorizontalAlignment = HorizontalAlignment.Left,
				VerticalAlignment = VerticalAlignment.Center,
			};
			return new VisualBrush(visual)
			{
				Stretch = Stretch.None,
				TileMode = TileMode.None,
				AlignmentX = AlignmentX.Left,
				AlignmentY = AlignmentY.Center,
			};
		}

		public static void SetPlaceHolderText(TextBox textBox, string placeHolder)
		{
			textBox.SetValue(PlaceHolderTextProperty, placeHolder);
		}

		public static string GetPlaceHolderText(TextBox textBox)
		{
			return textBox.GetValue(PlaceHolderTextProperty) as string;
		}
	}
}
