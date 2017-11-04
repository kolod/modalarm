/*
 * Copyright 2015 Mikhail Shiryaev
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
 * Modified : 2017
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace Scada.Server.Modules
{
    /// <summary>
    /// Module configuration
    /// <para>Конфигурация модуля</para>
    /// </summary>
    internal class Config
    {
        /// <summary>
        /// Имя аудиофайла
        /// </summary>
        public string SoundFileName = "";


        /// <summary>
        /// Имя аудиофайла
        /// </summary>
        public int ChanelNumber = -1;


        /// <summary>
        /// Имя файла конфигурации
        /// </summary>
        private const string ConfigFileName = "ModAlarm.xml";


        /// <summary>
        /// Конструктор, ограничивающий создание объекта без параметров
        /// </summary>
        private Config()
        {
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public Config(string configDir)
        {
            FileName = ScadaUtils.NormalDir(configDir) + ConfigFileName;
            SetToDefault();
        }


        /// <summary>
        /// Получить полное имя файла конфигурации
        /// </summary>
        private string FileName { get; set; }


        /// <summary>
        /// Установить значения параметров конфигурации по умолчанию
        /// </summary>
        private void SetToDefault()
        {
            ChanelNumber = -1;
            SoundFileName = "";
        }

        /// <summary>
        /// Загрузить конфигурацию модуля
        /// </summary>
        public bool Load(out string errMsg)
        {
            SetToDefault();

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(FileName);

                ChanelNumber  = xmlDoc.DocumentElement.GetChildAsInt("Chanel");
                SoundFileName = xmlDoc.DocumentElement.GetChildAsString("Sound");

                errMsg = "";
                return true;
            }
            catch (FileNotFoundException ex)
            {
                errMsg = ModPhrases.LoadModSettingsError + ": " + ex.Message + 
                    Environment.NewLine + ModPhrases.ConfigureModule;
                return false;
            }
            catch (Exception ex)
            {
                errMsg = ModPhrases.LoadModSettingsError + ": " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Сохранить конфигурацию модуля
        /// </summary>
        public bool Save(out string errMsg)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();

                XmlDeclaration xmlDecl = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
                xmlDoc.AppendChild(xmlDecl);

                XmlElement root = xmlDoc.CreateElement("ModAlarm");
                xmlDoc.AppendChild(root);

                root.AppendElem("Sound", SoundFileName);
                root.AppendElem("Chanel", ChanelNumber);

                xmlDoc.Save(FileName);
                errMsg = "";
                return true;
            }
            catch (Exception ex)
            {
                errMsg = ModPhrases.SaveModSettingsError + ": " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Клонировать конфигурацию модуля
        /// </summary>
        public Config Clone()
        {
            Config configCopy = new Config();
            configCopy.FileName = FileName;
            configCopy.ChanelNumber = ChanelNumber;
            configCopy.SoundFileName = SoundFileName;

            return configCopy;
        }
    }
}
