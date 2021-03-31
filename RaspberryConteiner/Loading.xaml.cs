using System;
using System.Windows;

namespace RaspberryConteiner
{
    /// <summary>
    /// Interaction logic for Loading.xaml
    /// </summary>
    public partial class Loading
    {
        public int EllipseSize { get; set; } = 8;
        public int SpinnerHeight { get; set; }
        public int SpinnerWidth { get; set; }


        // start positions
        public EllipseStartPosition EllipseN { get; private set; }
        public EllipseStartPosition EllipseNe { get; private set; }
        public EllipseStartPosition EllipseE { get; private set; }
        public EllipseStartPosition EllipseSe { get; private set; }
        public EllipseStartPosition EllipseS { get; private set; }
        public EllipseStartPosition EllipseSw { get; private set; }
        public EllipseStartPosition EllipseW { get; private set; }
        public EllipseStartPosition EllipseNw { get; private set; }

        public Loading()
        {
            InitializeComponent();
        }

        private void InitialSetup()
        {
            // ReSharper disable once PossibleLossOfFraction
            var horizontalCenter = (float)(SpinnerWidth / 2);
            // ReSharper disable once PossibleLossOfFraction
            var verticalCenter = (float)(SpinnerHeight / 2);
            var distance = (float)Math.Min(SpinnerHeight, SpinnerWidth) / 2;

            var angleInRadians = 44.8;
            var cosine = (float)Math.Cos(angleInRadians);
            var sine = (float)Math.Sin(angleInRadians);

            EllipseN = NewPos(left: horizontalCenter, top: verticalCenter - distance);
            EllipseNe = NewPos(left: horizontalCenter + (distance * cosine), top: verticalCenter - (distance * sine));
            EllipseE = NewPos(left: horizontalCenter + distance, top: verticalCenter);
            EllipseSe = NewPos(left: horizontalCenter + (distance * cosine), top: verticalCenter + (distance * sine));
            EllipseS = NewPos(left: horizontalCenter, top: verticalCenter + distance);
            EllipseSw = NewPos(left: horizontalCenter - (distance * cosine), top: verticalCenter + (distance * sine));
            EllipseW = NewPos(left: horizontalCenter - distance, top: verticalCenter);
            EllipseNw = NewPos(left: horizontalCenter - (distance * cosine), top: verticalCenter - (distance * sine));
        }

        private static EllipseStartPosition NewPos(float left, float top)
        {
            return new EllipseStartPosition() { Left = left, Top = top };
        }


        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            switch (e.Property.Name)
            {
                case "Height":
                    SpinnerHeight = Convert.ToInt32(e.NewValue);
                    break;
                case "Width":
                    SpinnerWidth = Convert.ToInt32(e.NewValue);
                    break;
            }

            if (SpinnerHeight > 0 && SpinnerWidth > 0)
            {
                InitialSetup();
            }

            base.OnPropertyChanged(e);
        }
    }

    public struct EllipseStartPosition
    {
        public float Left { get; set; }
        public float Top { get; set; }
    }
}
