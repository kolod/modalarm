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
using System.Runtime.InteropServices;
using Utils;

namespace Scada.Server.Modules
{
    internal static class NativeMethods
    {
        [DllImport("winmm.dll", EntryPoint = "PlaySound", SetLastError = true, CharSet = CharSet.Unicode, ThrowOnUnmappableChar = true)]
        public static extern bool PlaySound(
            string szSound,
            System.IntPtr hMod,
            PlaySoundFlags flags);

        [System.Flags]
        public enum PlaySoundFlags : int
        {
            SND_SYNC        = 0x00000000, // play synchronously (default)
            SND_ASYNC       = 0x00000001, // play asynchronously
            SND_NODEFAULT   = 0x00000002, // silence (!default) if sound not found
            SND_MEMORY      = 0x00000004, // pszSound points to a memory file
            SND_LOOP        = 0x00000008, // loop the sound until next sndPlaySound
            SND_NOSTOP      = 0x00000010, // don't stop any currently playing sound
            SND_NOWAIT      = 0x00002000, // don't wait if the driver is busy
            SND_ALIAS       = 0x00010000, // name is a registry alias
            SND_ALIAS_ID    = 0x00110000, // alias is a pre d ID
            SND_FILENAME    = 0x00020000, // name is file name
            SND_RESOURCE    = 0x00040004, // name is resource name or atom
            SND_PURGE       = 0000000040, // purge non-static events for task
            SND_APPLICATION = 0000000080, // look for application specific association
            SND_SENTRY      = 0x00080000, // Generate a SoundSentry event with this sound
            SND_RING        = 0x00100000, // Treat this as a "ring" from a communications app - don't duck me
            SND_SYSTEM      = 0x00200000  // Treat this as a system sound
        }
    }

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
            NativeMethods.PlaySound(
              config.SoundFileName, 
              new System.IntPtr(), 
              NativeMethods.PlaySoundFlags.SND_ASYNC | NativeMethods.PlaySoundFlags.SND_SYSTEM | NativeMethods.PlaySoundFlags.SND_LOOP
            );
        }


        /// <summary>
        /// Остановить воспроизведение звука
        /// </summary>
        private void StopAlarm()
        {
            NativeMethods.PlaySound(null, new System.IntPtr(), NativeMethods.PlaySoundFlags.SND_SYNC);
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
