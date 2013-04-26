using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Input;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Collections.ObjectModel;
using System.Diagnostics;


namespace MyScene
{
    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    /// 
    public partial class SurfaceWindow1 : SurfaceWindow
    {
        int curIndex = 0;
        List<ScatterViewItem> curClipArt = new List<ScatterViewItem>();

        //GLC - Class to simulate mouse input in order to streamline touch prototypting. Todo: Replace ContextMenu with a touch-compatible object.
        public class MouseSimulator
        {
            [DllImport("user32.dll", SetLastError = true)]
            static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);

            [StructLayout(LayoutKind.Sequential)]
            struct INPUT
            {
                public SendInputEventType type;
                public MouseKeybdhardwareInputUnion mkhi;
            }
            [StructLayout(LayoutKind.Explicit)]
            struct MouseKeybdhardwareInputUnion
            {
                [FieldOffset(0)]
                public MouseInputData mi;

                [FieldOffset(0)]
                public KEYBDINPUT ki;

                [FieldOffset(0)]
                public HARDWAREINPUT hi;
            }
            [StructLayout(LayoutKind.Sequential)]
            struct KEYBDINPUT
            {
                public ushort wVk;
                public ushort wScan;
                public uint dwFlags;
                public uint time;
                public IntPtr dwExtraInfo;
            }
            [StructLayout(LayoutKind.Sequential)]
            struct HARDWAREINPUT
            {
                public int uMsg;
                public short wParamL;
                public short wParamH;
            }
            struct MouseInputData
            {
                public int dx;
                public int dy;
                public uint mouseData;
                public MouseEventFlags dwFlags;
                public uint time;
                public IntPtr dwExtraInfo;
            }
            [Flags]
            enum MouseEventFlags : uint
            {
                MOUSEEVENTF_MOVE = 0x0001,
                MOUSEEVENTF_LEFTDOWN = 0x0002,
                MOUSEEVENTF_LEFTUP = 0x0004,
                MOUSEEVENTF_RIGHTDOWN = 0x0008,
                MOUSEEVENTF_RIGHTUP = 0x0010,
                MOUSEEVENTF_MIDDLEDOWN = 0x0020,
                MOUSEEVENTF_MIDDLEUP = 0x0040,
                MOUSEEVENTF_XDOWN = 0x0080,
                MOUSEEVENTF_XUP = 0x0100,
                MOUSEEVENTF_WHEEL = 0x0800,
                MOUSEEVENTF_VIRTUALDESK = 0x4000,
                MOUSEEVENTF_ABSOLUTE = 0x8000
            }
            enum SendInputEventType : int
            {
                InputMouse,
                InputKeyboard,
                InputHardware
            }

