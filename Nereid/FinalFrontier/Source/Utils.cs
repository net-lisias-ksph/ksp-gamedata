﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Nereid
{
   namespace FinalFrontier
   {
      static class Utils
      {

         private static void MoveFile(String from, String to)
         {
            try
            {
               File.Move(from, to);
            }
            catch
            {
               Log.Warning("failed to move  file '" + from + "' to '" + to + "'");
            }

         }

         public static String ToString<T>(List<T> list)
         {
            String result="";
            foreach(T x in list)
            {
               if (result.Length > 0) result = result + ",";
               result = result + x.ToString();
            }
            return result+" ("+list.Count+" entries)";
         }

         public static String Roman(int value)
         {
            switch (value)
            {
               case 0: return "";
               case 1: return "I";
               case 2: return "II";
               case 3: return "III";
               case 4: return "IV";
               case 5: return "V";
               case 6: return "VI";
               case 7: return "VII";
               case 8: return "VIII";
               case 9: return "IX";
               case 10: return "X";
               case 11: return "XI";
               case 12: return "XII";
               case 13: return "XIIV";
               case 14: return "XIV";
               case 15: return "XV";
            }
            return "?";
         }

         public static CelestialBody GetCelestialBody(String name)
         {
            foreach (CelestialBody body in PSystemManager.Instance.localBodies)
            {
               if (body.GetName().Equals(name)) return body;
            }
            return null;
         }

         public static void FileRotate(String filename, int maxNr)
         {
            Log.Info("rotating file '" + filename + "' (" + maxNr + " versions)");
            String obsolete = filename + "." + maxNr;
            if (File.Exists(obsolete))
            {
               try
               {
                  File.Delete(obsolete);
               }
               catch
               {
                  Log.Warning("failed to delete obsolete file '" + obsolete + "'");
               }
            }

            for (int n = maxNr - 1; n > 0; n--)
            {
               String oldFilename = filename + "." + n;
               String newFilename = filename + "." + (n + 1);
               if (File.Exists(oldFilename))
               {
                  MoveFile(oldFilename, newFilename);
               }
            }
            //
            MoveFile(filename, filename + ".1");
         }


         public static int ConvertDaysToSeconds(int days)
         {
            return days * 60 * 60 * 24;
         }

         public static int ConvertHoursToSeconds(int hours)
         {
            return hours * 60 * 60;
         }

         public static String GetRootPath()
         {
            String path = KSPUtil.ApplicationRootPath;
            path = path.Replace("\\", "/");
            if (path.EndsWith("/")) path = path.Substring(0, path.Length - 1);
            //
            return path;
         }

         public static String ConvertToKerbinDuration(double ut)
         {
            double hours = ut / 60.0 / 60.0;
            double kHours = Math.Floor(hours % 24.0);
            double kMinutes = Math.Floor((ut / 60.0) % 60.0);
            double kSeconds = Math.Floor(ut % 60.0);


            double kYears = Math.Floor(hours / 2556.5402); // Kerbin year is 2556.5402 hours
            double kDays = Math.Floor(hours % 2556.5402 / 24.0);
            return ((kYears > 0) ? (kYears.ToString() + " Years ") : "")
               + ((kDays > 0) ? (kDays.ToString() + " Days ") : "")
               + ((kHours > 0) ? (kHours.ToString() + " Hours ") : "")
               + ((kMinutes > 0) ? (kMinutes.ToString() + " Minutes ") : "")
               + ((kSeconds > 0) ? (kSeconds.ToString() + " Seconds ") : "");
         }


         public static String ConvertToKerbinTime(double ut)
         {
            double hours = ut / 60.0 / 60.0;
            double kHours = Math.Floor(hours % 6.0);
            double kMinutes = Math.Floor((ut / 60.0) % 60.0);
            double kSeconds = Math.Floor(ut % 60.0);


            double kYears = Math.Floor(hours / 2556.5402) + 1; // Kerbin year is 2556.5402 hours
            double kDays = Math.Floor(hours % 2556.5402 / 6.0) + 1;

            return "Year " + kYears.ToString() + ", Day " + kDays.ToString() + " " + " " + kHours.ToString("00") + ":" + kMinutes.ToString("00") + ":" + kSeconds.ToString("00");
         }


         public static String ConvertToEarthTime(double ut)
         {
            double hours = ut / 60.0 / 60.0;
            double eHours = Math.Floor(hours % 24.0);
            double eMinutes = Math.Floor((ut / 60.0) % 60.0);
            double eSeconds = Math.Floor(ut % 60.0);


            double eYears = Math.Floor(hours / (365 * 24)) + 1;
            double eDays = Math.Floor(hours % (365 * 24) / 24.0) + 1;

            return "Year " + eYears.ToString() + ", Day " + eDays.ToString() + " " + " " + eHours.ToString("00") + ":" + eMinutes.ToString("00") + ":" + eSeconds.ToString("00");
         }

         public static double GameTimeInDays(double time)
         {
            if (GameUtils.IsKerbinTimeEnabled())
            {
               return time / 6 / 60 / 60;
            }
            else
            {
               return time / 24 / 60 / 60;
            }
         }


         public static String GameTimeInDaysAsString(double time)
         {
            double inDays = GameTimeInDays(time);
            if (inDays >= 1000)
            {
               return (inDays / 1000).ToString("0") + "k";
            }
            else if (inDays >= 1000000)
            {
               return (inDays / 1000000).ToString("0") + "m";

            }
            return GameTimeInDays(time).ToString("0.00");
         }

         public static String GameTimeAsString(double time)
         {
            long seconds = (long)time;
            long days = (long)GameTimeInDays(time);
            long hours_per_day = GameUtils.IsKerbinTimeEnabled() ? Constants.HOURS_PER_KERBIN_DAY : Constants.HOURS_PER_EARTH_DAY;
            long seconds_per_day = Constants.SECONDS_PER_HOUR * hours_per_day;
            long hours = (seconds % seconds_per_day) / Constants.SECONDS_PER_HOUR;
            long minutes = (seconds % Constants.SECONDS_PER_HOUR) / Constants.SECONDS_PER_MINUTE;

            String hhmm = hours.ToString("00") + ":" + minutes.ToString("00");

            if (days == 0)
            {
               return hhmm;
            }
            else if (days < 1000)
            {
               return days.ToString("0") + "d " + hhmm;
            }
            else if (days >= 1000)
            {
               return (days / 1000).ToString("0") + "kd " + hhmm;
            }
            else
            {
               return (days / 1000000).ToString("0") + "md " + hhmm;
            }
         }
      }

   }
}
