<%@ WebHandler Language="C#" Class="YZProductionDeviceServices" %>
//Converted
using System;
using System.Web;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using BPM;
using BPM.Client;
using YZSoft.Web.DAL;
using Newtonsoft.Json.Linq;

public class YZProductionDeviceServices : YZServiceHandler
{


    public void Delete(HttpContext context)
    {
        YZRequest request = new YZRequest(context);
        JArray jPost = request.GetPostData<JArray>();
        List<int> ids = jPost.ToObject<List<int>>();

        using (SqlConnection cn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["BPMDATA"].ConnectionString))
        {
            cn.Open();

            foreach (int id in ids)
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.CommandText = " update BPM_WGZP_KHLF_MF set If_Delete=1  where TaskID=@id";
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                cmd.ExecuteNonQuery();
            }
        }

    }


    public void Hf(HttpContext context)
    {
        YZRequest request = new YZRequest(context);
        JArray jPost = request.GetPostData<JArray>();
        List<int> ids = jPost.ToObject<List<int>>();

        using (SqlConnection cn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["BPMDATA"].ConnectionString))
        {
            cn.Open();

            foreach (int id in ids)
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.CommandText = " update BPM_WGZP_KHLF_MF set If_Delete=0  where TaskID=@id";
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                cmd.ExecuteNonQuery();
            }
        }

    }

    public JObject Zysq(HttpContext context)
    {

        YZRequest request = new YZRequest(context);
        //string searchType = context.Request.Params["SearchType"];
        //string keyword = context.Request.Params["Keyword"];
        string searchType = request.GetString("SearchType", null);
        string keyword = request.GetString("Kwd", null);

        SqlServerProvider queryProvider = new SqlServerProvider();
        //string year2 = context.Request.Params["year"];
        #region 后期模板里添加的
        GridPageInfo gridPageInfo = new GridPageInfo(context);//后期模板里添加的
        int rowNum = gridPageInfo.RowNumStart;

        JsonItemCollection tabs;
        tabs = new JsonItemCollection();

        StringCollection warehouseCodes = new StringCollection();

        string state = "";
        string state2 = "";
        #endregion

        //获得查询条件
        string filter = null;

        if (searchType == "QuickSearch")
        {
            //应用关键字过滤
            //&&QuickSearch
        }

            //应用记录权限-只显示有权查看的记录

        string loginuser = YZAuthHelper.LoginUserAccount;
        User user = new User();
        using (BPMConnection cn = new BPMConnection())
        {
            cn.WebOpen();

            //获得当前用户
            user.Open(cn, loginuser);
        }
        string SID = user.SID;

        //判断是否是管理员
        string str_group1 = "";

        string str_sql = "select  count(groupname) as a  from  BPMSecurityGroupMembers where SID='" + SID + "' and groupname='Administrators' ";
        System.Data.SqlClient.SqlDataReader dr = SqlHelper.ExecuteReader(System.Configuration.ConfigurationManager.ConnectionStrings["BPMDB"].ToString(), System.Data.CommandType.Text, str_sql);
        if (dr.Read())
        {
            str_group1 = dr["a"].ToString();

        }
        dr.Close();
        //判断是否再看所有组
        string str_group2 = "";

        string str_sql2 = "select  count(groupname) as b  from  BPMSecurityGroupMembers where SID='" + SID + "' and groupname='' ";
        System.Data.SqlClient.SqlDataReader dr2 = SqlHelper.ExecuteReader(System.Configuration.ConfigurationManager.ConnectionStrings["BPMDB"].ToString(), System.Data.CommandType.Text, str_sql2);
        if (dr2.Read())
        {
            str_group2 = dr2["b"].ToString();

        }
        dr2.Close();

        if (!String.IsNullOrEmpty(filter))
        {
            if (str_group1 == "1" || str_group2 == "1")
            {
                filter = " WHERE 1=1 and " + filter;
            }
            else
            {
                filter = " WHERE (UserCode in (select useraccount from dbo.getChildpeople('" + loginuser + "'))) or (UserCode ='" + loginuser + "') and " + filter;
            }

        }
        else
        {
            if (str_group1 == "1" || str_group2 == "1")
            {
                filter = " WHERE  1=1";
            }
            else
            {
                filter = " WHERE (UserCode in (select useraccount from dbo.getChildpeople('" + loginuser + "'))) or (UserCode ='" + loginuser + "')";
            }

        }


        //获得排序子句
        //string order = queryProvider.GetSortString("id");
        string order = "TaskID desc";

       
        string query = @"
            WITH X AS(
                SELECT ROW_NUMBER() OVER(ORDER BY {0}) AS RowNum,* FROM &&TableName {1}
            ),
            Y AS(
                SELECT count(*) AS TotalRows FROM X
            ),
            Z AS(
                SELECT Y.TotalRows,X.* FROM Y,X
            )
            SELECT * FROM Z WHERE RowNum BETWEEN {2} AND {3}
        ";
        query = String.Format(query, order, filter, request.RowNumStart, request.RowNumEnd);

       
        JObject rv = new JObject();
        using (SqlConnection cn = new SqlConnection())
        {
            cn.ConnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["BPMCEDATA"].ConnectionString;
            cn.Open();

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = cn;
                cmd.CommandText = query;

                using (YZReader reader = new YZReader(cmd.ExecuteReader()))
                {
                    //将数据转化为Json集合
                    JArray children = new JArray();
                    rv["children"] = children;
                    int totalRows = 0;

                    while (reader.Read())
                    {
                        JObject item = new JObject();
                        children.Add(item);

                        if (totalRows == 0)
                            totalRows = reader.ReadInt32("TotalRows");

                           //&&ItemAttribute


                        if (reader.ReadInt32("Flag") == 0)
                        {
                            item["Flag"] = "审批中";
                        }
                        else if (reader.ReadInt32("Flag") == 1)
                        {

                            item["Flag"] = "已审批";
                        }
                        else if (reader.ReadInt32("Flag") == 2)
                        {

                            item["Flag"] = "已拒绝";
                        }






                    }

                    rv[YZJsonProperty.total] = totalRows;
                }
            }
        }
        return rv;



    }
}