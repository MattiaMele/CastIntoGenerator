using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CastIntoGeneratorBiz
{
    public class Test1
    {
        public int intero { get; set; }
        public double? doubles { get; set; }
        public string stringa { get; set; }
        public List<int> ListaInt { get; set; }

    }
    public class Test2 : Test1
    {
        public Guid[] GuidArray { get; set; }
        public Test1 ComplexType { get; set; }
    }
    public class Test3 : Test2
    {
        public List<Test1> ListaOggetti { get; set; }
        public Test1[] ArrayOggetti { get; set; }
    }

    public static class Extensions
    {
        public static T CastInto<T>(this Test1 input, T output = default) where T : Test1, new()
        {
            if (output == null)
                output = new T();

            output.intero = input.intero;
            output.doubles = input.doubles;
            output.stringa = input.stringa;
            output.ListaInt = input.ListaInt.ToList();

            return output;
        }
        public static T CastInto<T>(this Test2 input, T output = default) where T : Test2, new()
        {
            if (output == null)
                output = new T();

            (input as Test1).CastInto(output);

            output.GuidArray = input.GuidArray.ToArray();
            output.ComplexType = input.ComplexType.CastInto(new Test1());

            return output;
        }

        public static T CastInto<T>(this Test3 input, T output = default) where T : Test3, new()
        {
            if (output == null)
                output = new T();

            (input as Test2).CastInto(output);

            output.ListaOggetti = input.ListaOggetti.Select(x => x.CastInto(new Test1())).ToList();
            output.ArrayOggetti = input.ArrayOggetti.Select(x => x.CastInto(new Test1())).ToArray();

            return output;
        }

    }
}
