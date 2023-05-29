using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavisionBackupHelper
{
    public class BackupJob :IJob
    {
        private ILogger<BackupJob> _logger;
        private IConfiguration _config;
        private bool _isRunning = false;
        private Process _backup;

        public BackupJob(ILogger<BackupJob> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;

            _backup = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = _config.GetSection("NavisionBackup:Program").Get<string>(),
                    Arguments = _config.GetSection("NavisionBackup:Config").Get<string>()
                }
            };
        }

        public async Task Execute(IJobExecutionContext context)
        {
            if (!_isRunning)
            {
                _isRunning = true;
                Process _tempProcess = _backup;
                _tempProcess.Start();
                _logger.LogInformation("Backup started");
                _tempProcess.WaitForExitAsync();
            }
            else
            {
                _logger.LogInformation("Backup is running");
            }
            
        }
    }
}
