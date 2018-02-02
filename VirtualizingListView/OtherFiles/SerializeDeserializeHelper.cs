using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace VirtualizingListView
{
   public static class SerializeDeserializeHelper
    {
       public static T Deserialize<T>(this string toDeserialize)
       {
           if (toDeserialize == "") return default(T);

           XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
           StringReader textReader = new StringReader(toDeserialize);
           return (T)xmlSerializer.Deserialize(textReader);
       }

       public static string Serialize<T>(this T toSerialize)
       {
           XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
           StringWriter textwriter = new StringWriter();
           xmlSerializer.Serialize(textwriter, toSerialize);
           return textwriter.ToString();
       }

    }
}
