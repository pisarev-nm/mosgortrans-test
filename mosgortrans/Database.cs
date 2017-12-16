using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace mosgortrans
{
    //класс для работы с БД
    class Database
    {
        //метод для вытягивания данных, возвращающий готовый датасет. Параметр - дата, на которую требуется узнать плановый выход
        public DataSet getData(string data)
        {
            //соединение с источником
            SqlConnection con = new SqlConnection("Data Source=NIKOLAY-PC\\SERVER1;Initial Catalog=mosgortrans;User ID=sa;Password=Nikolay1994");
            con.Open();
            //sql команда
            SqlCommand com = new SqlCommand();
            //строковая переменная, которая будет содержать запрос
            String sqlquery = "";
            //последовательное добавление в неё кусков запроса - в несколько строчек для наглядности и удобства редактирования при необходимости
            sqlquery += "select q2.num [Номер маршрута], COUNT(*) [Плановый выход] from";
            sqlquery += "(";
            sqlquery += "select distinct q1.[Номер маршрута] num, q1.gr ent  from";
            sqlquery += "(";
            sqlquery += "select count(*) counted, t.route_id [Номер маршрута], t.trip_id tr, s.grafic gr from STOP_TIMES s INNER JOIN (TRIPS t INNER JOIN CALENDAR c ON t.route_id=c.route_id) ON s.trip_id = t.trip_id where";
            sqlquery += "(";
            sqlquery += "(c.start_date<'" + data + "') AND";
            sqlquery += "((c.end_date>'" + data + "') OR (c.end_date is null)) AND";
            sqlquery += "(";
            sqlquery += "(DATEPART(dw,'" + data + "') = 0) AND (c.sunday=1) OR";
            sqlquery += "(DATEPART(dw,'" + data + "') = 1) AND (c.monday=1) OR";
            sqlquery += "(DATEPART(dw,'" + data + "') = 2) AND (c.tuesday=1) OR";
            sqlquery += "(DATEPART(dw,'" + data + "') = 3) AND (c.wednesday=1) OR";
            sqlquery += "(DATEPART(dw,'" + data + "') = 4) AND (c.thursday=1) OR";
            sqlquery += "(DATEPART(dw,'" + data + "') = 5) AND (c.friday=1) OR";
            sqlquery += "(DATEPART(dw,'" + data + "') = 6) AND (c.saturday=1)";
            sqlquery += ")";
            sqlquery += "AND (t.trip_short_name NOT LIKE 'Ч%')";
            sqlquery += ")";
            sqlquery += "group by t.route_id, t.trip_id, s.grafic";
            sqlquery += ") q1 ) q2 group by q2.num order by q2.num asc";
            com.CommandText = sqlquery;
            com.Connection = con;

            //передача запроса в адаптер
            SqlDataAdapter adapter = new SqlDataAdapter(com);

            DataSet ds = new DataSet("dataset");

            //наполняем датасет данными из адаптера
            adapter.Fill(ds);
            con.Close();

            return ds;
        }
    }
}
