using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RK.Common.GraphicsEngine.Gui
{
    public partial class Direct3D11Image
    {
        private bool m_isDragging;
        private Point m_lastDragPoint;

        /// <summary>
        /// Called when user uses the mouse wheel for zooming.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            m_renderLoop.Camera.Zoom((float)(e.Delta / 100.0));
        }

        private void OnViewportGridLostFocus(object sender, RoutedEventArgs e)
        {
            StopCameraDragging();
        }

        private void OnViewportGridMouseDown(object sender, MouseButtonEventArgs e)
        {
            StartCameraDragging(e);
        }

        private void OnViewportGridMouseLeave(object sender, MouseEventArgs e)
        {
            StopCameraDragging();
        }

        private void OnViewportGridMouseMove(object sender, MouseEventArgs e)
        {
            if (m_isDragging)
            {
                Point newDragPoint = e.GetPosition(this);
                Vector2 moveDistance = new Vector2(
                    (float)(newDragPoint.X - m_lastDragPoint.X),
                    (float)(newDragPoint.Y - m_lastDragPoint.Y));

                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    m_renderLoop.Camera.Strave((float)((double)moveDistance.X / 50));
                    m_renderLoop.Camera.UpDown((float)(-(double)moveDistance.Y / 50));
                }
                else if (e.RightButton == MouseButtonState.Pressed)
                {
                    m_renderLoop.Camera.Rotate(
                        (float)(-(double)moveDistance.X / 300),
                        (float)(-(double)moveDistance.Y / 300));
                }

                m_lastDragPoint = newDragPoint;
            }
        }

        private void OnViewportGridMouseUp(object sender, MouseButtonEventArgs e)
        {
            StopCameraDragging();
        }

        private void OnViewportGridPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            StopCameraDragging();
        }

        private void StartCameraDragging(MouseButtonEventArgs e)
        {
            m_isDragging = true;
            this.Cursor = Cursors.Cross;
            m_lastDragPoint = e.GetPosition(this);
        }

        private void StopCameraDragging()
        {
            m_isDragging = false;
            this.Cursor = Cursors.Hand;
        }
    }
}
