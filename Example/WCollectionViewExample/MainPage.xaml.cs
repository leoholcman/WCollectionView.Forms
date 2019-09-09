using System;
using System.Collections.Generic;
using System.ComponentModel;
using Wapps.Forms.Controls;
using Xamarin.Forms;

namespace WCollectionViewExample
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            var cells = new List<CellInfo>();
            CellInfo cellInf;
            for (int i = 0; i < 80; i++)
            {
                cellInf = new CellInfo();
                if (i % 3 == 0)
                {
                    cellInf.ImageUrl = "http://img2.rtve.es/i/?w=1600&i=1555448732607.jpg";
                    cellInf.Title = "Mesi 1";
                }
                if (i % 3 == 1)
                {
                    cellInf.ImageUrl = "https://www.aljazeera.com/mritems/imagecache/mbdxxlarge/mritems/Images/2019/6/23/e4452fd524214d74b5af74a9ad19bd85_18.jpg";
                    cellInf.Title = "Mesi 2";
                }
                else
                {
                    cellInf.ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcS6RP2ZXIOSIO-dHZB7LbluObO-GWH26Zr8r9L45bLur7BgUSDr";
                    cellInf.Title = "Mesi 3";
                }

                cellInf.Title = "Messi " + (i + 1);

                cells.Add(cellInf);
            }

            Lv.ItemsSource = cells;
        }

        async void Lv_Refreshing(object sender, EventArgs args)
        {
            await System.Threading.Tasks.Task.Delay(3000);
            Lv.EndRefresh();
        }

        void WaterfallListView_ItemTapped(object sender, WItemTappedEventArgs args)
        {
            var messi = args.Item as CellInfo;
            DisplayAlert("Atencion!", messi.Title, "Ok");
        }
    }

    public class CellInfo
    {
        public string ImageUrl { get; set; }
        public string Title { get; set; }
    }
}
