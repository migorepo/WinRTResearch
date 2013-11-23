﻿using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;

namespace WinRT.Framework.Utils.Utils
{
    public static class TiltEffect
    {
        public static readonly DependencyProperty IsTiltEnabledProperty =
            DependencyProperty.RegisterAttached("IsTiltEnabled", typeof(bool), typeof(TiltEffect), new PropertyMetadata(default(bool), OnIsTiltEnabledChanged));

        public static void SetIsTiltEnabled(UIElement element, bool value)
        {
            element.SetValue(IsTiltEnabledProperty, value);
        }

        public static bool GetIsTiltEnabled(UIElement element)
        {
            return (bool)element.GetValue(IsTiltEnabledProperty);
        }

        private static void OnIsTiltEnabledChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var element = (UIElement)obj;
            if ((bool)args.NewValue)
            {
                element.PointerPressed += OnPointerPressed;
                element.PointerReleased += OnPointerReleased;
                element.PointerExited += OnPointerExited;
                element.PointerEntered += OnPointerEntered;
            }
            else
            {
                element.PointerPressed -= OnPointerPressed;
                element.PointerReleased -= OnPointerReleased;
                element.PointerExited -= OnPointerExited;
                element.PointerEntered -= OnPointerEntered;
            }
        }


        private static void OnPointerExited(object sender, PointerRoutedEventArgs pointerRoutedEventArgs)
        {
            ShowUpAnimation((UIElement)sender);
        }

        private static void OnPointerEntered(object sender, PointerRoutedEventArgs args)
        {
            if (args.Pointer.IsInContact)
                ShowDownAnimation((UIElement)sender);
        }

        private static void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            ShowUpAnimation((UIElement)sender);
        }

        private static void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            ShowDownAnimation((UIElement)sender);
        }


        private static void ShowUpAnimation(UIElement element)
        {
            var animation = new PointerUpThemeAnimation();
            Storyboard.SetTarget(animation, element);

            var storyboard = new Storyboard();
            storyboard.Children.Add(animation);
            storyboard.Begin();
        }

        private static void ShowDownAnimation(UIElement element)
        {
            var animation = new PointerDownThemeAnimation();
            Storyboard.SetTarget(animation, element);

            var storyboard = new Storyboard();
            storyboard.Children.Add(animation);
            storyboard.Begin();
        }
    }
}
