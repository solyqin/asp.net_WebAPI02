using Atlas_WebAPI_V02.JobExcutor;
using Atlas_WebAPI_V02.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atlas_WebAPI_V02.JobTrigger
{
    public class Trigger
    {
        public class TestJobTrigger : BaseJobTrigger
        {
            public TestJobTrigger() :
                base(TimeSpan.Zero,
                    TimeSpan.FromMinutes(19),  //循环时间间隔
                    new TestJobExcutor())   //执行对象
            {
            }
        }
        public class TestJobExcutor
                         : IJobExecutor
        {
            public void StartJob()
            {
                //LogUtil.Info("执行任务！");
               
                FileManage.DeleteExpiredFiles();
            }

            public void StopJob()
            {
                //  LogUtil.Info("系统终止任务");
               
            }
        }
    }
}
