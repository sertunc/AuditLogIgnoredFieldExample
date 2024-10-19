using System;
using System.Linq;
using System.Text.Json;

namespace AuditLogIgnoredFieldExample
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var student = new StudentAuditDecorator
            {
                Name = "John",
                Address = "123 Main St.",
                Photo = new byte[] { 0x01, 0x02, 0x03 }
            };

            DataBaseService.SaveDb(student);

            Console.ReadLine();
        }
    }

    public class Student
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public byte[] Photo { get; set; }
    }

    public class StudentAuditDecorator : Student
    {
        // Photo property'sini override ediyoruz
        public new byte[] Photo
        {
            get
            {
                // Stack trace'i kontrol ediyoruz
                var stackTrace = new System.Diagnostics.StackTrace();
                var stackTraceMethodNameList = stackTrace.GetFrames().Select(x => x.GetMethod()?.Name).ToArray();
                var isAuditMethod = stackTrace.GetFrames().Any(frame => frame.GetMethod()?.Name == "SaveAuditLog");

                return isAuditMethod ? null : base.Photo;
            }
            set => base.Photo = value;
        }
    }

    //This is external class.
    public static class DataBaseService
    {
        public static void SaveDb(object value)
        {
            string jsonValue = JsonSerializer.Serialize(value);
            Console.WriteLine(jsonValue);

            SaveAuditLog(value);
        }

        private static void SaveAuditLog(object value)
        {
            string jsonValue = JsonSerializer.Serialize(value);
            Console.WriteLine(jsonValue);
        }
    }
}