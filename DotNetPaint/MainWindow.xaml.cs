/*
* FILE : MainWindow.xaml.cs
* PROJECT : WinProg Final Project Option 4: DotNetPaint
* PROGRAMMER : Bobby Vu and Jason Gemanaru
* FIRST VERSION : 2016-12-09
* DESCRIPTION :
* This application replicates a simple version of Windows Paint
* It allows you to draw Lines, Rectangles and Ellipses with different Line Thickness, Line Color and Fill Color
* It also has simple Open and Save file functionality
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls.Primitives;
namespace DotNetPaint
{
    // Derived from INotifyPropertyChanged for the purpose of bindings between xaml elements and this class's properties
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        // Private data members
        private int thickness;
        private int cursorX;
        private int cursorY;

        private string filePath;
        private string fileName;
        private string fullTitle;


        // Constants
        private const string APP_NAME = " - .netPaint";
        private const int kLine = 0;
        private const int kRectangle = 1;
        private const int kEllipse = 2;

        // Properties of the class
        // These properties are binded with specific elements in XAML
        public string FilePath
        {
            get { return filePath; }
            set
            {
                if (fileName != value)
                {
                    filePath = value;

                    // Set FileName as well when FilePath is set
                    if (filePath == "")
                    {
                        FileName = "Untitled";
                    }
                    else
                    {
                        FileName = System.IO.Path.GetFileName(filePath);
                    }                    
                }
            }
        }

        public string FileName
        {
            get { return fileName; }
            set
            {
                if (fileName != value)
                {
                    fileName = value;
                    // Set FullTitle as well when FileName is set
                    FullTitle = fileName + APP_NAME;

                    // Notify the change to xaml
                    OnPropertyChanged();
                }
            }
        }

        public string FullTitle
        {
            get { return fullTitle; }
            set
            {
                if (fullTitle != value)
                {
                    fullTitle = value;
                    // Notify the change to xaml
                    OnPropertyChanged();
                }
            }
        }

        public int Thickness
        {
            get { return thickness; }
            set
            {
                if (thickness != value)
                {
                    thickness = value;
                    // Notify the change to xaml
                    OnPropertyChanged();
                }
            }
        }

        public int CurrentCursorX
        {
            get { return cursorX; }
            set
            {
                if (cursorX != value)
                {
                    cursorX = value;
                    // Notify the change to xaml
                    OnPropertyChanged();
                }
            }
        }

        public int CurrentCursorY
        {
            get { return cursorY; }
            set
            {
                if (cursorY != value)
                {
                    cursorY = value;
                    // Notify the change to xaml
                    OnPropertyChanged();
                }
            }
        }

        // Event for when a property is set
        public event PropertyChangedEventHandler PropertyChanged;


        /*
        * FUNCTION : OnPropertyChanged
        *
        * DESCRIPTION : Notify the appropriate element in XAML to change the data when its binded property is set
        *
        * PARAMETERS : string propertyName - Name of the property
        *
        * RETURNS : NONE
        */
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        // Global variables
        private int drawingObject; // Keep track of what shape is currently drawn
        private Color lineColor; // Keep track of the line color of the shape
        private Color fillColor; // Keep track of the fill color of the shape
        private Point startingPoint; // The point when the mouse start drawing a shape

        bool canvasModified = false; // See if the drawing is modified (this is used for SaveFileDialog)
        Shape obj; // Current drawing shape

        private bool mouseDown; // Keep track of the mouse status

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            // Instantiate code for XAML elements
            comboBox.Items.Add("Line");
            comboBox.Items.Add("Rectangle");
            comboBox.Items.Add("Ellipse");
            comboBox.SelectedIndex = kLine; // Starting Shape: Line
            lineColor = Color.FromRgb(0, 0, 0); // Starting Line Color: Black
            fillColor = Color.FromRgb(255, 255, 255); // Starting Fill Color: White
            Thickness = 1; // Starting Thickness: 1

            btn_select_lineColor.Background = new SolidColorBrush(lineColor);
            btn_select_fillColor.Background = new SolidColorBrush(fillColor);
            mouseDown = false;
            FilePath = "";
        }

        /*
        * FUNCTION : wrapperCanvas_MouseDown
        *
        * DESCRIPTION : This function is triggered when the user mouse click on the canvas
        *               It instantiate the drawing by creating a shape in the canvas
        *               
        * PARAMETERS : object sender - The drawing canvas
        *              MouseButtonEventArgs e - The mouse click event that contains the position of the mouse
        *
        * RETURNS : NONE
        */
        private void wrapperCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mouseDown = true;
            canvasModified = true;
            startingPoint = e.GetPosition(wrapperCanvas); // Get mouse position from the event

            switch (drawingObject)
            {
                // Create an object depending on what kind of shape is being drawn
                // The shape has dashed black line color
                case kLine:
                    obj = new Line() { Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0)), StrokeDashArray = new DoubleCollection() { 4, 2 } };

                    // Initiate points for line shape as well
                    ((Line)obj).X1 = startingPoint.X;
                    ((Line)obj).Y1 = startingPoint.Y;
                    ((Line)obj).X2 = startingPoint.X;
                    ((Line)obj).Y2 = startingPoint.Y;
                    break;

                case kRectangle:
                    obj = new Rectangle() { Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0)), StrokeDashArray = new DoubleCollection() { 4, 2 } };
                    break;

                case kEllipse:
                    obj = new Ellipse() { Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0)), StrokeDashArray = new DoubleCollection() { 4, 2 } };
                    break;
            }

            if (drawingObject != kLine)
            {
                // If the shape is not a line, set the position of the shape in relative with the canvas
                Canvas.SetLeft(obj, startingPoint.X);
                Canvas.SetTop(obj, startingPoint.X);
            }

            // Add the shape into the canvas
            wrapperCanvas.Children.Add(obj);

            // Capture mouse so the canvas can detect 2 other Mouse Events even when the mouse is not inside the canvas
            wrapperCanvas.CaptureMouse();
        }


        /*
        * FUNCTION : wrapperCanvas_MouseMove
        *
        * DESCRIPTION : This function is triggered when the user move the mouse around the canvas
        *               Since I called CaptureMouse in MouseDown, if the mouse is pressed, this event will be triggered
        *                   even when the mouse is outside of the canvas
        *               If the mouse is held down, this function updates the size of the current drawn shape
        *               
        * PARAMETERS : object sender - The drawing canvas
        *              MouseButtonEventArgs e - The mouse click event that contains the position of the mouse
        *
        * RETURNS : NONE
        */
        private void wrapperCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown && obj != null)
            {
                // If the mouse is currently held down
                // Update the visibility of the status bar
                foreach (TextBlock t in statusBar.Items)
                {
                    t.Visibility = Visibility.Visible;
                }

                // Get the mouse position
                Point currentPoint = Mouse.GetPosition(wrapperCanvas);

                // Update these properties for the XAML elements to show them
                CurrentCursorX = (int)currentPoint.X;
                CurrentCursorY = (int)currentPoint.Y;

                // Update the shape's size in the canvas
                UpdateSize(currentPoint);
            }
            else
            {
                // Hide the status bar
                foreach (TextBlock t in statusBar.Items)
                {
                    t.Visibility = Visibility.Hidden;
                }
            }

        }




        /*
        * FUNCTION : wrapperCanvas_MouseUp
        *
        * DESCRIPTION : This function is triggered when the user release the mouse click button
        *               If the mouse is held down, this function clears the temporary drawn object and 
        *                   draw a new object with specified Line Color, Thickness and Fill Color
        *               
        * PARAMETERS : object sender - The drawing canvas
        *              MouseButtonEventArgs e - The mouse click event that contains the position of the mouse
        *
        * RETURNS : NONE
        */
        private void wrapperCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (mouseDown)
            {
                // If the mouse was initially pressed inside the canvas
                mouseDown = false;

                // Remove the temporary shape (dotted line one)
                wrapperCanvas.Children.Remove(obj);
                obj = null;

                // Get current mouse position
                Point lastPoint = Mouse.GetPosition(wrapperCanvas);


                switch (drawingObject)
                {
                    // Create new shape with specified Line Color, Fill Color and Thickness
                    case kLine:
                        obj = new Line()
                        {
                            Stroke = new SolidColorBrush(lineColor),
                            StrokeThickness = Thickness
                        };
                        ((Line)obj).X1 = startingPoint.X;
                        ((Line)obj).Y1 = startingPoint.Y;
                        break;

                    case kRectangle:
                        obj = new Rectangle()
                        {
                            Stroke = new SolidColorBrush(lineColor),
                            Fill = new SolidColorBrush(fillColor),
                            StrokeThickness = Thickness
                        };
                        break;

                    case kEllipse:
                        obj = new Ellipse()
                        {
                            Stroke = new SolidColorBrush(lineColor),
                            Fill = new SolidColorBrush(fillColor),
                            StrokeThickness = Thickness
                        };
                        break;
                }

                // Update their size using the mouse postion
                UpdateSize(lastPoint);

                // Add that shape into the canvas
                wrapperCanvas.Children.Add(obj);

                // Release mouse capturing outside of canvas
                wrapperCanvas.ReleaseMouseCapture();
            }
            
        }


        /*
        * FUNCTION : UpdateSize
        *
        * DESCRIPTION : This function updates the size of the shape currently being drawn on the canvas
        *               
        * PARAMETERS : Point p - Current mouse position
        *
        * RETURNS : NONE
        */
        private void UpdateSize(Point p)
        {
            if (drawingObject != kLine)
            {
                // If Rectangle or Ellipse is being drawn
                // Update their Position
                double x = Math.Min(p.X, startingPoint.X);
                double y = Math.Min(p.Y, startingPoint.Y);
                Canvas.SetLeft(obj, x);
                Canvas.SetTop(obj, y);

                // Update Width and Height
                double width = Math.Abs(p.X - startingPoint.X);
                double height = Math.Abs(p.Y - startingPoint.Y);
                obj.Width = width;
                obj.Height = height;
            }
            else
            {
                // If Line is being drawn
                // Update the second point of the line
                ((Line)obj).X2 = p.X;
                ((Line)obj).Y2 = p.Y;
            }




        }


        /*
        * FUNCTION : comboBox_SelectionChanged
        *
        * DESCRIPTION : This function updates 
        *               what kind of shape is being drawn everytime the combo box selection is changed
        *               
        * PARAMETERS : object sender, SelectionChangedEventArgs e
        *
        * RETURNS : NONE
        */
        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            drawingObject = comboBox.SelectedIndex;
        }


        /*
        * FUNCTION : btnLineWidthDown_Click
        *
        * DESCRIPTION : This function decreases the line thickness by 1 every time the decrease button is clicked
        *               
        * PARAMETERS : object sender, RoutedEventArgs e
        *
        * RETURNS : NONE
        */
        private void btnLineWidthDown_Click(object sender, RoutedEventArgs e)
        {
            // Lower Range = 1
            if (Thickness != 1)
            {
                Thickness--;
            }
        }


        /*
        * FUNCTION : btnLineWidthUp_Click
        *
        * DESCRIPTION : This function increases the line thickness by 1 every time the increase button is clicked
        *               
        * PARAMETERS : object sender, RoutedEventArgs e
        *
        * RETURNS : NONE
        */
        private void btnLineWidthUp_Click(object sender, RoutedEventArgs e)
        {
            // Upper Range = 99
            if (Thickness != 99)
            {
                Thickness++;
            }
        }




        /*
        * FUNCTION : btn_clear_Click
        *
        * DESCRIPTION : This functions clear the current drawing canvas whenever the "Clear" button is clicked
        *               
        * PARAMETERS : object sender, RoutedEventArgs e
        *
        * RETURNS : NONE
        */
        private void btn_clear_Click(object sender, RoutedEventArgs e)
        {
            wrapperCanvas.Children.Clear();
            wrapperCanvas.Background = null;
            wrapperCanvas.Background = Brushes.White;
        }



        /*
        * FUNCTION : btn_select_lineColor_Click
        *
        * DESCRIPTION : This functions calls SelectColor to choose a new line color for the shape
        *               
        * PARAMETERS : object sender, RoutedEventArgs e
        *
        * RETURNS : NONE
        */
        private void btn_select_lineColor_Click(object sender, RoutedEventArgs e)
        {
            lineColor = SelectColor();
            btn_select_lineColor.Background = new SolidColorBrush(lineColor);
        }


        /*
        * FUNCTION : SelectColor
        *
        * DESCRIPTION : This functions calls a ColorDialog to choose a color
        *               
        * PARAMETERS : NONE
        *
        * RETURNS : Color - chosen color (black if nothing was chosen)
        */
        private Color SelectColor()
        {
            // Open ColorDialog
            System.Windows.Forms.ColorDialog colorD = new System.Windows.Forms.ColorDialog();
            colorD.AllowFullOpen = true;
            colorD.ShowDialog();

            // return that color
            Color c = new Color();
            c.A = colorD.Color.A;
            c.B = colorD.Color.B;
            c.G = colorD.Color.G;
            c.R = colorD.Color.R;

            return c;
        }


        /*
        * FUNCTION : btn_select_fillColor_Click
        *
        * DESCRIPTION : This functions calls SelectColor to choose a new fill color for the shape
        *               
        * PARAMETERS : object sender, RoutedEventArgs e
        *
        * RETURNS : NONE
        */
        private void btn_select_fillColor_Click(object sender, RoutedEventArgs e)
        {
            fillColor = SelectColor();
            btn_select_fillColor.Background = new SolidColorBrush(fillColor);
        }




        /*
        * FUNCTION : menuItem_save_Click
        *
        * DESCRIPTION : This functions allows user to 
        *               save current canvas to a saved file (or a new file if the user refresh the canvas)
        *               
        * PARAMETERS : object sender, RoutedEventArgs e
        *
        * RETURNS : NONE
        */
        private void menuItem_save_Click(object sender, RoutedEventArgs e)
        {
            // Render canvas into a bitmap image
            RenderTargetBitmap rtb = RenderCanvas();

            // Save that image into a png file
            System.Windows.Forms.SaveFileDialog dl1 = new System.Windows.Forms.SaveFileDialog();

            // Instantiate result as 'OK', so if the file already has a filepath, 
            // the file will be saved to that filepath without asking the user to specify it
            System.Windows.Forms.DialogResult result = System.Windows.Forms.DialogResult.OK;

            if (FilePath == "")
            {
                // If there is no file path (User just started the application or user clicked 'New')
                // Show SaveFileDialog to choose a file
                dl1.DefaultExt = ".png";
                dl1.Filter = "Image documents (.png)|*.png";
                result = dl1.ShowDialog();
                FilePath = dl1.FileName;
            }

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                // If something was chosen, save the rendered image to the file
                WriteToFile(rtb, FilePath);
                canvasModified = false;
            }
            
        }



        /*
        * FUNCTION : menuItem_saveAs_Click
        *
        * DESCRIPTION : This functions allows user to save current canvas to a new file
        *                              
        * PARAMETERS : object sender, RoutedEventArgs e
        *
        * RETURNS : NONE
        */
        private void menuItem_saveAs_Click(object sender, RoutedEventArgs e)
        {
            // Render canvas into a bitmap image
            RenderTargetBitmap rtb = RenderCanvas();

            // Save that image into a png file
            System.Windows.Forms.SaveFileDialog dl1 = new System.Windows.Forms.SaveFileDialog();
            dl1.DefaultExt = ".png";
            dl1.Filter = "Image documents (.png)|*.png";
            System.Windows.Forms.DialogResult result = dl1.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                FilePath = dl1.FileName;
                WriteToFile(rtb, FilePath);
                canvasModified = false;
            }
        }


        /*
        * FUNCTION : menuItem_new_Click
        *
        * DESCRIPTION : This functions allows to open a new empty canvas
        *                              
        * PARAMETERS : object sender, RoutedEventArgs e
        *
        * RETURNS : NONE
        */
        private void menuItem_new_Click(object sender, RoutedEventArgs e)
        {
            // Prompt to save the image first
            if (PromptOnDelete() == true)
            {
                FilePath = "";
            }
        }




        /*
        * FUNCTION : menuItem_exit_Click
        *
        * DESCRIPTION : This functions allows the user to open exit the application
        *                              
        * PARAMETERS : object sender, RoutedEventArgs e
        *
        * RETURNS : NONE
        */
        private void menuItem_exit_Click(object sender, RoutedEventArgs e)
        {
            // Ask to save first
            if (PromptOnDelete() == true)
            {
                Application.Current.Shutdown();
            }
            
        }




        /*
        * FUNCTION : RenderCanvas
        *
        * DESCRIPTION : This functions renders the canvas into a bitmap so its data can be written into an image file
        *                              
        * PARAMETERS : NONE
        *
        * RETURNS : RenderTargetBitmap - rendered canvas
        */
        private RenderTargetBitmap RenderCanvas()
        {
            Rect bounds = VisualTreeHelper.GetDescendantBounds(wrapperCanvas);
            double dpi = 96d;
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, dpi, dpi, System.Windows.Media.PixelFormats.Default);
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(wrapperCanvas);
                dc.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
            }

            rtb.Render(dv);

            return rtb;
        }



        /*
        * FUNCTION : RenderCanvas
        *
        * DESCRIPTION : This functions writes the rendered canvas data into a file
        *                              
        * PARAMETERS : RenderTargetBitmap rtb - rendered canvas data
        *              string path - file path
        *
        * RETURNS : NONE
        */
        private void WriteToFile(RenderTargetBitmap rtb, string path)
        {
            PngBitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(rtb));

            //save to memory stream
            var stm = System.IO.File.Create(path);

            pngEncoder.Save(stm);
            stm.Close();
            //System.IO.File.WriteAllBytes(filePath, ms.ToArray());
        }


        /*
        * FUNCTION : menuItem_open_Click
        *
        * DESCRIPTION : This functions shows a OpenFileDialog allowing the user to open a png file and cast it into the canvas
        *                              
        * PARAMETERS : object sender, RoutedEventArgs e
        *
        * RETURNS : NONE
        */
        private void menuItem_open_Click(object sender, RoutedEventArgs e)
        {
            // Ask for saving first
            if (PromptOnDelete() == true)
            {
                // Open File Dialog
                System.Windows.Forms.OpenFileDialog dl1 = new System.Windows.Forms.OpenFileDialog();
                dl1.DefaultExt = ".png";
                dl1.Filter = "Image documents (.png)|*.png"; ;

                if (dl1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    FilePath = dl1.FileName;

                    // Create a image brush, load data from that file
                    ImageBrush brush = new ImageBrush();
                    BitmapImage b = new BitmapImage();
                    b.BeginInit();
                    b.CacheOption = BitmapCacheOption.OnLoad;
                    b.UriSource = new Uri(FilePath);
                    b.EndInit();
                    brush.ImageSource = b;

                    // Set the canvas Width and Height accordingly
                    wrapperCanvas.Width = b.Width;
                    wrapperCanvas.Height = b.Height;

                    // Cast the image into the canvas
                    wrapperCanvas.Background = brush;
                }
            }
            
        }



        /*
        * FUNCTION : txtb_KeyDow
        *
        * DESCRIPTION : This functions loses focus from a text box when the user hit Enter inside that text box 
        *               (so the binded data can be updated)
        *                              
        * PARAMETERS : object sender, KeyEventArgs e
        *
        * RETURNS : NONE
        */
        private void txtb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                comboBox.Focus();
            }
        }



        /*
        * FUNCTION : PromptOnDelete
        *
        * DESCRIPTION : This functions prompts the user to save the current canvas 
        *               before he/she switches to something else
        *                              
        * PARAMETERS : NONE
        *
        * RETURNS : bool - true if the user wants to continue (the user hitted 'Yes' or 'No')
        *                  false if the user hitted 'Cancel'
        */
        private bool PromptOnDelete()
        {
            bool cont = true;

            if (canvasModified)
            {
                // Ask to save work
                MessageBoxResult r = MessageBox.Show("Do you want to save the changes?", "Confirmation", MessageBoxButton.YesNoCancel);

                if (r != MessageBoxResult.Cancel)
                {
                    // 'Yes' or 'No'
                    if (r == MessageBoxResult.Yes)
                    {
                        // Call save function
                        menuItem_save.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));
                    }
                }
                else
                {
                    // 'Cancel'
                    cont = false;
                }
            }


            if (cont)
            {
                // If the user wants to continue
                canvasModified = false;
                // Clear the canvas
                btn_clear.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }

            return cont;
        }



        /*
        * FUNCTION : MenuItem_Click
        *
        * DESCRIPTION : This functions shows the about box
        *                              
        * PARAMETERS : object sender, RoutedEventArgs e
        *
        * RETURNS : NONE
        */
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            AboutBox b = new AboutBox();
            b.ShowDialog();
        }
    }



    /*
    * NAME : CanvasSizeConverter (inheritted from IValueConverter)
    * PURPOSE : The width and height of the canvas is of double type and they are binded with the modifying textboxes.
    *           These textboxes will look ridiculous if there are double type values in them.
    *           This class's purpose is to convert the canvas size data between double and int
    */
    class CanvasSizeConverter : IValueConverter
    {
        /*
        * FUNCTION : Convert
        *
        * DESCRIPTION : Convert from double to int (when the textboxes need to be updated)
        *                              
        * PARAMETERS : object value - value from the Canvas's width and height property
        *              System.Type targetType, object parameter, System.Globalization.CultureInfo culture
        *
        * RETURNS : object - int converted value (for the XAML textboxes)
        */
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Math.Round((double)value);
        }



        /*
        * FUNCTION : ConvertBac
        *
        * DESCRIPTION : // Convert from int to double (when the textboxes are updating the canvas)
        *                              
        * PARAMETERS : object value - value from the XAML textboxes
        *              System.Type targetType, object parameter, System.Globalization.CultureInfo culture
        *
        * RETURNS : object - double converted value (for the canvas's height and width properties)
        */

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Double.Parse((string)value);
        }
    }

}

