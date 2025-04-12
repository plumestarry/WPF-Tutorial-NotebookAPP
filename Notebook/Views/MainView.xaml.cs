﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Notebook.Extensions;

namespace Notebook.Views
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : Window
    {
        public MainView(IEventAggregator aggregator, IDialogHostService dialogHostService)
        {
            InitializeComponent();

            //注册提示消息
            aggregator.RegisterMessage(arg =>
            {
                Snackbar.MessageQueue.Enqueue(arg.Message);
            });

            //注册等待消息窗口
            aggregator.Register(arg =>
            {
                DialogHost.IsOpen = arg.IsOpen;

                if (DialogHost.IsOpen)
                    DialogHost.DialogContent = new ProgressView();
            });

            btnMin.Click += (s, e) => { this.WindowState = WindowState.Minimized; };
            btnMax.Click += (s, e) =>
            {
                this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            };
            btnClose.Click += async (s, e) =>
            {
                var dialogResult = await dialogHostService.Question("温馨提示", "确认退出系统?");
                if (dialogResult.Result != Prism.Dialogs.ButtonResult.OK) return;
                this.Close();
            };

            ColorZone.MouseMove += (s, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    this.DragMove();
                }
            };
            ColorZone.MouseDoubleClick += (s, e) =>
            {
                this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            };

            menuBar.SelectionChanged += (s, e) =>
            {
                drawerHost.IsLeftDrawerOpen = false;
            };
        }
    }
}
