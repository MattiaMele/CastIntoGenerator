using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CastIntoGeneratorBiz
{
    public static class CastIntoCodeGenerator
    {
        public static string GetCode(string codicePropieta)
        {
            if (string.IsNullOrWhiteSpace(codicePropieta)) { return null; }
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("        public static T CastInto<T>(this TYPE input, T output = default) where T : TYPE, new()");
            sb.AppendLine("        {");
            sb.AppendLine("            if (output == null)");
            sb.AppendLine("                output = new T();");
            sb.AppendLine();
            sb.AppendLine("            BASETYPE.CastInto(input, output);");
            sb.AppendLine();

            string[] righe = codicePropieta.Trim().Split('\n');
            foreach(string riga in righe)
            {
                if (string.IsNullOrWhiteSpace(riga)) { continue; }
                string[] rigaSplittata = riga.Trim().Split(' ');
                if(rigaSplittata.Length > 2 && rigaSplittata[2].StartsWith("{"))
                { sb.AppendLine($"            output.{rigaSplittata[1]}=input.{rigaSplittata[1]};"); }
                else if (rigaSplittata.Length > 3 && rigaSplittata[0] == "public" && rigaSplittata[3].StartsWith("{"))
                { sb.AppendLine($"            output.{rigaSplittata[2]}=input.{rigaSplittata[2]};"); }
            }

            sb.AppendLine();
            sb.AppendLine("            return output;");
            sb.AppendLine("        }");

            return sb.ToString();
        }
    }

}
