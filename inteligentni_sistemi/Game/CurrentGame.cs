using etf.dotsandboxes.cl160127d.AI;

namespace etf.dotsandboxes.cl160127d.Game
{
    class CurrentGame
    {
        private int[] score = new int[2];
        private BasePlayer opponent;

        public CurrentGame(int tableSizeX, int tableSizeY, BasePlayer opponent = null)
        {
            TableSizeX = tableSizeX;
            TableSizeY = tableSizeY;
            Turn = Player.BLUE;
            Score = score;

            score[0] = score[1] = 0;
            this.opponent = opponent;
        }

        public int TableSizeX { get; set; }
        public int TableSizeY { get; set; }

        public bool GameOver { get; set; }
        public Player Turn { get; set; }
        public int[] Score { get; }

        public BasePlayer Opponent
        {
            get
            {
                return opponent;
            }
        }
    }
}