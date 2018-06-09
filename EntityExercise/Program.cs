using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Net;

namespace EntityExercise
{
    class Program
    {
        public static MCSEntities db = new MCSEntities();
        public static UserHistory record;
        static void Main(string[] args)
        {
            AddDataToUserHistoryRecord();
            //Task1Eager();
            //Task2Explicit();
            //Task3Lazy();
            Transaction();
            showInfo("192.168.110.64");
            Console.ReadKey();
            record.LeaveTime = DateTime.Now;
            db.UserHistory.Add(record);
            db.SaveChanges();
        }


        public static void AddDataToUserHistoryRecord()
        {
            record = new UserHistory();
            string host = Dns.GetHostName();
            // Получение ip-адреса.
            IPAddress ip = Dns.GetHostByName(host).AddressList[0];
            record.UserIP = ip.ToString();
            record.EnterTime = DateTime.Now;
        }
        public static void Task1Eager()
        {
            List<newEquipment> neList = new List<newEquipment>();
            neList = db.newEquipment.Include(c => c.TablesModel).Include(s => s.TablesManufacturer).ToList();
            foreach (var item in neList)
            {
                Console.WriteLine($"{item.intGarageRoom,4}-{item.TablesModel.strName,7}-{item.strSerialNo,10}-{item.TablesManufacturer.strName,20}");
            }
        }
        public static void Task2Explicit()
        {
            List<newEquipment> neList = new List<newEquipment>();
            foreach (var item in db.newEquipment)
            {
                db.Entry(item).Reference(s => s.TablesModel).Load();
                db.Entry(item).Reference(s => s.TablesManufacturer).Load();
                Console.WriteLine($"{item.intGarageRoom,4}-{item.TablesModel.strName,7}-{item.strSerialNo,10}-{item.TablesManufacturer.strName,20}");
            }

            foreach (var item in neList)
            {
                Console.WriteLine($"{item.intGarageRoom,4}-{item.TablesModel.strName,7}-{item.strSerialNo,10}-{item.TablesManufacturer.strName,20}");
            }
        }
        public static void Task3Lazy()
        {
            foreach (var item in db.newEquipment)
            {
                Console.WriteLine($"{item.intGarageRoom,4}-{item.TablesModel.strName,7}-{item.strSerialNo,10}-{item.TablesManufacturer.strName,20}");
            }
        }  
        public static void Transaction()
        {
            using (var dbContextTransaction=db.Database.BeginTransaction())
            {
                try
                {

                    throw null;
                    db.SaveChanges();
                    dbContextTransaction.Commit();
                    record.Comment = "Successful transaction";
                }
                catch(Exception ex)
                { 
                    record.Comment =ex.Message;
                    dbContextTransaction.Rollback();
                }
            }
        }
        
        public static void showInfo(string ip)
        {
            var query = db.UserHistory.Where(w => w.UserIP == ip).ToList();

            if (query.Count > 0)
            {
                foreach (var item in query)
                {
                    Console.WriteLine($"{item.UserHistoryId}-{item.EnterTime}-{item.LeaveTime}-{item.Comment}");
                }
            }
            else
            {
                Console.WriteLine("No data for this IP");
            }
         
        }
           
    }
}
