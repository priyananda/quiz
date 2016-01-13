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
            if (VProps.ShowSecondSet)
                VProps.ShowThirdSet = true;
            else if (VProps.ShowFirstSet)
                VProps.ShowSecondSet = true;

            base.Apply();
        }
    }

    class EnableAllInset : ReusableMetaModifier
    {
        public override void Apply()
        {
            VProps.ShowFirstSet = true;
            base.Apply();
        }
    }

    class HalvesDuration : SingleShotMetaModifier
    {
        public override void Apply()
        {
            if (VProps.ShowThirdSet)
                VProps.ShowThirdSet = false;

            base.Apply();
        }
    }

    class RedYellowGreen : SingleShotMetaModifier
    {
        public override void Apply()
        {
            VProps.ShowTrafficLights = true;

            base.Apply();
        }
    }

    class DoAKejriwal : TimeLimitedMetaModifier
    {
        public DoAKejriwal() : base(6) { }

        public override void Apply()
        {
            base.Apply();
            if (this.State == MetaModifierState.Finished)
                VProps.EvenOddState = KejriwalState.ShowAll;
            else if (m_counter % 2 == 0)
                VProps.EvenOddState = KejriwalState.ShowOnlyEven;
            else
                VProps.EvenOddState = KejriwalState.ShowOnlyOdd;
        }
    }

    class DoARahulGandhi : TimeLimitedMetaModifier
    {
        public DoARahulGandhi() : base(6) { }

        public override void Apply()
        {
            base.Apply();
            if (Quiz.Current.Questions.CountOpenWomen() == 0)
                m_state = MetaModifierState.Finished;
            if (this.State == MetaModifierState.Finished && VProps.ShowOnlyWomen)
                VProps.ShowOnlyWomen = false;
            else
                VProps.ShowOnlyWomen = true;
        }
    }

    class DoAJayalalitha : SingleShotMetaModifier
    {
        public override void Apply()
        {
            VProps.ShowJayalalitha = true;

            base.Apply();
        }
    }

    class DoASalKhan : SingleShotMetaModifier
    {
        public override void Apply()
        {
            VProps.ShowMathMode = true;

            base.Apply();
        }
    }

    class DoADonald : SingleShotMetaModifier
    {
        public override void Apply()
        {
            VProps.ShowWhitifiedProfile = true;

            base.Apply();
        }
    }
}
