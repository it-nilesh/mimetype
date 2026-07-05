using MimeType;
using System;
namespace MimeTypeTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //Adding Resource file 
            Mime.AddMimeResources(typeof(MimeType));

            //Using static method get mime Type From the Resources
            string name = Mime.Get("ami");
            Console.WriteLine(" ami = " + name);

            IMime mimeType = new Mime();
            var mimeType1 = mimeType.Get("3dml");
            Console.WriteLine(" 3dml = " + mimeType1);

            var mimeType2 = mimeType["z3"];
            Console.WriteLine(" z3 = " + mimeType2);

            Console.WriteLine(" avif = " + Mime.Get("image.avif"));
            Console.WriteLine(" wasm = " + Mime.Get("module.wasm"));
            Console.WriteLine(" zst = " + Mime.Get("archive.zst"));
            Console.WriteLine(" unknown = " + Mime.GetOrDefault("file.unknown"));

            Console.WriteLine("Hello World!");
        }
    }
}
