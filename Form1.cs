using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

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
                return;
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

        private void WriteLog(string strProcessName, ListBox ltBoxLog, string strMsg = "")
        {
            ltBoxLog.Items.Add(strProcessName + strMsg);

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

            bool bFindTable = false;//判断是否发现表名
            bool bFindItem = false;//判断是否发现表名
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
                    while ((line = sr.ReadLine()) != null)
                    {
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
                            line = "{";
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
                            line = "expanded = true,";
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
                            line = "{";
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

                        }

                        //strContent = strContent + line + "\r\n ";


                    }
                    sr.Close();
                    fsr.Close();
                }

                string strOutFileName = Directory.GetCurrentDirectory() + "\\ProModuleTreeNew.ashx";
                //if (!File.Exists(strOutFileName))
                //{
                //    File.Create(strOutFileName);
                //}
                FileStream fsw = new FileStream(strOutFileName, FileMode.Open, FileAccess.Write);
                using (StreamWriter sw = new StreamWriter(fsw, Encoding.UTF8))
                {
                    sw.Write(strContent);
                    //sw.Write("");
                    sw.Close();
                    fsw.Close();

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

                    string strPatternFolder = "LOGProcess";//只有这个文件夹才替换
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
                                        //if (JsFile.Name.Contains("gonggao"))

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
    }
}
