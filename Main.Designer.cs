namespace Piealytics
{
    partial class MainWindow
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.chartFrame = new System.Windows.Forms.PictureBox();
            this.mainMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.statusBar_statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.input_historyLength = new System.Windows.Forms.NumericUpDown();
            this.statusBar_frequency = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusBar_range = new System.Windows.Forms.ToolStripStatusLabel();
            this.label_historyLength = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chartFrame)).BeginInit();
            this.statusBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.input_historyLength)).BeginInit();
            this.SuspendLayout();
            // 
            // chartFrame
            // 
            this.chartFrame.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chartFrame.Location = new System.Drawing.Point(0, 42);
            this.chartFrame.Margin = new System.Windows.Forms.Padding(0);
            this.chartFrame.Name = "chartFrame";
            this.chartFrame.Size = new System.Drawing.Size(783, 489);
            this.chartFrame.TabIndex = 0;
            this.chartFrame.TabStop = false;
            // 
            // mainMenu
            // 
            this.mainMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(61, 4);
            // 
            // statusBar
            // 
            this.statusBar.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusBar_statusLabel,
            this.statusBar_frequency,
            this.statusBar_range});
            this.statusBar.Location = new System.Drawing.Point(0, 528);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(782, 25);
            this.statusBar.TabIndex = 3;
            this.statusBar.Text = "Status: Connected";
            // 
            // statusBar_statusLabel
            // 
            this.statusBar_statusLabel.Name = "statusBar_statusLabel";
            this.statusBar_statusLabel.Size = new System.Drawing.Size(200, 20);
            this.statusBar_statusLabel.Text = "Status: Warte auf Verbindung";
            // 
            // input_historyLength
            // 
            this.input_historyLength.Location = new System.Drawing.Point(130, 7);
            this.input_historyLength.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.input_historyLength.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.input_historyLength.Name = "input_historyLength";
            this.input_historyLength.Size = new System.Drawing.Size(80, 22);
            this.input_historyLength.TabIndex = 4;
            this.input_historyLength.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.input_historyLength.ValueChanged += new System.EventHandler(this.Input_historyLength_ValueChanged);
            // 
            // statusBar_frequency
            // 
            this.statusBar_frequency.Name = "statusBar_frequency";
            this.statusBar_frequency.Size = new System.Drawing.Size(0, 20);
            // 
            // statusBar_range
            // 
            this.statusBar_range.Name = "statusBar_range";
            this.statusBar_range.Size = new System.Drawing.Size(0, 20);
            // 
            // label_historyLength
            // 
            this.label_historyLength.AutoSize = true;
            this.label_historyLength.Location = new System.Drawing.Point(12, 9);
            this.label_historyLength.Name = "label_historyLength";
            this.label_historyLength.Size = new System.Drawing.Size(112, 17);
            this.label_historyLength.TabIndex = 5;
            this.label_historyLength.Text = "Zeitfenster (ms):";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(782, 553);
            this.Controls.Add(this.label_historyLength);
            this.Controls.Add(this.input_historyLength);
            this.Controls.Add(this.chartFrame);
            this.Controls.Add(this.statusBar);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "MainWindow";
            this.Text = "Piealytics";
            ((System.ComponentModel.ISupportInitialize)(this.chartFrame)).EndInit();
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.input_historyLength)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox chartFrame;
        private System.Windows.Forms.ContextMenuStrip mainMenu;
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel statusBar_statusLabel;
        private System.Windows.Forms.NumericUpDown input_historyLength;
        private System.Windows.Forms.ToolStripStatusLabel statusBar_frequency;
        private System.Windows.Forms.ToolStripStatusLabel statusBar_range;
        private System.Windows.Forms.Label label_historyLength;
    }
}

