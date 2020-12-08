using Atlas_WebAPI_V02.JobExcutor;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

//教程：https://www.cnblogs.com/lxhbky/p/12218988.html

namespace Atlas_WebAPI_V02
{
    public abstract class BaseJobTrigger
        : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly TimeSpan _dueTime;
        private readonly TimeSpan _periodTime;

        private readonly IJobExecutor _jobExcutor;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dueTime">到期执行时间</param>
        /// <param name="periodTime">间隔时间</param>
        /// <param name="jobExcutor">任务执行者</param>
        protected BaseJobTrigger(TimeSpan dueTime,
             TimeSpan periodTime,
             IJobExecutor jobExcutor)
        {
            _dueTime = dueTime;
            _periodTime = periodTime;
            _jobExcutor = jobExcutor;
        }

        #region  计时器相关方法

        private void StartTimerTrigger()
        {
            if (_timer == null)
                _timer = new Timer(ExcuteJob, _jobExcutor, _dueTime, _periodTime);
            else
                _timer.Change(_dueTime, _periodTime);
        }

        private void StopTimerTrigger()
        {
            _timer?.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void ExcuteJob(object obj)
        {
            try
            {
                var excutor = obj as IJobExecutor;
                excutor?.StartJob();
            }
            catch (Exception e)
            {
                //LogUtil.Error($"执行任务({nameof(GetType)})时出错，信息：{e}");
               Console.WriteLine($"执行任务({nameof(GetType)})时出错，信息：{e}");
            }
        }
        #endregion

        /// <summary>
        ///  系统级任务执行启动
        /// </summary>
        /// <returns></returns>
        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                StartTimerTrigger();
            }
            catch (Exception e)
            {
                // LogUtil.Error($"启动定时任务({nameof(GetType)})时出错，信息：{e}");
                Console.WriteLine($"启动定时任务({nameof(GetType)})时出错，信息：{e}");
            }
            return Task.CompletedTask;
        }

        /// <summary>
        ///  系统级任务执行关闭
        /// </summary>
        /// <returns></returns>
        public virtual Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                _jobExcutor.StopJob();
                StopTimerTrigger();
            }
            catch (Exception e)
            {
                // LogUtil.Error($"停止定时任务({nameof(GetType)})时出错，信息：{e}");
                Console.WriteLine($"停止定时任务({nameof(GetType)})时出错，信息：{e}");
            }
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
