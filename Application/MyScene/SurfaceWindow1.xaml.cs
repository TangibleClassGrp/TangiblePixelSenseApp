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

namespace MyScene
{
    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    public partial class SurfaceWindow1 : SurfaceWindow
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public SurfaceWindow1()
        {
            InitializeComponent();

            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();

            // CJT Add all Background Images to the default Background Selection Box
            string[] BackgroundFileNames = Directory.GetFiles(@"..\..\Resources\Images\Backgrounds\", "*.jpg");
            for (int i=0; i < BackgroundFileNames.Length; i++) 
            {
               BackgroundList.Items.Add(BackgroundFileNames[i]);
            }
            // CJT should add a LibraryBar.SetIsItemDataEnabled(whichitem, false); to disable what is currently on the main window

            // CJT Add all the ClipArt Images to the default ClipArt Selection Box
            string[] ClipArtFileNames = Directory.GetFiles(@"..\..\Resources\Images\ClipArt\", "*.png");
            for (int j = 0; j < ClipArtFileNames.Length; j++)
            {
                ClipArtList.Items.Add(ClipArtFileNames[j]);
            }

            

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
            // create new image object for the dragged object
            Image img = new Image();
            img.Source = new BitmapImage(new Uri(ImagePath, UriKind.RelativeOrAbsolute));
            img.Stretch = System.Windows.Media.Stretch.UniformToFill;
            // overwrite the Main Window with this new image
            MyMainWin.Content = img;
            // be sure the dragged item is re-enabled in the Library
            BackgroundList.SetIsItemDataEnabled(e.Cursor.Data, true);
            // need to return existing item to the LibraryBar or do cleanup of MyMainWin to prevent extra images hanging out there
            // do with LibraryBar.SetIsItemDataEnabled(whichitem, true); -- see http://social.msdn.microsoft.com/Forums/en-US/surfaceappdevelopment/thread/8d341171-8ae9-4ccc-8e5c-84a3aa8a4d29/

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

        #region Sidebar Code
        void TopButton1Clicked(object sender, RoutedEventArgs e)
        {
            ClipArtListView.Visibility = System.Windows.Visibility.Visible;
            ClipArtListView.Center = new Point(900, 300);
            ClipArtListView.SetRelativeZIndex(RelativeScatterViewZIndex.Topmost);
        }

        
        void BottomButton1Clicked(object sender, RoutedEventArgs e)
        {
            ClipArtListView.Visibility = System.Windows.Visibility.Visible;
            ClipArtListView.Center = new Point(1100, 1700);
            ClipArtListView.SetRelativeZIndex(RelativeScatterViewZIndex.Topmost);
        }

        
        #endregion Sidebar Code

    }
}