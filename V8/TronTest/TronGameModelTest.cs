using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tron.Model;
using Tron.Persistence;
using Moq;

namespace TronTest
{
    [TestClass]
    public class TronGameModelTest
    {
        private TronGameModel _model = null!;
        private TronTable _mockedTable = null!;
        private Mock<ITronDataAccess> _mock = null!;

        [TestInitialize]
        public void Initialize()
        {
            _mockedTable = new TronTable(24);

            _mock = new Mock<ITronDataAccess>();
            _mock.Setup(mock => mock.LoadAsync(It.IsAny<String>()))
                .Returns(() => Task.FromResult(_mockedTable));

            _model = new TronGameModel(_mock.Object);

            _model.GameOver += new EventHandler<TronEventArgs>(Model_GameOver);

        }

        [TestMethod]

        public void TronGameModelNewGameSmallBoard()
        {
            _model.BoardSize = BoardSize.Small;
            _model.NewGame();

            Assert.AreEqual(BoardSize.Small, _model.BoardSize);
            Assert.AreEqual(12, _model.Table.GridSize);

        }

        [TestMethod]
        public void TronGameModelNewGameMediumBoard()
        {
            _model.NewGame();

            Assert.AreEqual(BoardSize.Medium, _model.BoardSize);
            Assert.AreEqual(24, _model.Table.GridSize);

        }

        [TestMethod]
        public void TronGameModelNewGameMediumLarge()
        {
            _model.BoardSize = BoardSize.Large;
            _model.NewGame();

            Assert.AreEqual(BoardSize.Large, _model.BoardSize);
            Assert.AreEqual(36, _model.Table.GridSize);

        }

        [TestMethod]
        public void TronGameModelTurnTest()
        {
            _model.AdvanceGame();
            Assert.AreEqual(1, _model.Table.Blue.X); // A kék játékos balra
            Assert.AreEqual(22, _model.Table.Red.X); // a piros játékos jobbra helyezkedik
        }

        [TestMethod]
        public async Task TronGameModelLoadTest()
        {
            _model.NewGame();

            await _model.LoadGameAsync(String.Empty);

            for(int i = 0; i < 5; i++) 
            {
                for (int j = 0; j < 5; j++)
                {
                    Assert.AreEqual(_mockedTable.Grid[i, j], _model.Table.Grid[i, j]);
                }
            }

            Assert.AreEqual(_mockedTable.GridSize, _model.Table.GridSize);
        }


        [TestMethod]
        private void Model_GameOver(object? sender, TronEventArgs e)
        {
            Assert.IsTrue(_model._isGameOver);
        }
    }
}
