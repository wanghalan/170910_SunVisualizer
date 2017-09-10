using System;
using UnityEngine;
using UnityEngine.UI;

public static class MathD{ //gives Degree, returns Radian
    public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T> {
        if (val.CompareTo(min) < 0) return min;
        else if (val.CompareTo(max) > 0) return max;
        else return val;
    }
    public static bool HasValue(this double value) {
        return !Double.IsNaN(value) && !Double.IsInfinity(value);
    }
    public static double RadianToDegree(double angle) {
        return angle * (180.0 / Math.PI);
    }
    public static double DegreeToRadian(double angle) {
        return Math.PI * angle / 180.0;
    }
    public static double Sin(double angleInDegrees) {
        return Math.Sin(DegreeToRadian(angleInDegrees));
    }
    public static double Cos(double angleInDegrees) {
        return Math.Cos(DegreeToRadian(angleInDegrees));
    }
    public static double Asin(double angleInDegrees) {
        return Math.Asin(DegreeToRadian(angleInDegrees));
    }
    public static double Acos(double angleInDegrees) {
        return Math.Acos(DegreeToRadian(angleInDegrees));
    }
}

public class SunAngleCalculator : MonoBehaviour {
    public DateTime datetime;
    public float lat= 34.052235f;
    public float lon= -118.243683f;
    public double timezone=-7; //UTC
    public GameObject Sun;
    public Text timeText;
    public float minutesToAddPerTick = 0.5f;
    bool runTestDays = true;

    void Start() {
        //datetime = DateTime.Now;
        datetime = new DateTime(2017, 3, 22);

        Debug.Log(datetime.ToLocalTime());
        setSun();
        if (!runTestDays) {
            InvokeRepeating("UpdateSun", 1, 0.01f);
        }
        else {
            InvokeRepeating("TestUpdateSun", 1, 0.01f);
        }
    }

    void TestUpdateSun() { //visually check if march, june, september, and december have different or same suns
        datetime = datetime.AddMinutes(minutesToAddPerTick);
        setSun();

        //if (datetime.Month == 12 && datetime.Day == 23) {
        //    CancelInvoke();
        //}
        //Debug.Log(datetime.Day);
        if (datetime.Day > 22) {
            datetime= datetime.AddMonths(3);
            datetime= datetime.AddDays(-1);
        }
    }

    void UpdateSun() {
        //datetime = datetime.AddHours(1);
        datetime = datetime.AddMinutes(0.5);
        setSun();
    }

    public void setSun() { //Sets sun to the current postion given latitude, longitude, and time
        if (Sun!= null) {
            double[] ZenAzi = getZenithAndAzimuth();
            //Debug.LogFormat("'{0}:{1}','{2}:{3}'", ZenAzi[0], MathD.HasValue(ZenAzi[0]), ZenAzi[1], MathD.HasValue(ZenAzi[1]));
            //Debug.Log(datetime);
            //Debug.LogFormat("{0}: {1}, {2}", datetime, ZenAzi[0], ZenAzi[1]);
            if (MathD.HasValue(ZenAzi[0]) && MathD.HasValue(ZenAzi[1])) { 
                Sun.transform.localEulerAngles = new Vector3((float)ZenAzi[0], (float)ZenAzi[1], 0);        
            }

            if (timeText!= null) {
                timeText.text = datetime.ToString();
            }
        }
    }

    public double getLocalHour(DateTime dt) {
        return dt.Hour + dt.Minute / 60 + dt.Second/3600; //fractional GMT time
    }

    public double[] getZenithAndAzimuth() {
        //Debug.LogFormat("Day of year: {0}", datetime.DayOfYear);
        return getZenithAndAzimuth(datetime, lon, lat, timezone);
    }

    //Maybe...http://answers.google.com/answers/threadview/id/782886.html
    //https://en.wikipedia.org/wiki/Position_of_the_Sun
    //https://en.wikipedia.org/wiki/Solar_azimuth_angle
    //http://www.itacanet.org/the-sun-as-a-source-of-energy/part-3-calculating-solar-angles/

    public double[] getZenithAndAzimuth(DateTime time, double longitude, double latitude, double timezone) {


        int daysInYear = 365;
        if (DateTime.IsLeapYear(time.Year)) {
            daysInYear = 366;
        }


        //radians
        double fractionalYear = (2 * Math.PI / daysInYear) * (time.DayOfYear - 1 + ((time.Hour - 12) / 24)); 

        //eqptime in Minutes
        double eqtime = 229.18 * (0.000075 + 0.001868 * Math.Cos(fractionalYear) - 0.032077 * Math.Sin(fractionalYear) - 0.014615 * Math.Cos(2 * fractionalYear) - 0.040849 * Math.Sin(2 * fractionalYear)); 
        
        //decl in Radians
        double decl = 0.006918 - 0.399912* Math.Cos(fractionalYear) + 0.070257  *Math.Sin(fractionalYear) - 0.006758 * Math.Cos(2*fractionalYear) + 0.000907 * Math.Sin(2*fractionalYear) - 0.002697 * Math.Cos(3*fractionalYear) + 0.00148*Math.Sin(3*fractionalYear); 

        double time_offset= eqtime + 4 * longitude - 60 * timezone;
        double tst = time.Hour * 60 + time.Minute + time.Second / 60 + time_offset; //Tst= true solar time

        double ha = (tst / 4) - 180; //Solar Hour Angle in degrees

        //Debug.Log(ha);


        //Solar Zenith Angle, and solar azimuth angle in radians

        double sza = Math.Acos(MathD.Sin(latitude) * Math.Sin(decl) + MathD.Cos(latitude) * Math.Cos(decl) * MathD.Cos(ha)); //Solar Zenith Angle in radians
        double saa = -(Math.Acos(-(MathD.Sin(latitude) * Math.Cos(sza) - Math.Sin(decl)) / (MathD.Cos(latitude) * Math.Sin(sza)))- Math.PI); //Solar Azimuth Angle

        //Wikiperdia solar azimuth angle
        //double saa = Math.Acos(
        //        (Math.Sin(decl) * Math.Cos(latitude) - Math.Cos(ha) * Math.Cos(decl) * Math.Sin(latitude)) / Math.Sin(sza)
        //    );

        double[] answer = new double[2];
        answer[0] = MathD.RadianToDegree(sza);
        answer[1] = MathD.RadianToDegree(saa);


        if (ha < 0 || ha > 180) {
            //answer[0] *= -1;
            answer[0] += 180;
        }

        Debug.LogFormat("Time: {0},\tZenith: {1},\tAzimuth: {2},\tHour Angle: {3}", time.ToString(), answer[0], answer[1], ha);

        return answer;
    }

    //https://www.ncdc.noaa.gov/gridsat/docs/Angle_Calculations.pdf
    public double[] getZenithAndAzimuth2(DateTime time, double longitude, double latitude, double timezone) {

        double hourAngle = -(getLocalHour(time) - 12 / 12);
        //Are the units correct? Degrees or radians?!
        double solarDeclinationAngle = -23.45 * Math.Cos(MathD.DegreeToRadian(2 * Math.PI * time.DayOfYear / 365 + 20 * Math.PI / 365));



        double[] answer = new double[2];


       // answer[0] = MathD.RadianToDegree(sza);
        //answer[1] = MathD.RadianToDegree(saa) % 360;
        return answer;
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, new Vector3(1000, 0, 0));
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, new Vector3(0, 0, 1000));
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, new Vector3(-1000, 0, 0));
        Gizmos.color = Color.gray;
        Gizmos.DrawLine(transform.position, new Vector3(0, 0, 1000));
    }
}
