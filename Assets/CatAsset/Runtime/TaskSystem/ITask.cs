﻿namespace CatAsset.Runtime
{
    /// <summary>
    /// 任务接口
    /// </summary>
    public interface ITask
    {
        /// <summary>
        /// 持有者
        /// </summary>
        TaskRunner Owner { get; }
        
        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// 状态
        /// </summary>
        TaskState State { get; }
        
        /// <summary>
        /// 进度
        /// </summary>
        float Progress { get; }

        /// <summary>
        /// 添加子任务
        /// </summary>
        void AddChild(ITask child);
        
        /// <summary>
        /// 运行任务
        /// </summary>
        void Run();
        
        /// <summary>
        /// 轮询任务
        /// </summary>
        void Update();
    }
}