using System;
using System.Collections.Generic;
using NUnit.Framework;
using SpecsFor;
using SpecsFor.ShouldExtensions;

namespace PluggableWorkers.Tests
{
    public class SettingsFactorySpecs
    {
        public class when_building_settings_of_only_primitive_types : SpecsFor<SettingsFactory>
        {
            public class Settings
            {
                public string SomeString { get; set; }
                public int SomeInt { get; set; }
                public uint SomeUnsignedInt { get; set; }
                public decimal SomeDecimal { get; set; }
                public bool SomeBoolean { get; set; }
                public char SomeChar { get; set; }
                public double SomeDouble { get; set; }
                public float SomeFloat { get; set; }
                public ulong SomeUnsignedLong { get; set; }
                public long SomeLong { get; set; }
                public ushort SomeUnsignedShort { get; set; }
                public short SomeShort { get; set; }
                public byte SomeByte { get; set; }
                public sbyte SomeSignedByte { get; set; }
            }

            private Settings _settings;

            protected override void When()
            {
                _settings = SUT.GetSettingsFor(typeof (Settings),
                                               new Dictionary<string, string>
                                                   {
                                                       {"SomeString", "Some string value"},
                                                       {"SomeInt", "-2,147,483,648"},
                                                       {"SomeUnsignedInt", "4,294,967,295"},
                                                       {"SomeDecimal", "79,228,162,514,264,337,593,543,950,335"},
                                                       {"SomeBoolean", "true"},
                                                       {"SomeChar", "a"},
                                                       {"SomeDouble", "1.7976931348623e308"},
                                                       {"SomeFloat", "3.402823e38"},
                                                       {"SomeUnsignedLong", "18,446,744,073,709,551,615"},
                                                       {"SomeLong", "-922,337,203,685,477,508"},
                                                       {"SomeUnsignedShort", "65,535"},
                                                       {"SomeShort", "-32,768"},
                                                       {"SomeByte", "255"},
                                                       {"SomeSignedByte", "-128"}
                                                   }) as Settings;
            }

            [Test]
            public void then_it_build_the_correct_settings_object()
            {
                _settings.ShouldLookLike(new Settings
                                             {
                                                 SomeString = "Some string value",
                                                 SomeInt = -2147483648,
                                                 SomeUnsignedInt = 4294967295,
                                                 SomeDecimal = 79228162514264337593543950335m,
                                                 SomeBoolean = true,
                                                 SomeChar = 'a',
                                                 SomeDouble = 1.7976931348623E+308,
                                                 SomeFloat = 3.402823e38f,
                                                 SomeUnsignedLong = 18446744073709551615,
                                                 SomeLong = -922337203685477508,
                                                 SomeUnsignedShort = 65535,
                                                 SomeShort = -32768,
                                                 SomeByte = 255,
                                                 SomeSignedByte = -128
                                             });
            }
        }

        public class when_building_settings_with_dates : SpecsFor<SettingsFactory>
        {
            public class Settings
            {
                public DateTime NormalDate { get; set; }
                public DateTime YesterdaysDate { get; set; }
                public DateTime TodaysDate { get; set; }
                public DateTime TomorrowsDate { get; set; }
            }

            private Settings _settings;
            
            protected override void When()
            {
                _settings = SUT.GetSettingsFor(typeof (Settings), new Dictionary<string, string>
                                                                      {
                                                                          {"NormalDate", "4/1/2013"},
                                                                          {"YesterdaysDate", "YESTERDAY"},
                                                                          {"TodaysDate", "TODAY"},
                                                                          {"TomorrowsDate", "TOMORROW"}
                                                                      }) as Settings;
            }

            [Test]
            public void then_it_should_identify_the_correct_dates()
            {
                _settings.ShouldLookLike(new Settings
                                             {
                                                 NormalDate = new DateTime(2013, 4, 1),
                                                 YesterdaysDate = DateTime.Today.AddDays(-1),
                                                 TodaysDate = DateTime.Today,
                                                 TomorrowsDate = DateTime.Today.AddDays(1)
                                             });
            }
        }

        public class when_building_settings_with_arrays : SpecsFor<SettingsFactory>
        {
            public class Settings
            {
                public string[] StringArray { get; set; }
                public int[] IntArray { get; set; }
                public DateTime[] DateArray { get; set; }
            }

            private Settings _settings;

            protected override void When()
            {
                _settings = SUT.GetSettingsFor(typeof (Settings)
                                               , new Dictionary<string, string>
                                                     {
                                                         {"StringArray", "One Value;Two Value;Three Value"},
                                                         {"IntArray", "1;2;3"},
                                                         {"DateArray", "4/1/2013;TODAY;YESTERDAY;TOMORROW;12/21/2012"}
                                                     }) as Settings;
            }

            [Test]
            public void then_it_parsed_the_correct_values()
            {
                _settings.ShouldLookLike(new Settings
                                             {
                                                 StringArray = new[] {"One Value", "Two Value", "Three Value"},
                                                 IntArray = new[] {1, 2, 3},
                                                 DateArray = new[]
                                                                 {
                                                                     new DateTime(2013, 4, 1),
                                                                     DateTime.Today,
                                                                     DateTime.Today.AddDays(-1),
                                                                     DateTime.Today.AddDays(1),
                                                                     new DateTime(2012, 12, 21)
                                                                 }
                                             });
            }
        }

        public class when_building_settings_with_a_comma_for_a_character_data_type : SpecsFor<SettingsFactory>
        {
            public class Settings
            {
                public char Delimiter { get; set; }
            }

            private Settings _settings;

            protected override void When()
            {
                _settings = SUT.GetSettingsFor(typeof (Settings),
                                               new Dictionary<string, string>
                                                   {
                                                       {"Delimiter", ","}
                                                   }) as Settings;
            }

            [Test]
            public void then_it_build_the_correct_settings_object()
            {
                _settings.ShouldLookLike(new Settings
                                             {
                                                 Delimiter = ','
                                             });
            }

        }
    }
}
