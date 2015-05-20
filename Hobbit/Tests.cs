using System.Linq;
using HelperLibrary;
using Hobbit.Classes;
using Hobbit.Classes.GameObjects;
using Hobbit.Interfaces;
using NUnit.Framework;

namespace Hobbit
{
    [TestFixture]
    public class MapLoaderTests
    {

        [Test]
        public void LoadsMapCorrectly()
        {
            var loader = new MapLoader("test_map.m");
            IMap tempMap = new Map(3, 2);
            tempMap[new Point(0, 0)] = new Road();
            tempMap[new Point(0, 1)] = new Wall();
            tempMap[new Point(1, 0)] = new Wall();
            tempMap[new Point(1, 1)] = new Road();
            tempMap[new Point(2, 0)] = new Road();
            tempMap[new Point(2, 1)] = new Road();
            var expected = tempMap.ToList();
            var map = loader.LoadMap().ToList();
            for (var i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i], map[i]);
            }
        }
    }

    [TestFixture]
    public class ForestTests
    {
        [Test]
        public void ProcessTurnCorrectly()
        {
            IMap tempMap = new Map(2, 1);
            tempMap[new Point(0, 0)] = new Road();
            tempMap[new Point(1, 0)] = new Wall();
            IForest forest = new Forest(tempMap);
            Assert.True(forest.SetResidentAt(new Resident(1, "A", 2, new Point(0, 0))));
            Assert.False(forest.SetResidentAt(new Resident(2, "B", 1, new Point(1, 0))));
        }
    }
}