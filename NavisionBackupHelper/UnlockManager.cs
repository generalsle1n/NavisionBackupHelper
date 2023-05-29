using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavisionBackupHelper
{
    public class UnlockManager
    {
        private ILogger<UnlockManager> _logger;

        public UnlockManager(ILogger<UnlockManager> logger)
        {
            _logger = logger;
        }
    }
}
