using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CastIntoGeneratorBiz
{
    public static class CastIntoCodeGenerator
    {
        /// <summary>
        /// if set to false property types will be ignored
        /// </summary>
        public static bool AdvancedParsing = true;

        static string[] __BaseTypes = new string[]
        {
            "Byte"
            ,"SByte"
            ,"Int32"
            ,"UInt32"
            ,"Int16"
            ,"UInt16"
            ,"Int64"
            ,"UInt64"
            ,"Single"
            ,"Double"
            ,"Char"
            ,"Boolean"
            ,"String"
            ,"Decimal"
            ,"DateTime"
            ,"TimeSpan"
            ,"Guid"
        };
        static string[] __BaseTypesAliases = new string[]
        {
            "byte"
            ,"sbyte"
            ,"int"
            ,"uint"
            ,"short"
            ,"ushort"
            ,"long"
            ,"ulong"
            ,"float"
            ,"double"
            ,"char"
            ,"bool"
            ,"string"
            ,"decimal"
        };
        static string[] __BaseTypesNamesCombination;
        static string[] _baseTypesNames
        {
            get
            {
                if (__BaseTypesNamesCombination == null)
                {
                    __BaseTypesNamesCombination = __BaseTypesAliases
                        .Concat(__BaseTypesAliases.Select(tipo => $"{tipo}?"))
                        .Concat(__BaseTypesAliases.Select(tipo => $"Nullable<{tipo}>"))
                        .Concat(__BaseTypesAliases.Select(tipo => $"System.Nullable<{tipo}>"))
                        .Concat(__BaseTypes)
                        .Concat(__BaseTypes.Select(Struct => $"{Struct}?"))
                        .Concat(__BaseTypes.Select(Struct => $"System.{Struct}?"))
                        .Concat(__BaseTypes.Select(Struct => $"Nullable<{Struct}>"))
                        .Concat(__BaseTypes.Select(Struct => $"Nullable<System.{Struct}>"))
                        .Concat(__BaseTypes.Select(Struct => $"System.Nullable<{Struct}>"))
                        .Concat(__BaseTypes.Select(Struct => $"System.Nullable<System.{Struct}>"))
                        .ToArray();
                }
                return __BaseTypesNamesCombination;
            }
        }

        public static string GetCode(string codicePropieta)
        {
            if (string.IsNullOrWhiteSpace(codicePropieta)) { return null; }
            StringBuilder sb = new StringBuilder();

            

            Regex OneSpace = new Regex("[ ]{2,}");
            string ClassName = "BASETYPE";
            List<string> BaseClassesName = new List<string>();
            bool ClassNamesFound = false;

            string[] righe = codicePropieta.Trim().Split('\n');
            foreach (string riga in righe)
            {
                if (string.IsNullOrWhiteSpace(riga)) { continue; }
                string[] rigaSplittata = OneSpace.Replace(riga.Trim(), " ").Split(' ');

                if(ClassNamesFound == false && _IsClassNameRow(rigaSplittata)) 
                {
                    ClassNamesFound = true;
                    var Nomi = _GetClassNames(rigaSplittata);
                    sb.AppendLine($"        public static T CastInto<T>(this {Nomi.ClassName} input, T output) where T : {Nomi.ClassName}");
                    sb.AppendLine("        {");
                    if(Nomi.BaseClassesList.Count > 0)
                    {
                        foreach (string BaseClass in Nomi.BaseClassesList)
                        {
                            sb.AppendLine($"            (input as {BaseClass}).CastInto(output);");
                        }
                        sb.AppendLine();
                    }
                }
                else if (rigaSplittata.Length > 2 && rigaSplittata[2].StartsWith("{"))
                { sb.AppendLine(_GetCastLine(rigaSplittata[1], rigaSplittata[0])); }
                else if (rigaSplittata.Length > 3 && rigaSplittata[0] == "public" && rigaSplittata[3].StartsWith("{"))
                { sb.AppendLine(_GetCastLine(rigaSplittata[2], rigaSplittata[1])); }
            }

            sb.AppendLine();
            sb.AppendLine("            return output;");
            if (ClassNamesFound) { sb.AppendLine("        }"); }

            return sb.ToString();
        }

        private static bool _IsClassNameRow(string[] rigaSplittata)
        {
            return rigaSplittata.Length > 2
                && (rigaSplittata[0] == "public" && (rigaSplittata[1] == "class" || rigaSplittata[1] == "interface") || rigaSplittata[0] == "interface");
        }
        private static (string ClassName, List<string> BaseClassesList) _GetClassNames(string[] rigaSplittata)
        {
            string className;
            List<string> baseClassesList = new List<string>();

            if (rigaSplittata[0] == "public") 
            {
                className = rigaSplittata[2];
            }
            else if (rigaSplittata[0] == "interface")
            {
                className = rigaSplittata[1];
            }
            else 
            { 
                className = "BASETYPE";
            }

            int j = 0;
            for (int i = 0; i < rigaSplittata.Length; i++)
            {
                if (rigaSplittata[i] == ":")
                {
                    j = i;
                    break;
                }
            }
            if (j != 0)
            {
                for (int i = j + 1; i < rigaSplittata.Length; i++)
                {
                    baseClassesList.Add(rigaSplittata[i]);
                }
            }

            return (className, baseClassesList);
        }
        private static string _GetCastLine(string nomePropieta, string tipoPropieta)
        {
            if(AdvancedParsing == false)
            {
                return $"            output.{nomePropieta}=input.{nomePropieta};";
            }
            if (tipoPropieta.StartsWith("List<")) 
            {
                string type = tipoPropieta.Substring(5, tipoPropieta.Length - 6);
                if (_baseTypesNames.Contains(type)) 
                { return $"            output.{nomePropieta}=input.{nomePropieta}?.ToList();"; }
                else 
                { return $"            output.{nomePropieta}=input.{nomePropieta}?.Select(x=>x?.CastInto(new {type}())).ToList();"; }
            }
            else if (tipoPropieta.EndsWith("[]"))
            {
                string type = tipoPropieta.Substring(0, tipoPropieta.Length - 2);
                if (_baseTypesNames.Contains(type))
                { return $"            output.{nomePropieta}=input.{nomePropieta}?.ToArray();"; }
                else
                { return $"            output.{nomePropieta}=input.{nomePropieta}?.Select(x=>x?.CastInto(new {type}())).ToArray();"; }
            }
            else
            {
                if (_baseTypesNames.Contains(tipoPropieta))
                { return $"            output.{nomePropieta}=input.{nomePropieta};"; }
                else
                { return $"            output.{nomePropieta}=input.{nomePropieta}?.CastInto(new {tipoPropieta}());"; }
                   
            }
        }
        
    }

}
