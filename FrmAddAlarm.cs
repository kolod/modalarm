/*
 * Copyright 2017-2018 Alexandr Kolodkin
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
 * Summary  : Config form
 * 
 * Author   : Alexandr Kolodkin
 * Created  : 2018
 * Modified : 2018
 */


using Scada.Client;
using Scada.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Scada.Server.Modules
{
    public partial class FrmAddAlarm : Form
    {
        private AppDirs appDirs;       // директории приложения
        public string SoundFilePath;
        public int Channel;


        public FrmAddAlarm(AppDirs appDirs)
        {
            this.appDirs = appDirs;
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                inputPath.Text = openFileDialog.FileName;
            }
        }

        private void inputChannel_ValueChanged(object sender, EventArgs e)
        {
            Channel = Decimal.ToInt32(inputChannel.Value);
        }

        private void inputPath_TextChanged(object sender, EventArgs e)
        {
            SoundFilePath = inputPath.Text;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void FrmAddAlarm_Load(object sender, EventArgs e)
        {
            // локализация модуля
            string errMsg;
            if (!Localization.UseRussian)
            {
                if (Localization.LoadDictionaries(appDirs.LangDir, "ModAlarm", out errMsg))
                    Translator.TranslateForm(this, "Scada.Server.Modules.Alarm.FrmAddAlarm");
                else
                    ScadaUiUtils.ShowError(errMsg);
            }
        }
    }
}
