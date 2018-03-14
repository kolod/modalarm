/*
 * Copyright 2017-2018 Alexandr Kolodkin <alexandr.kolodkin@gmail.com>
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
 * Summary  : Server module logic
 * 
 * Author   : Alexandr Kolodkin
 * Created  : 2017
 * Modified : 2018
 */

using Scada.Data.Tables;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using System.Threading;
using Utils;
using NAudio.Wave;

namespace Scada.Server.Modules
{
    /// <summary>
    /// Server module logic
    /// <para>Логика работы серверного модуля</para>
    /// </summary>
    public class ModAlarmLogic : ModLogic
    {
        /// <summary>
        /// Имя файла журнала работы модуля
        /// </summary>
        internal const string LogFileName = "ModAlarm.log";

        /// <summary>
        /// Имя файла информации о работе модуля
        /// </summary>
        private const string InfoFileName = "ModAlarm.txt";

        private bool normalWork;          // признак нормальной работы модуля
        private string workState;         // строковая запись состояния работы
        private string infoFileName;      // полное имя файла информации
        private Log log;                  // журнал работы модуля
        private Config config;            // конфигурация модуля
        private SortedDictionary<int, bool> lastState = new SortedDictionary<int, bool>();          // предыдущее состояние сигнала аварии
        private SortedDictionary<int, WaveOut> waveOuts = new SortedDictionary<int, WaveOut>();     //


        /// <summary>
        /// Конструктор
        /// </summary>
        public ModAlarmLogic()
        {
            normalWork = true;
            workState = Localization.UseRussian ? "норма" : "normal";
        }


        /// <summary>
        /// Получить имя модуля
        /// </summary>
        public override string Name
        {
            get { return "ModAlarm"; }
        }


        /// <summary>
        /// Записать в файл информацию о работе модуля
        /// </summary>
        private void WriteInfo()
        {
            try
            {
                // формирование текста
                StringBuilder sbInfo = new StringBuilder();

                if (Localization.UseRussian)
                {
                    sbInfo
                        .AppendLine("Модуль аварийной сигнализации")
                        .AppendLine("----------------------")
                        .Append("Состояние: ").AppendLine(workState).AppendLine();
                    foreach (KeyValuePair<int, string> channel in config.channels)
                    {
                        sbInfo
                            .Append("Канал: ").Append(channel.Key)
                            .Append(", Аудио файл: ").AppendLine(channel.Value);

                        if (!File.Exists(channel.Value))
                        {
                            log.WriteAction(string.Format("Ошибка файл '{0}' не найден.", channel.Value));
                        }
                    }
                }
                else
                {
                    sbInfo
                        .AppendLine("Sound Alarm Module")
                        .AppendLine("------------------")
                        .Append("State: ").AppendLine(workState).AppendLine();

                    foreach (KeyValuePair<int, string> channel in config.channels)
                    {
                        sbInfo
                            .Append("Channel: ").Append(channel.Key)
                            .Append(", Sound file: ").AppendLine(channel.Value);

                        if (!File.Exists(channel.Value))
                        {
                            log.WriteAction(string.Format("Error file '{0}' not found.", channel.Value));
                        }
                    }
                }

                // вывод в файл
                using (StreamWriter writer = new StreamWriter(infoFileName, false, Encoding.UTF8))
                    writer.Write(sbInfo.ToString());
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception ex)
            {
                log.WriteAction(ModPhrases.WriteInfoError + ": " + ex.Message, Log.ActTypes.Exception);
            }
        }


        /// <summary>
        /// Выполнить действия при запуске работы сервера
        /// </summary>
        public override void OnServerStart()
        {
            // определение полного имени файла информации
            infoFileName = AppDirs.LogDir + InfoFileName;

            // вывод в журнал
            log = new Log(Log.Formats.Simple);
            log.Encoding = Encoding.UTF8;
            log.FileName = AppDirs.LogDir + LogFileName;
            log.WriteBreak();
            log.WriteAction(string.Format(ModPhrases.StartModule, Name));
            
            // загрузка конфигурации
            config = new Config(AppDirs.ConfigDir);
            string errMsg;

            if (config.Load(out errMsg))
            {
                foreach (int channel in config.channels.Keys)
                {
                    lastState.Add(channel, false);
                }
                WriteInfo();
            }
            else
            {
                normalWork = false;
                workState = Localization.UseRussian ? "ошибка" : "error";
                log.WriteAction(errMsg);
                log.WriteAction(ModPhrases.NormalModExecImpossible);
            }
        }


