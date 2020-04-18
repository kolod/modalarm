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


using System;
using System.Windows.Forms;
using System.IO;

// This is the code for your desktop app.
// Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.

namespace Scada.Server.Modules.Alarm
{
    public partial class ModAlarmTest : Form
    {
        private Button btnRunTestRu;
        private Button btnRunTestEn;
        private AppDirs appDirs;
        private ModView view;

        public void loadMod(string lang)
        {
            Localization.ChangeCulture(lang);

            view = new ModAlarmView();

            AppDirs appDirs = new AppDirs();
            appDirs.Init(Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(ModAlarmView)).Location));

            Directory.CreateDirectory(appDirs.ConfigDir);
            Directory.CreateDirectory(appDirs.LangDir);
            Directory.CreateDirectory(appDirs.LogDir);
            Directory.CreateDirectory(appDirs.ModDir);
            Directory.CreateDirectory(appDirs.StorageDir);

            view.AppDirs = appDirs;
        }

        public ModAlarmTest()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Click on the link below to continue learning how to build a desktop app using WinForms!
            System.Diagnostics.Process.Start("http://aka.ms/dotnet-get-started-desktop");

        }


        private void InitializeComponent()
        {
            this.btnRunTestRu = new System.Windows.Forms.Button();
            this.btnRunTestEn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnRunTestRu
            // 
            this.btnRunTestRu.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRunTestRu.Location = new System.Drawing.Point(12, 12);
            this.btnRunTestRu.Name = "btnRunTestRu";
            this.btnRunTestRu.Size = new System.Drawing.Size(296, 42);
            this.btnRunTestRu.TabIndex = 0;
            this.btnRunTestRu.Text = "Show russian config...";
            this.btnRunTestRu.UseVisualStyleBackColor = true;
            this.btnRunTestRu.Click += new System.EventHandler(this.btnRunTestRu_Click);
            // 
            // btnRunTestEn
            // 
            this.btnRunTestEn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRunTestEn.Location = new System.Drawing.Point(12, 60);
            this.btnRunTestEn.Name = "btnRunTestEn";
            this.btnRunTestEn.Size = new System.Drawing.Size(296, 42);
            this.btnRunTestEn.TabIndex = 1;
            this.btnRunTestEn.Text = "Show english config...";
            this.btnRunTestEn.UseVisualStyleBackColor = true;
            this.btnRunTestEn.Click += new System.EventHandler(this.btnRunTestEn_Click);
            // 
            // ModAlarmTest
            // 
            this.ClientSize = new System.Drawing.Size(320, 112);
            this.Controls.Add(this.btnRunTestEn);
            this.Controls.Add(this.btnRunTestRu);
            this.Name = "ModAlarmTest";
            this.Text = "ModAlarm Test";
            this.ResumeLayout(false);

        }

        private void btnRunTestRu_Click(object sender, EventArgs e)
        {
            loadMod("ru-RU");
            if ((view != null) && view.CanShowProps)
            {
                view.ShowProps();
            }
        }

        private void btnRunTestEn_Click(object sender, EventArgs e)
        {
            loadMod("en-GB");
            if ((view != null) && view.CanShowProps)
            {
                view.ShowProps();
            }
        }
    }
}
