namespace KursAIS
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.IPtextbox = new System.Windows.Forms.TextBox();
            this.Porttextbox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Passtextbox = new System.Windows.Forms.TextBox();
            this.Logintextbox = new System.Windows.Forms.TextBox();
            this.DBtextbox = new System.Windows.Forms.TextBox();
            this.connectBtn = new System.Windows.Forms.Button();
            this.backupBtn = new System.Windows.Forms.Button();
            this.backupTextbox = new System.Windows.Forms.TextBox();
            this.restoreBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 12);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(619, 361);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView1_DataError);
            this.dataGridView1.RowValidating += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dataGridView1_RowValidating);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(638, 13);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(142, 21);
            this.comboBox1.TabIndex = 1;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // IPtextbox
            // 
            this.IPtextbox.Location = new System.Drawing.Point(6, 33);
            this.IPtextbox.Name = "IPtextbox";
            this.IPtextbox.Size = new System.Drawing.Size(100, 20);
            this.IPtextbox.TabIndex = 2;
            this.IPtextbox.Tag = "";
            this.IPtextbox.Text = "192.168.0.103";
            // 
            // Porttextbox
            // 
            this.Porttextbox.Location = new System.Drawing.Point(6, 59);
            this.Porttextbox.Name = "Porttextbox";
            this.Porttextbox.Size = new System.Drawing.Size(100, 20);
            this.Porttextbox.TabIndex = 3;
            this.Porttextbox.Text = "1433";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.Passtextbox);
            this.groupBox1.Controls.Add(this.Logintextbox);
            this.groupBox1.Controls.Add(this.DBtextbox);
            this.groupBox1.Controls.Add(this.IPtextbox);
            this.groupBox1.Controls.Add(this.Porttextbox);
            this.groupBox1.Location = new System.Drawing.Point(638, 40);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(142, 170);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Параметры подключения";
            // 
            // Passtextbox
            // 
            this.Passtextbox.Location = new System.Drawing.Point(6, 137);
            this.Passtextbox.Name = "Passtextbox";
            this.Passtextbox.PasswordChar = '*';
            this.Passtextbox.Size = new System.Drawing.Size(100, 20);
            this.Passtextbox.TabIndex = 5;
            // 
            // Logintextbox
            // 
            this.Logintextbox.Location = new System.Drawing.Point(6, 111);
            this.Logintextbox.Name = "Logintextbox";
            this.Logintextbox.Size = new System.Drawing.Size(100, 20);
            this.Logintextbox.TabIndex = 5;
            this.Logintextbox.Text = "SA";
            // 
            // DBtextbox
            // 
            this.DBtextbox.Location = new System.Drawing.Point(6, 85);
            this.DBtextbox.Name = "DBtextbox";
            this.DBtextbox.Size = new System.Drawing.Size(100, 20);
            this.DBtextbox.TabIndex = 4;
            this.DBtextbox.Text = "AISKurs";
            // 
            // connectBtn
            // 
            this.connectBtn.Location = new System.Drawing.Point(644, 217);
            this.connectBtn.Name = "connectBtn";
            this.connectBtn.Size = new System.Drawing.Size(131, 23);
            this.connectBtn.TabIndex = 5;
            this.connectBtn.Text = "Подключение";
            this.connectBtn.UseVisualStyleBackColor = true;
            this.connectBtn.Click += new System.EventHandler(this.connectBtn_Click);
            // 
            // backupBtn
            // 
            this.backupBtn.Location = new System.Drawing.Point(644, 246);
            this.backupBtn.Name = "backupBtn";
            this.backupBtn.Size = new System.Drawing.Size(131, 23);
            this.backupBtn.TabIndex = 7;
            this.backupBtn.Text = "Бэкап";
            this.backupBtn.UseVisualStyleBackColor = true;
            this.backupBtn.Click += new System.EventHandler(this.backupBtn_Click);
            // 
            // backupTextbox
            // 
            this.backupTextbox.Location = new System.Drawing.Point(644, 304);
            this.backupTextbox.Name = "backupTextbox";
            this.backupTextbox.Size = new System.Drawing.Size(131, 20);
            this.backupTextbox.TabIndex = 8;
            // 
            // restoreBtn
            // 
            this.restoreBtn.Location = new System.Drawing.Point(644, 275);
            this.restoreBtn.Name = "restoreBtn";
            this.restoreBtn.Size = new System.Drawing.Size(131, 23);
            this.restoreBtn.TabIndex = 9;
            this.restoreBtn.Text = "Восстановление";
            this.restoreBtn.UseVisualStyleBackColor = true;
            this.restoreBtn.Click += new System.EventHandler(this.restoreBtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(787, 390);
            this.Controls.Add(this.restoreBtn);
            this.Controls.Add(this.backupTextbox);
            this.Controls.Add(this.backupBtn);
            this.Controls.Add(this.connectBtn);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.dataGridView1);
            this.Name = "Form1";
            this.Text = "Курсовая работа по АИС Киселёв Андрей МО-312";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.TextBox IPtextbox;
        private System.Windows.Forms.TextBox Porttextbox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox Passtextbox;
        private System.Windows.Forms.TextBox Logintextbox;
        private System.Windows.Forms.TextBox DBtextbox;
        private System.Windows.Forms.Button connectBtn;
        private System.Windows.Forms.Button backupBtn;
        private System.Windows.Forms.TextBox backupTextbox;
        private System.Windows.Forms.Button restoreBtn;
    }
}

