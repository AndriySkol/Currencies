﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using EntityFramework.BulkInsert.Extensions;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity;
using System.Threading;


namespace Currency_Interlogic
{
    public class DatabaseProxy : IObservable
    {
        static DatabaseProxy data = new DatabaseProxy();

        public Database database = new Database();
        List<IObserver> observers = new List<IObserver>();
        static public DatabaseProxy Data
        {
            get { return data; }
            set { data = value; }
        }
        bool isReady = false;
        public currencyEntities DataBaseRef { get { return database.DataBaseRef; } set { database.DataBaseRef = value; } }

        public bool IsReady
        {
            get { return database.IsReady; }
        }



        public List<Names> Names
        {
            get { return database.Names; }
            set { database.Names = value; }
        }
        public List<currencies> Currencies
        {
            get { return database.Currencies; }
            set { database.Currencies = value; }
        }

        DatabaseProxy()
        {
            loadBase();

        }
        async void loadBase()
        {
            database = await Task.Factory.StartNew<Database>((Object i) =>
            {
                Database newdata = new Database();
                newdata.LoadFromBase();
                return newdata;
            }, 4);
            NotifyMessengers();
        }
      
        public void Subscribe(IObserver observ)
        {
            observers.Add(observ);

        }

        public void Unscribe(IObserver observ)
        {
            observers.Remove(observ);
        }

        public void NotifyMessengers()
        {
            foreach (var x in observers)
            {
                x.UpdatedObservable();
            }
        }
    }
}
