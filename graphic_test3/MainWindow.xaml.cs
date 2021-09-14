using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Threading;



// https://www.youtube.com/watch?v=Nb89r13Z1q4
// https://www.youtube.com/watch?v=Vjldip84CXQ
// https://www.youtube.com/watch?v=b9G7uxhAzt0
//
//https://wpf-tutorial.com/misc-controls/the-border-control/



namespace graphic_test3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DBConnection db;

        LogClass mylogs = new LogClass();

        //  string photos_root_folder = @"C:/xampp/htdocs/vega_356/photos/20210817/";

        public int mypage = 1;
        public int total_pages = 0;
        public string datefrom = "1980-01-01";
        public string dateto = "2021-09-12";

        public List<string> data = new List<string>();
        

        //public static KeyValuePair<int, string>[] tripLengthList;



        public MainWindow()
        {
            InitializeComponent();

            // Image Background
            ImageBrush myBrush = new ImageBrush();
            Image bgimage = new Image();
            bgimage.Source = new BitmapImage(new Uri(@"C:/xampp/htdocs/vega_356/views/theme_default/bgthemes/body/countryside_02.jpg"));
            bgimage.Stretch = Stretch.Fill;
            flowPanelBody.ImageSource = bgimage.Source;

            //======================================================================================================

            DispatcherTimer LiveTime = new DispatcherTimer();
            LiveTime.Interval = TimeSpan.FromSeconds(1);
            LiveTime.Tick += timer_Tick;
            LiveTime.Start();


            //======================================================================================================

            db = new DBConnection("localhost", "3306", "vega_db_356", "root", "1964Pib3");


            reload_page(mypage, datefrom, dateto);
                                  

            for (var p = 1; p <= total_pages; p++) // build combobox with a list of page numbers
            {
                    data.Add(p.ToString());
            }
            
            //==========================================================================================
        }




        void reload_page(int page = 1, string datefrom = "1980-01-01", string dateto = "2021-09-12") {

            int[] pg = db.GetPaginatedValues(page, 12); // page number, items per page

            total_pages = pg[2] - 1;

            PageNumber.Content = page + " / " + total_pages;  // displays the page number along with the maximum number of pages available

            wrappanel.Children.Clear(); // clears out all photos from wrappanel body

            var data_inserts = db.LoadPhotosToList(pg[0], pg[1], datefrom, dateto);


            foreach (var mydata in data_inserts) // update remote/local database with tag inserts
            {
                add_Image(mydata.pk_id, "C:/xampp/htdocs/vega_356/photos/20210909/preview/" + mydata.photocode);
            }
        }





        void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            // ... Assign the ItemsSource to the List.
            PageSelectCombo.ItemsSource = data;

            // ... Make the first item selected.
            PageSelectCombo.SelectedIndex = 0;
        }



        void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // ... Get the ComboBox.
            var comboBox = sender as ComboBox;

            // ... Set SelectedItem as Window Title.
            string value = comboBox.SelectedItem as string;
            this.Title = "Selected: " + value;

            mypage = Int32.Parse(value);

            reload_page(mypage, datefrom, dateto);
        }





        void timer_Tick(object sender, EventArgs e) // the system clock
        {
            LiveTimeLabel.Content = DateTime.Now.ToString("ddd dd-MM-yyyy  HH:mm:ss");
        }
        
        

        void add_Image(string id, string image_and_path) {

            //===== Border

            Border myborder = new Border();
            myborder.Name = "border_" + id;
            myborder.BorderBrush = Brushes.Gray;
            myborder.Background = Brushes.LightGray;
            myborder.Margin = new Thickness(15, 15, 15, 15);
            myborder.CornerRadius = new CornerRadius(5);
            myborder.BorderThickness = new Thickness(2);
            DockPanel.SetDock(myborder, Dock.Top);

            // <Border Name="border" BorderBrush="Gray" Background="LightGray" Margin="15 15 15 15" CornerRadius="6" BorderThickness="2" DockPanel.Dock="Top">

            //===== Border.Effect

            DropShadowEffect shadow = new DropShadowEffect();
            shadow.Color = Colors.Black;
            shadow.BlurRadius = 10;
            shadow.ShadowDepth = 6;
            shadow.Opacity = 0.6;

            //<Border.Effect>
            //<DropShadowEffect ShadowDepth="5" BlurRadius = "10" Opacity="0.5"/>

            //===== StackPanel

            //<ToolTip Background = "#D5F0F0FF">
                        
            StackPanel toppanel = new StackPanel();
            toppanel.VerticalAlignment = VerticalAlignment.Top;
            toppanel.Background = new SolidColorBrush(Color.FromArgb(90, 120, 120, 120));
            
            // toppanel.Background = Brushes.Transparent;
            
            // <StackPanel Name="toppanel" VerticalAlignment="Top">

            //---

            StackPanel stackimage = new StackPanel();
            stackimage.Name = "stackimage";
            stackimage.Orientation = Orientation.Horizontal;
            stackimage.Margin = new Thickness(0, 0, 0, 0);
            
            

            // <StackPanel Name="stackimage" Margin="0 0 0 0" Background="LightGray" Orientation="Horizontal">

            //---

            StackPanel stackbuttons = new StackPanel();
            stackbuttons.Name = "stackbuttons";
            stackbuttons.Margin = new Thickness(0, 0, 15, 5);
            stackbuttons.HorizontalAlignment = HorizontalAlignment.Right;
            stackbuttons.Orientation = Orientation.Horizontal;

            // <StackPanel Name="stackbuttons" Margin="0 0 15 5" HorizontalAlignment="Right" Orientation="Horizontal">

            //===== Label

            Label mylabel = new Label();
            mylabel.VerticalAlignment = VerticalAlignment.Top;
            mylabel.Margin = new Thickness(10, 0, 10, 0);
            mylabel.Name = "title";
            mylabel.Content = "tst_7223_1001";

            // <Label Name="title" VerticalAlignment="Top" Margin="10 0 0 0">tst_7223_1001</Label>

            //===== Image

            Image photo = new Image()
            {
                Name = "newImage",
                Margin = new Thickness(10, 0, 10, 10),
                Width = 327,
                Height = 241,
                Source = new BitmapImage(new Uri(image_and_path, UriKind.Absolute)),
            };
            
            //====== Buttons
            
            Button button1 = new Button
            {
                Width = 30,
                Height = 30,
                Padding = new Thickness(3),
                BorderThickness = new Thickness(0),
                Name = "cart_" + id,
                Content = new Image
                {
                    Source = new BitmapImage(new Uri("C:/Users/Paul_/source/repos/graphic_test3/graphic_test3/assests/Basket_Add24.png")),
                    VerticalAlignment = VerticalAlignment.Center
                }
            };
            button1.Click += Cart_Button;


            Button button2 = new Button
            {
                Width = 30,
                Height = 30,
                Padding = new Thickness(3),
                BorderThickness = new Thickness(0),
                Name = "fav_" + id,
                Content = new Image
                {
                    Source = new BitmapImage(new Uri("C:/Users/Paul_/source/repos/graphic_test3/graphic_test3/assests/Heart_Add24.png")),
                    VerticalAlignment = VerticalAlignment.Center
                }
            };
            button2.Click += Fav_Button;


            Button button3 = new Button
            {
                Width = 30,
                Height = 30,
                Padding = new Thickness(3),
                BorderThickness = new Thickness(0),
                Name = "tag_" + id,
                Content = new Image
                {
                    Source = new BitmapImage(new Uri("C:/Users/Paul_/source/repos/graphic_test3/graphic_test3/assests/tag.png")),
                    VerticalAlignment = VerticalAlignment.Center
                }
            };
            button3.Click += Tag_Button;


            Button button4 = new Button
            {
                Width = 30,
                Height = 30,
                Padding = new Thickness(3),
                BorderThickness = new Thickness(0),
                Name = "zoom_" + id,
                Content = new Image
                {
                    Source = new BitmapImage(new Uri("C:/Users/Paul_/source/repos/graphic_test3/graphic_test3/assests/Zoom_In24.png")),
                    VerticalAlignment = VerticalAlignment.Center
                }
            };
            button4.Click += Zoom_Button;

            //====== Separator

            Separator separator1 = new Separator { Opacity = 0, Width = 20 };
            Separator separator2 = new Separator { Opacity = 0, Width = 20 };
            Separator separator3 = new Separator { Opacity = 0, Width = 20 };

            // <Separator Name="" Width="10" />

            //--------------------------------------------------------


            toppanel.Children.Add(mylabel);

            stackimage.Children.Add(photo);

            toppanel.Children.Add(stackimage);

            stackbuttons.Children.Add(button4);
            stackbuttons.Children.Add(separator3);
            stackbuttons.Children.Add(button3);
            stackbuttons.Children.Add(separator2);
            stackbuttons.Children.Add(button2);
            stackbuttons.Children.Add(separator1);
            stackbuttons.Children.Add(button1);

            toppanel.Children.Add(stackbuttons);

            myborder.Effect = shadow;

            myborder.Child = toppanel;
                        

            wrappanel.Children.Add(myborder);
        }



        //=========================================================================================


        void Previous_Click(object sender, EventArgs e)
        {
            if (mypage > 1) {
                reload_page(--mypage);
            }
        }



        void Next_Click(object sender, EventArgs e)
        {
        //    MessageBox.Show("total_pages: " + total_pages);

            if (mypage < total_pages)
            {
                reload_page(++mypage);
            }
        }


        //=========================================================================================


         void Zoom_Button(object sender, EventArgs e)
         {
            Button button = sender as Button;

            MessageBox.Show("Zoom_Button: " + button.Name);



             Border myborder = new Border();

            //var element = myborder.Children.OfType<FrameworkElement>().FirstOrDefault(e => e.Name == "border_775")


            // identify which button was clicked and perform necessary actions
        }



        void Tag_Button(object sender, EventArgs e)
         {
             Button button = sender as Button;

             MessageBox.Show("Tag_Button: " + button.Name);

            // identify which button was clicked and perform necessary actions
         }


         void Fav_Button(object sender, EventArgs e)
         {
            Button button = sender as Button;

            MessageBox.Show("Fav_Button: " + button.Name);

            // identify which button was clicked and perform necessary actions
         }

        
         void Cart_Button(object sender, EventArgs e)
         {
            Button button = sender as Button;

            MessageBox.Show("Cart_Button: " + button.Name);

            // identify which button was clicked and perform necessary actions
         }


        //================================


        void MainGrid_Loaded(Object sender, RoutedEventArgs e)
        {         
            DoubleAnimation fadeAnimation = new DoubleAnimation();
            fadeAnimation.Duration = TimeSpan.FromSeconds(1.0d);
            fadeAnimation.From = 0.0d;
            fadeAnimation.To = 1.0d;
            wrappanel.BeginAnimation(Grid.OpacityProperty, fadeAnimation);         
        }

        private void FromDate_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }
    }
}


