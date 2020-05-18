using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Perimeter.Controls
{
	public class CustomMap : Map
    {
      
        public CustomCircle Circle { get; set; }
        public Pin pin { get; set; }

        private const int DefaultCircleRadius = 0;
        private const float DefaultLatitude = 51.500152f;
        private const float DefaultLongitude = -0.126236f;
        private const int DefaultMapRadius = 10;

        public static readonly BindableProperty CircleRadiusProperty = BindableProperty.Create("CircleRadius", typeof(int), typeof(CustomMap), DefaultCircleRadius, BindingMode.TwoWay, propertyChanged: OnCircleRadiusPropertyChanged);
        public static readonly BindableProperty LatitudeProperty = BindableProperty.Create("Latitude", typeof(float), typeof(CustomMap), DefaultLatitude, BindingMode.TwoWay, propertyChanged: OnLatitudePropertyChanged);
        public static readonly BindableProperty LongitudeProperty = BindableProperty.Create("Longitude", typeof(float), typeof(CustomMap), DefaultLongitude, BindingMode.TwoWay, propertyChanged: OnLongitudePropertyChanged);
        public static readonly BindableProperty MapRadiusProperty = BindableProperty.Create("MapRadius", typeof(int), typeof(CustomMap), DefaultMapRadius, BindingMode.TwoWay, propertyChanged: OnMapRadiusPropertyChanged);
        public static BindableProperty PointsProperty = BindableProperty.Create(nameof(Points), typeof(List<Models.Point>), typeof(CustomMap), new List<Models.Point>());

        public CustomMap() : base(MapSpan.FromCenterAndRadius(new Position(DefaultLatitude, DefaultLongitude), Distance.FromMeters(DefaultMapRadius))) 
        {
            Circle = new CustomCircle
            {
                Position = new Xamarin.Forms.Maps.Position(DefaultLongitude, DefaultLongitude),
                Radius = DefaultCircleRadius
            };

            //pin = new Pin
            //{
            //    Type = PinType.Place,
            //    Position = new Xamarin.Forms.Maps.Position(DefaultLongitude, DefaultLongitude),
            //    Label = "London, UK"

            //};

            //Pins.Add(pin);

        }

        public List<Models.Point> Points
        {
            get => GetValue(PointsProperty) as List<Models.Point>;
            set => SetValue(PointsProperty, value);
        }

        public int CircleRadius
        {
            get => (int)GetValue(CircleRadiusProperty);
            set => SetValue(CircleRadiusProperty, value);
        }

        public float Latitude
        {
            get => (float)GetValue(LatitudeProperty);
            set => SetValue(LatitudeProperty, value);
        }

        public float Longitude
        {
            get => (float)GetValue(LongitudeProperty);
            set => SetValue(LongitudeProperty, value);
        }

        public int MapRadius
        {
            get => (int)GetValue(MapRadiusProperty);
            set => SetValue(MapRadiusProperty, value);
        }

        private static void OnCircleRadiusPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var circleMap = (CustomMap)bindable;
            circleMap.CircleRadius = (int)newValue;
        }

        private static void OnLatitudePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var circleMap = (CustomMap)bindable;
            circleMap.Latitude = (float)newValue;

            MoveToRegion(circleMap);
        }

        private static void OnLongitudePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var circleMap = (CustomMap)bindable;
            circleMap.Longitude = (float)newValue;

            MoveToRegion(circleMap);
        }

        private static void OnMapRadiusPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var circleMap = (CustomMap)bindable;
            circleMap.MapRadius = (int)newValue;

            MoveToRegion(circleMap);
        }

        private static void MoveToRegion(CustomMap circleMap)
        {
            circleMap.Circle.Position = new Position(circleMap.Latitude, circleMap.Longitude);

            circleMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(circleMap.Latitude, circleMap.Longitude), Distance.FromMeters(circleMap.MapRadius)));
        }
    }

}


