using System;
using System.Windows;


namespace HumidityW10
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        static class Calculator
        {
            // values and equations taken from
            // https://www.cactus2000.de/js/calchum.pdf

            const double WaterMolarMass = 18.01534;
            const double DryAirMolarMass = 28.9644;
            const double UniversalGasConstant = 8.31447215;
            const double Kelvin = 273.15;

            public static double VaporPressure(double temp)
            {
                double WaterPressure, IcePressure;
                //Func<double, double, double, double, double, double, double, double, double> VP = (double temperature, double a0, double a1, double a2, double a3, double a4, double a5, double a6) => a0 + temperature * (a1 + temperature * (a2 + temperature * (a3 + temperature * (a4 + temperature * (a5 + temperature * a6)))));
                Func<double, double[], double> VP = (double temperature, double[] array) => array[0] + temperature * (array[1] + temperature * (array[2] + temperature * (array[3] + temperature * (array[4] + temperature * (array[5] + temperature * array[6])))));
                double[] VaporPressureIce = { 6.109177956, 5.034698970e-1, 1.886013408e-2, 4.176223716e-4, 5.824720280e-6, 4.838803174e-8, 1.838826904e-10 };
                double[] VaporPressureWater = { 6.107799961, 4.436518521e-1, 1.428945805e-2, 2.650648471e-4, 3.031240396e-6, 2.034080948e-8, 6.136820929e-11 };
                //WaterPressure = VP(temp, VaporPressureWater[0], VaporPressureWater[1], VaporPressureWater[2], VaporPressureWater[3], VaporPressureWater[4], VaporPressureWater[5], VaporPressureWater[6]);
                //IcePressure = VP(temp, VaporPressureIce[0], VaporPressureIce[1], VaporPressureIce[2], VaporPressureIce[3], VaporPressureIce[4], VaporPressureIce[5], VaporPressureIce[6]);
                WaterPressure = VP(temp, VaporPressureWater);
                IcePressure = VP(temp, VaporPressureIce);
                return Math.Min(WaterPressure, IcePressure);
            }
            public static double AirDensity(double pressure, double temperature)
            {
                return (pressure * 100) / (UniversalGasConstant * (temperature + Kelvin));
            }
            public static double VolumeMixingRatio(double pressure, double temperature, double humidity)  // * 1000 for promiles
            {
                return (humidity * VaporPressure(temperature) / 100) / pressure;
            }
            public static double MassConcentration(double pressure, double temperature, double humidity) // in g/m^3
            {
                return VolumeMixingRatio(pressure, temperature, humidity) * AirDensity(pressure, temperature) * WaterMolarMass;
            }
            public static double SpecificHumidity(double pressure, double temperature, double humidity) // in g/kg
            {
                return 1000 * (VolumeMixingRatio(pressure, temperature, humidity) * WaterMolarMass) / (VolumeMixingRatio(pressure, temperature, humidity) * WaterMolarMass + (1 - VolumeMixingRatio(pressure, temperature, humidity)) * DryAirMolarMass);
            }
            public static string HumidityCompare(double humidityin, double humidityout)
            {
                if (humidityin > humidityout)
                {
                    return "You will dehumidify your room by airing it";
                }
                else if (humidityin > humidityout)
                {
                    return "You will humidify your room by airing it";
                }
                else return "Something went wrong!";
            }
        }
        public double OutsideTemperature { get; set; }
        public double InsideTemperature { get; set; }
        public double Pressure { get; set; }
        public double OutsideHumidity { get; set; }
        public double InsideHumidity { get; set; }
        public string MasterTextString { get; set; }
        public string ResultTextString { get; set; }
        public MainWindow()
        {
            ResultTextString = "Results will be shown here!";
            MasterTextString = "This program will calculate specific humidity in two places. You can use this information to help you decide whether you should air your room, depending on whether you want to humidify or dehumidify that place.\n\nInput values as instructed and press the CALCULATE button. This field will update with specific humidity, and information whether opening INSIDE's windows will increase or decrease internal humidity.";
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            double InsideHumidityCalc = Calculator.SpecificHumidity(Pressure, InsideTemperature, InsideHumidity);
            double OutsideHumidityCalc = Calculator.SpecificHumidity(Pressure, OutsideTemperature, OutsideHumidity);
            ResultTextString = $"Inside specific humidity (g/kg): {InsideHumidityCalc,10:F2}\nOutside specific humidity (g/kg):  {OutsideHumidityCalc,7:F2}\n{Calculator.HumidityCompare(InsideHumidityCalc, OutsideHumidityCalc)}";
            ResultTextBlock.Text = ResultTextString;
        }
    }
}
