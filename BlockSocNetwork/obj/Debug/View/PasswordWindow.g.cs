﻿#pragma checksum "..\..\..\View\PasswordWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "DE0BF2C06060954190BF3AD2D563474C"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

using BlockSocNetwork;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace BlockSocNetwork {
    
    
    /// <summary>
    /// PasswordWindow
    /// </summary>
    public partial class PasswordWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 2 "..\..\..\View\PasswordWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal BlockSocNetwork.PasswordWindow PswdWindow;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\..\View\PasswordWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock Error;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\..\View\PasswordWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel FirstEnter;
        
        #line default
        #line hidden
        
        
        #line 46 "..\..\..\View\PasswordWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock Enter;
        
        #line default
        #line hidden
        
        
        #line 50 "..\..\..\View\PasswordWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.PasswordBox Password;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/BlockSocNetwork;component/view/passwordwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\View\PasswordWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.PswdWindow = ((BlockSocNetwork.PasswordWindow)(target));
            
            #line 10 "..\..\..\View\PasswordWindow.xaml"
            this.PswdWindow.Closing += new System.ComponentModel.CancelEventHandler(this.PasswordWindow_Closing);
            
            #line default
            #line hidden
            return;
            case 2:
            this.Error = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 3:
            this.FirstEnter = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 4:
            this.Enter = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 5:
            this.Password = ((System.Windows.Controls.PasswordBox)(target));
            return;
            case 6:
            
            #line 53 "..\..\..\View\PasswordWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

