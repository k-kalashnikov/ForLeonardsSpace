using Masofa.Client.Qwen.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masofa.Client.Qwen
{
    public class QwenUnitOfWork
    {
        public QwenRepository QwenRepository { get; }
        private HttpClient HttpClient { get; set; }

        public QwenUnitOfWork(ILogger<QwenUnitOfWork> logger)
        {
            HttpClient = new HttpClient();
            HttpClient.Timeout = TimeSpan.FromMinutes(5);
            QwenRepository = new QwenRepository(HttpClient, logger);
        }
    }
}
