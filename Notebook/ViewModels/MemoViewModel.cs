using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaterialDesignThemes.Wpf;
using Notebook.Common.Models;
using Notebook.Extensions;
using Notebook.Service;
using Notebook.Shared.Dtos;
using QueryParameter = Notebook.Shared.Parameters.QueryParameter;

namespace Notebook.ViewModels
{
    public class MemoViewModel : NavigationViewModel
    {

        private readonly IDialogHostService dialogHost;

        private readonly IMemoService service;

        public MemoViewModel(IMemoService service, IContainerProvider provider) : base(provider)
        {
            MemoDtos = new ObservableCollection<MemoDto>();
            ExecuteCommand = new DelegateCommand<string>(Execute);
            SelectedCommand = new DelegateCommand<MemoDto>(Selected);
            DeleteCommand = new DelegateCommand<MemoDto>(Delete);
            dialogHost = provider.Resolve<IDialogHostService>();
            this.service = service;
        }

        public DelegateCommand<string> ExecuteCommand { get; private set; }
        public DelegateCommand<MemoDto> SelectedCommand { get; private set; }
        public DelegateCommand<MemoDto> DeleteCommand { get; private set; }

        private ObservableCollection<MemoDto> memoDtos;
        public ObservableCollection<MemoDto> MemoDtos
        {
            get { return memoDtos; }
            set { memoDtos = value; RaisePropertyChanged(); }
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
        /// 编辑选中/新增时对象
        /// </summary>
        private MemoDto currentDto;
        public MemoDto CurrentDto
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
        /// 添加备忘录
        /// </summary>
        private void Add()
        {
            CurrentDto = new MemoDto();
            IsRightDrawerOpen = true;
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        async void GetDataAsync()
        {
            UpdateLoading(true);

            var memoResult = await service.GetAllAsync(new QueryParameter()
            {
                PageIndex = 0,
                PageSize = 100,
                Search = Search,
            });

            if (memoResult.Status)
            {
                MemoDtos.Clear();
                foreach (var item in memoResult.Result.Items)
                {
                    MemoDtos.Add(item);
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
                        var memo = MemoDtos.FirstOrDefault(t => t.Id == CurrentDto.Id);
                        if (memo != null)
                        {
                            memo.Title = CurrentDto.Title;
                            memo.Content = CurrentDto.Content;
                        }
                    }
                    IsRightDrawerOpen = false;
                }
                else
                {
                    var addResult = await service.AddAsync(CurrentDto);
                    if (addResult.Status)
                    {
                        MemoDtos.Add(addResult.Result);
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

        private async void Selected(MemoDto obj)
        {
            try
            {
                UpdateLoading(true);
                var memoResult = await service.GetFirstOfDefaultAsync(obj.Id);
                if (memoResult.Status)
                {
                    CurrentDto = memoResult.Result;
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

        private async void Delete(MemoDto obj)
        {
            try
            {
                var dialogResult = await dialogHost.Question("温馨提示", $"确认删除备忘录 {obj.Title} ?");
                if (dialogResult.Result != Prism.Dialogs.ButtonResult.OK) return;

                UpdateLoading(true);
                var deleteResult = await service.DeleteAsync(obj.Id);
                if (deleteResult.Status)
                {
                    var model = MemoDtos.FirstOrDefault(t => t.Id.Equals(obj.Id));
                    if (model != null)
                        MemoDtos.Remove(model);
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
            GetDataAsync();
        }
    }
}
