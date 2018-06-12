namespace RobotTraider
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
            this.textBox1 = new System.Windows.Forms.RichTextBox();
            this.Salestb = new System.Windows.Forms.RichTextBox();
            this.Buystb = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(120, 12);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(102, 307);
            this.textBox1.TabIndex = 3;
            this.textBox1.Text = "";
            // 
            // Salestb
            // 
            this.Salestb.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.Salestb.ForeColor = System.Drawing.Color.Red;
            this.Salestb.Location = new System.Drawing.Point(12, 12);
            this.Salestb.Name = "Salestb";
            this.Salestb.Size = new System.Drawing.Size(102, 149);
            this.Salestb.TabIndex = 4;
            this.Salestb.Text = "";
            // 
            // Buystb
            // 
            this.Buystb.ForeColor = System.Drawing.Color.ForestGreen;
            this.Buystb.Location = new System.Drawing.Point(12, 167);
            this.Buystb.Name = "Buystb";
            this.Buystb.Size = new System.Drawing.Size(102, 152);
            this.Buystb.TabIndex = 5;
            this.Buystb.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(299, 469);
            this.Controls.Add(this.Buystb);
            this.Controls.Add(this.Salestb);
            this.Controls.Add(this.textBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.RichTextBox textBox1;
        private System.Windows.Forms.RichTextBox Salestb;
        private System.Windows.Forms.RichTextBox Buystb;
    }
}

