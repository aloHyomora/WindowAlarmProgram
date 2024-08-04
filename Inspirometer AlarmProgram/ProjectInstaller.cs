using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Configuration.Install;

namespace Inspirometer_AlarmProgram
{    
    [RunInstaller(true)]
    public class ProjectInstaller : Installer
    {
        private ServiceProcessInstaller processInstaller;
        private ServiceInstaller serviceInstaller;

        public ProjectInstaller()
        {
            processInstaller = new ServiceProcessInstaller();
            serviceInstaller = new ServiceInstaller();

            // 서비스 계정 설정
            processInstaller.Account = ServiceAccount.LocalSystem;

            // 서비스 정보 설정
            serviceInstaller.DisplayName = "AlarmService";
            serviceInstaller.StartType = ServiceStartMode.Automatic;
            serviceInstaller.ServiceName = "AlarmService";

            Installers.Add(serviceInstaller);
            Installers.Add(processInstaller);
        }
    }
}
