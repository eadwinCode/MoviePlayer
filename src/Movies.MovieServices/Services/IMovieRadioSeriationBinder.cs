using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Movies.MovieServices.Services
{
    public class IMovieRadioSeriationBinder : SerializationBinder
    {
        public string Format { get; private set; }
        public IMovieRadioSeriationBinder(string typeFormat)
        {
            Format = typeFormat;
        }

        public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = null;
            typeName = serializedType.Name;
        }

        public override Type BindToType(string assemblyName, string typeName)
        {
            var resolvedTypeName = string.Format(Format, typeName);
            return Type.GetType(typeName);
        }
    }
}
