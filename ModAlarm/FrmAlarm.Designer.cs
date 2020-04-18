/*
 * Copyright 2017-2020 Oleksandr Kolodkin <alexandr.kolodkin@gmail.com>
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 * 
 * Product  : Rapid SCADA
 * Module   : ModAlarm
 * Summary  : Module configuration
 * 
 * Author   : Alexandr Kolodkin
 * Created  : 2017
 * Modified : 2020
 */

namespace Scada.Server.Modules
{
    partial class FrmAlarm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
                waveOut.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCansel = new System.Windows.Forms.Button();
            this.lblChannel = new System.Windows.Forms.Label();
            this.inputChannel = new System.Windows.Forms.NumericUpDown();
            this.lblPath = new System.Windows.Forms.Label();
            this.inputPath = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.btnTest = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.inputChannel)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOk.Location = new System.Drawing.Point(121, 153);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(100, 28);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCansel
            // 
            this.btnCansel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCansel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCansel.Location = new System.Drawing.Point(15, 153);
            this.btnCansel.Name = "btnCansel";
            this.btnCansel.Size = new System.Drawing.Size(100, 28);
            this.btnCansel.TabIndex = 1;
            this.btnCansel.Text = "Отмена";
            this.btnCansel.UseVisualStyleBackColor = true;
            // 
            // lblChannel
            // 
            this.lblChannel.AutoSize = true;
            this.lblChannel.Location = new System.Drawing.Point(12, 9);
            this.lblChannel.Name = "lblChannel";
            this.lblChannel.Size = new System.Drawing.Size(168, 17);
            this.lblChannel.TabIndex = 2;
            this.lblChannel.Text = "Номер входного канала:";
            // 
            // inputChannel
            // 
            this.inputChannel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.inputChannel.Location = new System.Drawing.Point(15, 29);
            this.inputChannel.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.inputChannel.Name = "inputChannel";
            this.inputChannel.Size = new System.Drawing.Size(472, 22);
            this.inputChannel.TabIndex = 3;
            this.inputChannel.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.inputChannel.ValueChanged += new System.EventHandler(this.inputChannel_ValueChanged);
            // 
            // lblPath
            // 
            this.lblPath.AutoSize = true;
            this.lblPath.Location = new System.Drawing.Point(12, 54);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(87, 17);
            this.lblPath.TabIndex = 4;
            this.lblPath.Text = "Аудиофайл:";
            // 
            // inputPath
            // 
            this.inputPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.inputPath.Location = new System.Drawing.Point(15, 76);
            this.inputPath.Name = "inputPath";
            this.inputPath.Size = new System.Drawing.Size(472, 22);
            this.inputPath.TabIndex = 5;
            this.inputPath.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.inputPath.TextChanged += new System.EventHandler(this.inputPath_TextChanged);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(387, 153);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(100, 28);
            this.btnBrowse.TabIndex = 6;
            this.btnBrowse.Text = "Обзор";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "WAV аудио файл|*.wav";
            // 
            // btnTest
            // 
            this.btnTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTest.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnTest.Location = new System.Drawing.Point(281, 153);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(100, 28);
            this.btnTest.TabIndex = 7;
            this.btnTest.Text = "Проверка";
            this.btnTest.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.CheckedChanged += new System.EventHandler(this.btnTest_CheckedChanged);
            // 
            // FrmAddAlarm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCansel;
            this.ClientSize = new System.Drawing.Size(499, 193);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.inputPath);
            this.Controls.Add(this.lblPath);
            this.Controls.Add(this.inputChannel);
            this.Controls.Add(this.lblChannel);
            this.Controls.Add(this.btnCansel);
            this.Controls.Add(this.btnOk);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 240);
            this.Name = "FrmAddAlarm";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Добавить аварию";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmAddAlarm_FormClosing);
            this.Load += new System.EventHandler(this.FrmAddAlarm_Load);
            this.Shown += new System.EventHandler(this.FrmAddAlarm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.inputChannel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCansel;
        private System.Windows.Forms.Label lblChannel;
        private System.Windows.Forms.NumericUpDown inputChannel;
        private System.Windows.Forms.Label lblPath;
        private System.Windows.Forms.TextBox inputPath;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.CheckBox btnTest;
    }
}