using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Village;
using Village.Things.Config;
using Village.Things.Serialization;

namespace Tests
{
    public class ThingSerializationTests
    {
        public string filePath = $"{Application.dataPath}/Resources/Config/Things.xml";

        // A Test behaves as an ordinary method
        [Test]
        public void ShouldGetThignsFromXmlFile()
        {
            var s = new ThingSerialization();
            var things = s.LoadFromFile(filePath);
            
            Assert.IsInstanceOf<ThingConfig[]>(things);
            Assert.Greater(things.Length, 0);
        }

        [Test]
        public void ShouldGetThignsFromString()
        {
            var s = new ThingSerialization();
            var xml = @"
                <?xml version='1.0' encoding='utf-8'?>
                <ThingSerializationLayout xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'  xmlns:xsd='http://www.w3.org/2001/XMLSchema'>
                    <Parents></Parents>
                    <Things>
                        <Thing>
                            <Name>grass</Name>
                            <Sprite>colored_5</Sprite>
                            <TypeOfThing>Grass</TypeOfThing>
                            <BuildSite>true</BuildSite>
                        </Thing>
                    </Things>
                </ThingSerializationLayout>
            ";

            var things = s.LoadFromString(xml);
            
            Assert.IsInstanceOf<ThingConfig[]>(things);
            Assert.Greater(things.Length, 0);
        }

        [Test]
        public void ShouldSetParentPropertiesOnChild()
        {
            var s = new ThingSerialization();
            var xml = @"
                <?xml version='1.0' encoding='utf-8'?>
                <ThingSerializationLayout xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'  xmlns:xsd='http://www.w3.org/2001/XMLSchema'>
                    <Parents>
                        <Thing>
                            <Name>Floor</Name>
                            <Floor>true</Floor>
                            <Hitpoints>12345</Hitpoints>
                        </Thing>
                    </Parents>
                    <Things>
                        <Thing>
                            <Name>grass</Name>
                            <Sprite>colored_5</Sprite>
                            <TypeOfThing>Grass</TypeOfThing>
                            <BuildSite>true</BuildSite>
                            <Parent>Floor</Parent>
                        </Thing>
                    </Things>
                </ThingSerializationLayout>
            ";

            var things = s.LoadFromString(xml);
            
            Assert.IsInstanceOf<ThingConfig[]>(things);
            Assert.AreEqual(1, things.Length);
            Assert.AreEqual(12345, things[0].Hitpoints);
        }

        [Test]
        public void ChildShouldOverrideParent()
        {
            var s = new ThingSerialization();
            var xml = @"
                <?xml version='1.0' encoding='utf-8'?>
                <ThingSerializationLayout xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'  xmlns:xsd='http://www.w3.org/2001/XMLSchema'>
                    <Parents>
                        <Thing>
                            <Name>Floor</Name>
                            <Floor>true</Floor>
                            <Hitpoints>12345</Hitpoints>
                        </Thing>
                    </Parents>
                    <Things>
                        <Thing>
                            <Name>grass</Name>
                            <Sprite>colored_5</Sprite>
                            <TypeOfThing>Grass</TypeOfThing>
                            <BuildSite>true</BuildSite>
                            <Parent>Floor</Parent>
                            <Hitpoints>6789</Hitpoints>
                        </Thing>
                    </Things>
                </ThingSerializationLayout>
            ";

            var things = s.LoadFromString(xml);
            
            Assert.IsInstanceOf<ThingConfig[]>(things);
            Assert.AreEqual(1, things.Length);
            Assert.AreEqual(6789, things[0].Hitpoints);
        }
    }
}
