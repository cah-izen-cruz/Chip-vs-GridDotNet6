using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Globalization;


namespace GRIDLibraries.Libraries
{
    partial class GridLib
    {
        [Obsolete]
        public DateTime ConvertTimeZone(int OffSet)
        {
            DateTime ConvertTimeZoneRet = default;

            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            var localZone = TimeZone.CurrentTimeZone;

            // Dim currentDate As DateTime = DateTime.Now
            // Dim currentYear As Integer = currentDate.Year

            var currentUTC = localZone.ToUniversalTime(DateTime.Now);
            // Dim currentOffset As TimeSpan = localZone.GetUtcOffset(Now)

            ConvertTimeZoneRet = Conversions.ToDate(Strings.Format(currentUTC.AddHours(OffSet), "MM/dd/yyyy h:mm:ss tt"));
            return ConvertTimeZoneRet;




        }


        public string ConvertSecondsToHHMMSS(int Seconds)
        {
            var span = new TimeSpan(0, 0, Seconds);
            return span.ToString();
        }


    }
}
