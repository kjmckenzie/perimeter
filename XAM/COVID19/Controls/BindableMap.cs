using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Perimeter.Controls
{
    public class BindableMap : Map
    {
        public static readonly BindableProperty MapPinsProperty = BindableProperty.Create(
                 nameof(Pins),
                 typeof(ObservableCollection<Pin>),
                 typeof(BindableMap),
                 new ObservableCollection<Pin>(),
                 propertyChanged: (b, o, n) =>
                 {
                     var bindable = (BindableMap)b;
                     bindable.Pins.Clear();

                     var collection = (ObservableCollection<Pin>)n;
                     foreach (var item in collection)
                         bindable.Pins.Add(item);
                     collection.CollectionChanged += (sender, e) =>
                     {
                         Device.BeginInvokeOnMainThread(() =>
                         {
                             switch (e.Action)
                             {
                                 case NotifyCollectionChangedAction.Add:
                                 case NotifyCollectionChangedAction.Replace:
                                 case NotifyCollectionChangedAction.Remove:
                                     if (e.OldItems != null)
                                         foreach (var item in e.OldItems)
                                             bindable.Pins.Remove((Pin)item);
                                     if (e.NewItems != null)
                                         foreach (var item in e.NewItems)
                                             bindable.Pins.Add((Pin)item);
                                     break;
                                 case NotifyCollectionChangedAction.Reset:
                                     bindable.Pins.Clear();
                                     break;
                             }
                         });
                     };
                 });
        public IList<Pin> MapPins { get; set; }

        public static readonly BindableProperty MapPositionProperty = BindableProperty.Create(
                 nameof(MapPosition),
                 typeof(Position),
                 typeof(BindableMap),
                 new Position(0, 0),
                 propertyChanged: OnMapPositionPropertyChanged);

  
        public Position MapPosition
        {
            get => (Position)GetValue(MapPositionProperty);
            set => SetValue(MapPositionProperty, value);
        }
        private static void OnMapPositionPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((BindableMap)bindable).MoveToRegion(MapSpan.FromCenterAndRadius(
                         (Position)newValue,
                         Distance.FromMiles(1)));

            
        }

    }

}


