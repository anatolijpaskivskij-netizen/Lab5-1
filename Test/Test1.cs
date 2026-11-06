using Microsoft.VisualStudio.TestTools.UnitTesting;
using PoetDatabaseAbstractApp;
using System;

namespace PoetDatabaseTests
{
    [TestClass]
    public class PoetPerformanceTests
    {
        [TestMethod]
        public void GetLastNameLength_ReturnsCorrectLength()
        {
            // Arrange (підготовка даних)
            string testLastName = "Шевченко";
            int expectedLength = 8;

            PoetPerformance poet = new PoetPerformance(
                testLastName,
                "Українська",
                12,
                new DateTime(2025, 3, 9),
                "Національна філармонія",
                450
            );

            // Act (виконання методу)
            int actualLength = poet.GetLastNameLength();

            // Assert (перевірка результату)
            Assert.AreEqual(expectedLength, actualLength, $"Довжина прізвища має бути {expectedLength}, але отримано {actualLength}.");
        }
    }
}