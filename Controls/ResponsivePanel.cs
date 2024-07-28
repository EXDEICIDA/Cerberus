using System;
using System.Windows;
using System.Windows.Controls;

namespace Cerberus.Controls
{
    public class TwoColumnResponsivePanel : Panel
    {
        protected override Size MeasureOverride(Size availableSize)

        {
            double columnWidth = availableSize.Width / 2;
            double[] columnHeights = new double[2] { 0, 0 };

            foreach (UIElement child in Children)
            {
                child.Measure(new Size(columnWidth, double.PositiveInfinity));
                int column = columnHeights[0] <= columnHeights[1] ? 0 : 1;
                columnHeights[column] += child.DesiredSize.Height;
            }

            return new Size(availableSize.Width, Math.Max(columnHeights[0], columnHeights[1]));
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double columnWidth = finalSize.Width / 2;
            double[] columnHeights = new double[2] { 0, 0 };

            foreach (UIElement child in Children)
            {
                int column = columnHeights[0] <= columnHeights[1] ? 0 : 1;
                double x = column * columnWidth;
                double y = columnHeights[column];

                child.Arrange(new Rect(new Point(x, y), new Size(columnWidth, child.DesiredSize.Height)));
                columnHeights[column] += child.DesiredSize.Height;
            }

            return finalSize;
        }
    }
}