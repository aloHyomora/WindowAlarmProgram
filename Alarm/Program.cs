using System;
using System.Windows.Forms;
using System.Media;
using System.Diagnostics;
using System.IO;

namespace Alarm
{
    internal class Program
    {
        [STAThread]
        static void Main()
        {
            // 소리 재생
            PlaySound();

            // 메시지 박스를 사용하여 알람을 표시
            MessageBox.Show("호흡기 운동 시간입니다!", "알람", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private static void PlaySound()
        {
            try
            {
                // 사운드 파일 경로 (WAV 파일)
                // 데스크탑의 Alarm 폴더 경로 가져오기
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string soundFilePath = Path.Combine(desktopPath,"sound.mp3");
              
                using (SoundPlayer player = new SoundPlayer(soundFilePath))
                {
                    player.PlaySync(); // 소리를 재생하고 완료될 때까지 기다림
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("사운드를 재생할 수 없습니다: " + ex.Message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }            
        }
    }
}
