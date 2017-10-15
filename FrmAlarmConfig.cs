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

using System;
using System.Windows.Forms;

namespace Scada.Server.Modules
{
    /// <summary>
    /// About form
    /// <para>Форма о программе</para>
    /// </summary>
    public partial class FrmAlarmConfig : Form
    {
        public FrmAlarmConfig()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
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
    }
}
