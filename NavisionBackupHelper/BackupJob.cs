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
        private bool _unlockFiles = false;
        private List<string> _filesToUnlock = new List<string>();
        private List<string> _folderToUnlock = new List<string>();

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
            _unlockFiles = _config.GetValue<bool>("NavisionBackup:Unlock:Enabled");
            if (_unlockFiles)
            {
                _filesToUnlock = _config.GetSection("NavisionBackup:Unlock:UnlockFiles").Get<List<string>>();
                _folderToUnlock = _config.GetSection("NavisionBackup:Unlock:UnlockFolder").Get<List<string>>();
            }

        }

        public async Task Execute(IJobExecutionContext context)
        {
            if (!_isRunning)
            {
                try
                {
                    _isRunning = true;
                    Process _tempProcess = _backup;
                    _tempProcess.Start();
                    _logger.LogInformation("Backup started");
                    await _tempProcess.WaitForExitAsync();
                    _logger.LogInformation("Backup finished");
                }catch(Exception ex)
                {
                    _logger.LogError("Something failed in the backup");
                }
            }
            else
            {
                _logger.LogInformation("Backup is running");
            }
        }
    }
}
