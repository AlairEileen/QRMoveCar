using System;
using System.Collections.Generic;
using System.Text;

namespace We7Tools.MysqlTool
{
    //public class MysqlDBTool
    //{
    //    //public static string ConnectionString = "server=rm-8vb4q63qtp52c4i65o.mysql.zhangbei.rds.aliyuncs.com;database=we7;uid=yingketong_zhang;pwd=lizidan1984@;charset='utf8';SslMode=None";
    //    public static string ConnectionString = "server=47.94.56.159;database=we7_dev;uid=root;pwd=f73fce430e8e5f75;charset='utf8';SslMode=None";

    //    internal static MySqlConnection GetConnection()
    //    {
    //        return new MySqlConnection(ConnectionString);
    //    }
    //    public List<TestUser> GetTestUserList()
    //    {
    //        List<TestUser> list = new List<TestUser>();
    //        //连接数据库
    //        using (MySqlConnection msconnection = GetConnection())
    //        {
    //            msconnection.Open();
    //            //查找数据库里面的表
    //            MySqlCommand mscommand = new MySqlCommand("select * from user", msconnection);
    //            using (MySqlDataReader reader = mscommand.ExecuteReader())
    //            {
    //                //读取数据
    //                while (reader.Read())
    //                {
    //                    list.Add(new TestUser()
    //                    {
    //                        UserID = reader.GetInt32("userID"),
    //                        UserName = reader.GetString("userName"),
    //                        UserPwd = reader.GetString("userPwd")
    //                    });
    //                }
    //            }
    //        }
    //        return list;
    //    }

    //}
    //public class TestUser
    //{
    //    public int UserID { get; set; }
    //    public string UserName { get; set; }
    //    public string UserPwd { get; set; }
    //}
}
