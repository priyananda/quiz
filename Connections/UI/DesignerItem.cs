﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ConnQuiz.UI.Controls;
using ConnQuiz.Model;
using System.Windows.Shapes;

namespace ConnQuiz.UI
{
    //These attributes identify the types of the named parts that are used for templating
    [TemplatePart(Name = "PART_DragThumb", Type = typeof(DragThumb))]
    [TemplatePart(Name = "PART_ResizeDecorator", Type = typeof(Control))]
    [TemplatePart(Name = "PART_ConnectorDecorator", Type = typeof(Control))]
    [TemplatePart(Name = "PART_ContentPresenter", Type = typeof(ContentPresenter))]
    public class DesignerItem : ContentControl, ISelectable, IGroupable
    {
        #region ID
        private Guid id;
        private int questionId = -1;
        public Guid ID
        {
            get { return id; }
        }

        public int QuestionId
        {
            get { return questionId; }
            set
            {
                if (questionId == value)
                    return;
                questionId = value;
                SetText();
                Questions.Get(questionId).Answered += new Action<Question>(DesignerItem_Answered);
                Questions.OnPointsUpdate += new Action(SetText);
            }
        }

        #endregion

        #region ParentID
        public Guid ParentID
        {
            get { return (Guid)GetValue(ParentIDProperty); }
            set { SetValue(ParentIDProperty, value); }
        }
        public static readonly DependencyProperty ParentIDProperty = DependencyProperty.Register("ParentID", typeof(Guid), typeof(DesignerItem));
        #endregion

        #region IsGroup
        public bool IsGroup
        {
            get { return (bool)GetValue(IsGroupProperty); }
            set { SetValue(IsGroupProperty, value); }
        }
        public static readonly DependencyProperty IsGroupProperty =
            DependencyProperty.Register("IsGroup", typeof(bool), typeof(DesignerItem));
        #endregion

        #region IsSelected Property

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }
        public static readonly DependencyProperty IsSelectedProperty =
          DependencyProperty.Register("IsSelected",
                                       typeof(bool),
                                       typeof(DesignerItem),
                                       new FrameworkPropertyMetadata(false));

        #endregion

        #region DragThumbTemplate Property

        // can be used to replace the default template for the DragThumb
        public static readonly DependencyProperty DragThumbTemplateProperty =
            DependencyProperty.RegisterAttached("DragThumbTemplate", typeof(ControlTemplate), typeof(DesignerItem));

        public static ControlTemplate GetDragThumbTemplate(UIElement element)
        {
            return (ControlTemplate)element.GetValue(DragThumbTemplateProperty);
        }

        public static void SetDragThumbTemplate(UIElement element, ControlTemplate value)
        {
            element.SetValue(DragThumbTemplateProperty, value);
        }

        #endregion

        #region ConnectorDecoratorTemplate Property

        // can be used to replace the default template for the ConnectorDecorator
        public static readonly DependencyProperty ConnectorDecoratorTemplateProperty =
            DependencyProperty.RegisterAttached("ConnectorDecoratorTemplate", typeof(ControlTemplate), typeof(DesignerItem));

        public static ControlTemplate GetConnectorDecoratorTemplate(UIElement element)
        {
            return (ControlTemplate)element.GetValue(ConnectorDecoratorTemplateProperty);
        }

        public static void SetConnectorDecoratorTemplate(UIElement element, ControlTemplate value)
        {
            element.SetValue(ConnectorDecoratorTemplateProperty, value);
        }

        #endregion

        #region IsDragConnectionOver

        // while drag connection procedure is ongoing and the mouse moves over 
        // this item this value is true; if true the ConnectorDecorator is triggered
        // to be visible, see template
        public bool IsDragConnectionOver
        {
            get { return (bool)GetValue(IsDragConnectionOverProperty); }
            set { SetValue(IsDragConnectionOverProperty, value); }
        }
        public static readonly DependencyProperty IsDragConnectionOverProperty =
            DependencyProperty.Register("IsDragConnectionOver",
                                         typeof(bool),
                                         typeof(DesignerItem),
                                         new FrameworkPropertyMetadata(false));

