namespace ConsoleOutputTest
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
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.cmdOK = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// listBox1
			// 
			this.listBox1.BackColor = System.Drawing.Color.Navy;
			this.listBox1.Font = new System.Drawing.Font("DejaVu Sans Mono", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.listBox1.ForeColor = System.Drawing.Color.Chartreuse;
			this.listBox1.FormattingEnabled = true;
			this.listBox1.ItemHeight = 10;
			this.listBox1.Location = new System.Drawing.Point(12, 12);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(720, 424);
			this.listBox1.TabIndex = 0;
			// 
			// cmdOK
			// 
			this.cmdOK.Enabled = false;
			this.cmdOK.Font = new System.Drawing.Font("Segoe UI Emoji", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.cmdOK.Location = new System.Drawing.Point(345, 446);
			this.cmdOK.Name = "cmdOK";
			this.cmdOK.Size = new System.Drawing.Size(107, 33);
			this.cmdOK.TabIndex = 1;
			this.cmdOK.Text = "👌 OK";
			this.cmdOK.UseVisualStyleBackColor = true;
			this.cmdOK.Visible = false;
			this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(744, 490);
			this.Controls.Add(this.cmdOK);
			this.Controls.Add(this.listBox1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.Button cmdOK;
	}
}