            public static void ClickRightMouseButton()
            {
                INPUT mouseDownInput = new INPUT();
                mouseDownInput.type = SendInputEventType.InputMouse;
                mouseDownInput.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_RIGHTDOWN;
                SendInput(1, ref mouseDownInput, Marshal.SizeOf(new INPUT()));

                INPUT mouseUpInput = new INPUT();
                mouseUpInput.type = SendInputEventType.InputMouse;
                mouseUpInput.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_RIGHTUP;
                SendInput(1, ref mouseUpInput, Marshal.SizeOf(new INPUT()));
            }
        }
        /// <summary>
        /// Default constructor.
        /// </summary>
//#region OnInitialized


public SurfaceWindow1()
        {

            InitializeComponent();

            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();

            SurfaceButton mybtn2 = new SurfaceButton();
            mybtn2.Content = "Clear Screen";
            // mybtn2.Command = clearScreen;
            MainLibraryBar.Items.Add(Directory.GetCurrentDirectory() + @"\..\..\Resources\Images\clearscreen.png");
            scatterView.ContextMenu.Items.Add(mybtn2);

            // CJT Add all Background Images to the default Background Selection Box
            //var mystring = MyScene.App.Current.Resources.ToString();
            //ResourceAssembly.Location.ToString();
              ObservableCollection<string> items = new ObservableCollection<string>();
              string[] BackgroundFileNames = Directory.GetFiles(Directory.GetCurrentDirectory()+@"\..\..\Resources\Images\Backgrounds\", "*.png");
              //var mystring = Directory.GetCurrentDirectory();
              for (int i=0; i < BackgroundFileNames.Length; i++) 
              {
                  MainLibraryBar.Items.Add(BackgroundFileNames[i]);
                  SurfaceButton mybtn = new SurfaceButton();
                  Image myimg = new Image();
                  myimg.Source = new BitmapImage(new Uri(BackgroundFileNames[i], UriKind.RelativeOrAbsolute));
                  //myimg.Width = 100;
                  //myimg.Height = 75;
                  myimg.Stretch = System.Windows.Media.Stretch.Fill;

                  mybtn.Content = myimg;
                  mybtn.CommandParameter = BackgroundFileNames[i];
                  mybtn.Click += new RoutedEventHandler(this.changeBackground);
                  // add to context menu
                  scatterView.ContextMenu.Items.Add(mybtn);
                  //BGMenu.Items.Add(myimg);
                  // add to scatterviewitem as temporary fix
                 // BackgroundList.Items.Add(BackgroundFileNames[i]);
              }
              
           
            
            // CJT Add all the ClipArt Images to the default ClipArt Selection Box
            string[] curDir = Directory.GetDirectories(Directory.GetCurrentDirectory() + @"\..\..\Resources\Images\ClipArt\");
            string[] ClipArtFileNames = null;
            if (curDir != null && curDir.Length != 0)
            {
                for (int i = 0; i < curDir.Length; i++)
                {

                    ClipArtFileNames = Directory.GetFiles(curDir[i]+@"\", "*.png");
                    for (int j = 0; j < ClipArtFileNames.Length; j++)
                    {
                        //ClipArtList.Items.Add(ClipArtFileNames[j]);
                        ClipArtList1.Items.Add(ClipArtFileNames[j]);
                        ClipArtList2.Items.Add(ClipArtFileNames[j]);
                        ClipArtList3.Items.Add(ClipArtFileNames[j]);
                        ClipArtList4.Items.Add(ClipArtFileNames[j]);
                        ClipArtList5.Items.Add(ClipArtFileNames[j]);
                        ClipArtList6.Items.Add(ClipArtFileNames[j]);
                    }
                }
            }
          /*  ClipArtFileNames = Directory.GetFiles(Directory.GetCurrentDirectory() + @"\..\..\Resources\Images\ClipArt\", "*.png");
            for (int k = 0; k < ClipArtFileNames.Length; k++)
            {
                ClipArtList.Items.Add(ClipArtFileNames[k]);
                
            }*/
            // above not needed since adding as resources took care of moving all the files out of the main directory and now makes everything a dupe

            BGMenu.Visibility = Visibility.Hidden;
            Menu.Center = new Point(-500, -500);

        }

        /// <summary>
        /// Occurs when the window is about to close. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Remove handlers for window availability events
            RemoveWindowAvailabilityHandlers();
        }

        // CJT Added for changing the background image after clicking it on the menu
         void changeBackground(object sender, RoutedEventArgs e) // check what the params are for a menu click!!
        {
            // find the path information for the chosen object
             SurfaceButton findSourceA = (SurfaceButton)sender;
             SurfaceButton findSourceB = (SurfaceButton)findSourceA.Content;

             if (findSourceB.Content is Image) { // is a background image
                Image myimg = (Image)((SurfaceButton)findSourceA.Content).Content;
          
                string ImagePath = myimg.Source.ToString();
                // create new image object for the dragged object
                ImageBrush img = new ImageBrush();
                img.ImageSource = new BitmapImage(new Uri(ImagePath, UriKind.RelativeOrAbsolute));
                img.Stretch = System.Windows.Media.Stretch.Fill;
                // overwrite the Main Window with this new image
                scatterView.Items.Remove(scatterView.Background);
                scatterView.Background = img;
                // do with LibraryBar.SetIsItemDataEnabled(whichitem, true); -- see http://social.msdn.microsoft.com/Forums/en-US/surfaceappdevelopment/thread/8d341171-8ae9-4ccc-8e5c-84a3aa8a4d29/
            }
            else // is the clear screen item
            {
                clearScreen();
            }
        }

        // CJT Added for clearing the screen
         void clearScreen() // check the params for the menu click
        {
            while (curClipArt.Count > 0)
            {
                scatterView.Items.Remove(curClipArt[0]);
                curClipArt.RemoveAt(0);
            }
        }

        // CJT Added for swapping the background images on the main window
        private void scatterView_Drop(object sender, SurfaceDragDropEventArgs e)
        {
            // find the path information for the dragged object
            FrameworkElement findSource = (e.Cursor.Visual) as FrameworkElement;
            Image draggedElement = null;
            while (draggedElement == null && findSource != null)
            {
                if ((draggedElement = findSource as Image) == null)
                {
                    findSource = VisualTreeHelper.GetChild(findSource, 0) as FrameworkElement;
                }
            }

            if (draggedElement == null)
            {
                return;
            }

            string ImagePath = draggedElement.Source.ToString();
            if (ImagePath.IndexOf("Backgrounds") > 0) // shouldn't hit this anymore, but kept in case wanted it later
            {
                // create new image object for the dragged object
                ImageBrush img = new ImageBrush();
                img.ImageSource = new BitmapImage(new Uri(ImagePath, UriKind.RelativeOrAbsolute));
                img.Stretch = System.Windows.Media.Stretch.UniformToFill;
                // overwrite the Main Window with this new image
                scatterView.Items.Remove(scatterView.Background);
                scatterView.Background = img;
                MainLibraryBar.SetIsItemDataEnabled(e.Cursor.Data, true);
                BGMenu.Visibility = Visibility.Hidden;
                Menu.Center = new Point(-500, -500);
                // do with LibraryBar.SetIsItemDataEnabled(whichitem, true); -- see http://social.msdn.microsoft.com/Forums/en-US/surfaceappdevelopment/thread/8d341171-8ae9-4ccc-8e5c-84a3aa8a4d29/
            }
            else if (ImagePath.IndexOf("clearscreen") > 0)
            {
                MainLibraryBar.SetIsItemDataEnabled(e.Cursor.Data, true);
                BGMenu.Visibility = Visibility.Hidden;
                Menu.Center = new Point(-500, -500);
                clearScreen();

            } 
            else 
            {
                // do logic for dropping clipart items!!!
                Image caimg = new Image();
                caimg.Source = new BitmapImage(new Uri(ImagePath, UriKind.RelativeOrAbsolute));
                caimg.Stretch = System.Windows.Media.Stretch.Fill;
                //caimg.Parent = MyMainWin;
                //caimg.Tag = "ClipArtDraggedItem";

                ScatterViewItem casvi = new ScatterViewItem();
                casvi.Background = System.Windows.Media.Brushes.Transparent;
                casvi.MaxHeight = 1420;
                casvi.MaxWidth = 580;
                casvi.Height = caimg.Source.Height / 10;
                casvi.Width = caimg.Source.Width / 10;
                casvi.MinHeight = 60;
                casvi.MinWidth = 60;
                //casvi.Parent = MyMainWin;
                casvi.Tag = "ClipArtDraggedItem";
                casvi.Content = caimg;
                //casvi.BorderThickness = System.Windows.Thickness(0,0,0,0);
                casvi.CanMove = true;
                casvi.CanRotate = true;
                casvi.CanScale = true;
                casvi.Name = "DraggedClipArt" + curIndex;
                casvi.Center = e.Cursor.GetPosition(scatterView);
                //casvi.AddHandler(
                 //   SurfaceDragDrop.AddDragCompletedHandler(casvi , scatterView_DragLeave);
                
                //casvi.Orientation = e.Cursor.GetOrientation(ClipArtList); // need to find which window dragged this object


                // Add to global tracker of clipart spawned, then add to screen
                curClipArt.Add(casvi);
                scatterView.Items.Add(casvi);
                ClipArtList1.SetIsItemDataEnabled(e.Cursor.Data, true);
                ClipArtList2.SetIsItemDataEnabled(e.Cursor.Data, true);
                ClipArtList3.SetIsItemDataEnabled(e.Cursor.Data, true);
                ClipArtList4.SetIsItemDataEnabled(e.Cursor.Data, true); 
                ClipArtList5.SetIsItemDataEnabled(e.Cursor.Data, true);
                ClipArtList6.SetIsItemDataEnabled(e.Cursor.Data, true);

            }
            // CJT this is needed to re-enable the item in the library!!
            //BackgroundList.SetIsItemDataEnabled(e.Cursor.Data, true);
            //ClipArtList.SetIsItemDataEnabled(e.Cursor.Data, true); // need to do for all current clipart bars that are open OR look for which one dragged it here
        }



        /// <summary>
        /// Adds handlers for window availability events.
        /// </summary>
        private void AddWindowAvailabilityHandlers()
        {
            // Subscribe to surface window availability events
            ApplicationServices.WindowInteractive += OnWindowInteractive;
            ApplicationServices.WindowNoninteractive += OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable += OnWindowUnavailable;
        }

        /// <summary>
        /// Removes handlers for window availability events.
        /// </summary>
        private void RemoveWindowAvailabilityHandlers()
        {
            // Unsubscribe from surface window availability events
            ApplicationServices.WindowInteractive -= OnWindowInteractive;
            ApplicationServices.WindowNoninteractive -= OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable -= OnWindowUnavailable;
        }

        /// <summary>
        /// This is called when the user can interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowInteractive(object sender, EventArgs e)
        {
            //TODO: enable audio, animations here
        }

        /// <summary>
        /// This is called when the user can see but not interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowNoninteractive(object sender, EventArgs e)
        {
            //TODO: Disable audio here if it is enabled

            //TODO: optionally enable animations here
        }

        /// <summary>
        /// This is called when the application's window is not visible or interactive.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowUnavailable(object sender, EventArgs e)
        {
            //TODO: disable audio, animations here
        }

        System.Windows.Controls.ContentPresenter FindContentPresenter(DependencyObject obj)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is System.Windows.Controls.ContentPresenter)
                {
                    return (System.Windows.Controls.ContentPresenter)child;
                }
                else
                {
                    System.Windows.Controls.ContentPresenter childOfChild =
                        FindContentPresenter(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        // CJT created for removing the item when dropped on the clipart window -- needs some more work
        private void ClipArtListView_Drop(object sender, SurfaceDragDropEventArgs e)
        {
            // find the path information for the dragged object
        /*    FrameworkElement findSource = (e.Cursor.Visual) as FrameworkElement;
            ScatterViewItem draggedElement = null;
            while (draggedElement == null && findSource != null)
            {
                if ((draggedElement = findSource as ScatterViewItem) == null)
                {
                    findSource = VisualTreeHelper.GetChild(findSource, 0) as FrameworkElement;
                }
            }

            if (draggedElement == null)
            {
                return;
            }

            //string ImagePath = draggedElement.Source.ToString();
            scatterView.Items.Remove(draggedElement); // remove from screen
            curClipArt.Remove(draggedElement); // remove from list*/
        }

        private void c_previewTouchDown(object sender, SurfaceDragDropEventArgs e)
        {
            //
        }

        //GLC - Added "BGMenu" and touch functionality to the background menu.
        private void scatterView_TouchDown(object sender, TouchEventArgs e)
        {
            //if (BGMenu.Visibility == Visibility.Visible)
            //{
            //    //The menu is already up. Hide it.
            //    BGMenu.Visibility = Visibility.Hidden;
            //    Menu.Center = new Point(-500, -500);
            //}
            //else //Show the menu
            //{
            //    BGMenu.Visibility = Visibility.Visible;
            //    Menu.Center = new Point(960, 540);
            //}

            ////Mouse Prototyping - no longer used.
            ///* Bind this event to a click event for the context menu.
            //Artificially trigger a right-click with touch.
            //MouseSimulator.ClickRightMouseButton(); */
        }

        //GLC - Added touch functionality to the scatterview item "menu," which contains BGMenu.
        private void Menu_TouchDown(object sender, TouchEventArgs e)
        {
            //if (BGMenu.Visibility == Visibility.Hidden)
            //{
            //    e.Handled = false;
            //}
            //else
            //{
            //    //Do nothing
            //}
        }

        //GLC - Implemented the hold gesture for showing and hiding the menu to change backgrounds.
        private void scatterView_HoldGesture(object sender, TouchEventArgs e)
        {
            if (doIt)
            {
                if (BGMenu.Visibility == Visibility.Visible)
                {
                    //The menu is already up. Hide it.
                    BGMenu.Visibility = Visibility.Hidden;
                    Menu.Center = new Point(-500, -500);
                }
                else //Show the menu
                {
                    BGMenu.Visibility = Visibility.Visible;
                    Menu.Center = new Point(960, 540);
                }
                doIt = false;
            }

            //Mouse Prototyping - no longer used.
            /* Bind this event to a click event for the context menu.
            Artificially trigger a right-click with touch.
            MouseSimulator.ClickRightMouseButton(); */
        }
        //GLC - Menu hold gesture event handler
        private void Menu_HoldGesture(object sender, TouchEventArgs e)
        {
            if (BGMenu.Visibility == Visibility.Hidden)
            {
                e.Handled = false;
            }
            else
            {
                //Do nothing
            }
        }

        private void ScatterView_PreviewTapGesture(object sender, TouchEventArgs e)
        {

            if (IsDoubleTap(e))
                doIt = true;
            scatterView_HoldGesture(sender, e);
        }

        private readonly Stopwatch _doubleTapStopwatch = new Stopwatch();
        private Point _lastTapLocation;
        private bool doIt = false;

        private bool IsDoubleTap(TouchEventArgs e)
        {
            Point currentTapPosition = e.GetTouchPoint(this).Position;
            bool tapsAreCloseInDistance = System.Windows.Point.Subtract(currentTapPosition, _lastTapLocation).Length < 40;
            _lastTapLocation = currentTapPosition;

            TimeSpan elapsed = _doubleTapStopwatch.Elapsed;
            _doubleTapStopwatch.Restart();
            bool tapsAreCloseInTime = (elapsed != TimeSpan.Zero && elapsed < TimeSpan.FromSeconds(0.7));

            return tapsAreCloseInDistance && tapsAreCloseInTime;
        }

        private void scatterView_DragLeave(object sender, TargetChangedEventArgs e)
        {
    
            // If the operation is Move, remove the data from drag source.
            if ( (e.Cursor.GetPosition(scatterView).X >= 1900 || e.Cursor.GetPosition(scatterView).X <= 100 || e.Cursor.GetPosition(scatterView).Y >=895 || e.Cursor.GetPosition(scatterView).Y <=100))
            {
                FrameworkElement findSource = (e.Cursor.Visual) as FrameworkElement;
                ScatterViewItem draggedElement = null;
                while (draggedElement == null && findSource != null)
                {
                    if ((draggedElement = findSource as ScatterViewItem) == null)
                    {
                        findSource = VisualTreeHelper.GetChild(findSource, 0) as FrameworkElement;
                    }
                }

                if (draggedElement == null)
                {
                    return;
                }

                //string ImagePath = draggedElement.Source.ToString();
                scatterView.Items.Remove(draggedElement); // remove from screen
                curClipArt.Remove(draggedElement);
                
            }


        }

        private void scatterView_TargetChanged(object sender, TargetChangedEventArgs e)
        {

        }


        
        
    }
}