using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Temperature_Converter
{
    public enum SysOfUnits
    {
        Kelvin,
        Celsius,
        Fahrenheit
    }
    public class TempConverter
    {
        decimal _t; //Note, Always stored as Celsius

        public TempConverter(decimal temp, SysOfUnits u)
        {
            if(u==SysOfUnits.Celsius)
            {
                _t = temp;
            }
            else if(u == SysOfUnits.Kelvin)
            {
                _t = temp - 273.15m;
            }
            else
            {
                _t = (temp - 32m) * 5m/9m;
            }
            if(_t<-273.15m)
            {
                throw new ArgumentOutOfRangeException();
            }
        }
        public decimal InCelsius
        {
            get
            {
                return Math.Round(_t,2);
            }
        }
        public decimal InKelvin
        {
            get
            {
                return Math.Round(_t + 273.15m,2);
            }
        }
        public decimal InFahrenheit
        {
            get
            {
                return Math.Round((_t * 9m/5m) + 32m,2);
            }
        }
    }
}
