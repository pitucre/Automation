using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using ForECC.Helper;

namespace ForECC
{


    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }



        private void Form1_Load(object sender, EventArgs e)
        {


            XmlHelper xml = new XmlHelper();


            //int counter = 0;
            //string line;




            //System.IO.StreamReader file =
            //    new System.IO.StreamReader(@"D:\Source\ECC\FlowPortal BPM 6.x\WEB\YZModules\ITProcess\StoreDataService\gonggao_Data.ashx.exclude");
            //while ((line = file.ReadLine()) != null)
            //{
            //    //System.Console.WriteLine(line);
            //    rTBoxOrigin.Text = rTBoxOrigin.Text + line + "\r\n";

            //    //rTBoxTarget.Text = rTBoxTarget.Text;
            //    if (line.Contains(".Attributes.Add"))

            //    {
            //        //iLeft = line.IndexOf("Attributes");
            //        line = line.Replace(".Attributes.Add(", "[");
            //        line = line.Replace(",", "]=");
            //        line = line.Replace("))", ")");
            //        rTBoxTarget.Text = rTBoxTarget.Text + line + "\r\n";


            //        rTBoxOrigin.Select(rTBoxOrigin.GetFirstCharIndexFromLine(counter), line.Length);
            //        rTBoxOrigin.SelectionColor = Color.Red;
            //        rTBoxOrigin.SelectionColor = Color.Red;
            //    }
            //    else
            //    {
            //        rTBoxTarget.Text = rTBoxTarget.Text + line + "\r\n";
            //    }
            //    counter++;
            //}

            //file.Close();

        }
        /// <summary>
        /// 自动转化js文件为新格式
        /// </summary>
        /// <param name="strOriginFilePath">旧文件路径名字</param>
        /// <param name="strProcessName">旧文件名字</param>
        /// <param name="strProcessGroup">文件group</param>
        /// <param name="strProcessNameCN">文件调用的ProcessName</param>
        private void FormatJSFile(string strOriginFilePath, string strProcessName, string strProcessGroup, string strProcessNameCN = "")
        {
            if (strProcessName.Contains("backup"))//如果已经备份的文件不再处理
            {
                return;
            }
            #region 提取文件中的有用信息
            int counter = 0;
            string line;
            string strContent = "";//文本文件内容

            int iColumnsStart, iColumnsEnd;
            int iProcessNameStart, iProcessNameEnd;//记录窗口位置
            int iFormApplicationStart, iFormApplicationEnd;//记录表单Form位置
            string strOpenPostWindow = "", strPasteContent = "", strContructItems = "";
            string strOpenFormApplication = "";

            //读取文件的内容并保存起来

            using (StreamReader file = new StreamReader(strOriginFilePath))
            {
                while ((line = file.ReadLine()) != null)
                {
                    //特殊注释删掉
                    if (line.Trim().Length >= 2 && line.Trim().Substring(0, 2) == "//" && (line.Contains("YZSoft.BPM.FormManager.Open") || line.Contains("header") || line.Contains("autoExpandColumn")))
                    {
                        Console.WriteLine("注释语句删除");

                    }
                    else
                    {
                        strContent = strContent + line;
                    }

                    counter++;
                }

                file.Close();
            }

            if (strContent.Contains("Ext.define"))
            {
                //ltBoxConverted.Items.Add(strProcessName + ".js已更新过！");
                WriteLog(strProcessName, ltBoxConverted, ".js已更新过！");
                return;
            }
            //提取文件中的有用信息

            string strPatternHeader = "{ header";
            iColumnsStart = strContent.IndexOf(strPatternHeader);

            if (iColumnsStart == -1)//如果找不到
            {
                strPatternHeader = "{header";
                iColumnsStart = strContent.IndexOf("{header");
                if (iColumnsStart == -1)
                {
                    return;
                }
            }
            iColumnsEnd = strContent.IndexOf("bbar");

            strPasteContent = strContent.Substring(iColumnsStart, iColumnsEnd - iColumnsStart - 1).Replace("//", "\r\n//").Replace("],", "");//要粘贴的内容

            foreach (string strHeader in strPasteContent.Split(strPatternHeader))
            {
                if (!string.IsNullOrEmpty(strHeader))
                {
                    strContructItems = strContructItems + "{ header" + strHeader + "\r\n";
                }

            }

            strPasteContent = strContructItems.TrimEnd(',');

            string tag = "OpenPostWindow('";
            //string tag = "OpenFormApplication('";

            iProcessNameStart = strContent.IndexOf(tag);

            if (iProcessNameStart < 0)
            {
                WriteLog(strProcessName, ltBoxNoExistString, "未发现Window" + tag);
                //return;
            }
            else
            {
                iProcessNameEnd = strContent.IndexOf("', '',");
                strOpenPostWindow = strContent.Substring(iProcessNameStart + tag.Length, iProcessNameEnd - iProcessNameStart - tag.Length);

            }

            tag = "OpenFormApplication('";
            iFormApplicationStart = strContent.IndexOf(tag);
            if (iFormApplicationStart < 0)
            {
                //MessageBox.Show("没有发现字符串" + tag + "请确认！");
                //ltBoxNoExistString.Items.Add(strProcessName + "未发现Form 或者Window" + tag);
                WriteLog(strProcessName, ltBoxNoExistString, "未发现Form" + tag);
                return;
            }
            else
            {
                iFormApplicationEnd = strContent.IndexOf("',", iFormApplicationStart);
                strOpenFormApplication = strContent.Substring(iFormApplicationStart + tag.Length, iFormApplicationEnd - iFormApplicationStart - tag.Length);
            }

            //string strPattern = "[\u4E00-\u9FA5]{0,}";
            //foreach (Match match in Regex.Matches(strContent, strPattern))
            //    strOpenPostWindow = match.Value.Replace("OpenPostWindow('", "");
            ////正则查找
            ////string msg = string.Empty;
            //string tag = "OpenPostWindow('";
            //strOpenPostWindow = System.Text.RegularExpressions.Regex.Match(strContent, tag + strPattern).Value;
            #endregion

            FileInfo fi = new FileInfo(strOriginFilePath);
            strOriginFilePath = strOriginFilePath.Replace("_data", "", StringComparison.OrdinalIgnoreCase); //如果名字里面有_data,把_data删除
            fi.MoveTo(strOriginFilePath + "_backup"); //文件重命名
            string strModelJSFile;//模板JS文件

            strModelJSFile = Directory.GetCurrentDirectory() + "\\ITInResRequest.js";
            File.Copy(strModelJSFile, strOriginFilePath);
            //File.Copy(strOriginFilePath, strModelJSFile);//


            //      file =
            //new System.IO.StreamReader(strNewFileName);

            string strNewContent = "";

            using (StreamReader sr = new StreamReader(strOriginFilePath))
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Contains("&&ProcessName") || line.Contains("&&ProcessGroup"))
                    {
                        strNewContent = strNewContent + line.Replace("&&ProcessName", strProcessName.Replace(".js", "")).Replace("&&ProcessGroup", strProcessGroup) + "\r\n";
                    }
                    else if (line.Contains("&&PNameCN"))
                    {
                        strNewContent = strNewContent + line.Replace("&&PNameCN", strOpenPostWindow) + "\r\n";
                    }
                    else if (line.Contains("&&FormService"))
                    {
                        strNewContent = strNewContent + line.Replace("&&FormService", strOpenFormApplication) + "\r\n";
                    }
                    else if (line.Contains("//&&ItemList"))
                    {
                        strNewContent = strNewContent + line.Replace("//&&ItemList", strPasteContent) + "\r\n";
                    }
                    else
                    {
                        strNewContent = strNewContent + line + "\r\n";
                    }

                    //counter++;
                }


            using (StreamWriter sw = new StreamWriter(strOriginFilePath))
            {
                sw.Write(strNewContent);
                sw.Close();

                WriteLog(strProcessName + ".js", ltBoxModules);

                //this.ltBoxModules.Items.Add(strProcessName + ".js");

                //string logFileName = System.Environment.GetEnvironmentVariable("TEMP") + "\\" + Path.GetFileNameWithoutExtension(Application.ExecutablePath) + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
                //using (TextWriter logFile = TextWriter.Synchronized(File.AppendText(logFileName)))
                //{
                //    logFile.WriteLine(DateTime.Now + "\t" + strProcessName + ".js" + "\r\n");
                //    logFile.Flush();
                //    logFile.Close();
                //}
            }

        }


        private void MakeJSFile(string strOriginFilePath, string strProcessName, string strProcessGroup, string strProcessNameCN = "")
        {

            string strModelJSFile;//模板JS文件

            strModelJSFile = Directory.GetCurrentDirectory() + "\\ITInResRequest.js";
            File.Copy(strModelJSFile, strOriginFilePath);


            string strNewContent = "";
            string line = "";

            using (StreamReader sr = new StreamReader(strModelJSFile))
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Contains("&&ProcessName") || line.Contains("&&ProcessGroup"))
                    {
                        strNewContent = strNewContent + line.Replace("&&ProcessName", strProcessName.Replace(".js", "")).Replace("&&ProcessGroup", strProcessGroup) + "\r\n";
                    }
                    else if (line.Contains("&&PNameCN"))
                    {
                        strNewContent = strNewContent + line.Replace("&&PNameCN", txtBoxWindow.Text) + "\r\n";
                    }
                    else if (line.Contains("&&FormService"))
                    {
                        strNewContent = strNewContent + line.Replace("&&FormService", txtBoxForm.Text) + "\r\n";
                    }
                    //else if (line.Contains("//&&ItemList"))
                    //{
                    //    strNewContent = strNewContent + line.Replace("//&&ItemList", strPasteContent) + "\r\n";
                    //}
                    else
                    {
                        strNewContent = strNewContent + line + "\r\n";
                    }

                    //counter++;
                }


            using (StreamWriter sw = new StreamWriter(strOriginFilePath))
            {
                sw.Write(strNewContent);
                sw.Close();

                WriteLog(strProcessName + ".js", ltBoxModules);

                //this.ltBoxModules.Items.Add(strProcessName + ".js");

                //string logFileName = System.Environment.GetEnvironmentVariable("TEMP") + "\\" + Path.GetFileNameWithoutExtension(Application.ExecutablePath) + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
                //using (TextWriter logFile = TextWriter.Synchronized(File.AppendText(logFileName)))
                //{
                //    logFile.WriteLine(DateTime.Now + "\t" + strProcessName + ".js" + "\r\n");
                //    logFile.Flush();
                //    logFile.Close();
                //}
            }

        }


        private void FormatAspxFile(string strOriginFilePath, string strProcessName, string strProcessGroup, string strProcessNameCN = "")
        {
            if (strProcessName.Contains("_backup"))//如果已经备份的文件不再处理
            {
                return;
            }

            int iTableNameStart, iTableNameEnd;//数据库表的位置
            string strTableName = "";//数据库表名
            string line;
            string strContent = "";//文本文件内容

            int iCompanyFlag;
            int iCompanyNameStart, iCompanyNameEnd;//记录Company出现的位置

            string strReplaceContent = "";

            string strFrontContent = "";//上半部分内容
            string strBelowContent = "";//下班部分内容

            string strNewContent = "";//用于存储新内容

            string strTag = "";

            FileEncoding fEncoding = new FileEncoding();
            //读取文件的内容并保存起来

            ////创建XML文档类
            //XmlDocument xmlDoc = new XmlDocument();
            ////加载xml文件
            //xmlDoc.Load(strOriginFilePath); //从指定的位置加载xml文档
            ////获取根节点
            //XmlElement xmlRoot = xmlDoc.DocumentElement; //DocumentElement获取文档的跟
            ////遍历节点
            //foreach (XmlNode node in xmlRoot.ChildNodes)
            //{

            //    //根据节点名称查找节点对象
            //    Console.WriteLine(node["channelType"].InnerText + "\t" + node["tvChannel"].InnerText + "\t" + node["path"].InnerText);
            //}
            if (!File.Exists(strOriginFilePath + "_backup"))
            {
                File.Copy(strOriginFilePath, strOriginFilePath + "_backup");//备份文件
            }
            #region 循环替换标签透明和修改JS语法模块

            //4种编码  
            Encoding utf8 = Encoding.UTF8;
            Encoding utf16 = Encoding.Unicode;
            //Encoding gb = Encoding.GetEncoding("gbk");
            //Encoding b5 = Encoding.GetEncoding("big5");

            Encoding strFileEncoding;
            strFileEncoding = FileEncoding.GetEncoding(strOriginFilePath);
            FileStream fsr = new FileStream(strOriginFilePath, FileMode.Open, FileAccess.Read);
            using (StreamReader file = new StreamReader(strOriginFilePath, strFileEncoding))//超级重要，C#字符串默认是Unicode，必须以Unicode格式打开
            //using (StreamReader file = new StreamReader(strOriginFilePath))
            {

                while ((line = file.ReadLine()) != null)
                {
                    //line = utf8.GetString(Encoding.Convert(utf16, utf8, utf16.GetBytes(line)));
                    if (line.Contains("--Converted"))//如果文件已经更新过跳出逻辑
                    {
                        //ltBoxConverted.Items.Add(strProcessName + ".ashx文件已更新过！");
                        WriteLog(strProcessName, ltBoxAspx, ".aspx文件已更新过！");

                        file.Close();
                        fsr.Close();
                        return;
                    }
                    //特殊注释删掉
                    if (line.Trim().Length >= 4 && line.Trim().Substring(0, 4) == "<%--")
                    {

                    }
                    else
                    {
                        if (line.Contains("<aspxform:XLabel ") && !line.Contains("BackColor", StringComparison.OrdinalIgnoreCase))//批量加背景
                        {
                            line = line.Replace("<aspxform:XLabel ", "<aspxform:XLabel BackColor=\"Transparent\" ").Trim();
                            //line =utf8.GetString(Encoding.Convert(utf16, utf8, utf16.GetBytes(line)));
                        }

                        //
                        if (line.Contains("<table ") && !line.Contains("width", StringComparison.OrdinalIgnoreCase))//批量加背景
                        {
                            line = line.Replace("<table ", "<table width=\"95%\"").Trim();
                            //line =utf8.GetString(Encoding.Convert(utf16, utf8, utf16.GetBytes(line)));
                        }

                        if (line.Contains("getElementsByName", StringComparison.OrdinalIgnoreCase) && line.Contains("value", StringComparison.OrdinalIgnoreCase))
                        {
                            line = line.Replace("document.getElementsByName", "$").Replace(").value", ").down('.yz-xform-field-ele').dom.value");
                        }

                        if (line.Contains("getElementById", StringComparison.OrdinalIgnoreCase) && line.Contains("innerHTML", StringComparison.OrdinalIgnoreCase))
                        {
                            line = line.Replace("document.getElementById", "Ext.get").Replace("\").innerHTML", ".yz-xform-field-ele\").html()");
                        }
                        if (line.Contains("getElementById", StringComparison.OrdinalIgnoreCase) && line.Contains("value", StringComparison.OrdinalIgnoreCase))
                        {
                            line = line.Replace("document.getElementById", "Ext.get").Replace(").value", ").down('.yz-xform-field-ele').dom.value");
                        }
                        if (line.Contains("getElementById", StringComparison.OrdinalIgnoreCase) && line.Contains("innerText", StringComparison.OrdinalIgnoreCase))
                        {
                            line = line.Replace("document.getElementById", "$").Replace("\").innerText", ".yz-xform-field-ele\").html()");
                        }
                        Console.WriteLine("注释语句删除");
                        strContent = strContent + line + Environment.NewLine;
                    }

                    //counter++;
                }

                file.Close();
                fsr.Close();

            }
            #endregion
            #region 删除审批意见模块
            //删除审批意见
            string strPattern = @"<table style=""MARGIN.*?>[\s\S]*?</table>";
            //strPattern = @"<table.*?>[\s\S]*?<\/table>";
            foreach (Match match in Regex.Matches(strContent, strPattern))
                if (match.Value.Contains("审批意见"))
                {
                    strContent = strContent.Replace(match.Value, "");
                    //strContent = utf8.GetString(Encoding.Convert(utf16, utf8, utf16.GetBytes(strContent)));

                    break;
                }




            //strContent.Replace(strReplaceContent, "");
            #endregion




            #region 更改函数体
            //要添加的函数体
            if (!strContent.Contains("GenExcelReport"))//如果没有替换过
            {
                string strNewFunction = @"
                                          Ext.require([""YZSoft.src.ux.File""]);
                                          function excel_JDE_Export_New()
                                                {
                                                    var reportServiceUrl = '../../../../YZSoft.Services.REST/Reports/Report.ashx';
                                                    var bm = '';
                                                    var params = { deptname: bm };

                                          params['Method'] = 'GenExcelReport';
                                          params['ExcelFile'] = '~/YZModules/ISProcess/&&&';
                                          params['outputType'] = 'Export';
                                                    var pms = new Array();
                                                    Ext.Object.each(params, function(key, val) {
                                                        if (key != 'UserParamNames')
                                                            pms.push(key);
                                                    });
                                          params['UserParamNames'] = pms.join(',');
                                                YZSoft.src.ux.File.download(reportServiceUrl, params);
                                            }
                                           ";

                strPattern = @"Excel/.*.xls";
                string strDownloadFileName = "";
                strDownloadFileName = Regex.Match(strContent, strPattern).Value;

                strNewFunction = strNewFunction.Replace("&&&", strDownloadFileName);

                strPattern = @"function excel.+{[\s\S]+}///";//特意加///表示结尾，替换之前加上///
                strContent = Regex.Replace(strContent, strPattern, strNewFunction);
            }

            #endregion

            #region 获取信息后添加部门信息模块
            if (!strContent.Contains("XLabe66"))//如果没有替换过
            {
                strTag = "BPMCEDATA:";
                iTableNameStart = strContent.IndexOf(strTag);

                if (iTableNameStart > 0)
                {
                    iTableNameEnd = strContent.IndexOf(".", iTableNameStart);

                    strTableName = strContent.Substring(iTableNameStart + strTag.Length, iTableNameEnd - iTableNameStart - strTag.Length);

                    #region 要添加的下面的部门替换片段
                    if (!strContent.Contains("XPositionMap1") && strContent.Contains("XProcessButtonList"))//如果没有替换过
                    {
                        //要添加的下面的部门替换片段
                        string strMendContent = @"
                                        <aspxform:XPositionMap id=""XPositionMap1"" runat=""server"" DataMap=""OUName->BPMCEDATA:{0}.CompanyName; ParentOUName->BPMCEDATA:{0}.{1}"" OULevel=""2级部门""></aspxform:XPositionMap>
                              <aspxform:XLabel BackColor=""Transparent"" id=""XLabe33"" runat=""server"" XDataBind=""BPMCEDATA: ISECCJDEUerInformation_M.Dept"" text=""Label"" ValueToDisplayText Value HiddenExpress=""1 == 1"" ></aspxform:XLabel>

                    ";

                        strPattern = @"\.[a-zA-Z]*Dept[a-zA-Z]*";
                        string strDept = "";
                        strDept = Regex.Match(strContent, strPattern).Value.Replace(".", "");

                        strMendContent = string.Format(strMendContent, strTableName, strDept);

                        strPattern = @"<aspxform:XProcessButtonList.*?>[\s\S]*?</aspxform:XProcessButtonList>";
                        strContent = Regex.Replace(strContent, strPattern, strMendContent);
                    }
                    #endregion

                    //要增加的部门名字代码
                    string strAddContent = @" 
                        <td width=""169"" class=""Col0"">
                        <aspxform:XLabel id=""XLabe66"" runat=""server"" XDataBind=""BPMCEDATA:{0}.CompanyName"" BorderColor=""Transparent"" BackColor=""Transparent""></aspxform:XLabel >
                        </td >
";

                    strAddContent = string.Format(strAddContent, strTableName);


                    //strAddContent = utf8.GetString(Encoding.Convert(utf16, utf8, utf16.GetBytes(strAddContent)));

                    iCompanyFlag = strContent.IndexOf("基本信息");
                    if (iCompanyFlag > 0)
                    {
                        iCompanyNameStart = strContent.IndexOf("</tr", iCompanyFlag);

                        strFrontContent = strContent.Substring(0, iCompanyNameStart);
                        strBelowContent = strContent.Substring(iCompanyNameStart, strContent.Length - iCompanyNameStart);

                        strNewContent = "<%--converted--%>" + Environment.NewLine + strFrontContent + Environment.NewLine + strAddContent + strBelowContent;
                        //strNewContent = utf8.GetString(Encoding.Convert(utf16, utf8, utf16.GetBytes(strNewContent)));
                    }
                    else
                    {
                        strNewContent = strContent;
                        //MessageBox.Show("没有发现字符串基本信息");
                        WriteLog(strProcessName + ".aspx", ltBoxNoExistString, "没有发现字符串基本信息,编码格式为" + strFileEncoding);

                    }
                }
                else
                {
                    //MessageBox.Show("没有发现" + strTag);
                    WriteLog(strProcessName + ".aspx", ltBoxNoExistString, "没有发现" + strTag);
                }

            }

            #endregion
            //正则查找



            FileStream fsw = new FileStream(strOriginFilePath, FileMode.Create, FileAccess.Write);//FileMode.Create 复制之前清空文件防止编码问题导致错误
            using (StreamWriter sw = new StreamWriter(fsw, strFileEncoding))
            //using (StreamWriter sw = new StreamWriter(strOriginFilePath))
            {

                sw.Write(strNewContent);

                //foreach (string s in strNewContent.Split("&&&"))
                //{
                //    sw.WriteLine(s);

                //}
                //sw.WriteAsync(strNewContent);
                sw.Close();
                fsw.Close();

                WriteLog(strProcessName + ".aspx", ltBoxAspx);

                //this.ltBoxModules.Items.Add(strProcessName + ".js");

                //string logFileName = System.Environment.GetEnvironmentVariable("TEMP") + "\\" + Path.GetFileNameWithoutExtension(Application.ExecutablePath) + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
                //using (TextWriter logFile = TextWriter.Synchronized(File.AppendText(logFileName)))
                //{
                //    logFile.WriteLine(DateTime.Now + "\t" + strProcessName + ".js" + "\r\n");
                //    logFile.Flush();
                //    logFile.Close();
                //}
            }



        }

        private void WriteLog(string strProcessName, ListBox ltBoxLog, string strMsg = "")
        {
            ltBoxLog.Items.Add(strProcessName + strMsg);

            ltBoxLogAll.Items.Add(strProcessName + strMsg);

            string logFileName = System.Environment.GetEnvironmentVariable("TEMP") + "\\" + Path.GetFileNameWithoutExtension(Application.ExecutablePath) + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
            using (TextWriter logFile = TextWriter.Synchronized(File.AppendText(logFileName)))
            {
                logFile.WriteLine(DateTime.Now + "\t" + strProcessName + "\r\n");
                logFile.Flush();
                logFile.Close();
            }

        }



        private void FormatAshxFile(string strOriginFilePath, string strProcessName, string strProcessGroup)
        {
            if (strProcessName.Contains("backup"))//如果已经备份的文件不再处理
            {
                return;
            }
            #region 提取文件中的有用信息
            string line;
            string strContent = "";//文本文件内容

            int iTableNameStart, iTableNameEnd;
            int iTableNameStartBk, iTableNameEndBk;

            int iItemStart, iItemEnd, iItemEndTemp;
            string strTableName = "", strPasteContent = "";
            string strTableNameBk = "";//特殊规则查找的tablename
            string tag = "count(*) from ";
            string strLastItem = "";
            int iQuickSearchStart, iQuickSearchEnd;
            string strQuickSearch = "";

            bool bFindTable = false;//判断是否发现表名
            bool bFindItem = false;//判断是否发现表名
            bool bFindQuickSearch = false;//判断是否有快速搜索
            //读取文件的内容并保存起来

            using (StreamReader file = new StreamReader(strOriginFilePath))
            {
                while ((line = file.ReadLine()) != null)
                {
                    //一些特殊注释语句删掉
                    if (line.Trim().Length >= 2 && line.Trim().Substring(0, 2) == "//" && (line.Contains("YZSoft.BPM.FormManager.Open") || line.Contains("header") || line.Contains("autoExpandColumn") || line.Contains("item.Attributes.Add")))
                    {
                        Console.WriteLine("注释语句删除");

                    }
                    else
                    {
                        if (line.Contains("//Converted") || line.Contains("item["))//如果文件已经更新过跳出逻辑
                        {
                            //ltBoxConverted.Items.Add(strProcessName + ".ashx文件已更新过！");
                            WriteLog(strProcessName, ltBoxConverted, ".ashx文件已更新过！");
                            file.Close();
                            return;
                        }



                        //是否发现item属性模块
                        //if (line.Contains("item.Attributes.Add") && !line.Contains("flag", StringComparison.OrdinalIgnoreCase) && !line.Contains("CapFlag", StringComparison.OrdinalIgnoreCase))
                        if (line.Contains("item.Attributes.Add") && (line.Contains("reader.", StringComparison.OrdinalIgnoreCase) || line.Contains("")))
                        //if (line.Contains("item.Attributes.Add"))

                        {
                            bFindItem = true;

                            if (line.Contains("reader.", StringComparison.OrdinalIgnoreCase))//如果没有空字符串
                            {
                                line = line.Replace(".Attributes.Add(", "[");
                                line = line.Replace(",", "]=");
                                line = line.Replace("))", ")");
                                //ToString("") reader
                            }
                            else if (line.Contains("reader[", StringComparison.OrdinalIgnoreCase))
                            {
                                line = line.Replace(".Attributes.Add(", "[");
                                line = line.Replace(",", "]=");
                                line = line.Replace("reader[", "reader.ReadString(");
                                if (line.Contains("?"))//含有三元运算是，特别处理
                                {
                                    if (line.Contains(").ToString"))//如果末尾有该字符串特殊处理
                                    {
                                        line = line.Replace("));", ");");//末尾字符处理
                                    }
                                    else
                                    {
                                        line = line.Replace("]));", "));");//末尾字符处理

                                    }

                                    line = line.Replace("]))", ")))");//中间字符处理
                                    line = line.Replace("])", "))");//中间字符处理
                                }
                                else
                                {
                                    line = line.Replace("])", ")");
                                }

                                //line = line.Replace("])", ")");
                            }
                            else//如果有空字符串
                            {
                                line = line.Replace(".Attributes.Add(", "[");
                                line = line.Replace(",", "]=");
                                line = line.Replace(")", "");
                            }


                            strPasteContent = strPasteContent + line + "\r\n ";

                            strLastItem = line;

                        }

                        strContent = strContent + line + "\r\n ";




                        //是否发现tableName 模块
                        if (line.Contains(tag) && !line.Contains("//"))//获取table的名字
                        {


                            iTableNameStart = line.IndexOf(tag);

                            if (iTableNameStart < 0)
                            {
                                MessageBox.Show("没有发现字符串" + tag + "请确认！");
                                return;
                            }
                            else
                            {
                                bFindTable = true;
                                //iTableNameEnd = line.IndexOf(" ", iTableNameStart + tag.Length + 1);//查找发现字符串后一个出现"{"的位置
                                iTableNameEnd = line.IndexOf("{", iTableNameStart);//查找发现字符串后一个出现"{"的位置
                                if (iTableNameEnd < 0)
                                {
                                    iTableNameEnd = line.IndexOf(" ", iTableNameStart + tag.Length + 1);//查找发现字符串后一个出现" "的位置
                                    strTableName = line.Substring(iTableNameStart + tag.Length, iTableNameEnd - iTableNameStart - tag.Length);

                                }
                                else
                                {
                                    strTableName = line.Substring(iTableNameStart + tag.Length, iTableNameEnd - iTableNameStart - tag.Length - 1);

                                }
                            }

                            //if (iTableNameEnd<iTableNameStart)//特殊处理如果在前面存在{,就找另一个
                            //{
                            //    iTableNameEnd = line.IndexOf("{2}\"");
                            //}

                            //if (iTableNameEnd < 0)
                            //{
                            //    MessageBox.Show("没有发现字符串" + "{" + "请确认！");
                            //    return;
                            //}
                            //strTableName = line.Substring(iTableNameStart + tag.Length, iTableNameEnd - iTableNameStart - tag.Length-1);
                        }

                        if (line.Contains("if (!string.IsNullOrEmpty(keyword))"))
                        {
                            bFindQuickSearch = true;
                        }

                    }






                }

                file.Close();
            }

            //如果没有找到tablename再使用特别的规则再搜索一次
            if (bFindTable == false)
            {
                using (StreamReader fileRepeat = new StreamReader(strOriginFilePath))
                {
                    while ((line = fileRepeat.ReadLine()) != null)
                    {
                        if (line.Trim().Length >= 2 && !(line.Trim().Substring(0, 2) == "//") && line.Contains("select", StringComparison.OrdinalIgnoreCase) && line.Contains("from", StringComparison.OrdinalIgnoreCase))
                        {
                            string tagBk = "from ";
                            iTableNameStartBk = line.IndexOf(tagBk, StringComparison.OrdinalIgnoreCase);
                            iTableNameEndBk = line.IndexOf(" ", iTableNameStartBk + tagBk.Length + 1);//查找发现字符串后一个出现"{"的位置
                            strTableNameBk = line.Substring(iTableNameStartBk + tagBk.Length, iTableNameEndBk - iTableNameStartBk - tagBk.Length);
                            bFindTable = true;
                            break;
                        }
                        else
                        {

                        }
                    }
                    fileRepeat.Close();
                }
            }
            if (bFindTable == false)
            {
                //ltBoxNoExistTableName.Items.Add(strProcessName + "未发现表名");
                WriteLog(strProcessName, ltBoxNoExistTableName, "未发现表名");
            }


            if (bFindItem == false && bFindTable == false)
            {
                //ltBoxNoExistTableName.Items.Add(strProcessName + "未发现item和表名");
                WriteLog(strProcessName, ltBoxNoExistTableName, "未发现item和表名");
                return;
            }

            if (bFindItem == true)//如果存在要粘贴的内容
            {
                iItemStart = strContent.IndexOf("item[");
                //iItemEnd = strContent.IndexOf("rowNum++");
                //iItemEndTemp = strContent.IndexOf("}", iItemStart);
                //if (iItemEnd < 0)//如果没有rowNum++用}好代替结尾
                //{
                //    iItemEnd = iItemEndTemp;
                //}
                //else
                //{
                //    //if (iItemEnd>iItemEndTemp)
                //    //{
                //    //    iItemEnd = iItemEndTemp;
                //    //}
                //}


                //strPasteContent = strContent.Substring(iItemStart, iItemEnd - iItemStart - 1);//要粘贴的内容

                iItemEnd = strContent.IndexOf(strLastItem);
                strPasteContent = strContent.Substring(iItemStart, iItemEnd - iItemStart + strLastItem.Length);

                string strPasteContentTemp = strPasteContent.Replace("//{", "").Replace("//}", "");
                if (strPasteContentTemp.Contains("{"))
                {
                    int iCountLeft = 0;
                    int iCountRight = 0;
                    for (int i = 0; i < strPasteContentTemp.Length; i++)
                    {
                        if (strPasteContentTemp[i] == '{')
                        {
                            iCountLeft++;
                        }
                        if (strPasteContentTemp[i] == '}')
                        {
                            iCountRight++;
                        }
                    }

                    if (iCountRight < iCountLeft)
                    {
                        strPasteContent = strPasteContent + "}";
                    }
                }


            }
            else
            {

                //ltBoxNoExistTableName.Items.Add(strProcessName + "未发现item");
                WriteLog(strProcessName, ltBoxNoExistTableName, "未发现item");

            }
            //提取文件中的有用信息


            if (bFindQuickSearch == true)
            {
                iQuickSearchStart = strContent.IndexOf("if (!string.IsNullOrEmpty(keyword))");
                tag = "queryProvider.EncodeText(keyword)));";
                iQuickSearchEnd = strContent.LastIndexOf(tag);

                strQuickSearch = strContent.Substring(iQuickSearchStart, iQuickSearchEnd + tag.Length - iQuickSearchStart);
                if (strQuickSearch.Contains("else"))
                {
                    strQuickSearch = strQuickSearch + @"
                    }";
                }

            }


            #endregion


            #region 有用信息复制到模板文件
            FileInfo fi = new FileInfo(strOriginFilePath);
            strOriginFilePath = strOriginFilePath.Replace("_data", "", StringComparison.OrdinalIgnoreCase).Replace(".exclude", "", StringComparison.OrdinalIgnoreCase); //如果名字里面有_data和.exclude,把_data和.exclude删除
            fi.MoveTo(strOriginFilePath + "_backup"); //文件重命名
            string strModelAshxFile;//模板ashx文件

            strModelAshxFile = Directory.GetCurrentDirectory() + "\\ITInResRequest.ashx";
            if (!File.Exists(strOriginFilePath))
            {
                File.Copy(strModelAshxFile, strOriginFilePath);
            }



            string strNewContent = "";

            FileStream fsr = new FileStream(strOriginFilePath, FileMode.Open, FileAccess.Read);
            using (StreamReader sr = new StreamReader(strOriginFilePath, Encoding.UTF8))//超级重要，必须以UTF-8格式打开
            {
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Contains("&&TableName"))
                    {
                        if (!string.IsNullOrEmpty(strTableName))
                        {
                            strNewContent = strNewContent + line.Replace("&&TableName", strTableName.Replace("\"", "")) + "\r\n ";//如果获取的表名里面有
                        }
                        else
                        {
                            strNewContent = strNewContent + line.Replace("&&TableName", strTableNameBk.Replace("\"", "")) + "\r\n ";//如果正常规则找不到tablename，启用特殊名字
                        }

                    }
                    else if (line.Contains("&&ItemAttribute"))
                    {
                        strNewContent = strNewContent + line.Replace("//&&ItemAttribute", strPasteContent) + "\r\n ";
                    }
                    else if (line.Contains("//&&QuickSearch"))
                    {
                        strNewContent = strNewContent + line.Replace("//&&QuickSearch", strQuickSearch) + "\r\n ";

                    }

                    else
                    {
                        strNewContent = strNewContent + line + "\r\n ";//特别在回车换行符后面加了个空格，可以解决如 CheckCapBudgetMonth_SummaryInfo 这样后面多一个}的问题
                    }

                }
                sr.Close();
                fsr.Close();
            }

            FileStream fsw = new FileStream(strOriginFilePath, FileMode.Open, FileAccess.Write);
            using (StreamWriter sw = new StreamWriter(fsw, Encoding.UTF8))
            {
                //sw.Flush();
                sw.Write(strNewContent.Trim());
                //sw.Write("");
                sw.Close();
                fsw.Close();

                WriteLog(strProcessName + ".ashx", ltBoxStoreDataService);
                //this.ltBoxStoreDataService.Items.Add(strProcessName + ".ashx");
                //string logFileName = System.Environment.GetEnvironmentVariable("TEMP") + "\\" + Path.GetFileNameWithoutExtension(Application.ExecutablePath) + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
                //using (TextWriter logFile = TextWriter.Synchronized(File.AppendText(logFileName)))
                //{
                //    logFile.WriteLine(DateTime.Now + "\t" + strProcessName + ".ashx" + "\r\n");
                //    logFile.Flush();
                //    logFile.Close();
                //}
            }
            #endregion
        }



        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void rTBoxOrigin_MouseHover(object sender, EventArgs e)
        {

        }

        private void rTBoxOrigin_VScroll(object sender, EventArgs e)
        {
            int crntLastLine = GetLineNoVscroll(rTBoxOrigin);
            TrunRowsId(crntLastLine, rTBoxTarget);

            //int lineCounter = 0;
            //foreach (string line in rTBoxOrigin.Lines)
            //{
            //    if (line.Contains("Attributes"))
            //    {
            //        //add conditional statement if not selecting all the lines
            //        rTBoxOrigin.Select(rTBoxOrigin.GetFirstCharIndexFromLine(lineCounter), line.Length);
            //        rTBoxOrigin.SelectionColor = Color.Red;
            //        lineCounter++;
            //    }
            //}
        }

        private int GetLineNoVscroll(RichTextBox rtb)
        {
            //获得当前坐标信息
            Point p = rtb.Location;
            int crntFirstIndex = rtb.GetCharIndexFromPosition(p);
            int crntFirstLine = rtb.GetLineFromCharIndex(crntFirstIndex);
            return crntFirstLine;
        }
        private void TrunRowsId(int iCodeRowsID, RichTextBox rtb)
        {
            try
            {
                rtb.SelectionStart = rtb.GetFirstCharIndexFromLine(iCodeRowsID);
                rtb.SelectionLength = 0;
                rtb.ScrollToCaret();
            }
            catch
            {

            }
        }

        private void btnFileBrowse_Click(object sender, EventArgs e)
        {
            menuFD_ECC.FileName = Directory.GetCurrentDirectory() + "\\ProModuleTree.ashx";
            string line;

            int iCount;

            string strContent = "";

            Dictionary<int, string> dicMain = new Dictionary<int, string>();
            Dictionary<int, string> dicSub = new Dictionary<int, string>();

            string strSpec = "";

            if (menuFD_ECC.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FileStream fsr = new FileStream(menuFD_ECC.FileName, FileMode.Open, FileAccess.Read);
                using (StreamReader sr = new StreamReader(menuFD_ECC.FileName, Encoding.UTF8))//超级重要，必须以UTF-8格式打开
                {
                    bool bFirst = true;//用于标志是否是第一行
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Trim().Length >= 2 && line.Trim().Substring(0, 2) == "//")
                        {
                            continue;
                        }
                        //if (line.Contains("item.Attributes["))
                        //{
                        //    strContent = strContent + line + "/r/n";

                        //}

                        //            strSpec = @"            item = new JsonItem();
                        //rootItems.Add(item);
                        //                      ";
                        //            if (line.Contains(strSpec))
                        //            {
                        //                line = line.Replace(strSpec, @" }
                        //                                                }
                        //                                                new
                        //                                            {");
                        //                strContent = strContent + line + "\r\n ";
                        //            }

                        //strSpec = "item = new JsonItem();";
                        //if (line.Contains(strSpec))
                        //{
                        //    line = line.Replace(strSpec, "new");
                        //    strContent = strContent + line + "\r\n ";
                        //}
                        strSpec = "rootItems.Add(item);";
                        if (line.Contains(strSpec))
                        {
                            if (bFirst == true)
                            {
                                line = @" 
                                    new{";
                            }
                            else
                            {
                                line = @" }
                                    },
                                    new{";
                            }
                            bFirst = false;
                            strContent = strContent + line + "\r\n ";
                        }
                        strSpec = "item.Attributes[\"text\"]";
                        if (line.Contains(strSpec))
                        {
                            line = line.Replace(strSpec, "text").Replace(";", ",");
                            strContent = strContent + line + "\r\n ";
                        }

                        strSpec = "item.Attributes[\"expanded\"] ";
                        if (line.Contains(strSpec))
                        {
                            line = "expanded = false,";
                            strContent = strContent + line + "\r\n ";
                        }
                        strSpec = "children = new JsonItemCollection();";
                        if (line.Contains(strSpec))
                        {
                            line = "children = new object[]{";
                            strContent = strContent + line + "\r\n ";
                        }
                        strSpec = "children.Add(item);";
                        if (line.Contains(strSpec))
                        {
                            line = @"new 
                             {";
                            strContent = strContent + line + "\r\n ";
                        }

                        strSpec = "item.Attributes[\"id\"]";
                        if (line.Contains(strSpec))
                        {
                            line = line.Replace(strSpec, "id").Replace(";", ",");
                            strContent = strContent + line + "\r\n ";
                        }

                        strSpec = "item.Attributes[\"text\"]";
                        if (line.Contains(strSpec))
                        {
                            line = line.Replace(strSpec, "text").Replace(";", ",");
                            strContent = strContent + line + "\r\n ";
                        }
                        strSpec = "item.Attributes[\"moduleUrl\"]";

                        if (line.Contains(strSpec))
                        {
                            line = line.Replace(strSpec, "xclass").Replace(";", "},").Replace("/", ".").Replace(".js", "");
                            strContent = strContent + line + "\r\n ";


                        }

                        //strContent = strContent + line + "\r\n ";


                    }

                    if (strContent.Contains("{"))
                    {
                        int iCountLeft = 0, iCountRight = 0;
                        for (int i = 0; i < strContent.Length; i++)
                        {
                            if (strContent[i] == '{')
                            {
                                iCountLeft++;
                            }
                            if (strContent[i] == '}')
                            {
                                iCountRight++;
                            }
                        }

                        if (iCountRight < iCountLeft)
                        {
                            strContent = strContent + @"}
                                }";
                        }

                    }
                    sr.Close();
                    fsr.Close();
                }

                string strOutFileName = Directory.GetCurrentDirectory() + "\\ProModuleTreeNew.txt";
                if (!File.Exists(strOutFileName))
                {
                    FileStream fs = File.Create(strOutFileName);
                    fs.Close();
                }
                FileStream fsw = new FileStream(strOutFileName, FileMode.Open, FileAccess.Write);
                using (StreamWriter sw = new StreamWriter(fsw, Encoding.UTF8))
                {
                    sw.Write(strContent);
                    //sw.Write("");
                    sw.Close();
                    fsw.Close();
                    txtBoxStatus.Text = "在" + strOutFileName + "生成文件";

                    Process process = new Process();
                    ProcessStartInfo processStartInfo = new ProcessStartInfo(strOutFileName);
                    process.StartInfo = processStartInfo;
                    process.StartInfo.UseShellExecute = true;
                    process.Start();
                    //System.Diagnostics.Process.Start(strOutFileName);

                    File.OpenRead(strOutFileName);

                    WriteLog(strOutFileName, ltBoxStoreDataService);
                }
            }
            else
            {
                MessageBox.Show("您没有选择任何文件!");
            }

        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            int lineCounter = 0;
            rTBoxTarget.Text = "";
            foreach (string line in rTBoxOrigin.Lines)
            {
                if (line.Contains(".Attributes.Add"))

                {
                    string strLine = line.Trim();
                    //iLeft = line.IndexOf("Attributes");
                    strLine = strLine.Replace(".Attributes.Add(", "[");
                    strLine = strLine.Replace(",", "]=");
                    strLine = strLine.Replace("))", ")");
                    rTBoxTarget.Text = rTBoxTarget.Text + strLine + "\r\n";


                    rTBoxOrigin.Select(rTBoxOrigin.GetFirstCharIndexFromLine(lineCounter), line.Length);
                    rTBoxOrigin.SelectionColor = Color.Red;
                    rTBoxOrigin.SelectionColor = Color.Red;
                    lineCounter++;
                }
            }
        }

        private void rTBoxOrigin_TextChanged(object sender, EventArgs e)
        {

        }

        private void rTBoxTarget_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnECCFolder_Click(object sender, EventArgs e)
        {
            //folderBD_ECC.ShowDialog();
            //MessageBox.Show(folderBD_ECC.SelectedPath);
            if (folderBD_ECC.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (string.IsNullOrEmpty(folderBD_ECC.SelectedPath))
                {
                    MessageBox.Show("您没有选择任何文件夹！");
                }
                else
                {
                    bool bFind = false;
                    //C#遍历指定文件夹中的所有文件 
                    DirectoryInfo TheFolder = new DirectoryInfo(folderBD_ECC.SelectedPath);

                    string strPatternFolder = "HRProcess";//只有这个文件夹才替换
                                                          //遍历文件夹
                    foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories("*", SearchOption.AllDirectories))
                    {
                        if (NextFolder.Parent.Name == "YZModules")
                        {
                            cmbConvertFolder.Items.Add(NextFolder.Name);
                        }
                        if (NextFolder.Name == "YZModules")//如果找到 YZModules,开始进行循环处理
                        {
                            TheFolder = new DirectoryInfo(NextFolder.FullName);
                            foreach (DirectoryInfo ECCFolder in TheFolder.GetDirectories("*", SearchOption.AllDirectories))
                            {

                                if (ECCFolder.Name == "Modules")//如果找到Modules文件夹循环处理js文件
                                {
                                    foreach (FileInfo JsFile in ECCFolder.GetFiles())
                                    {
                                        //if (JsFile.Name.Contains("HRSysPostName"))

                                        //if (ckBoxConvertFolder.Checked==true)
                                        //{
                                        //strPatternFolder = cmbConvertFolder.SelectedText;//只有这个文件夹才替换

                                        //}

                                        //if (cmbConvertFolder.SelectedText!="ALL")
                                        //{
                                        //if (ECCFolder.Parent.Name == strPatternFolder)
                                        //{

                                        //    FormatJSFile(JsFile.FullName, JsFile.Name.Replace(".js", ""), ECCFolder.Parent.Name);


                                        //}
                                        //else
                                        //{
                                        //    FormatJSFile(JsFile.FullName, JsFile.Name.Replace(".js", ""), ECCFolder.Parent.Name);
                                        //    this.ltBoxModules.Items.Add(JsFile.Name);
                                        //}



                                        //}
                                        FormatJSFile(JsFile.FullName, JsFile.Name.Replace(".js", ""), ECCFolder.Parent.Name);
                                    }
                                }
                                if (ECCFolder.Name == "StoreDataService")//如果找到StoreDataService文件夹循环处理ashx文件
                                {
                                    foreach (FileInfo AshxFile in ECCFolder.GetFiles())
                                    {
                                        if (AshxFile.Name.Contains("SYSModuleTree.ashx"))
                                        {
                                            continue;//这个文件已经修改完成不需要遍历
                                        }
                                        //if (AshxFile.Name.Contains("gonggao"))
                                        //if (ECCFolder.Parent.Name == strPatternFolder)
                                        //{
                                        //    FormatAshxFile(AshxFile.FullName, AshxFile.Name.Replace(".exclude", "").Replace(".ashx", ""), ECCFolder.Parent.Name);
                                        //    this.ltBoxStoreDataService.Items.Add(AshxFile.Name);
                                        //}

                                        FormatAshxFile(AshxFile.FullName, AshxFile.Name.Replace(".exclude", "").Replace(".ashx", ""), ECCFolder.Parent.Name);
                                        //this.ltBoxStoreDataService.Items.Add(AshxFile.Name);

                                    }

                                }
                            }
                            bFind = true;
                            break;
                        }

                    }

                    lblCountJSFile.Text = ltBoxModules.Items.Count.ToString() + "个";

                    lblCountAshxFile.Text = ltBoxStoreDataService.Items.Count.ToString() + "个";
                    if (bFind == false)
                    {
                        MessageBox.Show("没有找到YZModules文件夹，请确认是否选择正确!");
                    }
                }

            }
        }

        private void btnFolderBrowseAspx_Click(object sender, EventArgs e)
        {
            if (folderBD_ECC.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (string.IsNullOrEmpty(folderBD_ECC.SelectedPath))
                {
                    MessageBox.Show("您没有选择任何文件夹！");
                }
                else
                {
                    bool bFind = false;
                    //C#遍历指定文件夹中的所有文件 
                    DirectoryInfo TheFolder = new DirectoryInfo(folderBD_ECC.SelectedPath);

                    string strPatternFolder = "LOGProcess";//只有这个文件夹才替换
                                                           //遍历文件夹
                    foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories("*", SearchOption.AllDirectories))
                    {

                        if (NextFolder.Name == "XForm")//如果找到 XForm,开始进行循环处理
                        {
                            TheFolder = new DirectoryInfo(NextFolder.FullName);
                            foreach (DirectoryInfo ECCFolder in TheFolder.GetDirectories("*", SearchOption.AllDirectories))
                            {

                                foreach (FileInfo AspxFile in ECCFolder.GetFiles())
                                {
                                    if (AspxFile.Name.Contains("ISContractApplication20.aspx"))
                                        FormatAspxFile(AspxFile.FullName, AspxFile.Name.Replace(".aspx", ""), ECCFolder.Parent.Name);


                                }


                            }
                            bFind = true;
                            break;
                        }

                    }

                    lblCountAspx.Text = ltBoxAspx.Items.Count.ToString() + "个";

                    //lblCountAshxFile.Text = ltBoxStoreDataService.Items.Count.ToString() + "个";
                    if (bFind == false)
                    {
                        MessageBox.Show("没有找到XForm文件夹，请确认是否选择正确!");
                    }
                }

            }

        }

        private void btnModuleFolder_Click(object sender, EventArgs e)
        {
            if (folderBD_ECC.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtBoxTarget.Text = folderBD_ECC.SelectedPath;
                if (!string.IsNullOrEmpty(txtBoxForm.Text))
                {

                }
                else
                {
                    MessageBox.Show("Form不能为空！");
                    return;
                }

                if (!string.IsNullOrEmpty(txtBoxWindow.Text))
                {

                }
                else
                {
                    MessageBox.Show("Window的标题不能为空！");
                    return;
                }

                string strModelJSFile = Directory.GetCurrentDirectory() + "\\ITInResRequest.js";
                //File.Copy(strModelJSFile, txtBoxTarget.Text+"\\Modules\\"+txt);

                string strModelAshxFile = Directory.GetCurrentDirectory() + "\\ITInResRequest.ashx";
                if (!File.Exists(txtBoxTarget.Text))
                {
                    File.Copy(strModelAshxFile, txtBoxTarget.Text);
                }
            }
            else
            {
                MessageBox.Show("您没有选择认为文件夹！");
            }
        }

        delegate string FormatContent(string strContent);

        private void btConvertJs_Click(object sender, EventArgs e)
        {


            FormatContent frContent;
            frContent = FormateDownloadExcelFunction;
            rtBoxExtjs6.Text = frContent(rtBoxExtjs4.Text);

            frContent = FormatElement;

            rtBoxExtjs6.Text = frContent(rtBoxExtjs6.Text);

        }
        /// <summary>
        /// 格式化语法
        /// </summary>
        /// <param name="strContent"></param>
        /// <returns></returns>
        private string FormatElement(string strContentOld)
        {
            string[] strS = strContentOld.Split(Environment.NewLine.ToCharArray());

            string strContentNew = "";

            string strTemp = "";

            foreach (string strContent in strS)
            {

                if (strContent.Contains("getElementsByName", StringComparison.OrdinalIgnoreCase) && strContent.Contains("value", StringComparison.OrdinalIgnoreCase))
                {
                    strTemp = strContent.Replace("document.getElementsByName", "$").Replace(").value", ").down('.yz-xform-field-ele').dom.value");
                }
                else if (strContent.Contains("getElementById", StringComparison.OrdinalIgnoreCase) && strContent.Contains("innerHTML", StringComparison.OrdinalIgnoreCase))
                {
                    strTemp = strContent.Replace("document.getElementById", "Ext.get").Replace("\").innerHTML", ".yz-xform-field-ele\").html()");
                }
                else if (strContent.Contains("getElementById", StringComparison.OrdinalIgnoreCase) && strContent.Contains("value", StringComparison.OrdinalIgnoreCase))
                {
                    strTemp = strContent.Replace("document.getElementById", "Ext.get").Replace(").value", ").down('.yz-xform-field-ele').dom.value");
                }
                else if (strContent.Contains("getElementById", StringComparison.OrdinalIgnoreCase) && strContent.Contains("innerText", StringComparison.OrdinalIgnoreCase))
                {
                    strTemp = strContent.Replace("document.getElementById", "$").Replace("\").innerText", ".yz-xform-field-ele\").html()");
                }
                else if (strContent.Contains("getElementsByName", StringComparison.OrdinalIgnoreCase))
                {
                    strTemp = strContent.Replace("document.getElementsByName", "$").Replace(")", ").down('.yz-xform-field-ele').dom");

                }
                else
                {
                    strTemp = strContent;
                }



                strContentNew = strContentNew + strTemp + Environment.NewLine;

            }

            return strContentNew;
        }
        /// <summary>
        /// 替换函数体
        /// </summary>
        /// <param name="strContent">要替换的内容</param>
        /// <param name="strPattern">正则表达式</param>
        private string FormateDownloadExcelFunction(string strContent)
        {
            if (!strContent.Contains("GenExcelReport"))//如果没有替换过
            {
                string strNewFunction = @"
                                          Ext.require([""YZSoft.src.ux.File""]);
                                          function excel_JDE_Export_New()
                                                {
                                                    var reportServiceUrl = '../../../../YZSoft.Services.REST/Reports/Report.ashx';
                                                    var bm = '';
                                                    var params = { deptname: bm };

                                          params['Method'] = 'GenExcelReport';
                                          params['ExcelFile'] = '~/YZModules/ISProcess/&&&';
                                          params['outputType'] = 'Export';
                                                    var pms = new Array();
                                                    Ext.Object.each(params, function(key, val) {
                                                        if (key != 'UserParamNames')
                                                            pms.push(key);
                                                    });
                                          params['UserParamNames'] = pms.join(',');
                                                YZSoft.src.ux.File.download(reportServiceUrl, params);
                                            }
                                           ";

                string strPattern = @"Excel/.*.xls";
                string strDownloadFileName = "";
                strDownloadFileName = Regex.Match(strContent, strPattern).Value;

                strNewFunction = strNewFunction.Replace("&&&", strDownloadFileName);

                strPattern = @"function excel.+{[\s\S]+}///";//特意加///表示结尾，替换之前加上///
                strContent = Regex.Replace(strContent, strPattern, strNewFunction);
                return strContent;
            }
            else
            {

                return strContent;
            }
        }

        private void btnConvertToCh_Click(object sender, EventArgs e)
        {
            if (folderBD_ECC.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (string.IsNullOrEmpty(folderBD_ECC.SelectedPath))
                {
                    MessageBox.Show("您没有选择任何文件夹！");
                }
                else
                {
                    bool bFind = false;
                    //C#遍历指定文件夹中的所有文件 
                    DirectoryInfo TheFolder = new DirectoryInfo(folderBD_ECC.SelectedPath);

                    //string strPatternFolder = "LOGProcess";//只有这个文件夹才替换
                    //遍历文件夹
                    foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories("*", SearchOption.AllDirectories))
                    {

                        if (NextFolder.Name == "XForm")//如果找到 XForm,开始进行循环处理
                        {
                            TheFolder = new DirectoryInfo(NextFolder.FullName);
                            foreach (DirectoryInfo ECCFolder in TheFolder.GetDirectories("*", SearchOption.AllDirectories))
                            {

                                foreach (FileInfo AspxFile in ECCFolder.GetFiles())
                                {
                                   // if (AspxFile.Name.Equals("Apply CarOrMeal(china)10.aspx"))
                                        //{
                                        //AspxFileHelper aspxFileHelper = new AspxFileHelper();
                                        //aspxFileHelper.FormatAspxFileConvertLableNameToCH(AspxFile.FullName, AspxFile.Name.Replace(".aspx", ""), ECCFolder.Parent.Name);
                                        FormatAspxFileConvertLableNameToCH(AspxFile.FullName, AspxFile.Name.Replace(".aspx", ""), ECCFolder.Parent.Name);

                                    //}

                                }


                            }
                            bFind = true;
                            break;
                        }

                    }

                    lblCountAspx.Text = ltBoxAspx.Items.Count.ToString() + "个";

                    //lblCountAshxFile.Text = ltBoxStoreDataService.Items.Count.ToString() + "个";
                    if (bFind == false)
                    {
                        MessageBox.Show("没有找到XForm文件夹，请确认是否选择正确!");
                    }
                }

            }
        }

        public void FormatAspxFileConvertLableNameToCH(string strOriginFilePath, string strProcessName, string strProcessGroup, string strProcessNameCN = "")
        {
            if (strProcessName.Contains("_backup") || strProcessName.Contains(".uf"))//如果已经备份的文件不再处理 .uf文件不处理
            {
                return;
            }

            int iTableNameStart, iTableNameEnd;//数据库表的位置
            string strTableName = "";//数据库表名
            string line;
            string strContent = "";//文本文件内容

            int iCompanyFlag;
            int iCompanyNameStart, iCompanyNameEnd;//记录Company出现的位置

            string strReplaceContent = "";

            string strFrontContent = "";//上半部分内容
            string strBelowContent = "";//下班部分内容

            string strNewContent = "";//用于存储新内容

            string strTag = "";

            FileEncoding fEncoding = new FileEncoding();

            if (!File.Exists(strOriginFilePath + "_backup_backup"))
            {
                File.Copy(strOriginFilePath, strOriginFilePath + "_backup_backup");//备份文件
            }
            #region 通用处理下aspx文件

            //4种编码  
            Encoding utf8 = Encoding.UTF8;
            Encoding utf16 = Encoding.Unicode;
            //Encoding gb = Encoding.GetEncoding("gbk");
            //Encoding b5 = Encoding.GetEncoding("big5");

            Encoding strFileEncoding;
            strFileEncoding = FileEncoding.GetEncoding(strOriginFilePath);
            FileStream fsr = new FileStream(strOriginFilePath, FileMode.Open, FileAccess.Read);
            using (StreamReader file = new StreamReader(strOriginFilePath, strFileEncoding))//超级重要，C#字符串默认是Unicode，必须以Unicode格式打开
            //using (StreamReader file = new StreamReader(strOriginFilePath))
            {

                while ((line = file.ReadLine()) != null)
                {

                    if (line.Contains("--ConvertedConverted"))//如果文件已经更新过跳出逻辑
                    {
                        WriteLog(strProcessName, ltBoxAspx, ".aspx文件已更新过！");

                        file.Close();
                        fsr.Close();
                        return;
                    }
                    //特殊注释删掉
                    if (line.Trim().Length >= 4 && line.Trim().Substring(0, 4) == "<%--")
                    {

                    }
                    else
                    {
                        //批量处理没有绑定的控件
                        if (line.Contains("<aspxform:") && !line.Contains("XDataBind") && !line.Contains("Visibility=\"False\""))
                        {
                            string strPatternCommon = @"<aspxform:[\w]+";//获取控件名字
                            string strControlCommon = "";
                            foreach (Match matchControl in Regex.Matches(line, strPatternCommon))
                            {
                                strControlCommon = matchControl.Value;

                            }
                            line = line.Replace(strControlCommon, strControlCommon + " Visibility=\"False\"");
                        }


                        Console.WriteLine("注释语句删除");
                        strContent = strContent + line + Environment.NewLine;
                    }

                }

                file.Close();
                fsr.Close();

            }
            #endregion

            #region 运用正则表达式读取中文数字并进行处理
            //删除审批意见
            //string strPattern = @"<p.*?>[\w\W]*?</aspxform:[\w]+>";//提取从p开始的字段
            string strPattern = @"[\s;][\u4e00-\u9fa5]+/?[\w\W]*?</aspxform:[\w]+>";//提取从p开始的字段
            string strChinese = "";
            string strFragment = "";//提取的片段
            string strNewFragment = "";//要替换的的片段
            string strControl = "";//控件
            foreach (Match match in Regex.Matches(strContent, strPattern))
            {
                strFragment = match.Value;

                if (Regex.IsMatch(strFragment, @"[\u4e00-\u9fa5]"))//判断里面是否有汉字
                {
                    //如果已经替换过的话跳出循环
                    if (strFragment.Contains("FieldName", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    strPattern = @"<aspxform:[\w]+";//获取控件名字
                    foreach (Match matchControl in Regex.Matches(strFragment, strPattern))
                    {
                        strControl = matchControl.Value;

                        break;

                    }
                    if (strControl.Contains("CheckBox", StringComparison.OrdinalIgnoreCase))//如果是checkbox的话不用替换
                    {
                        continue;
                    }
                    if (strControl.Contains("XRequiredFieldValidator", StringComparison.OrdinalIgnoreCase))//如果是checkbox的话不用替换
                    {
                        continue;
                    }
                    if (strControl.Contains("XCompareValidator", StringComparison.OrdinalIgnoreCase))//如果是checkbox的话不用替换
                    {
                        continue;
                    }
                    if (strControl.Contains("XRegularExpressionValidator", StringComparison.OrdinalIgnoreCase))//如果是checkbox的话不用替换
                    {
                        continue;
                    }
                    if (strControl.Contains("XRangeValidator", StringComparison.OrdinalIgnoreCase))//如果是checkbox的话不用替换
                    {
                        continue;
                    }
                    if (strControl.Contains("XCustomValidator", StringComparison.OrdinalIgnoreCase))//如果是checkbox的话不用替换
                    {
                        continue;
                    }
                    //XCompareValidator1 XRangeValidator1 XCustomValidator1
                    strPattern = "[\u4e00-\u9fa5]+";//是否有汉字正则表达式
                    strPattern = "(\\d?[\u4e00-\u9fa5]+/?)+";//是否有汉字正则表达式
                    foreach (Match matchChinese in Regex.Matches(strFragment, strPattern))
                    {
                        strChinese = matchChinese.Value;
                        if (strFragment.Contains("单号"))
                        {
                            if (strChinese == "单号")
                            {
                                break;//取到单号汉字就跳出了
                            }
                        }
                        else
                        {
                            break;

                        }


                    }

                    strNewFragment = strFragment.Replace(strControl, strControl + " FieldName=\"" + strChinese + "\" ");

                    strContent = strContent.Replace(strFragment, strNewFragment);
                }

            }

            strNewContent = "<%--ConvertedConverted--%>" + Environment.NewLine + strContent;//标记是否已经替换过


            #endregion







            FileStream fsw = new FileStream(strOriginFilePath, FileMode.Create, FileAccess.Write);//FileMode.Create 复制之前清空文件防止编码问题导致错误
            using (StreamWriter sw = new StreamWriter(fsw, strFileEncoding))

            {

                sw.Write(strNewContent);


                sw.Close();
                fsw.Close();
                WriteLog(strProcessName + ".aspx", ltBoxAspx);


            }



        }

        private void ltBoxModules_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