        #endregion

        static DesignerItem()
        {
            // set the key to reference the style for this control
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(
                typeof(DesignerItem), new FrameworkPropertyMetadata(typeof(DesignerItem)));
        }

        public DesignerItem(Guid id)
        {
            this.id = id;
            this.Loaded += new RoutedEventHandler(DesignerItem_Loaded);
            this.MouseDoubleClick += new MouseButtonEventHandler(DesignerItem_MouseDoubleClick);
        }

        public DesignerItem()
            : this(Guid.NewGuid())
        {
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            DesignerCanvas designer = VisualTreeHelper.GetParent(this) as DesignerCanvas;

            // update selection
            if (designer != null)
            {
                if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
                    if (this.IsSelected)
                    {
                        designer.SelectionService.RemoveFromSelection(this);
                    }
                    else
                    {
                        designer.SelectionService.AddToSelection(this);
                    }
                else if (!this.IsSelected)
                {
                    designer.SelectionService.SelectItem(this);
                }
                Focus();
            }

            e.Handled = false;
        }

        void DesignerItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (base.Template != null)
            {
                ContentPresenter contentPresenter =
                    this.Template.FindName("PART_ContentPresenter", this) as ContentPresenter;
                if (contentPresenter != null)
                {
                    UIElement contentVisual = VisualTreeHelper.GetChild(contentPresenter, 0) as UIElement;
                    if (contentVisual != null)
                    {
                        DragThumb thumb = this.Template.FindName("PART_DragThumb", this) as DragThumb;
                        if (thumb != null)
                        {
                            ControlTemplate template =
                                DesignerItem.GetDragThumbTemplate(contentVisual) as ControlTemplate;
                            if (template != null)
                                thumb.Template = template;
                        }
                    }
                }
            }
            ChangeColor(Questions.ColorForQuestion(this.QuestionId));
        }
        void DesignerItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DesignerItem item = sender as DesignerItem;
            if (Questions.Get(item.QuestionId).Type != QuestionType.Concept)
                new QuestionWindow(item.QuestionId).ShowDialog();
        }
        void DesignerItem_Answered(Question obj)
        {
            if (obj.IsAnswered)
                ChangeColor(Colors.Green);
        }
        private void ChangeColor(Color color)
        {
            if (Questions.Get(this.QuestionId).Type == QuestionType.Concept && color != Colors.Green)
                return;

            Path path = this.Content as Path;
            Ellipse ellipse = null;
            if (path == null)
            {
                Grid grid = this.Content as Grid;
                if (grid != null)
                {
                    path = grid.Children[0] as Path;
                    ellipse = grid.Children[0] as Ellipse;
                }
            }
            if (path != null || ellipse != null)
            {
                LinearGradientBrush lgb = new LinearGradientBrush(
                    Colors.White, color, 90);
                if (path != null)
                    path.Fill = lgb;
                else
                    ellipse.Fill = lgb;
            }
        }
        private void SetText()
        {
            bool clear = false;
            string sId = this.ID.ToString();
            if (sId == "37d74ba0-0c17-404a-aacb-468bde0c5ea3" || sId == "b1454fbe-cb51-4f36-beeb-4a0a3e84c34d" || sId == "31f4fe65-ad0b-4c76-8b12-13824ae83a96")
            {
                clear = true;
            }

            Grid grid = this.Content as Grid;
            if (grid == null)
            {
                grid = new Grid();
                UIElement currentContent = this.Content as UIElement;
                this.Content = null;
                grid.Children.Add(currentContent);
                grid.Children.Add(new TextBlock());
                this.Content = grid;
            }

            TextBlock tb = grid.Children[1] as TextBlock;
            if (tb != null)
            {
                Question q = Questions.Get(questionId);
                tb.Text = clear ? "" : q.GetText();
                tb.FontSize = q.Type == QuestionType.Concept ? 10 : 16;
                tb.HorizontalAlignment = HorizontalAlignment.Center;
                tb.VerticalAlignment = VerticalAlignment.Center;
            }
        }
    }
}
