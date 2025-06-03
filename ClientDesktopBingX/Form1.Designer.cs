namespace ClientDesktopBingX
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            plotView1 = new OxyPlot.WindowsForms.PlotView();
            CB_INSTRUMENTS = new ComboBox();
            label1 = new Label();
            label2 = new Label();
            CB_TIMEFRAME = new ComboBox();
            label3 = new Label();
            label4 = new Label();
            CB_INDICATORS = new ComboBox();
            label5 = new Label();
            L_TIME = new Label();
            label6 = new Label();
            L_RECOMMENDATION = new Label();
            UD_COUNT_BARS = new NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)UD_COUNT_BARS).BeginInit();
            SuspendLayout();
            // 
            // plotView1
            // 
            plotView1.Location = new Point(254, 35);
            plotView1.Name = "plotView1";
            plotView1.PanCursor = Cursors.Hand;
            plotView1.Size = new Size(946, 532);
            plotView1.TabIndex = 0;
            plotView1.Text = "plotView1";
            plotView1.ZoomHorizontalCursor = Cursors.SizeWE;
            plotView1.ZoomRectangleCursor = Cursors.SizeNWSE;
            plotView1.ZoomVerticalCursor = Cursors.SizeNS;
            // 
            // CB_INSTRUMENTS
            // 
            CB_INSTRUMENTS.DropDownStyle = ComboBoxStyle.DropDownList;
            CB_INSTRUMENTS.FormattingEnabled = true;
            CB_INSTRUMENTS.Location = new Point(26, 54);
            CB_INSTRUMENTS.Name = "CB_INSTRUMENTS";
            CB_INSTRUMENTS.Size = new Size(151, 28);
            CB_INSTRUMENTS.TabIndex = 1;
            CB_INSTRUMENTS.SelectedIndexChanged += CB_INSTRUMENTS_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(26, 31);
            label1.Name = "label1";
            label1.Size = new Size(149, 20);
            label1.TabIndex = 2;
            label1.Text = "Выбор инструмента";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(28, 85);
            label2.Name = "label2";
            label2.Size = new Size(149, 20);
            label2.TabIndex = 3;
            label2.Text = "Выбор таймфрейма";
            // 
            // CB_TIMEFRAME
            // 
            CB_TIMEFRAME.DropDownStyle = ComboBoxStyle.DropDownList;
            CB_TIMEFRAME.FormattingEnabled = true;
            CB_TIMEFRAME.Location = new Point(26, 108);
            CB_TIMEFRAME.Name = "CB_TIMEFRAME";
            CB_TIMEFRAME.Size = new Size(151, 28);
            CB_TIMEFRAME.TabIndex = 4;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(8, 139);
            label3.Name = "label3";
            label3.Size = new Size(185, 20);
            label3.TabIndex = 5;
            label3.Text = "Выбор количества баров";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(35, 192);
            label4.Name = "label4";
            label4.Size = new Size(142, 20);
            label4.TabIndex = 7;
            label4.Text = "Выбор индикатора";
            // 
            // CB_INDICATORS
            // 
            CB_INDICATORS.DropDownStyle = ComboBoxStyle.DropDownList;
            CB_INDICATORS.FormattingEnabled = true;
            CB_INDICATORS.Location = new Point(24, 215);
            CB_INDICATORS.Name = "CB_INDICATORS";
            CB_INDICATORS.Size = new Size(151, 28);
            CB_INDICATORS.TabIndex = 8;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 9F, FontStyle.Italic, GraphicsUnit.Point, 204);
            label5.Location = new Point(943, 9);
            label5.Name = "label5";
            label5.Size = new Size(154, 20);
            label5.TabIndex = 9;
            label5.Text = "Версия от 12.05.2025";
            // 
            // L_TIME
            // 
            L_TIME.AutoSize = true;
            L_TIME.Location = new Point(1015, 570);
            L_TIME.Name = "L_TIME";
            L_TIME.Size = new Size(185, 20);
            L_TIME.TabIndex = 10;
            L_TIME.Text = "Выбор количества баров";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(24, 494);
            label6.Name = "label6";
            label6.Size = new Size(164, 20);
            label6.TabIndex = 11;
            label6.Text = "Общая рекомендация";
            // 
            // L_RECOMMENDATION
            // 
            L_RECOMMENDATION.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            L_RECOMMENDATION.Location = new Point(24, 528);
            L_RECOMMENDATION.Name = "L_RECOMMENDATION";
            L_RECOMMENDATION.Size = new Size(164, 39);
            L_RECOMMENDATION.TabIndex = 12;
            L_RECOMMENDATION.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // UD_COUNT_BARS
            // 
            UD_COUNT_BARS.Location = new Point(28, 162);
            UD_COUNT_BARS.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            UD_COUNT_BARS.Name = "UD_COUNT_BARS";
            UD_COUNT_BARS.Size = new Size(150, 27);
            UD_COUNT_BARS.TabIndex = 13;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1236, 609);
            Controls.Add(UD_COUNT_BARS);
            Controls.Add(L_RECOMMENDATION);
            Controls.Add(label6);
            Controls.Add(L_TIME);
            Controls.Add(label5);
            Controls.Add(CB_INDICATORS);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(CB_TIMEFRAME);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(CB_INSTRUMENTS);
            Controls.Add(plotView1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "CDA - Commodity Data Analysis";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)UD_COUNT_BARS).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private OxyPlot.WindowsForms.PlotView plotView1;
        private ComboBox CB_INSTRUMENTS;
        private Label label1;
        private Label label2;
        private ComboBox CB_TIMEFRAME;
        private Label label3;
        private Label label4;
        private ComboBox CB_INDICATORS;
        private Label label5;
        private Label L_TIME;
        private Label label6;
        private Label L_RECOMMENDATION;
        private NumericUpDown UD_COUNT_BARS;
    }
}
