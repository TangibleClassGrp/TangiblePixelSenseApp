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
using System.Collections.ObjectModel;
using System.IO;

namespace Drag_and_Drop
{
    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    public partial class SurfaceWindow1 : SurfaceWindow
    {
        #region Collections
        private ObservableCollection<PhotoData> libraryItems;
        private ObservableCollection<PhotoData> scatterItems;

        public ObservableCollection<PhotoData> LibraryItems
        {
            get
            {
                if (libraryItems == null)
                {
                    libraryItems = new ObservableCollection<PhotoData>();
                }

                return libraryItems;
            }
        }

        public ObservableCollection<PhotoData> ScatterItems
        {
            get
            {
                if (scatterItems == null)
                {
                    scatterItems = new ObservableCollection<PhotoData>();
                }

                return scatterItems;
            }
        }
        #endregion

        List<PhotoData> curClipArt = new List<PhotoData>();
        long nextID = 0;
        /// <summary>
        /// Default constructor.
        /// </summary>
        public SurfaceWindow1()
        {
            this.DataContext = this;

            InitializeComponent();

            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            DataContext = this;

            // CJT Add all the ClipArt Images to the default ClipArt Selection Box
            string[] curDir = Directory.GetDirectories(Directory.GetCurrentDirectory() + @"\..\..\Resources\Images\ClipArt\");
            string[] ClipArtFileNames = null;
            if (curDir != null && curDir.Length != 0)
            {
                for (int i = 0; i < curDir.Length; i++)
                {

                    ClipArtFileNames = Directory.GetFiles(curDir[i] + @"\", "*.png");
                    for (int j = 0; j < ClipArtFileNames.Length; j++)
                    {
                        LibraryItems.Add(new PhotoData(ClipArtFileNames[j], curDir[i], -1));
                    }
                }
            }
            /*LibraryItems.Add(new PhotoData("Images/Chrysanthemum.jpg", "Chrysanthemum", -1));
            LibraryItems.Add(new PhotoData("Images/Desert.jpg", "Desert", -1));
            LibraryItems.Add(new PhotoData("Images/Hydrangeas.jpg", "Hydrangeas", -1));
            LibraryItems.Add(new PhotoData("Images/Jellyfish.jpg", "Jellyfish", -1));
            LibraryItems.Add(new PhotoData("Images/Koala.jpg", "Koala", -1));
            LibraryItems.Add(new PhotoData("Images/Lighthouse.jpg", "Lighthouse", -1));
            LibraryItems.Add(new PhotoData("Images/Penguins.jpg", "Penguins", -1));
            LibraryItems.Add(new PhotoData("Images/Tulips.jpg", "Tulips", -1));*/
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

        private void Scatter_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            FrameworkElement findSource = e.OriginalSource as FrameworkElement;
            ScatterViewItem draggedElement = null;

            // Find the ScatterViewitem object that is being touched.
            while (draggedElement == null && findSource != null)
            {
                if ((draggedElement = findSource as ScatterViewItem) == null)
                {
                    findSource = VisualTreeHelper.GetParent(findSource) as FrameworkElement;
                }
            }

            if (draggedElement == null)
            {
                return;
            }

            PhotoData datatemp = draggedElement.Content as PhotoData;
            PhotoData data = new PhotoData(datatemp.Source, datatemp.Caption, nextID);
            //data.Identifier = nextID;
            nextID++;

            // Create the cursor visual
            ContentControl cursorVisual = new ContentControl()
            {
                Content = draggedElement.DataContext,
                Style = FindResource("CursorStyle") as Style
            };

            // Create a list of input devices. Add the touches that
            // are currently captured within the dragged element and
            // the current touch (if it isn't already in the list).
            List<InputDevice> devices = new List<InputDevice>();
            devices.Add(e.TouchDevice);
            foreach (TouchDevice touch in draggedElement.TouchesCapturedWithin)
            {
                if (touch != e.TouchDevice)
                {
                    devices.Add(touch);
                }
            }

            // Get the drag source object
            ItemsControl dragSource = ItemsControl.ItemsControlFromItemContainer(draggedElement);

            SurfaceDragDrop.BeginDragDrop(
                dragSource,                     // The ScatterView object that the cursor is dragged out from.
                draggedElement,                 // The ScatterViewItem object that is dragged from the drag source.
                cursorVisual,                   // The visual element of the cursor.
                draggedElement.DataContext,     // The data attached with the cursor.
                devices,                        // The input devices that start dragging the cursor.
                DragDropEffects.Move);          // The allowed drag-and-drop effects of this operation.

            // Prevents the default touch behavior from happening and disrupting our code.
            e.Handled = true;

            // Hide the ScatterViewItem for now. We will remove it if the DragDrop is successful.
            draggedElement.Visibility = Visibility.Hidden;
        }

        private void Scatter_DragCanceled(object sender, SurfaceDragDropEventArgs e)
        {
            PhotoData data = e.Cursor.Data as PhotoData;
            ScatterViewItem svi = scatter.ItemContainerGenerator.ContainerFromItem(data) as ScatterViewItem;
            if (svi != null)
            {
                svi.Visibility = Visibility.Visible;
            }
        }

        private void Scatter_DragCompleted(object sender, SurfaceDragCompletedEventArgs e)
        {
            if (e.Cursor.CurrentTarget != scatter && e.Cursor.Effects == DragDropEffects.Move)
            {
                ScatterItems.Remove(e.Cursor.Data as PhotoData);
                e.Handled = true;
            }
            curClipArt.Add(e.Cursor.Data as PhotoData);
        }

        private void Scatter_Drop(object sender, SurfaceDragDropEventArgs e)
        {
            // If it isn't already on the ScatterView, add it to the source collection.
           // if (!ScatterItems.Contains(e.Cursor.Data))
           // {
                ScatterItems.Add((PhotoData)e.Cursor.Data);
           // }

            // Get the ScatterViewItem that Scatter automatically generated.
            ScatterViewItem svi =
                scatter.ItemContainerGenerator.ContainerFromItem(e.Cursor.Data) as ScatterViewItem;
            svi.Visibility = System.Windows.Visibility.Visible;
            svi.Width = e.Cursor.Visual.ActualWidth;
            svi.Height = e.Cursor.Visual.ActualHeight;
            svi.Center = e.Cursor.GetPosition(scatter);
            svi.Orientation = e.Cursor.GetOrientation(scatter);
            svi.Background = Brushes.Transparent;
            // Setting e.Handle to true ensures that default behavior is not performed.
            e.Handled = true;
        }

        //=============================
        private void ListBox_PreviewTouchDown0(object sender, TouchEventArgs e)
        {
            FrameworkElement findSource = e.OriginalSource as FrameworkElement;
            SurfaceListBoxItem draggedElement = null;

            // Find the SurfaceListBoxItem object that is being touched.
            while (draggedElement == null && findSource != null)
            {
                if ((draggedElement = findSource as SurfaceListBoxItem) == null)
                {
                    findSource = VisualTreeHelper.GetParent(findSource) as FrameworkElement;
                }
            }

            if (draggedElement == null)
            {
                return;
            }

            PhotoData data = draggedElement.Content as PhotoData;

            // Create the cursor visual
            ContentControl cursorVisual = new ContentControl()
            {
                Content = draggedElement.DataContext,
                Style = FindResource("CursorStyle") as Style
            };

            // Create a list of input devices. Add the touches that
            // are currently captured within the dragged element and
            // the current touch (if it isn't already in the list).
            List<InputDevice> devices = new List<InputDevice>();
            devices.Add(e.TouchDevice);
            foreach (TouchDevice touch in draggedElement.TouchesCapturedWithin)
            {
                if (touch != e.TouchDevice)
                {
                    devices.Add(touch);
                }
            }

            // Get the drag source object
            ItemsControl dragSource = ItemsControl.ItemsControlFromItemContainer(draggedElement);

            SurfaceDragDrop.BeginDragDrop(
                dragSource,
                draggedElement,
                cursorVisual,
                draggedElement.DataContext,
                devices,
                DragDropEffects.Move);

            // Prevents the default touch behavior from happening and disrupting our code.
            e.Handled = true;

            // Gray out the SurfaceListBoxItem for now. We will remove it if the DragDrop is successful.
            draggedElement.Opacity = 0.5;
        }

        private void ListBox_DragCanceled0(object sender, SurfaceDragDropEventArgs e)
        {
            PhotoData data = e.Cursor.Data as PhotoData;
            SurfaceListBoxItem boxItem = ListBox0.ItemContainerGenerator.ContainerFromItem(data) as SurfaceListBoxItem;
            if (boxItem != null)
            {
                boxItem.Opacity = 1.0;
            }
        }

        private void ListBox_DragCompleted0(object sender, SurfaceDragCompletedEventArgs e)
        {
            if (e.Cursor.CurrentTarget != ListBox0 && e.Cursor.Effects == DragDropEffects.Move)
            {
                //LibraryItems.Remove(e.Cursor.Data as PhotoData);
                SurfaceListBoxItem boxItem = ListBox0.ItemContainerGenerator.ContainerFromItem(e.Cursor.Data as PhotoData) as SurfaceListBoxItem;
                boxItem.Opacity = 1.0;
                e.Handled = true;
            }
            curClipArt.Remove(e.Cursor.Data as PhotoData);
        }

        private void ListBox_Drop0(object sender, SurfaceDragDropEventArgs e)
        {
            // If it isn't already on the ListBox, add it to the source collection.
            if (!LibraryItems.Contains(e.Cursor.Data))
            {
                //LibraryItems.Add((PhotoData)e.Cursor.Data);
            }

            // Get the SurfaceListBoxItem that ListBox automatically generated.
            SurfaceListBoxItem boxItem =
                ListBox0.ItemContainerGenerator.ContainerFromItem(e.Cursor.Data) as SurfaceListBoxItem;
            boxItem.Visibility = System.Windows.Visibility.Visible;
            // Setting e.Handle to true ensures that default behavior is not performed.
            e.Handled = true;
        }

        //===========
        private void ListBox_PreviewTouchDown1(object sender, TouchEventArgs e)
        {
            FrameworkElement findSource = e.OriginalSource as FrameworkElement;
            SurfaceListBoxItem draggedElement = null;

            // Find the SurfaceListBoxItem object that is being touched.
            while (draggedElement == null && findSource != null)
            {
                if ((draggedElement = findSource as SurfaceListBoxItem) == null)
                {
                    findSource = VisualTreeHelper.GetParent(findSource) as FrameworkElement;
                }
            }

            if (draggedElement == null)
            {
                return;
            }

            PhotoData data = draggedElement.Content as PhotoData;

            // Create the cursor visual
            ContentControl cursorVisual = new ContentControl()
            {
                Content = draggedElement.DataContext,
                Style = FindResource("CursorStyle") as Style
            };

            // Create a list of input devices. Add the touches that
            // are currently captured within the dragged element and
            // the current touch (if it isn't already in the list).
            List<InputDevice> devices = new List<InputDevice>();
            devices.Add(e.TouchDevice);
            foreach (TouchDevice touch in draggedElement.TouchesCapturedWithin)
            {
                if (touch != e.TouchDevice)
                {
                    devices.Add(touch);
                }
            }

            // Get the drag source object
            ItemsControl dragSource = ItemsControl.ItemsControlFromItemContainer(draggedElement);

            SurfaceDragDrop.BeginDragDrop(
                dragSource,
                draggedElement,
                cursorVisual,
                draggedElement.DataContext,
                devices,
                DragDropEffects.Move);

            // Prevents the default touch behavior from happening and disrupting our code.
            e.Handled = true;

            // Gray out the SurfaceListBoxItem for now. We will remove it if the DragDrop is successful.
            draggedElement.Opacity = 0.5;
        }

        private void ListBox_DragCanceled1(object sender, SurfaceDragDropEventArgs e)
        {
            PhotoData data = e.Cursor.Data as PhotoData;
            SurfaceListBoxItem boxItem = ListBox1.ItemContainerGenerator.ContainerFromItem(data) as SurfaceListBoxItem;
            if (boxItem != null)
            {
                boxItem.Opacity = 1.0;
            }
        }

        private void ListBox_DragCompleted1(object sender, SurfaceDragCompletedEventArgs e)
        {
            if (e.Cursor.CurrentTarget != ListBox1 && e.Cursor.Effects == DragDropEffects.Move)
            {
                //LibraryItems.Remove(e.Cursor.Data as PhotoData);
                SurfaceListBoxItem boxItem = ListBox1.ItemContainerGenerator.ContainerFromItem(e.Cursor.Data as PhotoData) as SurfaceListBoxItem;
                boxItem.Opacity = 1.0;
                e.Handled = true;
            }
            curClipArt.Remove(e.Cursor.Data as PhotoData);
        }

        private void ListBox_Drop1(object sender, SurfaceDragDropEventArgs e)
        {
            // If it isn't already on the ListBox, add it to the source collection.
            if (!LibraryItems.Contains(e.Cursor.Data))
            {
                //LibraryItems.Add((PhotoData)e.Cursor.Data);
            }

            // Get the SurfaceListBoxItem that ListBox automatically generated.
            SurfaceListBoxItem boxItem =
                ListBox1.ItemContainerGenerator.ContainerFromItem(e.Cursor.Data) as SurfaceListBoxItem;
            boxItem.Visibility = System.Windows.Visibility.Visible;
            // Setting e.Handle to true ensures that default behavior is not performed.
            e.Handled = true;
        }

        //=============================
        private void ListBox_PreviewTouchDown2(object sender, TouchEventArgs e)
        {
            FrameworkElement findSource = e.OriginalSource as FrameworkElement;
            SurfaceListBoxItem draggedElement = null;

            // Find the SurfaceListBoxItem object that is being touched.
            while (draggedElement == null && findSource != null)
            {
                if ((draggedElement = findSource as SurfaceListBoxItem) == null)
                {
                    findSource = VisualTreeHelper.GetParent(findSource) as FrameworkElement;
                }
            }

            if (draggedElement == null)
            {
                return;
            }

            PhotoData data = draggedElement.Content as PhotoData;

            // Create the cursor visual
            ContentControl cursorVisual = new ContentControl()
            {
                Content = draggedElement.DataContext,
                Style = FindResource("CursorStyle") as Style
            };

            // Create a list of input devices. Add the touches that
            // are currently captured within the dragged element and
            // the current touch (if it isn't already in the list).
            List<InputDevice> devices = new List<InputDevice>();
            devices.Add(e.TouchDevice);
            foreach (TouchDevice touch in draggedElement.TouchesCapturedWithin)
            {
                if (touch != e.TouchDevice)
                {
                    devices.Add(touch);
                }
            }

            // Get the drag source object
            ItemsControl dragSource = ItemsControl.ItemsControlFromItemContainer(draggedElement);

            SurfaceDragDrop.BeginDragDrop(
                dragSource,
                draggedElement,
                cursorVisual,
                draggedElement.DataContext,
                devices,
                DragDropEffects.Move);

            // Prevents the default touch behavior from happening and disrupting our code.
            e.Handled = true;

            // Gray out the SurfaceListBoxItem for now. We will remove it if the DragDrop is successful.
            draggedElement.Opacity = 0.5;
        }

        private void ListBox_DragCanceled2(object sender, SurfaceDragDropEventArgs e)
        {
            PhotoData data = e.Cursor.Data as PhotoData;
            SurfaceListBoxItem boxItem = ListBox2.ItemContainerGenerator.ContainerFromItem(data) as SurfaceListBoxItem;
            if (boxItem != null)
            {
                boxItem.Opacity = 1.0;
            }
        }

        private void ListBox_DragCompleted2(object sender, SurfaceDragCompletedEventArgs e)
        {
            if (e.Cursor.CurrentTarget != ListBox2 && e.Cursor.Effects == DragDropEffects.Move)
            {
                //LibraryItems.Remove(e.Cursor.Data as PhotoData);
                SurfaceListBoxItem boxItem = ListBox2.ItemContainerGenerator.ContainerFromItem(e.Cursor.Data as PhotoData) as SurfaceListBoxItem;
                boxItem.Opacity = 1.0;
                e.Handled = true;
            }
            curClipArt.Remove(e.Cursor.Data as PhotoData);
        }

        private void ListBox_Drop2(object sender, SurfaceDragDropEventArgs e)
        {
            // If it isn't already on the ListBox, add it to the source collection.
            if (!LibraryItems.Contains(e.Cursor.Data))
            {
                //LibraryItems.Add((PhotoData)e.Cursor.Data);
            }

            // Get the SurfaceListBoxItem that ListBox automatically generated.
            SurfaceListBoxItem boxItem =
                ListBox2.ItemContainerGenerator.ContainerFromItem(e.Cursor.Data) as SurfaceListBoxItem;
            boxItem.Visibility = System.Windows.Visibility.Visible;
            // Setting e.Handle to true ensures that default behavior is not performed.
            e.Handled = true;
        }

        //=============================
        private void ListBox_PreviewTouchDown3(object sender, TouchEventArgs e)
        {
            FrameworkElement findSource = e.OriginalSource as FrameworkElement;
            SurfaceListBoxItem draggedElement = null;

            // Find the SurfaceListBoxItem object that is being touched.
            while (draggedElement == null && findSource != null)
            {
                if ((draggedElement = findSource as SurfaceListBoxItem) == null)
                {
                    findSource = VisualTreeHelper.GetParent(findSource) as FrameworkElement;
                }
            }

            if (draggedElement == null)
            {
                return;
            }

            PhotoData data = draggedElement.Content as PhotoData;

            // Create the cursor visual
            ContentControl cursorVisual = new ContentControl()
            {
                Content = draggedElement.DataContext,
                Style = FindResource("CursorStyle") as Style
            };

            // Create a list of input devices. Add the touches that
            // are currently captured within the dragged element and
            // the current touch (if it isn't already in the list).
            List<InputDevice> devices = new List<InputDevice>();
            devices.Add(e.TouchDevice);
            foreach (TouchDevice touch in draggedElement.TouchesCapturedWithin)
            {
                if (touch != e.TouchDevice)
                {
                    devices.Add(touch);
                }
            }

            // Get the drag source object
            ItemsControl dragSource = ItemsControl.ItemsControlFromItemContainer(draggedElement);

            SurfaceDragDrop.BeginDragDrop(
                dragSource,
                draggedElement,
                cursorVisual,
                draggedElement.DataContext,
                devices,
                DragDropEffects.Move);

            // Prevents the default touch behavior from happening and disrupting our code.
            e.Handled = true;

            // Gray out the SurfaceListBoxItem for now. We will remove it if the DragDrop is successful.
            draggedElement.Opacity = 0.5;
        }

        private void ListBox_DragCanceled3(object sender, SurfaceDragDropEventArgs e)
        {
            PhotoData data = e.Cursor.Data as PhotoData;
            SurfaceListBoxItem boxItem = ListBox3.ItemContainerGenerator.ContainerFromItem(data) as SurfaceListBoxItem;
            if (boxItem != null)
            {
                boxItem.Opacity = 1.0;
            }
        }

        private void ListBox_DragCompleted3(object sender, SurfaceDragCompletedEventArgs e)
        {
            if (e.Cursor.CurrentTarget != ListBox3 && e.Cursor.Effects == DragDropEffects.Move)
            {
                //LibraryItems.Remove(e.Cursor.Data as PhotoData);
                SurfaceListBoxItem boxItem = ListBox3.ItemContainerGenerator.ContainerFromItem(e.Cursor.Data as PhotoData) as SurfaceListBoxItem;
                boxItem.Opacity = 1.0;
                e.Handled = true;
            }
            curClipArt.Remove(e.Cursor.Data as PhotoData);
        }

        private void ListBox_Drop3(object sender, SurfaceDragDropEventArgs e)
        {
            // If it isn't already on the ListBox, add it to the source collection.
            if (!LibraryItems.Contains(e.Cursor.Data))
            {
                //LibraryItems.Add((PhotoData)e.Cursor.Data);
            }

            // Get the SurfaceListBoxItem that ListBox automatically generated.
            SurfaceListBoxItem boxItem =
                ListBox3.ItemContainerGenerator.ContainerFromItem(e.Cursor.Data) as SurfaceListBoxItem;
            boxItem.Visibility = System.Windows.Visibility.Visible;
            // Setting e.Handle to true ensures that default behavior is not performed.
            e.Handled = true;
        }

        //=============================
        private void ListBox_PreviewTouchDown4(object sender, TouchEventArgs e)
        {
            FrameworkElement findSource = e.OriginalSource as FrameworkElement;
            SurfaceListBoxItem draggedElement = null;

            // Find the SurfaceListBoxItem object that is being touched.
            while (draggedElement == null && findSource != null)
            {
                if ((draggedElement = findSource as SurfaceListBoxItem) == null)
                {
                    findSource = VisualTreeHelper.GetParent(findSource) as FrameworkElement;
                }
            }

            if (draggedElement == null)
            {
                return;
            }

            PhotoData data = draggedElement.Content as PhotoData;

            // Create the cursor visual
            ContentControl cursorVisual = new ContentControl()
            {
                Content = draggedElement.DataContext,
                Style = FindResource("CursorStyle") as Style
            };

            // Create a list of input devices. Add the touches that
            // are currently captured within the dragged element and
            // the current touch (if it isn't already in the list).
            List<InputDevice> devices = new List<InputDevice>();
            devices.Add(e.TouchDevice);
            foreach (TouchDevice touch in draggedElement.TouchesCapturedWithin)
            {
                if (touch != e.TouchDevice)
                {
                    devices.Add(touch);
                }
            }

            // Get the drag source object
            ItemsControl dragSource = ItemsControl.ItemsControlFromItemContainer(draggedElement);

            SurfaceDragDrop.BeginDragDrop(
                dragSource,
                draggedElement,
                cursorVisual,
                draggedElement.DataContext,
                devices,
                DragDropEffects.Move);

            // Prevents the default touch behavior from happening and disrupting our code.
            e.Handled = true;

            // Gray out the SurfaceListBoxItem for now. We will remove it if the DragDrop is successful.
            draggedElement.Opacity = 0.5;
        }

        private void ListBox_DragCanceled4(object sender, SurfaceDragDropEventArgs e)
        {
            PhotoData data = e.Cursor.Data as PhotoData;
            SurfaceListBoxItem boxItem = ListBox4.ItemContainerGenerator.ContainerFromItem(data) as SurfaceListBoxItem;
            if (boxItem != null)
            {
                boxItem.Opacity = 1.0;
            }
        }

        private void ListBox_DragCompleted4(object sender, SurfaceDragCompletedEventArgs e)
        {
            if (e.Cursor.CurrentTarget != ListBox4 && e.Cursor.Effects == DragDropEffects.Move)
            {
                //LibraryItems.Remove(e.Cursor.Data as PhotoData);
                SurfaceListBoxItem boxItem = ListBox4.ItemContainerGenerator.ContainerFromItem(e.Cursor.Data as PhotoData) as SurfaceListBoxItem;
                boxItem.Opacity = 1.0;
                e.Handled = true;
            }
            curClipArt.Remove(e.Cursor.Data as PhotoData);
        }

        private void ListBox_Drop4(object sender, SurfaceDragDropEventArgs e)
        {
            // If it isn't already on the ListBox, add it to the source collection.
            if (!LibraryItems.Contains(e.Cursor.Data))
            {
                //LibraryItems.Add((PhotoData)e.Cursor.Data);
            }

            // Get the SurfaceListBoxItem that ListBox automatically generated.
            SurfaceListBoxItem boxItem =
                ListBox4.ItemContainerGenerator.ContainerFromItem(e.Cursor.Data) as SurfaceListBoxItem;
            boxItem.Visibility = System.Windows.Visibility.Visible;
            // Setting e.Handle to true ensures that default behavior is not performed.
            e.Handled = true;
        }

        //=============================
        private void ListBox_PreviewTouchDown5(object sender, TouchEventArgs e)
        {
            FrameworkElement findSource = e.OriginalSource as FrameworkElement;
            SurfaceListBoxItem draggedElement = null;

            // Find the SurfaceListBoxItem object that is being touched.
            while (draggedElement == null && findSource != null)
            {
                if ((draggedElement = findSource as SurfaceListBoxItem) == null)
                {
                    findSource = VisualTreeHelper.GetParent(findSource) as FrameworkElement;
                }
            }

            if (draggedElement == null)
            {
                return;
            }

            PhotoData data = draggedElement.Content as PhotoData;

            // Create the cursor visual
            ContentControl cursorVisual = new ContentControl()
            {
                Content = draggedElement.DataContext,
                Style = FindResource("CursorStyle") as Style
            };

            // Create a list of input devices. Add the touches that
            // are currently captured within the dragged element and
            // the current touch (if it isn't already in the list).
            List<InputDevice> devices = new List<InputDevice>();
            devices.Add(e.TouchDevice);
            foreach (TouchDevice touch in draggedElement.TouchesCapturedWithin)
            {
                if (touch != e.TouchDevice)
                {
                    devices.Add(touch);
                }
            }

            // Get the drag source object
            ItemsControl dragSource = ItemsControl.ItemsControlFromItemContainer(draggedElement);

            SurfaceDragDrop.BeginDragDrop(
                dragSource,
                draggedElement,
                cursorVisual,
                draggedElement.DataContext,
                devices,
                DragDropEffects.Move);

            // Prevents the default touch behavior from happening and disrupting our code.
            e.Handled = true;

            // Gray out the SurfaceListBoxItem for now. We will remove it if the DragDrop is successful.
            draggedElement.Opacity = 0.5;
        }

        private void ListBox_DragCanceled5(object sender, SurfaceDragDropEventArgs e)
        {
            PhotoData data = e.Cursor.Data as PhotoData;
            SurfaceListBoxItem boxItem = ListBox5.ItemContainerGenerator.ContainerFromItem(data) as SurfaceListBoxItem;
            if (boxItem != null)
            {
                boxItem.Opacity = 1.0;
            }
        }

        private void ListBox_DragCompleted5(object sender, SurfaceDragCompletedEventArgs e)
        {
            if (e.Cursor.CurrentTarget != ListBox5 && e.Cursor.Effects == DragDropEffects.Move)
            {
                //LibraryItems.Remove(e.Cursor.Data as PhotoData);
                SurfaceListBoxItem boxItem = ListBox5.ItemContainerGenerator.ContainerFromItem(e.Cursor.Data as PhotoData) as SurfaceListBoxItem;
                boxItem.Opacity = 1.0;
                e.Handled = true;
            }
            curClipArt.Remove(e.Cursor.Data as PhotoData);
        }

        private void ListBox_Drop5(object sender, SurfaceDragDropEventArgs e)
        {
            // If it isn't already on the ListBox, add it to the source collection.
            if (!LibraryItems.Contains(e.Cursor.Data))
            {
                //LibraryItems.Add((PhotoData)e.Cursor.Data);
            }

            // Get the SurfaceListBoxItem that ListBox automatically generated.
            SurfaceListBoxItem boxItem =
                ListBox5.ItemContainerGenerator.ContainerFromItem(e.Cursor.Data) as SurfaceListBoxItem;
            boxItem.Visibility = System.Windows.Visibility.Visible;
            // Setting e.Handle to true ensures that default behavior is not performed.
            e.Handled = true;
        }

        // CJT Added for changing the background image after clicking it on the menu
        void changeBackground(object sender, RoutedEventArgs e) // check what the params are for a menu click!!
        {
            // find the path information for the chosen object
            SurfaceButton findSourceA = (SurfaceButton)sender;
            SurfaceButton findSourceB = (SurfaceButton)findSourceA.Content;
            if (findSourceB.Content is Image)
            { // is a background image
                Image myimg = (Image)((SurfaceButton)findSourceA.Content).Content;

                string ImagePath = myimg.Source.ToString();
                // create new image object for the dragged object
                ImageBrush img = new ImageBrush();
                img.ImageSource = new BitmapImage(new Uri(ImagePath, UriKind.RelativeOrAbsolute));
                img.Stretch = System.Windows.Media.Stretch.Fill;
                // overwrite the Main Window with this new image
                scatter.Items.Remove(scatter.Background);
                scatter.Background = img;
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
                scatterItems.Remove(curClipArt[0]);
                curClipArt.RemoveAt(0);
            }
        }

        private void ListBox0_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {

        }
    }
}