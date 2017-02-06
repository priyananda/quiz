using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shenoy.Quiz.Model
{
    class DoublesDuration : ReusableMetaModifier
    {
        public override void Apply()
        {
            if (!VProps.ShowSecondSet)
                VProps.ShowSecondSet = true;
            base.Apply();
        }
    }

    class DoATeja : SingleShotMetaModifier
    {
        public override void Apply()
        {
            VProps.ShowMark = true;

            base.Apply();
        }
    }

    class DoAManojKumar : SingleShotMetaModifier
    {
        public override void Apply()
        {
            VProps.ShowHand = true;

            base.Apply();
        }
    }

    class DoAnUtpalDutt : SingleShotMetaModifier
    {
        public override void Apply()
        {
            if (VProps.ShowHand)
                VProps.ShowHand = false;
            VProps.ShowMoustache = true;

            base.Apply();
        }
    }
}
