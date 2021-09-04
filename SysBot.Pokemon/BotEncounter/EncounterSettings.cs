using System.ComponentModel;

namespace SysBot.Pokemon
{
    public class EncounterSettings
    {
        private const string Encounter = nameof(Encounter);
        public override string ToString() => "Encounter Bot Settings";

        [Category(Encounter), Description("The method by which the bot will encounter Pokémon.")]
        public EncounterMode EncounteringType { get; set; } = EncounterMode.VerticalLine;
        [Category(Encounter), Description("Personal duration values for CurryBot."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public CookingCategory Curry { get; set; } = new();

        [Category(Encounter)]
        [TypeConverter(typeof(CookingCategoryConverter))]
        public class CookingCategory
        {
            public override string ToString() => "CurryBot Duration Values";

            [Category(Encounter), Description("Time to wait before pressing \"A\" to enter camp. Default is 10000 Ms.")]
            public int EnterCamp { get; set; } = 10000;

            [Category(Encounter), Description("Time to mash \"A\" to fan curry. Default is 14000 Ms.")]
            public int FanningCurry { get; set; } = 14000;

            [Category(Encounter), Description("Time to stir the pot for. Default is 19000 Ms.")]
            public int StirringThePot { get; set; } = 19000;

            [Category(Encounter), Description("Time to wait before pressing \"A\" for a sprinkle of love, you want to have it land on the inner green ring. Default is 8000 Ms.")]
            public int SprinkleOfLove { get; set; } = 8000;

            [Category(Encounter), Description("Time to wait before pressing \"A\" on the screen when it shows what curry we have made before the eating cutscene. Default is 20000 Ms.")]
            public int FinalCutScene { get; set; } = 20000;
        }

        private sealed class CookingCategoryConverter : TypeConverter
        {
            public override bool GetPropertiesSupported(ITypeDescriptorContext context) => true;

            public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, System.Attribute[] attributes) => TypeDescriptor.GetProperties(typeof(CookingCategory));

            public override bool CanConvertTo(ITypeDescriptorContext context, System.Type destinationType) => destinationType != typeof(string) && base.CanConvertTo(context, destinationType);
        }
    }
}