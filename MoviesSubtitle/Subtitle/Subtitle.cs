
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Collections.Generic;

namespace MediaControl.Subtitles
{
    public static class SubtitleFileCompiler
    {
       static Hashtable table;
       static string spath;
       
       public static void ReadSubtitleFile(string path)
       {
           spath = path;
           if (File.Exists(spath))
           {
                try
                {
                    table = FileToString();
                }
                catch (Exception)
                {
                    throw;
                }
               
           }
       }

       public static string GetSubtitle(double totalseconds)
       {
           if ((table[new KeyCode(totalseconds)]) == null)
           {
               return "";
           }
           else
           {
               var sho = (table[new KeyCode(totalseconds)]) as string;
                return sho;
           }
           
       }

       private static Hashtable FileToString()
       {
           string[] delimeter = { "-", ">", " " };

           /*the file sub.txt is int this solution in this form:
            
            start_frame-end_frame-text
            345-543-How are you?
            
           */
           KeyCode key;
           Hashtable table = new Hashtable();
           //var s = File.ReadAllText(@"C:\Users\Eadwin Toochos\Desktop\subtitles\Hitch.srt");
           //var parts = s.Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
           //var subtitles = new string[parts.Length][];

           //for (int i = 0; i < subtitles.Length; i++)
           //{
           //    subtitles[i] = parts[i].Split('\n');
           //}
           //(?:\r?\n)*\d+\r?\n\d{2}:\d{2}:\d{2},\d{3} --> \d{2}:\d{2}:\d{2},\d{3}\r?\n
           var lines = File.ReadAllLines(spath);
            // Regex rgx = new Regex(@"\d{2}:\d{2}:\d{2}.\d{3}");
            int y;
            for (int i = 1; i < lines.Length; i++)
            {
                try
                {
                    if (lines[i] != "")
                    {
                        if (int.TryParse(lines[i].ToString(), out y))
                        {
                            i++;
                        }
                        string[] time = lines[i].Split(delimeter, StringSplitOptions.
                            RemoveEmptyEntries);
                        time[0] = time[0].Replace(",", ".");
                        time[1] = time[1].Replace(",", ".");
                        key = new KeyCode(TimeSpan.Parse(time[0]).TotalSeconds, TimeSpan.Parse(time[1]).TotalSeconds);
                        string message = AddMessage(ref i, lines);
                        try
                        {
                            table.Add(key, message);
                        }
                        catch (Exception) { }

                    }
                }
                catch (Exception)
                {

                    throw;
                }

            }
            //  string[] parts = lines.Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
            //string[] souceSrt = Regex.Split(lines, @"(?:\r?\n)*\d+\r?\n\d{2}:\d{2}:\d{2},\d{3} --> \d{2}:\d{2}:\d{2},\d{3}\r?\n");
            //string[] souceSr = Regex.Split(lines, @"\D+");

            //var subtitle = new string[parts.Length][];
            //for (int i = 0; i < subtitle.Length; i++)
            //{
            //    subtitle[i] = parts[i].Split('\n');
            //}

            //while (true)
            //{
            //    string[] field = lines.Split(delimeter, StringSplitOptions.RemoveEmptyEntries);
            //}
            //Stream stream = new FileStream(@"C:\Users\Eadwin Toochos\Desktop\subtitles\sub.txt", FileMode.Open);
            //StreamReader read = new StreamReader(stream);
            //while (true)
            //{
            //    //string line = read.ReadLine();
            //    //if (line == null)
            //    //    break;
            //    //string[] field = line.Split("-".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            //    //Key key = new Key(Convert.ToInt32(field[0]), Convert.ToInt32(field[1]));
            //    //table.Add(key, field[2]);
            //}
            return table;
       }

       private static string AddMessage(ref int i, string[] lines)
       {
           string message = string.Empty;
           for (int j = i; j < lines.Length; j++)
           {
               try
               {
                   if (lines[j + 1] == "")
                   {
                       i = j;
                     //  message += "\n ";
                      // message = message.Replace("\n", "");
                       break;
                   }
                   else
                   {
                       if (lines[j + 1].Contains("</"))
                       {
                           message += "<br>" + lines[j + 1] + " </br> ";
                       }
                       else
                           message += "<br><r>" + lines[j + 1] + "</r> </br> ";
                           
                   }
               }
               catch (Exception) { i = j; }
           }
           return message;
       }
        
    }

   class KeyCode
   {
       double min;
       double max;
       public KeyCode(double min, double max)
       {
           this.max = max;
           this.min = min;
       }

       public KeyCode(double pos)
       {
           this.max = pos;
           this.min = pos;
       }

       public override bool Equals(object obj)
       {
           KeyCode pom = (KeyCode)obj;
           if (pom.max <= max && pom.min >= min)
           {
               return true;
           }
           else return false;
       }

       public override int GetHashCode()
       {
           return 1;
       }

        public override string ToString()
        {
            return min + " - " + max;
        }
    }

  
}
