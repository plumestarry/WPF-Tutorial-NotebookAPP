﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notebook.Shared.Dtos
{
    /// <summary>
    /// 汇总
    /// </summary>
    public class SummaryDto : BaseDto
    {
        private int sum;
        private int completedCount;
        private int memoCount;
        private string completedRatio;


        /// <summary>
        /// 待办事项总数
        /// </summary>
        public int Sum
        {
            get { return sum; }
            set { sum = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 完成待办事项数量
        /// </summary>
        public int CompletedCount
        {
            get { return completedCount; }
            set { completedCount = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 备忘录数量
        /// </summary>
        public int MemoCount
        {
            get { return memoCount; }
            set { memoCount = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 完成比例
        /// </summary>
        public string CompletedRatio
        {
            get { return completedRatio; }
            set { completedRatio = value; OnPropertyChanged(); }
        }

        private ObservableCollection<NotebookDto> notebookList;
        private ObservableCollection<MemoDto> memoList;

        /// <summary>
        /// 待办事项列表
        /// </summary>
        public ObservableCollection<NotebookDto> NotebookList
        {
            get { return notebookList; }
            set { notebookList = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 备忘录列表
        /// </summary>
        public ObservableCollection<MemoDto> MemoList
        {
            get { return memoList; }
            set { memoList = value; OnPropertyChanged(); }
        }
    }
}
