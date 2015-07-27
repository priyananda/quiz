using System.Windows;

namespace RK.Common.GraphicsEngine.Objects.Wpf
{
    public class WpfGenericModel : WpfConstructed3DModel
    {
        public static readonly DependencyProperty ObjectTypeProperty =
            DependencyProperty.Register("ObjectType", typeof(ObjectType), typeof(WpfGenericModel), new PropertyMetadata(null));

        public override VertexStructure[] BuildStructures()
        {
            if (this.ObjectType != null) { return ObjectType.BuildStructure(); }
            else { return null; }
        }

        public ObjectType ObjectType
        {
            get { return (ObjectType)GetValue(ObjectTypeProperty); }
            set { SetValue(ObjectTypeProperty, value); }
        }
    }
}