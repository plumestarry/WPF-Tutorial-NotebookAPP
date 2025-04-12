using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Notebook.Common.Models;
using Notebook.Extensions;
using Prism.Dialogs;
using Prism.Navigation.Regions;

namespace Notebook.ViewModels
{
    public class IndexViewModel : NavigationViewModel
    {

        private readonly INotebookService notebookService;
        private readonly IMemoService memoService;

        private readonly IDialogHostService dialog;
        private readonly IRegionManager regionManager;

        public IndexViewModel(IContainerProvider provider, IDialogHostService dialog) : base(provider)
        {
            Title = $"你好，{AppSession.UserName} {DateTime.Now.GetDateTimeFormats('D')[1].ToString()}";
            CreateTaskBars();
            ExecuteCommand = new DelegateCommand<string>(Execute);
            this.notebookService = provider.Resolve<INotebookService>();
            this.memoService = provider.Resolve<IMemoService>();
            this.regionManager = provider.Resolve<IRegionManager>();
            this.dialog = dialog;
            EditMemoCommand = new DelegateCommand<MemoDto>(AddMemo);
            EditNotebookCommand = new DelegateCommand<NotebookDto>(AddNotebook);
            NotebookCompltedCommand = new DelegateCommand<NotebookDto>(Complted);
            NavigateCommand = new DelegateCommand<TaskBar>(Navigate);
        }

        private void Navigate(TaskBar obj)
        {
            if (string.IsNullOrWhiteSpace(obj.Target)) return;

            NavigationParameters param = new NavigationParameters();

            if (obj.Title == "已完成")
            {
                param.Add("Value", 2);
            }
            regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate(obj.Target, param);
        }


        public DelegateCommand<NotebookDto> NotebookCompltedCommand { get; private set; }
        public DelegateCommand<NotebookDto> EditNotebookCommand { get; private set; }
        public DelegateCommand<MemoDto> EditMemoCommand { get; private set; }
        public DelegateCommand<TaskBar> NavigateCommand { get; private set; }

        public DelegateCommand<string> ExecuteCommand { get; private set; }

        private ObservableCollection<TaskBar> taskBars;

        public ObservableCollection<TaskBar> TaskBars
        {
            get { return taskBars; }
            set { taskBars = value; RaisePropertyChanged(); }
        }

        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; RaisePropertyChanged(); }
        }

        private SummaryDto summary;

        /// <summary>
        /// 首页统计
        /// </summary>
        public SummaryDto Summary
        {
            get { return summary; }
            set { summary = value; RaisePropertyChanged(); }
        }

        void CreateTaskBars()
        {
            TaskBars = new ObservableCollection<TaskBar>();
            TaskBars.Add(new TaskBar() { Icon = "ClockFast", Title = "汇总", Color = "#FF0CA0FF", Target = "NotebookView" });
            TaskBars.Add(new TaskBar() { Icon = "ClockCheckOutline", Title = "已完成", Color = "#FF1ECA3A", Target = "NotebookView" });
            TaskBars.Add(new TaskBar() { Icon = "ChartLineVariant", Title = "完成比例", Color = "#FF02C6DC", Target = "" });
            TaskBars.Add(new TaskBar() { Icon = "PlaylistStar", Title = "备忘录", Color = "#FFFFA000", Target = "MemoView" });
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            try
            {
                UpdateLoading(true);
                var summaryResult = await notebookService.SummaryAsync();
                if (summaryResult.Status)
                {
                    Summary = summaryResult.Result;
                    Refresh();
                }
                base.OnNavigatedTo(navigationContext);
            }
            finally
            {
                UpdateLoading(false);
            }
        }

        void Refresh()
        {
            TaskBars[0].Content = summary.Sum.ToString();
            TaskBars[1].Content = summary.CompletedCount.ToString();
            TaskBars[2].Content = summary.CompletedRatio;
            TaskBars[3].Content = summary.MemoCount.ToString();
        }

        private void Execute(string obj)
        {
            switch (obj)
            {
                case "新增待办": AddNotebook(null); break;
                case "新增备忘录": AddMemo(null); break;
            }
        }

        private async void Complted(NotebookDto obj)
        {
            try
            {
                UpdateLoading(true);
                var updateResult = await notebookService.UpdateAsync(obj);
                if (updateResult.Status)
                {
                    var notebook = summary.NotebookList.FirstOrDefault(t => t.Id.Equals(obj.Id));
                    if (notebook != null)
                    {
                        summary.NotebookList.Remove(notebook);
                        summary.CompletedCount += 1;
                        summary.CompletedRatio = (summary.CompletedCount / (double)summary.Sum).ToString("0%");
                        this.Refresh();
                    }
                    aggregator.SendMessage("已完成!", "Main");
                }
            }
            finally
            {
                UpdateLoading(false);
            }
        }

        /// <summary>
        /// 添加待办事项
        /// </summary>
        async void AddNotebook(NotebookDto model)
        {
            DialogParameters param = new DialogParameters();
            if (model != null)
                param.Add("Value", model);

            var dialogResult = await dialog.ShowDialog("AddNotebookView", param);
            if (dialogResult.Result == ButtonResult.OK)
            {
                try
                {
                    UpdateLoading(true);
                    var notebook = dialogResult.Parameters.GetValue<NotebookDto>("Value");
                    if (notebook.Id > 0)
                    {
                        var updateResult = await notebookService.UpdateAsync(notebook);
                        if (updateResult.Status)
                        {
                            var notebookModel = summary.NotebookList.FirstOrDefault(t => t.Id.Equals(notebook.Id));
                            if (notebookModel != null)
                            {
                                notebookModel.Title = notebook.Title;
                                notebookModel.Content = notebook.Content;
                            }
                        }
                    }
                    else
                    {
                        var addResult = await notebookService.AddAsync(notebook);
                        if (addResult.Status)
                        {
                            summary.Sum += 1;
                            summary.NotebookList.Add(addResult.Result);
                            summary.CompletedRatio = (summary.CompletedCount / (double)summary.Sum).ToString("0%");
                            this.Refresh();
                        }
                    }
                }
                finally
                {
                    UpdateLoading(false);
                }
            }
        }

        /// <summary>
        /// 添加备忘录
        /// </summary>
        async void AddMemo(MemoDto model)
        {
            DialogParameters param = new DialogParameters();
            if (model != null)
                param.Add("Value", model);

            var dialogResult = await dialog.ShowDialog("AddMemoView", param);
            if (dialogResult.Result == ButtonResult.OK)
            {
                try
                {
                    UpdateLoading(true);
                    var memo = dialogResult.Parameters.GetValue<MemoDto>("Value");

                    if (memo.Id > 0)
                    {
                        var updateResult = await memoService.UpdateAsync(memo);
                        if (updateResult.Status)
                        {
                            var memoModel = summary.MemoList.FirstOrDefault(t => t.Id.Equals(memo.Id));
                            if (memoModel != null)
                            {
                                memoModel.Title = memo.Title;
                                memoModel.Content = memo.Content;
                            }
                        }
                    }
                    else
                    {
                        var addResult = await memoService.AddAsync(memo);
                        if (addResult.Status)
                        {
                            summary.MemoList.Add(addResult.Result);
                        }
                    }
                }
                finally
                {
                    UpdateLoading(false);
                }
            }
        }

    }
}
