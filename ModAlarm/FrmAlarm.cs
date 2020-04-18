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


using Scada.Client;
using Scada.UI;
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NAudio.Wave;

namespace Scada.Server.Modules
{
    public partial class FrmAlarm : Form
    {
        public string SoundFilePath;
        public int Channel;
        public bool isEdit = false;

        private AppDirs appDirs;         // директории приложения
        private WaveOut waveOut = null;

        private string localization()
        {
            return isEdit ? "Scada.Server.Modules.Alarm.FrmEditAlarm" : "Scada.Server.Modules.Alarm.FrmAddAlarm";
        }


        public FrmAlarm(AppDirs appDirs)
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

            // обновление состояние кнопки ОK
            UpdateOkButton();
        }

        private void inputPath_TextChanged(object sender, EventArgs e)
        {
            SoundFilePath = inputPath.Text;

            // обновление состояние кнопки ОK
            UpdateOkButton();
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
                {
                    Translator.TranslateForm(this, localization());

                    openFileDialog.Filter = Localization.Dictionaries[localization()]
                        .GetPhrase("openFileDialog.Filter", openFileDialog.Filter);
                }
                else
                    ScadaUiUtils.ShowError(errMsg);
            } 
            else if (isEdit)
            {
                Text = "Изменить аварию";
            }
        }

        private void btnTest_CheckedChanged(object sender, EventArgs e)
        {
            if (btnTest.Checked)
            {
                try
                {
                    WaveFileReader reader = new WaveFileReader(inputPath.Text);
                    LoopStream loop = new LoopStream(reader);
                    waveOut = new WaveOut();
                    waveOut.Init(loop);
                    waveOut.Play();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error");
                }
            }
            else
            {
                try
                {
                    waveOut.Stop();
                    waveOut.Dispose();
                    waveOut = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error");
                }
            }
        }

        private void FrmAddAlarm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (waveOut != null)
            {
                waveOut.Stop();
                waveOut.Dispose();
                waveOut = null;
            }
        }

        private void UpdateOkButton()
        {
            btnOk.Enabled = (File.Exists(SoundFilePath) && (Channel >= 0) && (Channel <= 65535));
        }

        private void FrmAddAlarm_Shown(object sender, EventArgs e)
        {
            // задание начальных значений
            inputChannel.Value = Channel;
            inputPath.Text = SoundFilePath;

            // обновление состояние кнопки ОK
            UpdateOkButton();
        }
    }
}
