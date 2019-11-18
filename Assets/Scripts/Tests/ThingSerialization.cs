using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Village;
using Village.Things;
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

        [Test]
        public void ShouldSetTileRuleConfig()
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
                            <TileRuleConfig>
                                <Sprites>
                                    <string>colored_5</string>
                                    <string>colored_6</string>
                                    <string>colored_7</string>
                                </Sprites>
                                <Type>RandomTiles</Type>
                            </TileRuleConfig>
                        </Thing>
                    </Things>
                </ThingSerializationLayout>
            ";

            var things = s.LoadFromString(xml);
            
            Assert.IsInstanceOf<ThingConfig[]>(things);
            Assert.AreEqual(1, things.Length);
            Assert.IsNotNull(things[0].TileRuleConfig);
            Assert.AreEqual(3, things[0].TileRuleConfig.Sprites.Length);
            Assert.AreEqual("RandomTiles", things[0].TileRuleConfig.Type);
        }

         [Test]
        public void ShouldSetBuildConfig()
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
                            <ConstructionConfig>
                                <Group>Floors</Group>
                                <Requires>None</Requires>
                            </ConstructionConfig>
                        </Thing>
                    </Things>
                </ThingSerializationLayout>
            ";

            var things = s.LoadFromString(xml);
            
            Assert.IsInstanceOf<ThingConfig[]>(things);
            Assert.AreEqual(1, things.Length);
            Assert.IsNotNull(things[0].ConstructionConfig);
            Assert.AreEqual(ConstructionGroup.Floors, things[0].ConstructionConfig.Group);
            Assert.AreEqual(TypeOfThing.None, things[0].ConstructionConfig.Requires);
        }

        [Test]
        public void ShouldSetFactoryConfig()
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
                            <FactoryConfig>
                                <Produces>
                                    <TypeOfThing>Iron</TypeOfThing>
                                </Produces>
                            </FactoryConfig>
                        </Thing>
                    </Things>
                </ThingSerializationLayout>
            ";

            var things = s.LoadFromString(xml);
            
            Assert.IsInstanceOf<ThingConfig[]>(things);
            Assert.AreEqual(1, things.Length);
            Assert.IsNotNull(things[0].FactoryConfig);
            Assert.AreEqual(1, things[0].FactoryConfig.Produces.Length);
        }
    }
}
