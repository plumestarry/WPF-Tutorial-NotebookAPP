using MaterialDesignThemes.Wpf;
using Notebook.Common;
using Notebook.Shared.Dtos;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notebook.ViewModels.Dialogs
{
    public class AddNotebookViewModel : BindableBase, IDialogHostAware
    {
        public AddNotebookViewModel()
        {
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);
        }

        private NotebookDto model;

        /// <summary>
        /// 新增或编辑的实体
        /// </summary>
        public NotebookDto Model
        {
            get { return model; }
            set { model = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 取消
        /// </summary>
        private void Cancel()
        {
            if (DialogHost.IsDialogOpen(DialogHostName))
                DialogHost.Close(DialogHostName, new DialogResult { Result = ButtonResult.No });
        }

        /// <summary>
        /// 确定
        /// </summary>
        private void Save()
        {
            if (string.IsNullOrWhiteSpace(Model.Title) || string.IsNullOrWhiteSpace(model.Content)) return;

            if (DialogHost.IsDialogOpen(DialogHostName))
            {
                //确定时,把编辑的实体返回并且返回OK
                DialogParameters param = new DialogParameters();
                param.Add("Value", Model);
                DialogHost.Close(DialogHostName, new DialogResult { Result = ButtonResult.OK, Parameters = param });
            }
        }

        public string DialogHostName { get; set; }
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }

        public void OnDialogOpend(IDialogParameters parameters)
        {
            if (parameters.ContainsKey("Value"))
            {
                Model = parameters.GetValue<NotebookDto>("Value");
            }
            else
                Model = new NotebookDto();
        }
    }
}
