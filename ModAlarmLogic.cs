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
 * Module   : ModAlarm
 * Summary  : Server module logic
 * 
 * Author   : Alexandr Kolodkin
 * Created  : 2017
 * Modified : 2017
 */
 
using Scada.Data.Tables;
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
        private bool lastState;           // предыдущее состояние сигнала аварии
        private WaveOut waveOut;          // 


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
                        .Append("Состояние: ").AppendLine(workState).AppendLine()
                        .Append("Аудио файл: ").AppendLine(config.SoundFileName);
                }
                else
                {
                    sbInfo
                        .AppendLine("Sound Alarm Module")
                        .AppendLine("------------------")
                        .Append("State: ").AppendLine(workState).AppendLine()
                        .Append("Sound file: ").AppendLine(config.SoundFileName);
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
            // обнуление состояния
            lastState = false;

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

            if (!config.Load(out errMsg))
            {
                normalWork = false;
                workState = Localization.UseRussian ? "ошибка" : "error";
                log.WriteAction(errMsg);
                log.WriteAction(ModPhrases.NormalModExecImpossible);
            }

            if (config.ChanelNumber < 0) normalWork = false;
            WriteInfo();
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
        /// Запустить воспроизведение звука (поддерживается wav формат)
        /// </summary>
        private void StartAlarm()
        {
            try
            {
                if (waveOut == null)
                {
                    WaveFileReader reader = new WaveFileReader(config.SoundFileName);
                    LoopStream loop = new LoopStream(reader);
                    waveOut = new WaveOut();
                    waveOut.Init(loop);
                    waveOut.Play();
                }
            }
            catch (Exception ex)
            {
                log.WriteAction(string.Format(Localization.UseRussian ?
                    "Ошибка при воспроизведении аудиофайла {0}: {1}" :
                    "Error playing audio file {0}: {1}", config.SoundFileName, ex.Message));
            }
        }


        /// <summary>
        /// Остановить воспроизведение звука
        /// </summary>
        private void StopAlarm()
        {
            try
            {
                if (waveOut != null)
                {
                    waveOut.Stop();
                    waveOut.Dispose();
                    waveOut = null;
                }
            }
            catch (Exception ex)
            {
                log.WriteAction(string.Format(Localization.UseRussian ?
                    "Ошибка при остановке воспроизведения аудиофайла {0}: {1}" :
                    "Error while stoping audio file {0}: {1}", config.SoundFileName, ex.Message));
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
                    SrezTableLight.CnlData cnlData;
                    if (curSrez.GetCnlData(config.ChanelNumber, out cnlData))
                    {
                        bool state = cnlData.Val > 0;
                        if (state != lastState)
                        {
                            if (state) StartAlarm(); else StopAlarm();
                        }
                        lastState = state;
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
