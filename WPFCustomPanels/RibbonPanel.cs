using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace WPFCustomPanels
{
    public class RibbonPanel : Panel
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (Children.Count < 1)
                return new System.Windows.Size(0,0);

            UIElement firstChild = Children[0];
            firstChild.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

            if (Children.Count < 2) return firstChild.DesiredSize;

            double numRows = Math.Ceiling((Children.Count - 1) / 3d);
            double maxWidthForEachRemainingChild = 0;

            for (int i = 1; i < Children.Count; i++)
            {
                // Ask each child for its desired size, given unlimited space
                UIElement child = Children[i];
                child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity)); // This will update the values of the Child.DesiredSize

                // keep track of the maximum width
                maxWidthForEachRemainingChild = Math.Max(child.DesiredSize.Width, maxWidthForEachRemainingChild);
            }

            return new Size(
                // total width
                firstChild.DesiredSize.Width + maxWidthForEachRemainingChild * numRows,
                // height = desired height of the first child
                firstChild.DesiredSize.Height);
        }


        /// <summary>
        /// Arrange all the children
        /// </summary>
        /// <param name="finalSize">Size of the Panel</param>
        /// <returns></returns>
        protected override System.Windows.Size ArrangeOverride(System.Windows.Size finalSize)
        {
            if (Children.Count < 1)
                return finalSize;

            // Give the first child its desired width but the height of the panel
            UIElement firstChild = Children[0];
            Point childOrigin = new Point();

            Size firstChildSize = new Size(firstChild.DesiredSize.Width, finalSize.Height);

            firstChild.Arrange(new Rect(childOrigin, firstChildSize));

            if (Children.Count < 2) return finalSize;

            // Determine the size for all the remaining children.
            double numRows = Math.Ceiling((Children.Count - 1) / 3d);
            Size childSize = new Size((finalSize.Width - firstChildSize.Width) / numRows, finalSize.Height / 3);
            childOrigin.X += firstChildSize.Width;

            for (int i = 1; i < Children.Count; i++)
            {
                UIElement child = Children[i];
                child.Arrange(new Rect(childOrigin, childSize));

                if (i % 3 == 0)
                {
                    // Start a new column
                    childOrigin.X += childSize.Width;
                    childOrigin.Y = 0;
                }
                else
                    childOrigin.Y += childSize.Height;
            }

            // Fill all the space given
            return finalSize;
        }

    }
}
