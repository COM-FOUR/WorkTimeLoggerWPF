using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Shell;
using Xceed.Wpf.Toolkit;

namespace WorkTimeLogger
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            WTLViewModel model = new WTLViewModel(this,e.Args);

            if (model.CurrProcessInfo.SilentProcessing)
            {
                if (!model.PerformLogin())
                {
                    Current.Shutdown();
                    return;
                }

                model.PerformSilentProcessing();
                Current.Shutdown();
                return;
            }

            model.PerformLogin();

            CreateJumpList();

            MainWindow mw = new MainWindow();

            mw.DataContext = model;
            mw.Show();
        }
        public static void CreateJumpList()
        {
            var myResourceDictionary = new ResourceDictionary();
            myResourceDictionary.Source =
                new Uri("pack://application:,,,/Resources/StringResources.xaml",
                        UriKind.RelativeOrAbsolute);  
            
            JumpList jl = new JumpList();
            JumpList.SetJumpList(Application.Current, jl);

            for (int i = 0; i < 6; i++)
            {
                JumpTask jt = new JumpTask();
                jt.IconResourcePath = AppDomain.CurrentDomain.BaseDirectory+"\\Icons.dll";

                switch (i)
                {
                    case 0:
                        jt.Title = (string)myResourceDictionary["BtnlblStartDay"];
                        jt.Description = (string)myResourceDictionary["BtnlblStartDay"];
                        jt.Arguments = "STARTDAY";
                        jt.IconResourceIndex = 1;
                        break;
                    case 1:
                        jt.Title = (string)myResourceDictionary["BtnlblEndDay"];
                        jt.Description = (string)myResourceDictionary["BtnlblEndDay"];
                        jt.Arguments = "ENDDAY";
                        jt.IconResourceIndex = 1;
                        break;
                    case 2:
                        jt.Title = (string)myResourceDictionary["BtnlblStartBreak"];
                        jt.Description = (string)myResourceDictionary["BtnlblStartBreak"];
                        jt.Arguments = "STARTBREAK";
                        jt.IconResourceIndex = 0;
                        break;
                    case 3:
                        jt.Title = (string)myResourceDictionary["BtnlblEndBreak"];
                        jt.Description = (string)myResourceDictionary["BtnlblEndBreak"];
                        jt.Arguments = "ENDBREAK";
                        jt.IconResourceIndex = 0;
                        break;
                    case 4:
                        jt.Title = (string)myResourceDictionary["BtnlblStartSmokingBreak"];
                        jt.Description = (string)myResourceDictionary["BtnlblStartSmokingBreak"];
                        jt.Arguments = "STARTSBREAK";
                        jt.IconResourceIndex = 2;
                        break;
                    case 5:
                        jt.Title = (string)myResourceDictionary["BtnlblEndSmokingBreak"];
                        jt.Description = (string)myResourceDictionary["BtnlblEndSmokingBreak"];
                        jt.Arguments = "ENDSBREAK";
                        jt.IconResourceIndex = 2;
                        break;
                }
                jl.JumpItems.Add(jt);   
            }
            jl.Apply();
        }
    }

    
    

}
