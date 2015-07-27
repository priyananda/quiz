﻿#pragma checksum "..\..\..\Gui\WpfModelViewerControl.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "ED99BCF2151310CC97B1ABBD2C806529"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using RK.Common.GraphicsEngine.Objects.Wpf;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
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


namespace RK.Common.GraphicsEngine.Gui {
    
    
    /// <summary>
    /// WpfModelViewerControl
    /// </summary>
    public partial class WpfModelViewerControl : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 11 "..\..\..\Gui\WpfModelViewerControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal RK.Common.GraphicsEngine.Gui.WpfModelViewerControl ControlMain;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\..\Gui\WpfModelViewerControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid m_viewportGrid;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\..\Gui\WpfModelViewerControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Viewport3D m_viewport3D;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\..\Gui\WpfModelViewerControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Media.Media3D.DirectionalLight m_mainLight;
        
        #line default
        #line hidden
        
        
        #line 46 "..\..\..\Gui\WpfModelViewerControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal RK.Common.GraphicsEngine.Objects.Wpf.WpfGrid3DModel m_grid3D;
        
        #line default
        #line hidden
        
        
        #line 56 "..\..\..\Gui\WpfModelViewerControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Media.Media3D.ContainerUIElement3D m_mainModelContainer;
        
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
            System.Uri resourceLocater = new System.Uri("/RK.Common.GraphicsEngine;component/gui/wpfmodelviewercontrol.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Gui\WpfModelViewerControl.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
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
            this.ControlMain = ((RK.Common.GraphicsEngine.Gui.WpfModelViewerControl)(target));
            
            #line 7 "..\..\..\Gui\WpfModelViewerControl.xaml"
            this.ControlMain.MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.OnMouseWheel);
            
            #line default
            #line hidden
            return;
            case 2:
            this.m_viewportGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 3:
            this.m_viewport3D = ((System.Windows.Controls.Viewport3D)(target));
            return;
            case 4:
            this.m_mainLight = ((System.Windows.Media.Media3D.DirectionalLight)(target));
            return;
            case 5:
            this.m_grid3D = ((RK.Common.GraphicsEngine.Objects.Wpf.WpfGrid3DModel)(target));
            return;
            case 6:
            this.m_mainModelContainer = ((System.Windows.Media.Media3D.ContainerUIElement3D)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
