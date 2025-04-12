using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using MaterialDesignThemes.Wpf;
using Notebook.Extensions;
using Notebook.Service;
using Notebook.Shared.Contact;

namespace Notebook.ViewModels
{
    public class NotebookViewModel : NavigationViewModel
    {

        private readonly IDialogHostService dialogHost;

        private readonly INotebookService service;

        public NotebookViewModel(INotebookService service, IContainerProvider provider) : base(provider)
        {
            NotebookDtos = new ObservableCollection<NotebookDto>();
            ExecuteCommand = new DelegateCommand<string>(Execute);
            SelectedCommand = new DelegateCommand<NotebookDto>(Selected);
            DeleteCommand = new DelegateCommand<NotebookDto>(Delete);
            dialogHost = provider.Resolve<IDialogHostService>();
            this.service = service;
        }

        public DelegateCommand<string> ExecuteCommand { get; private set; }
        public DelegateCommand<NotebookDto> SelectedCommand { get; private set; }
        public DelegateCommand<NotebookDto> DeleteCommand { get; private set; }

        private ObservableCollection<NotebookDto> notebookDtos;
        public ObservableCollection<NotebookDto> NotebookDtos
        {
            get { return notebookDtos; }
            set { notebookDtos = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 右侧编辑窗口是否展开
        /// </summary>
        private bool isRightDrawerOpen;
        public bool IsRightDrawerOpen
        {
            get { return isRightDrawerOpen; }
            set { isRightDrawerOpen = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 下拉列表选中状态值
        /// </summary>
        private int selectedIndex;
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { selectedIndex = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 编辑选中/新增时对象
        /// </summary>
        private NotebookDto currentDto;
        public NotebookDto CurrentDto
        {
            get { return currentDto; }
            set { currentDto = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 搜索条件
        /// </summary>
        private string search;
        public string Search
        {
            get { return search; }
            set { search = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 添加待办
        /// </summary>
        private void Add()
        {
            CurrentDto = new NotebookDto();
            IsRightDrawerOpen = true;
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        async void GetDataAsync()
        {
            UpdateLoading(true);

            int? Status = SelectedIndex == 0 ? null : SelectedIndex == 2 ? 1 : 0;

            var notebookResult = await service.GetAllFilterAsync(new NotebookParameter()
            {
                PageIndex = 0,
                PageSize = 100,
                Search = Search,
                Status = Status,
            });

            if (notebookResult.Status)
            {
                NotebookDtos.Clear();
                foreach (var item in notebookResult.Result.Items)
                {
                    NotebookDtos.Add(item);
                }
            }

            UpdateLoading(false);
        }

        private async void Save()
        {
            if (string.IsNullOrWhiteSpace(CurrentDto.Title) ||
                string.IsNullOrWhiteSpace(CurrentDto.Content))
                return;

            UpdateLoading(true);

            try
            {
                if (CurrentDto.Id > 0)
                {
                    var updateResult = await service.UpdateAsync(CurrentDto);
                    if (updateResult.Status)
                    {
                        var notebook = NotebookDtos.FirstOrDefault(t => t.Id == CurrentDto.Id);
                        if (notebook != null)
                        {
                            notebook.Title = CurrentDto.Title;
                            notebook.Content = CurrentDto.Content;
                            notebook.Status = CurrentDto.Status;
                        }
                    }
                    IsRightDrawerOpen = false;
                }
                else
                {
                    var addResult = await service.AddAsync(CurrentDto);
                    if (addResult.Status)
                    {
                        NotebookDtos.Add(addResult.Result);
                        IsRightDrawerOpen = false;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                UpdateLoading(false);
            }
        }

        private void Execute(string obj)
        {
            switch (obj)
            {
                case "新增": Add(); break;
                case "查询": GetDataAsync(); break;
                case "保存": Save(); break;
            }
        }

        private async void Selected(NotebookDto obj)
        {
            try
            {
                UpdateLoading(true);
                var notebookResult = await service.GetFirstOfDefaultAsync(obj.Id);
                if (notebookResult.Status)
                {
                    CurrentDto = notebookResult.Result;
                    IsRightDrawerOpen = true;
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                UpdateLoading(false);
            }
        }

        private async void Delete(NotebookDto obj)
        {
            try
            {
                var dialogResult = await dialogHost.Question("温馨提示", $"确认删除待办事项: {obj.Title} ?");
                if (dialogResult.Result != Prism.Dialogs.ButtonResult.OK) return;

                UpdateLoading(true);
                var deleteResult = await service.DeleteAsync(obj.Id);
                if (deleteResult.Status)
                {
                    var model = NotebookDtos.FirstOrDefault(t => t.Id.Equals(obj.Id));
                    if (model != null)
                        NotebookDtos.Remove(model);
                }
            }
            finally
            {
                UpdateLoading(false);
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            if (navigationContext.Parameters.ContainsKey("Value"))
                SelectedIndex = navigationContext.Parameters.GetValue<int>("Value");
            else
                SelectedIndex = 0;
            GetDataAsync();
        }
    }
}
