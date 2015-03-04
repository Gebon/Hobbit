using System.Drawing;
using System.Linq;
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
            var loader = new MapLoader("map.m");
            IMap tempMap = new Map(3, 2);
            tempMap[new Point(0, 0)] = new Road();
            tempMap[new Point(0, 1)] = new Wall();
            tempMap[new Point(1, 0)] = new Wall();
            tempMap[new Point(1, 1)] = new Road();
            tempMap[new Point(2, 0)] = new Road();
            tempMap[new Point(2, 1)] = new Road();
            var expected = tempMap.ToList();
            var map = loader.LoadMap().ToList();
            for (int i = 0; i < expected.Count; i++)
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
            IMap tempMap = new Map(3, 2);
            tempMap[new Point(0, 0)] = new Road();
            tempMap[new Point(0, 1)] = new Wall();
            tempMap[new Point(1, 0)] = new Wall();
            tempMap[new Point(1, 1)] = new Road();
            tempMap[new Point(2, 0)] = new Road();
            tempMap[new Point(2, 1)] = new Road();
            IForest forest = new Forest(tempMap);
            Assert.True(forest.SetResidentAt(new Point(0, 0), "A", 2));
            Assert.False(forest.SetResidentAt(new Point(1, 0), "B", 1 ));
        }
    }
}