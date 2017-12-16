using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace mosgortrans
{
    public partial class Form1 : Form
    {
        private string data;
        private BindingSource bindsourse;
        public Form1()
        {
            InitializeComponent();
            //Получние данных на основе даты по умолчанию (сегодня)
            viewData();            
        }


        // метод с помощью объекта класса Database получает данные и отображает их в dataGridView1
        // такой своеобразный предпросмотр перед экспортом данных в формат csv
        private void viewData()
        {
            data = getDate(monthCalendar1);
            label1.Text = data;
            bindsourse = new BindingSource();
            Database db1 = new Database();
            bindsourse.DataSource = db1.getData(data).Tables[0];
            dataGridView1.DataSource = bindsourse;
        }

        //Экспорт полученных данных в файл
        //После выбора расположения файла и его имени и подтверждения этого выбора происходит запись данных 
        //с помощью метода createfile. Параметром выступает объект класса SaveFileDialog
        private void button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Сохранить cvs-файл";
            sfd.InitialDirectory = "C:\\Users\\Nikolay\\Documents\\Visual Studio 2013\\Projects\\mosgortrans\\mosgortrans";
            sfd.FileName = data;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                createFile(sfd);
            }   
        }
        
        //метод на основе выбранной в элементе MonthCalendar даты возвращает её в виде строки в используемом базой данных формате ГГГГ-ММ-ДД, 
        //при необходимости добавляя нули в тех случаях, когда месяц и день содержат 1 разряд
        private String getDate(MonthCalendar selectedDate)
        {
            string date;

            string month;
            string year;
            string day;

            if (selectedDate.SelectionRange.Start.Month <10)
            {
                month = "0" + selectedDate.SelectionRange.Start.Month;
            }
            else
            {
                month = selectedDate.SelectionRange.Start.Month.ToString();
            }

            if (selectedDate.SelectionRange.Start.Day < 10)
            {
                day = "0" + selectedDate.SelectionRange.Start.Day;
            }
            else
            {
                day = selectedDate.SelectionRange.Start.Day.ToString();
            }

            
            year = monthCalendar1.SelectionRange.Start.Year.ToString();
            date = year + "-" + month + "-" + day;
            return date;
        }

        //Обновление данных при смене даты в MonthCalendar
        private void monthCalendar1_DateSelected(object sender, DateRangeEventArgs e)
        {
            viewData();
        }
        
        // запись данных из датасета в файл создаваемый savefile диалогом
        // в качестве разделителя столбцов ";"
        private void createFile(SaveFileDialog sfd)
        {
            Database db1 = new Database();

            using (System.IO.StreamWriter msw = new System.IO.StreamWriter(sfd.FileName))
            {
                msw.Write("route_id;plan;");
                msw.WriteLine();
                foreach (DataRow row in db1.getData(data).Tables[0].Rows)
                {
                    foreach (object item in row.ItemArray)
                    {
                        msw.Write(item.ToString() + ";");
                    }
                    msw.WriteLine();
                }
                Console.WriteLine("Finished");
                label2.Visible = true;
                label2.Text = "файл успешно создан!";
            }
        }
    }
}
