/*
 * Copyright 2017 Alexandr Kolodkin
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
 * Module   : ModTest
 * Summary  : About form
 * 
 * Author   : Alexandr Kolodkin
 * Created  : 2017
 * Modified : 2017
 */

using Scada.Client;
using Scada.UI;
using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace Scada.Server.Modules.Alarm
{
    /// <summary>
    /// Module configuration form
    /// <para>Форма конфигурации</para>
    /// </summary>
    public partial class FrmAlarmConfig : Form
    {
        private AppDirs appDirs;       // директории приложения
        private ServerComm serverComm; // объект для обмена данными со SCADA-Сервером

        private Config config;         // конфигурация модуля
        private Config configCopy;     // копия конфигурации модуля для реализации отмены изменений
        private bool modified;         // признак изменения конфигурации
        private bool changing;         // происходит изменение значений элементов управления


        /// <summary>
        /// Конструктор, ограничивающий создание формы без параметров
        /// </summary>
        public FrmAlarmConfig()
        {
            InitializeComponent();

            config = null;
            configCopy = null;
            modified = false;
            changing = false;
        }


        /// <summary>
        /// Получить или установить признак изменения конфигурации
        /// </summary>
        private bool Modified
        {
            get
            {
                return modified;
            }
            set
            {
                modified = value;
                btnSave.Enabled = modified;
                btnCancel.Enabled = modified;
            }
        }


        /// <summary>
        /// Отобразить форму модально
        /// </summary>
        public static void ShowDialog(AppDirs appDirs, ServerComm serverComm)
        {
            FrmAlarmConfig frmAlarmConfig = new FrmAlarmConfig();
            frmAlarmConfig.appDirs = appDirs;
            frmAlarmConfig.serverComm = serverComm;
            frmAlarmConfig.ShowDialog();
        }
        

        /// <summary>
        /// Сохранить конфигурацию модуля
        /// </summary>
        private bool SaveConfig()
        {
            if (Modified)
            {
                string errMsg;
                if (config.Save(out errMsg))
                {
                    Modified = false;
                    return true;
                }
                else
                {
                    ScadaUiUtils.ShowError(errMsg);
                    return false;
                }
            }
            else
            {
                return true;
            }
        }


        /// <summary>
        /// Отобразить конфигурацию
        /// </summary>
        private void ConfigToControls()
        {
            changing = true;
            inputChanel.SetValue(config.ChanelNumber);
            inputPath.Text = config.SoundFileName;
            changing = false;
        }


        /// <summary>
        /// Отобразить конфигурацию
        /// </summary>
        private void FrmAlarmConfig_Load(object sender, EventArgs e)
        {
            // локализация модуля
            string errMsg;
            if (!Localization.UseRussian)
            {
                if (Localization.LoadDictionaries(appDirs.LangDir, "ModAlarm", out errMsg))
                    Translator.TranslateForm(this, "Scada.Server.Modules.Alarm.FrmAlarmConfig");
                else
                    ScadaUiUtils.ShowError(errMsg);
            }

            // загрузка конфигурации
            config = new Config(appDirs.ConfigDir);
            if (!config.Load(out errMsg)) ScadaUiUtils.ShowError(errMsg);

            // создание копии конфигурации
            configCopy = config.Clone();

            // отображение конфигурации
            ConfigToControls();

            // снятие признака изменения конфигурации
            Modified = false;
        }


        private void linkAuthor_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:alexandr.kolodkin@gmail.com");
        }


        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                inputPath.Text = openFileDialog.FileName;
            }
        }


        private void inputChanel_ValueChanged(object sender, EventArgs e)
        {
            if (!changing)
            {
                config.ChanelNumber = Decimal.ToInt32(inputChanel.Value);
                Modified = true;
            }
        }


        private void inputPath_TextChanged(object sender, EventArgs e)
        {
            if (!changing)
            {
                config.SoundFileName = inputPath.Text;
                Modified = true;
            }
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            // сохранение конфигурации модуля
            SaveConfig();
        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            // отмена изменений конфигурации
            config = configCopy;
            configCopy = config.Clone();
            ConfigToControls();
            Modified = false;
        }


        private void FrmAlarmConfig_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Modified)
            {
                DialogResult result = MessageBox.Show(ModPhrases.SaveModSettingsConfirm,
                    CommonPhrases.QuestionCaption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                switch (result)
                {
                    case DialogResult.Yes:
                        if (!SaveConfig())
                            e.Cancel = true;
                        break;
                    case DialogResult.No:
                        break;
                    default:
                        e.Cancel = true;
                        break;
                }
            }
        }
    }
}
