using Notebook.Extensions;
using Notebook.Service;
using Notebook.Shared.Dtos;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notebook.ViewModels.Dialogs
{
    public class LoginViewModel : BindableBase, IDialogAware
    {

        private readonly ILoginService loginService;
        private readonly IEventAggregator aggregator;

        // 添加一个标志位来跟踪登录是否成功
        private bool _isLoggedIn = false;

        public LoginViewModel(ILoginService loginService, IEventAggregator aggregator)
        {
            UserDto = new RegisterUserDto();
            ExecuteCommand = new DelegateCommand<string>(Execute);
            this.loginService = loginService;
            this.aggregator = aggregator;
        }

        public string Title { get; set; } = "Notebook";

        //DialogCloseListener IDialogAware.RequestClose { get; }

        //public event Action<IDialogResult> RequestClose;

        public DialogCloseListener RequestClose { get; } // 实现为属性

        public event Action<IDialogResult> RequestCloseEvent; // 保留您现有的事件，用于在 ViewModel 内部触发关闭请求


        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            // 只有在 _isLoggedIn 为 false（即登录不成功或用户主动登出）时才调用 LoginOutInternal()
            //if (!_isLoggedIn)
            //{
            //    LoginOut();
            //}
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            //_isLoggedIn = false; // 在对话框打开时重置登录状态
        }

        #region **************************************************Login**************************************************

        private string userName;

        public string UserName
        {
            get { return userName; }
            set { userName = value; RaisePropertyChanged(); }
        }

        private string passWord;

        public string PassWord
        {
            get { return passWord; }
            set { passWord = value; RaisePropertyChanged(); }
        }

        private int selectIndex;

        public int SelectIndex
        {
            get { return selectIndex; }
            set { selectIndex = value; RaisePropertyChanged(); }
        }

        private RegisterUserDto userDto;

        public RegisterUserDto UserDto
        {
            get { return userDto; }
            set { userDto = value; RaisePropertyChanged(); }
        }

        public DelegateCommand<string> ExecuteCommand { get; private set; }


        private void Execute(string obj)
        {
            switch (obj)
            {
                case "Login": Login(); break;
                case "LoginOut": LoginOut(); break;
                case "Register": Register(); break;
                case "RegisterPage": SelectIndex = 1; break;
                case "Return": SelectIndex = 0; break;
            }
        }

        async void Login()
        {
            if (string.IsNullOrWhiteSpace(UserName) ||
                string.IsNullOrWhiteSpace(PassWord))
            {
                return;
            }

            var loginResult = await loginService.Login(new Shared.Dtos.UserDto()
            {
                Account = UserName,
                PassWord = PassWord,
                UserName = "",
            });

            if (loginResult != null && loginResult.Status)
            {
                AppSession.UserName = loginResult.Result.UserName;
                // Corrected the invocation of `RequestClose` to directly call the `Invoke` method
                //RequestCloseEvent?.Invoke(new DialogResult(ButtonResult.OK));
                RequestClose.Invoke(ButtonResult.OK);
            }
            else
            {
                //登录失败提示...
                aggregator.SendMessage(loginResult.Message ?? string.Empty, "Login");
            }
        }

        private async void Register()
        {
            if (string.IsNullOrWhiteSpace(UserDto.Account) ||
                string.IsNullOrWhiteSpace(UserDto.UserName) ||
                string.IsNullOrWhiteSpace(UserDto.PassWord) ||
                string.IsNullOrWhiteSpace(UserDto.NewPassWord))
            {
                aggregator.SendMessage("请输入完整的注册信息！", "Login");
                return;
            }

            if (UserDto.PassWord != UserDto.NewPassWord)
            {
                aggregator.SendMessage("密码不一致,请重新输入！", "Login");
                return;
            }

            var registerResult = await loginService.Register(new Shared.Dtos.UserDto()
            {
                Account = UserDto.Account,
                UserName = UserDto.UserName,
                PassWord = UserDto.PassWord
            });

            if (registerResult != null && registerResult.Status)
            {
                aggregator.SendMessage("注册成功", "Login");
                //注册成功,返回登录页页面
                SelectIndex = 0;
            }
            else
                aggregator.SendMessage(registerResult.Message ?? string.Empty, "Login");
                //aggregator.SendMessage("registerResult.Message", "Login");
        }

        void LoginOut()
        {
            // Corrected the invocation of `RequestClose` to directly call the `Invoke` method
            RequestClose.Invoke(ButtonResult.No);
        }

        #endregion **************************************************Login**************************************************
    }
}
