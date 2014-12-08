using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Currency_Interlogic
{
    public partial class DatabaseTable : Form, IObserver
    {
        public DatabaseTable()
        {

            InitializeComponent();
            this.FormClosing += Graph_FormClosing;
            DatabaseProxy.Data.Subscribe(this);
            CreateTable();
            if (DatabaseProxy.Data.IsReady)
            {
                labelDatabaseIsntReady.Hide();
                progressBar1.Hide();
            }

        }
        void Graph_FormClosing(Object sender, FormClosingEventArgs e)
        {
            DatabaseProxy.Data.Unscribe(this);
        }
        public void CreateTable()
        {
            if (DatabaseProxy.Data.IsReady)
            {
                mainTable.AutoGenerateColumns = true;
                var querry = (from el in DatabaseProxy.Data.Currencies
                              where el.ID <= index + 500 && el.ID >= index
                              select new
                              {
                                  ID = el.ID,
                                  Name = DatabaseProxy.Data.Names.Where(b => b.ID == el.Name).Select(b => b.name).FirstOrDefault(),
                                  Value = el.Value,
                                  Change = el.Change,
                                  ChangePercent = el.ChangePercent,
                                  IsChangeDown = el.IsChangeDown,
                                  Date = el.Date
                              });
                mainTable.DataSource = querry.ToList();
            }
        }

        private void aheadMove_Click(object sender, EventArgs e)
        {
            if (index < maxIndex)
            {
                index += 500;
                CreateTable();

                mainTable.Update();
            }
        }

        private void backMove_Click(object sender, EventArgs e)
        {
            if (index > 0)
            {
                index -= 500;
                CreateTable();
                mainTable.Update();
            }
        }

        public void UpdatedObservable()
        {
            Invoke((MethodInvoker)delegate
            {
                if (DatabaseProxy.Data.IsReady)
                {
                    labelDatabaseIsntReady.Hide();
                    progressBar1.Hide();
                }
                progressBar1.Value = DatabaseProxy.Data.ProgressLoad;
                CreateTable();
                Update();
            });
        }
    }
}
