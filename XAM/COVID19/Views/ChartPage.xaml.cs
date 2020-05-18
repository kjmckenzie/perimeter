using Perimeter.Models;
using Perimeter.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Microcharts;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SkiaSharp;

namespace Perimeter.Views
{

  
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class ChartPage : ContentPage
    {
     
        public ChartPage(ShieldPageViewModel vm)
        {
            InitializeComponent();

            int total = 0;

            try
            {
                List<Microcharts.Entry> entries = new List<Microcharts.Entry>();
                
                var labels = new List<string>();
                DateTime history = DateTime.Now.AddHours(-24);
                Random random = new Random(DateTime.Now.Second);

                for (int i = 0; i < 24; i++)
                {

                    string date = history.AddHours(i).ToString();

                    int hour = history.AddHours(i).Hour;

                    int sum = vm.DeviceList.Count(x => x.DateCreated.Year.Equals(history.AddHours(i).Year) &&
                    x.DateCreated.Month.Equals(history.AddHours(i).Month) &&
                    x.DateCreated.Day.Equals(history.AddHours(i).Day) &&
                    x.DateCreated.Hour.Equals(history.AddHours(i).Hour));

                    //uncomment for test data
                    //if (hour == 8 || hour == 9 || hour == 12 || hour == 18 || hour == 19)
                    //    sum = 10 + random.Next(50);

                    //total += sum;
                    entries.Add(new Microcharts.Entry(sum) {Label=i.ToString(),ValueLabel=sum.ToString(), Color = SKColor.Parse("#092ABB") });
                    string when = history.AddHours(i).ToString("htt");  //hour < 12 ? "am" : "pm";
                    labels.Add(when);
                }

                chart.Chart= new BarChart()
                {
                    Entries = entries,
                    LabelTextSize=20
                };
            }
            catch (Exception ex)
            {

            }

            if(total==0)
            {
                lblStatus.IsVisible = true;
                chart.IsVisible = false;

            }

        }
    

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }


        protected override void OnDisappearing()
        {
            base.OnDisappearing();

         
        }

    }
}