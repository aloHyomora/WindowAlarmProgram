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

        private System.Timers.Timer timer;
        private DateTime alarmTime;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // 알람 시간 설정
            alarmTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 14, 0, 0); // 오후 2시로 설정

            // 타이머 설정: 1분마다 체크
            timer = new System.Timers.Timer(60000);
            timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            timer.Start();
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            CheckAlarm();
        }


        protected override void OnStop()
        {
            timer.Stop();
        }

        private void CheckAlarm()
        {
            // 현재 시간과 알람 시간 비교
            if (DateTime.Now >= alarmTime && DateTime.Now < alarmTime.AddMinutes(1))
            {
                // 알람 발생
                TriggerAlarm();
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
