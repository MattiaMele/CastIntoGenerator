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
                    sb.AppendLine($"        public static T CastInto<T>(this {Nomi.ClassName} input, T output = default) where T : {Nomi.ClassName}, new()");
                    sb.AppendLine("        {");
                    sb.AppendLine("            if (output == null)");
                    sb.AppendLine("                output = new T();");
                    if(Nomi.BaseClassesList.Count > 0)
                    {
                        sb.AppendLine();
                        foreach (string BaseClass in Nomi.BaseClassesList)
                        {
                            sb.AppendLine($"            (input as {BaseClass}).CastInto(output);");
                        }
                    }
                    sb.AppendLine();
                }
                else if (rigaSplittata.Length > 2 && rigaSplittata[2].StartsWith("{"))
                { sb.AppendLine($"            output.{rigaSplittata[1]}=input.{rigaSplittata[1]};"); }
                else if (rigaSplittata.Length > 3 && rigaSplittata[0] == "public" && rigaSplittata[3].StartsWith("{"))
                { sb.AppendLine($"            output.{rigaSplittata[2]}=input.{rigaSplittata[2]};"); }
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
        
    }

}
