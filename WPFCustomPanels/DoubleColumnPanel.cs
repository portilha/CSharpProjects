using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace WPFCustomPanels
{
    public enum Side
    {
        Rigth,
        Left
    }


    public class DoubleColumnPanel : Panel
    {
        #region Side

        /// <summary>
        /// Side Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty SideProperty =
            DependencyProperty.RegisterAttached("Side", typeof(Side), typeof(DoubleColumnPanel),
                new UIPropertyMetadata((Side)Side.Left));

        /// <summary>
        /// Gets the Side property. This dependency property 
        /// indicates ....
        /// </summary>
        public static Side GetSide(UIElement d)
        {
            return (Side)d.GetValue(SideProperty);
        }

        /// <summary>
        /// Sets the Side property. This dependency property 
        /// indicates ....
        /// </summary>
        public static void SetSide(UIElement d, Side value)
        {
            d.SetValue(SideProperty, value);
        }

        #endregion

        int leftElement = 0;
        int rightElements = 0;

        double maxChildHeight = 0;
        double maxWidth = 0;


        protected override Size MeasureOverride(Size availableSize)
        {
            leftElement = rightElements = 0;

            double[] widths = new double[Children.Count];

            for (int i = 0; i < Children.Count; i++)
            {
                UIElement child = Children[i];
                child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

                maxChildHeight = Math.Max(child.DesiredSize.Height, maxChildHeight);

                Side side = GetSide(child);

                switch (side)
                {
                    case Side.Rigth:
                        {   
                            widths[rightElements] +=  child.DesiredSize.Width;
                            maxWidth = Math.Max(maxWidth, widths[rightElements]);
                            rightElements += 1;
                        }
                        break;
                    case Side.Left:
                        {
                            widths[leftElement] += child.DesiredSize.Width;
                            maxWidth = Math.Max(maxWidth,widths[leftElement]);
                            leftElement += 1;
                        }
                        break;
                }
                
            }

            return new Size(Math.Max(maxWidth, availableSize.Width), 
                maxChildHeight * Math.Max(leftElement, rightElements));
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            int leftIndex = 0;
            int rightIndex = 0;

            for (int i = 0; i < Children.Count; i++)
            {
                UIElement child = InternalChildren[i];
                Side side = GetSide(child);
                switch (side)
                {
                    case Side.Rigth:
                        child.Arrange(new Rect(new Point(finalSize.Width - child.DesiredSize.Width, rightIndex * maxChildHeight), new Size(child.DesiredSize.Width, maxChildHeight)));
                        rightIndex++;
                        break;
                    case Side.Left:
                        child.Arrange(new Rect(new Point(0, leftIndex * maxChildHeight), new Size(child.DesiredSize.Width, maxChildHeight)));
                        leftIndex++;
                        break;
                    default:
                        break;
                }
            }

            return finalSize;
        }
    }
}
