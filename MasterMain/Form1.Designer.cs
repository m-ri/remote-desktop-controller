namespace MasterMain
{
    partial class Form1
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button3 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnHook = new System.Windows.Forms.Button();
            this.radioButtonHookGlobal = new System.Windows.Forms.RadioButton();
            this.radioButtonHookLocal = new System.Windows.Forms.RadioButton();
            this.rdbMouseRel = new System.Windows.Forms.RadioButton();
            this.rdbMouseAbsResc = new System.Windows.Forms.RadioButton();
            this.rdbMouseAbs = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnSlavesNew = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.lsbSlaves = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnHotkeyManage = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(317, 270);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(107, 23);
            this.button3.TabIndex = 2;
            this.button3.Text = "Slave server";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(433, 270);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 23);
            this.button5.TabIndex = 4;
            this.button5.Text = "esci";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Visible = false;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnHook);
            this.panel1.Controls.Add(this.radioButtonHookGlobal);
            this.panel1.Controls.Add(this.radioButtonHookLocal);
            this.panel1.Location = new System.Drawing.Point(307, 177);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(117, 75);
            this.panel1.TabIndex = 5;
            this.panel1.Visible = false;
            // 
            // btnHook
            // 
            this.btnHook.Location = new System.Drawing.Point(10, 49);
            this.btnHook.Name = "btnHook";
            this.btnHook.Size = new System.Drawing.Size(115, 23);
            this.btnHook.TabIndex = 2;
            this.btnHook.Text = "Intercetta eventi";
            this.btnHook.UseVisualStyleBackColor = true;
            this.btnHook.Click += new System.EventHandler(this.btnHook_Click);
            // 
            // radioButtonHookGlobal
            // 
            this.radioButtonHookGlobal.AutoSize = true;
            this.radioButtonHookGlobal.Location = new System.Drawing.Point(3, 26);
            this.radioButtonHookGlobal.Name = "radioButtonHookGlobal";
            this.radioButtonHookGlobal.Size = new System.Drawing.Size(206, 17);
            this.radioButtonHookGlobal.TabIndex = 1;
            this.radioButtonHookGlobal.Text = "Eventi globali (richiede privilegi elevati)";
            this.radioButtonHookGlobal.UseVisualStyleBackColor = true;
            // 
            // radioButtonHookLocal
            // 
            this.radioButtonHookLocal.AutoSize = true;
            this.radioButtonHookLocal.Checked = true;
            this.radioButtonHookLocal.Location = new System.Drawing.Point(3, 3);
            this.radioButtonHookLocal.Name = "radioButtonHookLocal";
            this.radioButtonHookLocal.Size = new System.Drawing.Size(82, 17);
            this.radioButtonHookLocal.TabIndex = 0;
            this.radioButtonHookLocal.TabStop = true;
            this.radioButtonHookLocal.Text = "Eventi locali";
            this.radioButtonHookLocal.UseVisualStyleBackColor = true;
            // 
            // rdbMouseRel
            // 
            this.rdbMouseRel.AutoSize = true;
            this.rdbMouseRel.Checked = true;
            this.rdbMouseRel.Location = new System.Drawing.Point(6, 63);
            this.rdbMouseRel.Name = "rdbMouseRel";
            this.rdbMouseRel.Size = new System.Drawing.Size(64, 17);
            this.rdbMouseRel.TabIndex = 3;
            this.rdbMouseRel.TabStop = true;
            this.rdbMouseRel.Text = "Relative";
            this.rdbMouseRel.UseVisualStyleBackColor = true;
            // 
            // rdbMouseAbsResc
            // 
            this.rdbMouseAbsResc.AutoSize = true;
            this.rdbMouseAbsResc.Location = new System.Drawing.Point(6, 40);
            this.rdbMouseAbsResc.Name = "rdbMouseAbsResc";
            this.rdbMouseAbsResc.Size = new System.Drawing.Size(114, 17);
            this.rdbMouseAbsResc.TabIndex = 2;
            this.rdbMouseAbsResc.Text = "Absolute Rescaled";
            this.rdbMouseAbsResc.UseVisualStyleBackColor = true;
            // 
            // rdbMouseAbs
            // 
            this.rdbMouseAbs.AutoSize = true;
            this.rdbMouseAbs.Location = new System.Drawing.Point(6, 19);
            this.rdbMouseAbs.Name = "rdbMouseAbs";
            this.rdbMouseAbs.Size = new System.Drawing.Size(66, 17);
            this.rdbMouseAbs.TabIndex = 1;
            this.rdbMouseAbs.Text = "Absolute";
            this.rdbMouseAbs.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdbMouseRel);
            this.groupBox1.Controls.Add(this.rdbMouseAbsResc);
            this.groupBox1.Controls.Add(this.rdbMouseAbs);
            this.groupBox1.Location = new System.Drawing.Point(438, 167);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(109, 90);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Mouse policy";
            this.groupBox1.Visible = false;
            // 
            // btnSlavesNew
            // 
            this.btnSlavesNew.Location = new System.Drawing.Point(66, 276);
            this.btnSlavesNew.Name = "btnSlavesNew";
            this.btnSlavesNew.Size = new System.Drawing.Size(66, 23);
            this.btnSlavesNew.TabIndex = 11;
            this.btnSlavesNew.Text = "Add";
            this.btnSlavesNew.UseVisualStyleBackColor = true;
            this.btnSlavesNew.Click += new System.EventHandler(this.btnSlavesNew_Click);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(138, 276);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(75, 23);
            this.button8.TabIndex = 12;
            this.button8.Text = "Remove";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // lsbSlaves
            // 
            this.lsbSlaves.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.lsbSlaves.HideSelection = false;
            this.lsbSlaves.Location = new System.Drawing.Point(6, 14);
            this.lsbSlaves.MultiSelect = false;
            this.lsbSlaves.Name = "lsbSlaves";
            this.lsbSlaves.ShowGroups = false;
            this.lsbSlaves.Size = new System.Drawing.Size(277, 256);
            this.lsbSlaves.TabIndex = 13;
            this.lsbSlaves.UseCompatibleStateImageBehavior = false;
            this.lsbSlaves.View = System.Windows.Forms.View.Details;
            this.lsbSlaves.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lsbSlaves_MouseDoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "ID Slave";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "IP";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "porta";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnHotkeyManage);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(307, 15);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(246, 146);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "HotKey";
            // 
            // btnHotkeyManage
            // 
            this.btnHotkeyManage.Location = new System.Drawing.Point(72, 123);
            this.btnHotkeyManage.Name = "btnHotkeyManage";
            this.btnHotkeyManage.Size = new System.Drawing.Size(75, 23);
            this.btnHotkeyManage.TabIndex = 1;
            this.btnHotkeyManage.Text = "Inizia";
            this.btnHotkeyManage.UseVisualStyleBackColor = true;
            this.btnHotkeyManage.Click += new System.EventHandler(this.btnHotkeyManage_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(217, 104);
            this.label1.TabIndex = 0;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lsbSlaves);
            this.groupBox3.Controls.Add(this.button8);
            this.groupBox3.Controls.Add(this.btnSlavesNew);
            this.groupBox3.Location = new System.Drawing.Point(12, 5);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(289, 307);
            this.groupBox3.TabIndex = 15;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Master";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(559, 317);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button3);
            this.Name = "Form1";
            this.Text = "Form1 by Marco R";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton radioButtonHookGlobal;
        private System.Windows.Forms.RadioButton radioButtonHookLocal;
        private System.Windows.Forms.Button btnHook;
        private System.Windows.Forms.RadioButton rdbMouseRel;
        private System.Windows.Forms.RadioButton rdbMouseAbsResc;
        private System.Windows.Forms.RadioButton rdbMouseAbs;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnSlavesNew;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.ListView lsbSlaves;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnHotkeyManage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.GroupBox groupBox3;

    }
}

