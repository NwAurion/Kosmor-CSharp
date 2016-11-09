using System;
using System.Windows;
using System.Windows.Controls;

namespace DistanceAndTravelTimeCalculator
{
    /// <summary>
    /// Interaktionslogik für Calculator.xaml
    /// </summary>
    public partial class Calculator : UserControl
    {
        public Calculator()
        {
            InitializeComponent();
        }

        double xCoordinateFirstObject;
        double yCoordinateFirstObject;
        double xCoordinateSecondObject;
        double yCoordinateSecondObject;

        public static readonly DependencyProperty DistanceProperty = DependencyProperty.Register("Distance", typeof(Double), typeof(UserControl));
        public Double Distance
        {
            get { return (double)GetValue(DistanceProperty); }
            set
            {
                SetValue(DistanceProperty, value);
            }
        }

        public static readonly DependencyProperty TravelTimeProperty = DependencyProperty.Register("TravelTime", typeof(Double), typeof(UserControl));
        public Double TravelTime
        {
            get { return (double)GetValue(TravelTimeProperty); }
            set
            {
                SetValue(TravelTimeProperty, value);
            }
        }



        private void calculate()
        {
            calculateDistance();
            calculateTravelTime();
        }

        private void calculateDistance()
        {

            Double x = (xCoordinateFirstObject - xCoordinateSecondObject);
            Double y = (yCoordinateFirstObject - yCoordinateSecondObject);

            // Pythagorean theorem, a² + b² = c² ..
            Double c = Math.Pow(x, 2) + Math.Pow(y, 2);

            Distance = Math.Round(Math.Sqrt(c) * 100) / 100;
        }


        private void calculateTravelTime()
        {
            bool doJump = cbJump.IsChecked.Value;
            bool isIndependent = cbIndependence.IsChecked.Value;
            Double distance = Distance;
            int travelTime = 0;
            int jump = 0;

            if (doJump)
            {
                if (!isIndependent)
                {
                    {
                        while (distance > 0)
                        {
                            if (distance > 120 && jump == 0)
                            {
                                distance = distance - 150.0;
                                jump = 4;
                                travelTime++;
                            }
                            else if (distance <= 120 || jump != 0)
                            {
                                distance = distance - 60.0;
                                jump--;
                                travelTime++;
                            }
                            distance = (double)Math.Round(distance * 1000) / 1000;
                        }
                    }
                }
                else
                {
                    while (distance > 0)
                    {
                        if (distance >= 480 && jump == 0)
                        {
                            distance = distance - 600.0;
                            jump = 4;
                            travelTime++;
                        }
                        else if (distance >= 240 && jump == 0)
                        {
                            distance = distance - 300.0;
                            jump = 4;
                            travelTime++;
                        }
                        else if (distance > 120 && jump == 0)
                        {
                            distance = distance - 150.0;
                            jump = 4;
                            travelTime++;
                        }
                        else if (Distance <= 120 || jump != 0)
                        {
                            distance = distance - 60.0;
                            jump--;
                            travelTime++;
                        }
                        distance = (double)Math.Round(distance * 1000) / 1000;
                    }
                }
            }
            else if (!doJump)
            {
                while (distance > 0)
                {
                    distance = distance - 60;
                    travelTime++;
                }
            }

            TravelTime = travelTime;
        }

        private void xCoordFirstObject_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool number = Double.TryParse(xCoordFirstObject.Text, out xCoordinateFirstObject);
            calculate();
        }

        private void yCoordFirstObject_TextChanged(object sender, TextChangedEventArgs e)
        {

            bool number = Double.TryParse(yCoordFirstObject.Text, out yCoordinateFirstObject);
            calculate();
        }

        private void xCoordSecondObject_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool number = Double.TryParse(xCoordSecondObject.Text, out xCoordinateSecondObject);
            calculate();
        }

        private void yCoordSecondObject_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool number = Double.TryParse(yCoordSecondObject.Text, out yCoordinateSecondObject);
            calculate();
        }

        private void cbJump_Checked(object sender, RoutedEventArgs e)
        {
            calculateTravelTime();
        }

        private void cbIndependence_Checked(object sender, RoutedEventArgs e)
        {
            calculateTravelTime();
        }
    }
}