        /// <summary>
        /// Выполнить действия при остановке работы сервера
        /// </summary>
        public override void OnServerStop()
        {
            // вывод информации
            workState = Localization.UseRussian ? "остановлен" : "stopped";
            WriteInfo();
            log.WriteAction(string.Format(ModPhrases.StopModule, Name));
            log.WriteBreak();
        }


        /// <summary>
        /// Создать экземпляр ласса для воспроизведения файла
        /// </summary>
        private bool AddWaweOut(int channel)
        {
            try
            {
                if (waveOuts.ContainsKey(channel))
                {
                    log.WriteAction(string.Format("ContainsKey: {0}", channel));

                    if (config.channels[channel] == null)
                    {
                        log.WriteAction(string.Format("Channel: {0}", channel));
                        WaveFileReader reader = new WaveFileReader(config.channels[channel]);
                        LoopStream loop = new LoopStream(reader);
                        waveOuts[channel] = new WaveOut();
                        waveOuts[channel].Init(loop);
                        return true;
                    }
                    else return true;
                }
                else
                {
                    log.WriteAction(string.Format("Create: {0}", channel));

                    WaveFileReader reader = new WaveFileReader(config.channels[channel]);
                    LoopStream loop = new LoopStream(reader);
                    waveOuts.Add(channel, new WaveOut());
                    waveOuts[channel].Init(loop);
                    return true;
                }
            }
            catch(Exception ex)
            {
                log.WriteAction(string.Format(Localization.UseRussian ?
                    "Ошибка при создании класса waveOut для '{0}': {1}" :
                    "Error while creating of the waveOut class for '{0}': {1}", config.channels[channel], ex.Message));
                return false;
            }
        }


        /// <summary>
        /// Запустить воспроизведение звука (поддерживается wav формат)
        /// </summary>
        private void StartAlarm(int channel)
        {
            try
            {
                if (AddWaweOut(channel)) waveOuts[channel].Play();
            }
            catch (Exception ex)
            {
                log.WriteAction(string.Format(Localization.UseRussian ?
                    "Ошибка при воспроизведении аудиофайла {0}: {1}" :
                    "Error playing audio file {0}: {1}", config.channels[channel], ex.Message));
            }
        }


        /// <summary>
        /// Остановить воспроизведение звука
        /// </summary>
        private void StopAlarm(int channel)
        {
            try
            {
                if (waveOuts.ContainsKey(channel) && (waveOuts[channel] != null))
                {
                    log.WriteAction(string.Format("Delete: {0}", channel));
                    waveOuts[channel].Stop();
                    waveOuts[channel].Dispose();
                    waveOuts.Remove(channel);
                }
            }
            catch (Exception ex)
            {
                log.WriteAction(string.Format(Localization.UseRussian ?
                    "Ошибка при остановке воспроизведения аудиофайла '{0}': {1}" :
                    "Error while stoping audio file '{0}': {1}", config.channels[channel], ex.Message));
            }
        }


        /// <summary>
        /// Метод выполняется после вычисления дорасчётных каналов текущего среза (примерно каждые 100 мс)
        /// </summary>
        public override void OnCurDataCalculated(int[] cnlNums, SrezTableLight.Srez curSrez)
        {
            if (normalWork)
            {
                try
                {
                    foreach (int channel in config.channels.Keys)
                    {
                        SrezTableLight.CnlData cnlData;

                        if (curSrez.GetCnlData(channel, out cnlData))
                        {
                            bool state = Math.Abs(cnlData.Val) > 0.001;
                            if (lastState[channel] != state)
                            {
                                lastState[channel] = state;
                                if (state) StartAlarm(channel); else StopAlarm(channel);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.WriteException(ex);
                }
            }
        }
    }
}
