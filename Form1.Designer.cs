
namespace ForECC
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.rTBoxOrigin = new System.Windows.Forms.RichTextBox();
            this.rTBoxTarget = new System.Windows.Forms.RichTextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.btnFileBrowse = new System.Windows.Forms.Button();
            this.btnFolderBrowse = new System.Windows.Forms.Button();
            this.folderBD_ECC = new System.Windows.Forms.FolderBrowserDialog();
            this.btnECCFolder = new System.Windows.Forms.Button();
            this.ltBoxModules = new System.Windows.Forms.ListBox();
            this.ltBoxStoreDataService = new System.Windows.Forms.ListBox();
            this.lblJSFileName = new System.Windows.Forms.Label();
            this.lblAshxFileName = new System.Windows.Forms.Label();
            this.ltBoxConverted = new System.Windows.Forms.ListBox();
            this.lblConverted = new System.Windows.Forms.Label();
            this.ltBoxNoExistString = new System.Windows.Forms.ListBox();
            this.lblNoExistString = new System.Windows.Forms.Label();
            this.cmbConvertFolder = new System.Windows.Forms.ComboBox();
            this.ckBoxConvertFolder = new System.Windows.Forms.CheckBox();
            this.btnConvert = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblCountJSFile = new System.Windows.Forms.Label();
            this.lblCountAshxFile = new System.Windows.Forms.Label();
            this.ltBoxNoExistTableName = new System.Windows.Forms.ListBox();
            this.lblNoExistTableName = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rTBoxOrigin
            // 
            this.rTBoxOrigin.Location = new System.Drawing.Point(23, 316);
            this.rTBoxOrigin.Name = "rTBoxOrigin";
            this.rTBoxOrigin.Size = new System.Drawing.Size(94, 244);
            this.rTBoxOrigin.TabIndex = 0;
            this.rTBoxOrigin.Text = "";
            this.rTBoxOrigin.Visible = false;
            this.rTBoxOrigin.VScroll += new System.EventHandler(this.rTBoxOrigin_VScroll);
            this.rTBoxOrigin.TextChanged += new System.EventHandler(this.rTBoxOrigin_TextChanged);
            this.rTBoxOrigin.MouseHover += new System.EventHandler(this.rTBoxOrigin_MouseHover);
            // 
            // rTBoxTarget
            // 
            this.rTBoxTarget.Location = new System.Drawing.Point(141, 316);
            this.rTBoxTarget.Name = "rTBoxTarget";
            this.rTBoxTarget.Size = new System.Drawing.Size(97, 244);
            this.rTBoxTarget.TabIndex = 0;
            this.rTBoxTarget.Text = "";
            this.rTBoxTarget.Visible = false;
            this.rTBoxTarget.TextChanged += new System.EventHandler(this.rTBoxTarget_TextChanged);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // btnFileBrowse
            // 
            this.btnFileBrowse.Location = new System.Drawing.Point(144, 640);
            this.btnFileBrowse.Name = "btnFileBrowse";
            this.btnFileBrowse.Size = new System.Drawing.Size(114, 35);
            this.btnFileBrowse.TabIndex = 2;
            this.btnFileBrowse.Text = "选择ashx文件";
            this.btnFileBrowse.UseVisualStyleBackColor = true;
            this.btnFileBrowse.Visible = false;
            this.btnFileBrowse.Click += new System.EventHandler(this.btnFileBrowse_Click);
            // 
            // btnFolderBrowse
            // 
            this.btnFolderBrowse.Location = new System.Drawing.Point(288, 640);
            this.btnFolderBrowse.Name = "btnFolderBrowse";
            this.btnFolderBrowse.Size = new System.Drawing.Size(107, 35);
            this.btnFolderBrowse.TabIndex = 2;
            this.btnFolderBrowse.Text = "选择ashx文件夹";
            this.btnFolderBrowse.UseVisualStyleBackColor = true;
            this.btnFolderBrowse.Visible = false;
            // 
            // btnECCFolder
            // 
            this.btnECCFolder.Location = new System.Drawing.Point(141, 577);
            this.btnECCFolder.Margin = new System.Windows.Forms.Padding(2);
            this.btnECCFolder.Name = "btnECCFolder";
            this.btnECCFolder.Size = new System.Drawing.Size(181, 35);
            this.btnECCFolder.TabIndex = 3;
            this.btnECCFolder.Text = "选择ECC文件夹进行自动变换";
            this.btnECCFolder.UseVisualStyleBackColor = true;
            this.btnECCFolder.Click += new System.EventHandler(this.btnECCFolder_Click);
            // 
            // ltBoxModules
            // 
            this.ltBoxModules.FormattingEnabled = true;
            this.ltBoxModules.ItemHeight = 17;
            this.ltBoxModules.Location = new System.Drawing.Point(11, 64);
            this.ltBoxModules.Margin = new System.Windows.Forms.Padding(2);
            this.ltBoxModules.Name = "ltBoxModules";
            this.ltBoxModules.Size = new System.Drawing.Size(256, 497);
            this.ltBoxModules.TabIndex = 4;
            // 
            // ltBoxStoreDataService
            // 
            this.ltBoxStoreDataService.FormattingEnabled = true;
            this.ltBoxStoreDataService.ItemHeight = 17;
            this.ltBoxStoreDataService.Location = new System.Drawing.Point(303, 63);
            this.ltBoxStoreDataService.Margin = new System.Windows.Forms.Padding(2);
            this.ltBoxStoreDataService.Name = "ltBoxStoreDataService";
            this.ltBoxStoreDataService.Size = new System.Drawing.Size(256, 497);
            this.ltBoxStoreDataService.TabIndex = 4;
            // 
            // lblJSFileName
            // 
            this.lblJSFileName.AutoSize = true;
            this.lblJSFileName.Location = new System.Drawing.Point(11, 26);
            this.lblJSFileName.Name = "lblJSFileName";
            this.lblJSFileName.Size = new System.Drawing.Size(115, 17);
            this.lblJSFileName.TabIndex = 5;
            this.lblJSFileName.Text = "更新的Js文件名单：";
            // 
            // lblAshxFileName
            // 
            this.lblAshxFileName.AutoSize = true;
            this.lblAshxFileName.Location = new System.Drawing.Point(303, 26);
            this.lblAshxFileName.Name = "lblAshxFileName";
            this.lblAshxFileName.Size = new System.Drawing.Size(131, 17);
            this.lblAshxFileName.TabIndex = 5;
            this.lblAshxFileName.Text = "更新的Ashx文件名单：";
            // 
            // ltBoxConverted
            // 
            this.ltBoxConverted.FormattingEnabled = true;
            this.ltBoxConverted.ItemHeight = 17;
            this.ltBoxConverted.Location = new System.Drawing.Point(575, 63);
            this.ltBoxConverted.Margin = new System.Windows.Forms.Padding(2);
            this.ltBoxConverted.Name = "ltBoxConverted";
            this.ltBoxConverted.Size = new System.Drawing.Size(256, 497);
            this.ltBoxConverted.TabIndex = 4;
            // 
            // lblConverted
            // 
            this.lblConverted.AutoSize = true;
            this.lblConverted.Location = new System.Drawing.Point(575, 26);
            this.lblConverted.Name = "lblConverted";
            this.lblConverted.Size = new System.Drawing.Size(104, 17);
            this.lblConverted.TabIndex = 5;
            this.lblConverted.Text = "已更新过的文件：";
            // 
            // ltBoxNoExistString
            // 
            this.ltBoxNoExistString.FormattingEnabled = true;
            this.ltBoxNoExistString.ItemHeight = 17;
            this.ltBoxNoExistString.Location = new System.Drawing.Point(835, 63);
            this.ltBoxNoExistString.Margin = new System.Windows.Forms.Padding(2);
            this.ltBoxNoExistString.Name = "ltBoxNoExistString";
            this.ltBoxNoExistString.Size = new System.Drawing.Size(256, 497);
            this.ltBoxNoExistString.TabIndex = 4;
            // 
            // lblNoExistString
            // 
            this.lblNoExistString.AutoSize = true;
            this.lblNoExistString.Location = new System.Drawing.Point(835, 26);
            this.lblNoExistString.Name = "lblNoExistString";
            this.lblNoExistString.Size = new System.Drawing.Size(140, 17);
            this.lblNoExistString.TabIndex = 5;
            this.lblNoExistString.Text = "未发现特征字符的文件：";
            // 
            // cmbConvertFolder
            // 
            this.cmbConvertFolder.FormattingEnabled = true;
            this.cmbConvertFolder.Location = new System.Drawing.Point(0, 0);
            this.cmbConvertFolder.Name = "cmbConvertFolder";
            this.cmbConvertFolder.Size = new System.Drawing.Size(144, 25);
            this.cmbConvertFolder.TabIndex = 6;
            this.cmbConvertFolder.Text = "ALL";
            // 
            // ckBoxConvertFolder
            // 
            this.ckBoxConvertFolder.AutoSize = true;
            this.ckBoxConvertFolder.Location = new System.Drawing.Point(178, 2);
            this.ckBoxConvertFolder.Name = "ckBoxConvertFolder";
            this.ckBoxConvertFolder.Size = new System.Drawing.Size(123, 21);
            this.ckBoxConvertFolder.TabIndex = 7;
            this.ckBoxConvertFolder.Text = "只更新这个文件夹";
            this.ckBoxConvertFolder.UseVisualStyleBackColor = true;
            // 
            // btnConvert
            // 
            this.btnConvert.Location = new System.Drawing.Point(580, 640);
            this.btnConvert.Name = "btnConvert";
            this.btnConvert.Size = new System.Drawing.Size(99, 35);
            this.btnConvert.TabIndex = 1;
            this.btnConvert.Text = "ashx文件转换";
            this.btnConvert.UseVisualStyleBackColor = true;
            this.btnConvert.Visible = false;
            this.btnConvert.Click += new System.EventHandler(this.btnConvert_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cmbConvertFolder);
            this.panel1.Controls.Add(this.ckBoxConvertFolder);
            this.panel1.Location = new System.Drawing.Point(364, 583);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(315, 29);
            this.panel1.TabIndex = 8;
            this.panel1.Visible = false;
            // 
            // lblCountJSFile
            // 
            this.lblCountJSFile.AutoSize = true;
            this.lblCountJSFile.Location = new System.Drawing.Point(157, 26);
            this.lblCountJSFile.Name = "lblCountJSFile";
            this.lblCountJSFile.Size = new System.Drawing.Size(0, 17);
            this.lblCountJSFile.TabIndex = 9;
            // 
            // lblCountAshxFile
            // 
            this.lblCountAshxFile.AutoSize = true;
            this.lblCountAshxFile.Location = new System.Drawing.Point(454, 26);
            this.lblCountAshxFile.Name = "lblCountAshxFile";
            this.lblCountAshxFile.Size = new System.Drawing.Size(0, 17);
            this.lblCountAshxFile.TabIndex = 9;
            // 
            // ltBoxNoExistTableName
            // 
            this.ltBoxNoExistTableName.FormattingEnabled = true;
            this.ltBoxNoExistTableName.ItemHeight = 17;
            this.ltBoxNoExistTableName.Location = new System.Drawing.Point(1095, 63);
            this.ltBoxNoExistTableName.Margin = new System.Windows.Forms.Padding(2);
            this.ltBoxNoExistTableName.Name = "ltBoxNoExistTableName";
            this.ltBoxNoExistTableName.Size = new System.Drawing.Size(256, 497);
            this.ltBoxNoExistTableName.TabIndex = 4;
            // 
            // lblNoExistTableName
            // 
            this.lblNoExistTableName.AutoSize = true;
            this.lblNoExistTableName.Location = new System.Drawing.Point(1095, 26);
            this.lblNoExistTableName.Name = "lblNoExistTableName";
            this.lblNoExistTableName.Size = new System.Drawing.Size(116, 17);
            this.lblNoExistTableName.TabIndex = 5;
            this.lblNoExistTableName.Text = "未发现表名的文件：";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1370, 687);
            this.Controls.Add(this.lblCountAshxFile);
            this.Controls.Add(this.lblCountJSFile);
            this.Controls.Add(this.btnConvert);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblNoExistTableName);
            this.Controls.Add(this.lblNoExistString);
            this.Controls.Add(this.lblConverted);
            this.Controls.Add(this.lblAshxFileName);
            this.Controls.Add(this.lblJSFileName);
            this.Controls.Add(this.ltBoxNoExistTableName);
            this.Controls.Add(this.ltBoxNoExistString);
            this.Controls.Add(this.ltBoxConverted);
            this.Controls.Add(this.ltBoxStoreDataService);
            this.Controls.Add(this.ltBoxModules);
            this.Controls.Add(this.btnECCFolder);
            this.Controls.Add(this.btnFolderBrowse);
            this.Controls.Add(this.btnFileBrowse);
            this.Controls.Add(this.rTBoxTarget);
            this.Controls.Add(this.rTBoxOrigin);
            this.Name = "Form1";
            this.Text = "*";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox rTBoxOrigin;
        private System.Windows.Forms.RichTextBox rTBoxTarget;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button btnFileBrowse;
        private System.Windows.Forms.Button btnFolderBrowse;
        private System.Windows.Forms.FolderBrowserDialog folderBD_ECC;
        private System.Windows.Forms.Button btnECCFolder;
        private System.Windows.Forms.ListBox ltBoxModules;
        private System.Windows.Forms.ListBox ltBoxStoreDataService;
        private System.Windows.Forms.Label lblJSFileName;
        private System.Windows.Forms.Label lblAshxFileName;
        private System.Windows.Forms.ListBox ltBoxConverted;
        private System.Windows.Forms.Label lblConverted;
        private System.Windows.Forms.ListBox ltBoxNoExistString;
        private System.Windows.Forms.Label lblNoExistString;
        private System.Windows.Forms.ComboBox cmbConvertFolder;
        private System.Windows.Forms.CheckBox ckBoxConvertFolder;
        private System.Windows.Forms.Button btnConvert;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblCountJSFile;
        private System.Windows.Forms.Label lblCountAshxFile;
        private System.Windows.Forms.ListBox ltBoxNoExistTableName;
        private System.Windows.Forms.Label lblNoExistTableName;
    }
}

