using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Inspirometer_AlarmProgram
{
    public partial class Service1 : ServiceBase
    {
        // 타이머를 사용하여 알람 시간을 체크하고 알람이 발생하면 프로그램을 실행
        private FileSystemWatcher watcher;
        private System.Timers.Timer timer;
        private DateTime? alarmTime;
        private string logFilePath = "C:\\Users\\user\\AppData\\LocalLow\\DefaultCompany\\tmp-inspirometer"; // 로그 파일이 저장되는 폴더 경로
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // 유동적인 경로 설정
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            logFilePath = Path.Combine(userProfile, "AppData\\LocalLow\\DefaultCompany\\tmp-inspirometer");

            // 파일 시스템 감시자 설정            
            watcher = new FileSystemWatcher();
            watcher.Path = logFilePath;
            watcher.Filter = "*.log"; // 로그 파일의 확장자
            watcher.Created += new FileSystemEventHandler(OnLogFileChanged);
            watcher.Changed += new FileSystemEventHandler(OnLogFileChanged);
            watcher.EnableRaisingEvents = true;            

            // 타이머 설정: 1분마다 체크
            timer = new System.Timers.Timer(60000);
            timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            timer.Start();
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            CheckAlarm();
        }
        private void OnLogFileChanged(object source, FileSystemEventArgs e)
        {
            // 로그 파일에서 시간 데이터를 읽어 알람 시간 설정
            alarmTime = GetAlarmTimeFromLog(e.FullPath);
        }
        private DateTime? GetAlarmTimeFromLog(string filePath)
        {
            try
            {
                string[] lines = File.ReadAllLines(filePath);
                // 로그 파일에서 시간 데이터를 추출하는 로직 구현 (예: 마지막 줄의 시간을 읽음)
                string lastLine = lines[lines.Length - 1];
                if (DateTime.TryParse(lastLine, out DateTime logTime))
                {
                    return logTime.AddHours(1); // 로그 시간으로부터 1시간 뒤 알람 시간 설정
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Failed to read log file: " + ex.Message);
            }
            return null;
        }
        protected override void OnStop()
        {
            timer.Stop();
        }

        private void CheckAlarm()
        {
            // 현재 시간과 알람 시간 비교
            if (alarmTime.HasValue && DateTime.Now >= alarmTime.Value && DateTime.Now < alarmTime.Value.AddMinutes(1))
            {
                // 알람 발생
                TriggerAlarm();
                alarmTime = null; // 알람 시간이 한번 실행되면 초기화
            }
        }
        private void TriggerAlarm()
        {            
            // 데스크탑의 Alarm 폴더 경로 가져오기
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string alarmAppPath = Path.Combine(desktopPath, "Alarm", "AlarmApp.exe");

            // 알람 프로그램 실행
            if (File.Exists(alarmAppPath))
            {
                Process.Start(alarmAppPath);
            }
            else
            {
                // 경로가 존재하지 않으면 로깅 또는 예외 처리
                EventLog.WriteEntry("AlarmApp.exe not found in the specified path: " + alarmAppPath);
            }
        }
    }
}
